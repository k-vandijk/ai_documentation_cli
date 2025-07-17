using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace ai_documentation_cli.Application.Operations;

/// <summary>
/// A utility class providing operations for listing relevant files in a directory based on specified extensions.
/// </summary>
public static class DirectoryOperations
{
    /// <summary>
    /// Retrieves and returns a list of relevant files from the specified directory path based on the given list of file extensions.
    /// </summary>
    /// <param name="path">The directory path from which to retrieve the files.</param>
    /// <param name="extensions">A list of file extensions to filter the relevant files.</param>
    /// <returns>A sorted list of paths to the relevant files filtered by the provided file extensions.</returns>
    public static List<string> ListRelevantFiles(string path, List<string> extensions)
    {
        var matcher = DocumentignoreOperations.BuildMatcher(path);
        var dirInfo = new DirectoryInfo(path);
        var result = matcher.Execute(new DirectoryInfoWrapper(dirInfo));

        var matched = result.Files
            .Select(f => f.Path)
            .Where(p => extensions
                .Contains(Path.GetExtension(p)
                    .ToLowerInvariant()));

        return matched.OrderBy(m => m).ToList();
    }
}
