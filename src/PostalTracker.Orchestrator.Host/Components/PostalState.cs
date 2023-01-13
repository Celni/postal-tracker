using Automatonymous;

namespace PostalTracker.Orchestrator.Host.Components;

public class PostalState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    
    public string CurrentState { get; set; }

    public DateTime Timestamp { get; set; }

    public string AddressDelivery { get; set; }

    public string AddressSender { get; set; }
    
    public Guid? InWayDurationToken { get; set; }
}