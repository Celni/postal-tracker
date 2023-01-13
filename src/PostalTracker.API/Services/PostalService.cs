using MassTransit;
using PostalTracker.API.Models;
using PostalTracker.Contracts.Events;
using PostalTracker.System.Exceptions;

namespace PostalTracker.API.Services;

public class PostalService
{
    private readonly IRequestClient<PostalStatusRequest> _postalCheckClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public PostalService(
        IRequestClient<PostalStatusRequest> postalCheckClient,
        IPublishEndpoint publishEndpoint)
    {
        _postalCheckClient = postalCheckClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<PostalStatus> GetPostalStatusAsync(Guid id)
    {
        var (status, notFound) = await _postalCheckClient.GetResponse<PostalStatus, PostalNotFound>(new { Id = id }).ConfigureAwait(false);
        if (status.IsCompletedSuccessfully)
        {
            var response = await status.ConfigureAwait(false);
            return response.Message;
        }
        else
        {
            var response = await notFound.ConfigureAwait(false);
            throw new PostalNotFoundException($"Postal: {response.Message.Id} - not found");
        }
    }

    public Task CreatePostalAsync(CreatePostalDto postalDto)
    {
        return _publishEndpoint.Publish<PostalCreate>(new
        {
            Id = postalDto.PostalId,
            InVar.Timestamp,
            postalDto.AddressDelivery,
            postalDto.AddressSender
        });
    }

    public Task ReceivePostalAsync(Guid id)
    {
        return _publishEndpoint.Publish<PostalReceived>(new
        {
            Id = id
        });
    }

    public Task PayPostalAsync(Guid postalId)
    {
        return _publishEndpoint.Publish<PostalPaid>(new
        {
            Id = postalId
        });
    }

    public Task LostPostalAsync(Guid postalId)
    {
        return _publishEndpoint.Publish<PostalLost>(new
        {
            Id = postalId
        });
    }

    public Task ReturnPostalAsync(Guid postalId)
    {
        return _publishEndpoint.Publish<PostalReturn>(new
        {
            Id = postalId
        });
    }
}