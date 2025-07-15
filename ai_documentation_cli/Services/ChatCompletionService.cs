using System.ClientModel;
using ai_documentation_cli.Exceptions;
using ai_documentation_cli.Interfaces;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace ai_documentation_cli.Services;

public class ChatCompletionService : IChatCompletionService
{
    private readonly ChatClient _chatClient;

    public ChatCompletionService(ILogger<ChatCompletionService> logger)
    {
        var apiUrl = Environment.GetEnvironmentVariable("AZURE_OPENAI_URL") ?? throw new InvalidOperationException("AZURE_OPENAI_URL not set in environment variables");
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY") ?? throw new InvalidOperationException("AZURE_OPENAI_KEY not set in environment variables");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_DEPLOYMENT_NAME") ?? throw new InvalidOperationException("AZURE_DEPLOYMENT_NAME not set in environment variables");

        var azureClient = new AzureOpenAIClient(new Uri(apiUrl), new ApiKeyCredential(apiKey));
        _chatClient = azureClient.GetChatClient(deploymentName);
    }

    public async Task<string> GetChatCompletionAsync(string prompt, string instruction)
    {
        var systemMessage = new SystemChatMessage(instruction);
        var userMessage = new UserChatMessage(prompt);
        var messages = new List<ChatMessage> { systemMessage, userMessage };

        ChatCompletion completion;
        try
        {
            completion = await _chatClient.CompleteChatAsync(messages);
        }
        catch (Exception ex)
        {
            throw new ChatCompletionException("Failed to get chat completion.", ex);
        }

        var response = completion.Content.FirstOrDefault()?.Text ?? string.Empty;

        return response;
    }
}