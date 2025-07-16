using ai_documentation_cli.Application.Interfaces;
using ai_documentation_cli.Application.Operations;
using ai_documentation_cli.Domain.Models;
using Cocona;

namespace ai_documentation_cli.App.Commands;

// TODO this code needs to be refactored.
// TODO Make sure that when documentation already exists, it determines whether to overwrite it or not.

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
            throw new ArgumentException("The 'file' option must be provided.");
        }

        var lines = FileParser.GetFileLines(file);

        var classes = FileParser.ParseClasses(lines);
        var functions = FileParser.ParseFunctions(lines);
        var parsedFile = new ParsedFile { Lines = lines, Classes = classes, Functions = functions, };

        foreach (var c in parsedFile.Classes)
        {
            var stringifiedClass = string.Join("\n", c.Lines.Select(l => l.Content));
            var classSummary = await _chatCompletionService.GetChatCompletionAsync(stringifiedClass, Instructions.ClassDocumentationInstructions);

            c.Summary = classSummary;

            var linesToInsert = FileInserter.SplitSummaryIntoLines(classSummary);
            lines = FileInserter.InsertLinesAt(c.Lines.First().UniqueIdentifier, linesToInsert, lines);

            await File.WriteAllLinesAsync(file, lines.Select(l => l.Content));
        }

        foreach (var f in parsedFile.Functions.Where(f => f.Lines.First().Content.Trim().StartsWith("public")))
        {
            var stringifiedFunction = string.Join("\n", f.Lines.Select(l => l.Content));
            var functionSummary = await _chatCompletionService.GetChatCompletionAsync(stringifiedFunction, Instructions.FunctionDocumentationInstructions);
            
            f.Summary = functionSummary;
            
            var linesToInsert = FileInserter.SplitSummaryIntoLines(functionSummary);
            lines = FileInserter.InsertLinesAt(f.Lines.First().UniqueIdentifier, linesToInsert, lines);
            
            await File.WriteAllLinesAsync(file, lines.Select(l => l.Content));
        }
    }
}