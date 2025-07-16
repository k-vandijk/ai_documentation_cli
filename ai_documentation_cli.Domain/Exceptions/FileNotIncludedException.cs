namespace ai_documentation_cli.Domain.Exceptions;

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