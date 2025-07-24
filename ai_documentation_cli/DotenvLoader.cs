namespace ai_documentation_cli;

/// <summary>
/// Provides functionality to load environment variables from a .env file located in the current or parent directories.
/// </summary>
public static class DotenvLoader
{
    /// <summary>
    /// Loads environment variables from a specified file or the closest ancestor file named ".env".
    /// </summary>
    /// <param name="fileName">The name of the file to load the environment variables from. Defaults to ".env" if not provided.</param>
    /// <param name="maxLevels">The maximum number of levels to search upwards for the file. Defaults to 3 if not provided.</param>
    public static void Load(string? fileName = null, int maxLevels = 3)
    {
        fileName ??= ".env";
        var filePath = FindFileUpwards(fileName, maxLevels);

        if (filePath == null)
        {
            throw new FileNotFoundException(".env file not found in any parent directory.");
        }

        SetEnvironmentVariables(filePath);
    }

    private static string? FindFileUpwards(string fileName, int maxLevels)
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        int level = 0;

        while (dir != null && level < maxLevels)
        {
            var fullPath = Path.Combine(dir.FullName, fileName);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            dir = dir.Parent;
            level++;
        }

        return null;
    }

    private static void SetEnvironmentVariables(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            var parts = line.Split("=", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                continue;
            }

            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }
}
