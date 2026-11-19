namespace Milestone04_Database.Services;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
