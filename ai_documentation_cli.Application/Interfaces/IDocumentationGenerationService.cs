namespace ai_documentation_cli.Application.Interfaces;

public interface IDocumentationGenerationService
{
    Task HandleDocumentationGenerationForFile(string file);
}