namespace ai_documentation_cli.Domain.Models;

public class ParsedFile
{
    public List<Line> Lines { get; set; } = new List<Line>();
    public List<ClassDocumentation> Classes { get; set; } = new List<ClassDocumentation>();
    public List<FunctionDocumentation> Functions { get; set; } = new List<FunctionDocumentation>();
}