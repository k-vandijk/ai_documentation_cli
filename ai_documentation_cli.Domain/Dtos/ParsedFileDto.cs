namespace ai_documentation_cli.Domain.Dtos;

public class ParsedFileDto
{
    public List<LineDto> Lines { get; set; } = new List<LineDto>();
    public List<ClassDocumentationDto> Classes { get; set; } = new List<ClassDocumentationDto>();
    public List<FunctionDocumentationDto> Functions { get; set; } = new List<FunctionDocumentationDto>();
}