using ai_documentation_cli.Application.Ai;
using ai_documentation_cli.Application.Interfaces;
using ai_documentation_cli.Application.Operations;
using ai_documentation_cli.Domain.Models;
using kvandijk.Common.Interfaces;

namespace ai_documentation_cli.Infrastructure.Services;

/// <summary>
/// A service responsible for generating documentation for classes and functions in code files.
/// </summary>
public class DocumentationGenerationService : IDocumentationGenerationService
{
    private readonly IChatCompletionService _chatCompletionService;

    public DocumentationGenerationService(IChatCompletionService chatCompletionService)
    {
        _chatCompletionService = chatCompletionService;
    }

    /// <summary>
    /// Asynchronously generates documentation for a file by parsing its classes and functions,
    /// and processing the documentation for each class and public function found in the file.
    /// </summary>
    /// <param name="file">The path of the file to generate documentation for.</param>
    /// <param name="dir">The directory where the file is located. It is optional, default is null.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task HandleDocumentationGenerationForFile(string file, string? dir = null)
    {
        var path = string.IsNullOrEmpty(dir) ? file : Path.Combine(dir, file);

        var lines = FileParser.GetFileLines(path);
        var classes = FileParser.ParseClasses(lines);
        var functions = FileParser.ParseFunctions(lines);
        var parsedFile = new ParsedFile { Lines = lines, Classes = classes, Functions = functions, };

        foreach (var c in parsedFile.Classes)
        {
            lines = await ProcessClassDocumentation(c, lines);
        }

        foreach (var f in parsedFile.Functions.Where(f => f.Lines.First().Content.Trim().StartsWith("public")))
        {
            lines = await ProcessFunctionDocumentation(f, lines);
        }

        await File.WriteAllLinesAsync(path, lines.Select(l => l.Content));
    }

    private static bool ShouldUpdateSummary(string summary)
    {
        return !summary.Trim().Contains("<sufficient>");
    }

    private static List<Line> ReplaceExistingSummary(string anchorId, string newSummary, List<Line> lines)
    {
        var existingSummaryLines = FileParser.GetXmlDocumentationLinesAt(anchorId, lines);

        // Remove any existing summary lines.
        if (existingSummaryLines.Any())
        {
            lines = lines.Where(l => existingSummaryLines.All(el => el.UniqueIdentifier != l.UniqueIdentifier)).ToList();
        }

        var newLines = FileInserter.SplitSummaryIntoLines(newSummary);
        return FileInserter.InsertLinesAt(anchorId, newLines, lines);
    }

    private async Task<List<Line>> ProcessClassDocumentation(ClassDocumentation c, List<Line> lines)
    {
        var content = string.Join("\n", c.Lines.Select(l => l.Content));
        var prompt = PromptBuilder.BuildPrompt(content, c.Summary);
        var summary = await _chatCompletionService.GetChatCompletionAsync(prompt, Instructions.ClassDocumentationInstructions);

        if (ShouldUpdateSummary(summary))
        {
            c.Summary = summary;
            lines = ReplaceExistingSummary(c.Lines.First().UniqueIdentifier, summary, lines);
        }

        return lines;
    }

    private async Task<List<Line>> ProcessFunctionDocumentation(FunctionDocumentation f, List<Line> lines)
    {
        var content = string.Join("\n", f.Lines.Select(l => l.Content));
        var prompt = PromptBuilder.BuildPrompt(content, f.Summary);
        var summary = await _chatCompletionService.GetChatCompletionAsync(prompt, Instructions.FunctionDocumentationInstructions);

        if (ShouldUpdateSummary(summary))
        {
            f.Summary = summary;
            lines = ReplaceExistingSummary(f.Lines.First().UniqueIdentifier, summary, lines);
        }

        return lines;
    }
}
