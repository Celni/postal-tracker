using MassTransit;

namespace PostalTracker.Orchestrator.Host;

public class MassTransitHostedService :
    IHostedService
{
    private readonly IBusControl _busControl;

    public MassTransitHostedService(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _busControl.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _busControl.StopAsync(cancellationToken);
    }
}