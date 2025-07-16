using ai_documentation_cli.Domain.Models;

namespace ai_documentation_cli.Application.Operations;

/// <summary>
/// This static class provides methods for inserting lines at a specific point in a list of lines and splitting a summary into separate lines.
/// </summary>
public static class FileInserter
{
    /// <summary>
    /// Inserts a list of lines at a specific line identified by the given lineId in another list of lines.
    /// </summary>
    /// <param name="lineId">The unique identifier of the line where the new lines will be inserted.</param>
    /// <param name="linesToInsert">The list of lines to be inserted at the specified line.</param>
    /// <param name="lines">The original list of lines where the new lines will be inserted.</param>
    /// <returns>A new list of lines with the lines inserted at the specified lineId within each line's indentation.</returns>
    public static List<Line> InsertLinesAt(string lineId, List<Line> linesToInsert, List<Line> lines)
    {
        if (linesToInsert == null || linesToInsert.Count == 0)
        {
            throw new ArgumentException("The lines to insert cannot be null or empty.");
        }

        var result = new List<Line>();
        foreach (var line in lines)
        {
            if (line.UniqueIdentifier == lineId)
            {
                var indentation = new string(' ', line.Content.TakeWhile(char.IsWhiteSpace).Count());

                foreach (var insertLine in linesToInsert)
                {
                    result.Add(new Line
                    {
                        UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(),
                        Content = indentation + insertLine.Content.TrimStart(),
                    });
                }
            }

            result.Add(line);
        }

        return result;
    }

    // TODO: This function should be moved to a separate class, as it is not related to file insertion.

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
