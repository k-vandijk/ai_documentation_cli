using ai_documentation_cli.Domain.Models;

namespace ai_documentation_cli.Application.Operations;

/// <summary>
/// This static class provides methods for inserting lines at a specific point in a list of lines and splitting a summary into separate lines.
/// </summary>
public static class FileInserter
{
    /// <summary>
    /// Inserts a list of lines (typically XML documentation) before the declaration line identified by the given lineId,
    /// ensuring the inserted lines appear above any attribute lines.
    /// </summary>
    /// <param name="lineId">The unique identifier of the target declaration line (e.g., a method or class).</param>
    /// <param name="linesToInsert">The lines to insert (e.g., XML documentation).</param>
    /// <param name="lines">The full list of lines where insertion will happen.</param>
    /// <returns>A new list of lines with the inserted lines positioned above any attributes.</returns>
    public static List<Line> InsertLinesAt(string lineId, List<Line> linesToInsert, List<Line> lines)
    {
        if (linesToInsert == null || linesToInsert.Count == 0)
        {
            // TODO use a more specific custom exception type.
            throw new ArgumentException("The lines to insert cannot be null or empty.", nameof(linesToInsert));
        }

        var result = new List<Line>();
        bool hasInserted = false;

        for (int currentIndex = 0; currentIndex < lines.Count; currentIndex++)
        {
            var currentLine = lines[currentIndex];

            // Insert when we reach the target declaration line
            if (!hasInserted && currentLine.UniqueIdentifier == lineId)
            {
                // Determine the insertion index before any [Attribute] lines
                int insertIndex = result.Count;
                while (insertIndex > 0 && result[insertIndex - 1].Content.TrimStart().StartsWith("["))
                {
                    insertIndex--;
                }

                // Match the indentation of the declaration line
                var indentation = new string(' ', currentLine.Content.TakeWhile(char.IsWhiteSpace).Count());

                // Insert documentation lines before the attribute block
                foreach (var docLine in linesToInsert)
                {
                    result.Insert(insertIndex++, new Line
                    {
                        UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(),
                        Content = indentation + docLine.Content.TrimStart(),
                    });
                }

                hasInserted = true;
            }

            result.Add(currentLine);
        }

        return result;
    }

    /// <summary>
    /// This function takes a summary string and splits it into individual lines.
    /// </summary>
    /// <param name="summary">The summary string to be split into lines</param>
    /// <returns>A List of LineDto objects containing unique identifiers and trimmed content for each line in the summary</returns>
    public static List<Line> SplitSummaryIntoLines(string summary)
    {
        var lines = summary.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        return lines.Select(line => new Line
        {
            UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(),
            Content = line.Trim(),
        }).ToList();
    }
}
