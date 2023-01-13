using Microsoft.AspNetCore.Mvc;
using PostalTracker.API.Models;
using PostalTracker.API.Services;
using PostalTracker.Contracts.Events;

namespace PostalTracker.API.Controllers;

[ApiController, Route("postal")]
public class PostalController : ControllerBase
{
    private readonly PostalService _postalService;

    public PostalController(PostalService postalService)
    {
        _postalService = postalService;
    }
    
    /// <summary>
    /// Get current state postal
    /// </summary>
    /// <param name="postalId">Postal id</param>
    /// <returns>Current state and postal id</returns>
    [HttpGet("state/{postalId:guid}")]
    public async Task<ActionResult<PostalStatus>> GetPostalStatusAsync([FromRoute] Guid postalId)
    {
        var result = await _postalService.GetPostalStatusAsync(postalId).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Create new postal
    /// </summary>
    /// <param name="postalDto">Postal model</param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<ActionResult> CreatePostalAsync([FromBody] CreatePostalDto postalDto)
    {
        await _postalService.CreatePostalAsync(postalDto).ConfigureAwait(false);
        return Accepted();
    }
    
    /// <summary>
    /// Postal payment
    /// </summary>
    /// <param name="postalId">Postal id</param>
    /// <returns></returns>
    [HttpPatch("pay/{postalId:guid}")]
    public async Task<ActionResult> PayPostalAsync([FromRoute] Guid postalId)
    {
        await _postalService.PayPostalAsync(postalId).ConfigureAwait(false);
        return Accepted();
    }
    
    /// <summary>
    /// Receive postal
    /// </summary>
    /// <param name="postalId">Postal id</param>
    /// <returns></returns>
    [HttpPatch("receive/{postalId:guid}")]
    public async Task<ActionResult> ReceivePostalAsync([FromRoute] Guid postalId)
    {
        await _postalService.ReceivePostalAsync(postalId).ConfigureAwait(false);
        return Accepted();
    }
    
    /// <summary>
    /// Lost postal
    /// </summary>
    /// <param name="postalId">Postal id</param>
    /// <returns></returns>
    [HttpPatch("lost/{postalId:guid}")]
    public async Task<ActionResult> LostPostalAsync([FromRoute] Guid postalId)
    {
        await _postalService.LostPostalAsync(postalId).ConfigureAwait(false);
        return Accepted();
    }
    
    /// <summary>
    /// Return postal
    /// </summary>
    /// <param name="postalId">Postal id</param>
    /// <returns></returns>
    [HttpPatch("return/{postalId:guid}")]
    public async Task<ActionResult> ReturnPostalAsync([FromRoute] Guid postalId)
    {
        await _postalService.ReturnPostalAsync(postalId).ConfigureAwait(false);
        return Accepted();
    }

}