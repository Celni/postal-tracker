namespace PostalTracker.System.Exceptions;

public class PostalBadRequestException : PostalException
{
    public PostalBadRequestException(string message) : base(message, 400)
    {
    }
}