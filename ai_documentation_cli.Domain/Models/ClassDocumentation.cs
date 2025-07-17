using ai_documentation_cli.Domain.Interfaces;

namespace ai_documentation_cli.Domain.Models;

/// <summary>
/// Represents a class that stores document lines and a summary.
/// </summary>
public class ClassDocumentation : IDocumentable
{
    public string? Summary { get; set; } = string.Empty;
    public List<Line> Lines { get; set; } = new ();
}
