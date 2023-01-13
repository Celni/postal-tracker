namespace PostalTracker.Contracts.Events;

public interface PostalCreate : PostalId
{
    DateTime Timestamp { get; }
    
    string AddressDelivery { get; }
    
    string AddressSender { get; }
}