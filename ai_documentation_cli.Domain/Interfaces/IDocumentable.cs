using ai_documentation_cli.Domain.Dtos;

namespace ai_documentation_cli.Domain.Interfaces;

internal interface IDocumentable
{
    public string Summary { get; set; }
    public List<LineDto> Lines { get; set; }
}