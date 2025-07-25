namespace ai_documentation_cli.Domain.Models;

/// <summary>
/// Represents a parsed file containing information about lines, classes, and functions extracted from the file.
/// </summary>
public class ParsedFile
{
    public List<Line> Lines { get; set; } = new ();
    public List<ClassDocumentation> Classes { get; set; } = new ();
    public List<FunctionDocumentation> Functions { get; set; } = new ();
}
