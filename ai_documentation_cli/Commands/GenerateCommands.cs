using System.Diagnostics;
using ai_documentation_cli.Dtos;
using ai_documentation_cli.Interfaces;
using ai_documentation_cli.Operations;
using Cocona;

namespace ai_documentation_cli.Commands;

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

        var sw = Stopwatch.StartNew();
        var classes = FileParser.ParseClasses(lines);
        sw.Stop();
        Console.WriteLine($"Parsed {classes.Count} classes in {sw.ElapsedMilliseconds} ms");

        sw = Stopwatch.StartNew();
        var functions = FileParser.ParseFunctions(lines);
        sw.Stop();
        Console.WriteLine($"Parsed {functions.Count} functions in {sw.ElapsedMilliseconds} ms");

        var parsedFile = new ParsedFileDto { Lines = lines, Classes = classes, Functions = functions, };

        foreach (var c in parsedFile.Classes)
        {
            var stringifiedClass = string.Join("\n", c.Lines.Select(l => l.Content));
            var classSummary = await _chatCompletionService.GetChatCompletionAsync(stringifiedClass, Instructions.ClassDocumentationInstructions);

            c.Summary = classSummary;

            var linesToInsert = FileInserter.SplitSummaryIntoLines(classSummary);
            lines = FileInserter.InsertLinesAt(c.Lines.First().UniqueIdentifier, linesToInsert, lines);

            await File.WriteAllLinesAsync(file, lines.Select(l => l.Content));
        }

        // foreach (var f in parsedFile.Functions)
        // {
        //     var stringifiedFunction = string.Join("\n", f.Lines.Select(l => l.Content));
        //     var functionSummary = await _chatCompletionService.GetChatCompletionAsync(stringifiedFunction, Instructions.FunctionDocumentationInstructions);
        //     
        //     f.Summary = functionSummary;
        //     
        //     var linesToInsert = FileInserter.SplitSummaryIntoLines(functionSummary);
        //     lines = FileInserter.InsertLinesAt(f.Lines.First().UniqueIdentifier, linesToInsert, lines);
        //     
        //     await File.WriteAllLinesAsync(file, lines.Select(l => l.Content));
        // }
    }
}