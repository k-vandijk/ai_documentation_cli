using ai_documentation_cli.Domain.Interfaces;

namespace ai_documentation_cli.Domain.Models;

public class ClassDocumentation: IDocumentable
{
    public string? Summary { get; set; } = string.Empty;
    public List<Line> Lines { get; set; } = new ();
}