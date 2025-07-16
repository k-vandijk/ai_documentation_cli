using ai_documentation_cli.Application.Interfaces;
using ai_documentation_cli.Application.Operations;
using ai_documentation_cli.Domain.Models;
using Cocona;

namespace ai_documentation_cli.App.Commands;

// TODO this code needs to be refactored.
// TODO Make sure that when documentation already exists, it determines whether to overwrite it or not.
// - I already have instructed the LLM
// - All needed to do is to parse the existing documentation, and return it with the Documentation model
// - Then test the code with a file that has existing documentation, and fix any issues that arise.

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
            var prompt = PromptBuilder.BuildPrompt(stringifiedClass, c.Summary);
            var classSummary = await _chatCompletionService.GetChatCompletionAsync(prompt, Instructions.ClassDocumentationInstructions);

            if (!string.IsNullOrEmpty(classSummary.Trim()))
            {
                c.Summary = classSummary;

                var linesToInsert = FileInserter.SplitSummaryIntoLines(classSummary);
                lines = FileInserter.InsertLinesAt(c.Lines.First().UniqueIdentifier, linesToInsert, lines);

                await File.WriteAllLinesAsync(file, lines.Select(l => l.Content));
            }
        }
    }
}