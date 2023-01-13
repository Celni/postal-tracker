namespace PostalTracker.System.Exceptions;

public class PostalException : Exception
{
    public int HttpStatusCode { get; }
    
    public PostalException(string message, int httpStatusCode) : base(message)
    {
        HttpStatusCode = httpStatusCode;
    }

    public PostalException(string message) : this(message, 500)
    {
    }
}