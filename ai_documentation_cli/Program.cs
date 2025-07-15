using ai_documentation_cli.Commands;
using ai_documentation_cli.Interfaces;
using ai_documentation_cli.Services;
using Cocona;
using kvandijk.Common.Utils;
using Microsoft.Extensions.DependencyInjection;

DotenvLoader.Load(".env");

var builder = CoconaApp.CreateBuilder();

builder.Services.AddSingleton<IChatCompletionService, ChatCompletionService>();

var app = builder.Build();

app.AddCommands<GenerateCommands>();
app.AddCommands<ListCommands>();

await app.RunAsync();