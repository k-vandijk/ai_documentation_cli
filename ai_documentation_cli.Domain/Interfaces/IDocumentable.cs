using ai_documentation_cli.Domain.Models;

namespace ai_documentation_cli.Domain.Interfaces;

internal interface IDocumentable
{
    public string? Summary { get; set; }
    public List<Line> Lines { get; set; }
}