namespace ai_documentation_cli.Domain.Models;

/// <summary>
/// Represents a parameter with a type and name.
/// </summary>
public class Parameter
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
