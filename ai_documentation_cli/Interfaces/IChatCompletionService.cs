namespace ai_documentation_cli.Interfaces;

public interface IChatCompletionService
{
    Task<string> GetChatCompletionAsync(string prompt, string instruction);
}