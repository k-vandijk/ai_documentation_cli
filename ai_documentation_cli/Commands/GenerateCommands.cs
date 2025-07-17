using ai_documentation_cli.App;
using ai_documentation_cli.Application.Operations;
using ai_documentation_cli.Domain.Models;
using Cocona;
using kvandijk.Common.Interfaces;

namespace ai_documentation_cli.Commands;

// TODO: Add validation, for example, check if there are not too many files in the directory.
// TODO: Make sure the Xml is added before the function attributes.

/// <summary>
/// Generates documentation for files or directories based on provided input. Handles the generation process for commands.
/// </summary>
public class GenerateCommands
{
    private readonly IChatCompletionService _chatCompletionService;

    public GenerateCommands(IChatCompletionService chatCompletionService)
    {
        _chatCompletionService = chatCompletionService;
    }

    /// <summary>
    /// Generates documentation for either a file or all relevant files in a directory.
    /// </summary>
    /// <param name="file">The path of the file for which documentation needs to be generated.</param>
    /// <param name="dir">The directory path containing files for documentation generation.</param>
    /// <param name="additionalQuery">Additional query parameters for the documentation generation process.</param>
    /// <returns>Task representing the asynchronous documentation generation process.</returns>
    [Command("generate")]
    public async Task Execute([Option("file")] string? file, [Option("dir")] string? dir, [Option("query")] string? additionalQuery)
    {
        if (string.IsNullOrEmpty(file) && string.IsNullOrEmpty(dir))
        {
            throw new InvalidOperationException("You must provide either a file or a directory to generate documentation.");
        }

        if (!string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(dir))
        {
            throw new InvalidOperationException("You cannot provide both a file and a directory to generate documentation.");
        }

        if (!string.IsNullOrEmpty(file))
        {
            await HandleDocumentationGenerationForFile(file);
        }
        else if (!string.IsNullOrEmpty(dir))
        {
            var extensions = new List<string> { ".cs" };
            var currentDirectory = Environment.CurrentDirectory;
            var directory = Path.Join(currentDirectory, dir);
            var relevantFiles = DirectoryOperations.ListRelevantFiles(directory, extensions);

            foreach (var relevantFile in relevantFiles)
            {
                await HandleDocumentationGenerationForFile(relevantFile);
            }
        }
    }

    // TODO: RELOCATE: This file is meant only for Commands, any handlers or helpers should be moved elsewhere.

    private static bool ShouldUpdateSummary(string summary)
    {
        return !summary.Trim().Contains("<sufficient>");
    }

    // TODO: RELOCATE: This file is meant only for Commands, any handlers or helpers should be moved elsewhere.

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

    // TODO: RELOCATE: This file is meant only for Commands, any handlers or helpers should be moved elsewhere.

    private async Task HandleDocumentationGenerationForFile(string file)
    {
        var lines = FileParser.GetFileLines(file);
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

        await File.WriteAllLinesAsync(file, lines.Select(l => l.Content));
    }

    // TODO: RELOCATE: This file is meant only for Commands, any handlers or helpers should be moved elsewhere.

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

    // TODO: RELOCATE: This file is meant only for Commands, any handlers or helpers should be moved elsewhere.

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
