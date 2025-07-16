using ai_documentation_cli.Domain.Interfaces;

namespace ai_documentation_cli.Domain.Dtos;

public class FunctionDocumentationDto : IDocumentable
{
    public string Summary { get; set; } = string.Empty;
    public List<LineDto> Lines { get; set; } = new ();
}
