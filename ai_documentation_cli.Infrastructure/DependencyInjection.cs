using ai_documentation_cli.Application.Interfaces;
using ai_documentation_cli.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ai_documentation_cli.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IChatCompletionService, ChatCompletionService>();

        return services;
    }
}