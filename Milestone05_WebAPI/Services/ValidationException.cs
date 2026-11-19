namespace Milestone05_WebAPI.Services;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
