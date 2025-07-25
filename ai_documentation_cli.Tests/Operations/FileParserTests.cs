using ai_documentation_cli.Application.Operations;
using Xunit;

namespace ai_documentation_cli.Unittests.Operations;

public class FileParserTests
{
    [Fact]
    public void GetFileLines_ReturnsLineObjects_WhenFileExists()
    {
        // Arrange
        var tempPath = Path.GetTempFileName();
        var testLines = new[] { "First line", "Second line" };
        File.WriteAllLines(tempPath, testLines);

        // Act
        var result = FileParser.GetFileLines(tempPath);

        // Assert
        Assert.Equal(testLines.Length, result.Count);
        for (int i = 0; i < testLines.Length; i++)
        {
            Assert.Equal(testLines[i], result[i].Content);
            Assert.False(string.IsNullOrWhiteSpace(result[i].UniqueIdentifier));
        }

        // Cleanup
        File.Delete(tempPath);
    }

    [Fact]
    public void GetFileLines_ThrowsFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var fakePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");

        // Act + Assert
        var ex = Assert.Throws<FileNotFoundException>(() =>
            FileParser.GetFileLines(fakePath)
        );
        Assert.Contains("does not exist", ex.Message);
    }

    // TODO: Test ParseClasses and ParseFunctions methods (inlcuding ParseGeneric)


}