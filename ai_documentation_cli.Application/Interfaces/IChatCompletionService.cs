namespace ai_documentation_cli.Application.Interfaces;

public interface IChatCompletionService
{
    Task<string> GetChatCompletionAsync(string prompt, string instruction);
}