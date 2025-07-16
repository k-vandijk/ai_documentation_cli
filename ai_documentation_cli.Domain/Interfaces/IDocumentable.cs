using ai_documentation_cli.Domain.Models;

namespace ai_documentation_cli.Domain.Interfaces;

internal interface IDocumentable
{
    /// <summary>
    /// Summary excluding indentation; e.g. /// "<summary>\n/// ...\n/// </summary>"
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Lines include indentation!
    /// </summary>
    public List<Line> Lines { get; set; }
}