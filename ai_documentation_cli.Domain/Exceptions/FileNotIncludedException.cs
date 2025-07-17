namespace ai_documentation_cli.Domain.Exceptions;

/// <summary>
/// Represents an exception thrown when a required file is not included.
/// </summary>
public class FileNotIncludedException : Exception
{
    public FileNotIncludedException()
    {
    }

    public FileNotIncludedException(string message) 
        : base(message)
    {
    }

    public FileNotIncludedException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
