using ai_documentation_cli.Domain.Interfaces;

namespace ai_documentation_cli.Domain.Models;

/// <summary>
/// Represents a class for documenting functions with summary and lines.
/// </summary>
public class FunctionDocumentation : IDocumentable
{
    public string? Summary { get; set; } = string.Empty;
    public List<Line> Lines { get; set; } = new ();
}
