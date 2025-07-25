using ai_documentation_cli.Application.Operations;
using ai_documentation_cli.Domain.Models;

namespace ai_documentation_cli.Unittests.Operations;

public class FileInserterTests
{
    [Fact]
    public void InsertLinesAt_ReturnsNewListOfLinesWithLinesInsertedAtCorrectIndex()
    {
        RunInsertLinesTest(
            "InsertLinesAt_ReturnsNewListOfLinesWithLinesInsertedAtCorrectIndex"
        );
    }

    [Fact]
    public void InsertLinesAt_ReturnsNewListOfLinesWithLinesInsertedAtCorrectIndex_WhenFunctionHasAttributes()
    {
        RunInsertLinesTest(
            "InsertLinesAt_ReturnsNewListOfLinesWithLinesInsertedAtCorrectIndex_WhenFunctionHasAttributes"
        );
    }

    private void RunInsertLinesTest(string testCaseName)
    {
        // Arrange
        var inputLines = LoadLinesFromFixture($"{testCaseName}.input.fix");
        var expectedLines = LoadLinesFromFixture($"{testCaseName}.expected.fix");

        var inputSummaryLines = BuildSummaryLines();

        var index = inputLines.First(line => 
            line.Content.Contains("public static List<Line> InsertLinesAt(string lineId, List<Line> linesToInsert, List<Line> lines)")
        ).UniqueIdentifier;

        // Act
        var result = FileInserter.InsertLinesAt(index, inputSummaryLines, inputLines);

        // Assert
        Assert.Equal(expectedLines.Count, result.Count);
        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i].Content, result[i].Content);
            Assert.NotNull(result[i].UniqueIdentifier);
        }
    }

    private List<Line> LoadLinesFromFixture(string filename)
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", filename);
        var rawLines = File.ReadLines(fixturePath);

        return rawLines.Select(line => new Line
        {
            UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(),
            Content = line
        }).ToList();
    }

    private List<Line> BuildSummaryLines() => new()
    {
        new() { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "/// <summary>" },
        new() { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "/// Inserts a list of lines (typically XML documentation) before the declaration line identified by the given lineId," },
        new() { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "/// ensuring the inserted lines appear above any attribute lines." },
        new() { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "/// </summary>" },
        new() { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "/// <param name=\"lineId\">The unique identifier of the target declaration line (e.g., a method or class).</param>" },
        new() { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "/// <param name=\"linesToInsert\">The lines to insert (e.g., XML documentation).</param>" },
        new() { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "/// <param name=\"lines\">The full list of lines where insertion will happen.</param>" },
        new() { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "/// <returns>A new list of lines with the inserted lines positioned above any attributes.</returns>" },
    };
    
    [Fact]
    public void SplitSummaryIntoLines_ReturnsCorrectListOfLines()
    {
        // Arrange
        var input = """"
            This is a summary.
            It has multiple lines.
        """";
        
        var expectedLines = new List<Line>
        {
            new Line { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "This is a summary." },
            new Line { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = "It has multiple lines." }
        };

        // Act
        var result = FileInserter.SplitSummaryIntoLines(input);

        // Assert
        Assert.Equal(expectedLines.Count, result.Count);
        for (int i = 0; i < expectedLines.Count; i++)
        {
            Assert.Equal(expectedLines[i].Content, result[i].Content);
            Assert.NotNull(result[i].UniqueIdentifier);
        }
    }
}