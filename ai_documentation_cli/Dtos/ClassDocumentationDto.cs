namespace ai_documentation_cli.Dtos;

public class ClassDocumentationDto
{
    public string Summary { get; set; } = string.Empty;
    public List<LineDto> Lines { get; set; } = new ();
}