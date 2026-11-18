namespace Milestone03_LayeredConsole.Services;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
