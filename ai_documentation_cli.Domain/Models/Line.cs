namespace ai_documentation_cli.Domain.Models;

/// <summary>
/// Represents a line with a unique identifier and content.
/// </summary>
public class Line
{
    public string UniqueIdentifier { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
