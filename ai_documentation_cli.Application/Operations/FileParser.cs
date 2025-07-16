using ai_documentation_cli.Domain.Models;
using System.Text.RegularExpressions;

namespace ai_documentation_cli.Application.Operations;

/// <summary>
/// This class provides methods for parsing a file containing C# code to extract class and method definitions along with their documentation.
/// It includes functionality to read lines from a file, parse classes, parse functions, and extract code blocks for classes or functions.
/// </summary>
public static class FileParser
{
    public static List<Line> GetFileLines(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file '{path}' does not exist.");
        }

        return File.ReadLines(path).Select(l => new Line { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = l }).ToList();
    }

    public static List<ClassDocumentation> ParseClasses(List<Line> lines)
    {
        // This regex matches class, record, or struct definitions in C# code.
        // It looks for the keywords 'class', 'record', or 'struct' followed by one or more whitespace characters and then a valid identifier (name).
        // It does not match generic types or nested classes.
        // It assumes that the class definition is on a single line, which is common in C#.
        var classRegex = new Regex(@"\b(class|record|struct)\s+\w+", RegexOptions.Compiled);
        var classes = new List<ClassDocumentation>();

        for (int i = 0; i < lines.Count; i++)
        {
            var trimmed = lines[i].Content.TrimStart();
            if (trimmed.StartsWith("//"))
            {
                continue;
            }

            if (classRegex.IsMatch(lines[i].Content.Trim()))
            {
                var block = ExtractBlock(lines, ref i);

                classes.Add(new ClassDocumentation
                {
                    Summary = string.Empty,
                    Lines = block,
                });
            }
        }

        foreach (var classDoc in classes)
        {
            var summaryLines = GetXmlDocumentationLinesAt(classDoc.Lines.First().UniqueIdentifier, lines);

            if (summaryLines.Any())
            {
                classDoc.Summary = string.Join("\n", summaryLines.Select(l => l.Content.Trim()));
            }
        }

        return classes;
    }

    public static List<FunctionDocumentation> ParseFunctions(List<Line> lines)
    {
        // This regex matches method definitions in C# code.
        // It looks for access modifiers (public, protected, internal, private), optional static and async keywords,
        // followed by a return type (which can include generics), a method name, and a parameter list.
        // It does not match method signatures that are split across multiple lines.
        var methodRegex = new Regex(@"^\s*(public|protected|internal|private)\s+(static\s+)?(async\s+)?[\w<>\[\],]+\s+\w+\s*\(.*?\)\s*$", RegexOptions.Compiled);
        var functions = new List<FunctionDocumentation>();

        for (int i = 0; i < lines.Count; i++)
        {
            var trimmed = lines[i].Content.TrimStart();
            if (trimmed.StartsWith("//"))
            {
                continue;
            }

            if (methodRegex.IsMatch(lines[i].Content.Trim()))
            {
                var block = ExtractBlock(lines, ref i);

                var sigLine = block.FirstOrDefault()?.Content.Trim() ?? string.Empty;

                functions.Add(new FunctionDocumentation
                {
                    Summary = string.Empty,
                    Lines = block,
                });
            }
        }

        return functions;
    }

    private static List<Line> GetXmlDocumentationLinesAt(string lineId, List<Line> lines)
    {
        int i = lines.FindIndex(l => l.UniqueIdentifier == lineId) - 1; // Start from the line before the method or class declaration
        var result = new List<Line>();
        while (i >= 0 && lines[i].Content.TrimStart().StartsWith("///"))
        {
            result.Insert(0, lines[i]);
            i--;
        }

        return result;
    }

    private static List<Line> ExtractBlock(List<Line> lines, ref int index)
    {
        var block = new List<Line>();
        int braceCount = 0;
        bool started = false;

        for (; index < lines.Count; index++)
        {
            var line = lines[index];
            block.Add(line);

            foreach (char c in line.Content)
            {
                if (c == '{')
                {
                    braceCount++;
                    started = true;
                }
                else if (c == '}')
                {
                    braceCount--;
                }
            }

            if (started && braceCount == 0)
            {
                break;
            }
        }

        return block;
    }
}
