namespace Milestone05_WebAPI.Services;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
