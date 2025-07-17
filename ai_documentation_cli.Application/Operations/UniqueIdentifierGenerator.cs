namespace ai_documentation_cli.Application.Operations;

/// <summary>
/// Provides functionality to generate short unique identifiers.
/// </summary>
public static class UniqueIdentifierGenerator
{
    /// <summary>
    /// Generates a short unique identifier.
    /// </summary>
    /// <returns>A string representing a unique identifier with a length of 8 characters.</returns>
    public static string GenerateShortUniqueIdentifier()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}
