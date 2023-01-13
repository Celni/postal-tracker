namespace PostalTracker.Contracts.Events;

public interface PostalStatus : PostalCreate
{
    string State { get; }
}