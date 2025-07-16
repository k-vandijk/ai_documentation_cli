using ai_documentation_cli.Application.Interfaces;
using ai_documentation_cli.Application.Operations;
using ai_documentation_cli.Domain.Exceptions;
using ai_documentation_cli.Domain.Models;
using Cocona;

namespace ai_documentation_cli.App.Commands;

// TODO this code needs to be refactored.

public class GenerateCommands
{
    private readonly IChatCompletionService _chatCompletionService;

    public GenerateCommands(IChatCompletionService chatCompletionService)
    {
        _chatCompletionService = chatCompletionService;
    }

    [Command("generate")]
    public async Task Execute([Option("file")] string? file, [Option("query")] string? additionalQuery)
    {
        if (string.IsNullOrEmpty(file))
        {
            throw new FileNotIncludedException("The 'file' parameter must be provided.");
        }

        var lines = FileParser.GetFileLines(file);

        var classes = FileParser.ParseClasses(lines);
        var functions = FileParser.ParseFunctions(lines);
        var parsedFile = new ParsedFile { Lines = lines, Classes = classes, Functions = functions, };

        foreach (var c in parsedFile.Classes)
        {
            var stringifiedClass = string.Join("\n", c.Lines.Select(l => l.Content));
            var prompt = PromptBuilder.BuildPrompt(stringifiedClass, c.Summary);
            var classSummary = await _chatCompletionService.GetChatCompletionAsync(prompt, Instructions.ClassDocumentationInstructions);

            if (!classSummary.Trim().Contains("<sufficient>"))
            {
                c.Summary = classSummary;

                // Remove existing summary lines if they exist
                var existingSummaryLines = FileParser.GetXmlDocumentationLinesAt(c.Lines.First().UniqueIdentifier, lines);
                if (existingSummaryLines.Any())
                {
                    lines = lines.Where(l => existingSummaryLines.All(el => el.UniqueIdentifier != l.UniqueIdentifier)).ToList();
                }

                var linesToInsert = FileInserter.SplitSummaryIntoLines(classSummary);
                lines = FileInserter.InsertLinesAt(c.Lines.First().UniqueIdentifier, linesToInsert, lines);

                await File.WriteAllLinesAsync(file, lines.Select(l => l.Content));
            }
        }

        foreach (var f in parsedFile.Functions)
        {
            var stringifiedFunction = string.Join("\n", f.Lines.Select(l => l.Content));
            var prompt = PromptBuilder.BuildPrompt(stringifiedFunction, f.Summary);
            var classSummary = await _chatCompletionService.GetChatCompletionAsync(prompt, Instructions.FunctionDocumentationInstructions);

            if (!classSummary.Trim().Contains("<sufficient>"))
            {
                f.Summary = classSummary;

                // Remove existing summary lines if they exist
                var existingSummaryLines = FileParser.GetXmlDocumentationLinesAt(f.Lines.First().UniqueIdentifier, lines);
                if (existingSummaryLines.Any())
                {
                    lines = lines.Where(l => existingSummaryLines.All(el => el.UniqueIdentifier != l.UniqueIdentifier)).ToList();
                }

                var linesToInsert = FileInserter.SplitSummaryIntoLines(classSummary);
                lines = FileInserter.InsertLinesAt(f.Lines.First().UniqueIdentifier, linesToInsert, lines);

                await File.WriteAllLinesAsync(file, lines.Select(l => l.Content));
            }
        }
    }
}