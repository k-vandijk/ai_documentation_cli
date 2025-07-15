namespace ai_documentation_cli.Dtos;

public class FunctionDocumentationDto
{
    public string Summary { get; set; } = string.Empty;
    public List<LineDto> Lines { get; set; } = new ();
    public List<ParameterDto> Parameters { get; set; } = new ();
    public ReturnTypeDto ReturnType { get; set; } = new ();
}
