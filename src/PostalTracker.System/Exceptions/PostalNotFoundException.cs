namespace PostalTracker.System.Exceptions;

public class PostalNotFoundException : PostalException
{
    public PostalNotFoundException(string message) : base(message, 404)
    {
    }
}