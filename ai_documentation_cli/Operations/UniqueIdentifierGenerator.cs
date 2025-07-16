namespace ai_documentation_cli.Operations;

public static class UniqueIdentifierGenerator
{
    public static string GenerateShortUniqueIdentifier()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}