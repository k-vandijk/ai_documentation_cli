using Microsoft.Extensions.FileSystemGlobbing;

namespace ai_documentation_cli.Application.Operations;

/// <summary>
/// Provides functionality to build a matcher based on document ignore patterns specified in a '.documentignore' file.
/// </summary>
public static class DocumentignoreOperations
{
    /// <summary>
    /// Build a matcher instance based on the list of document ignore patterns for a specified root path.
    /// </summary>
    /// <param name="rootPath">The root path directory where the document ignore patterns should be applied.</param>
    /// <returns>A constructed Matcher object configured with include and exclude patterns for files and directories.</returns>
    public static Matcher BuildMatcher(string rootPath)
    {
        var patterns = ReadDocumentIgnorePatterns(rootPath);
        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude("**/*");

        foreach (var pattern in patterns)
        {
            if (pattern.StartsWith("!"))
            {
                matcher.AddInclude(pattern.Substring(1));
            }
            else
            {
                matcher.AddExclude(pattern);
            }
        }

        return matcher;
    }

    private static List<string> ReadDocumentIgnorePatterns(string path)
    {
        var ignoreFile = Path.Combine(path, ".documentignore");

        if (!File.Exists(ignoreFile))
        {
            return new List<string>();
        }

        return File.ReadAllLines(ignoreFile)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0 && !line.StartsWith("#"))
            .ToList();
    }
}
