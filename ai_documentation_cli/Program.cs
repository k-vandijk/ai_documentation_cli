using ai_documentation_cli;
using ai_documentation_cli.Commands;
using ai_documentation_cli.Infrastructure;
using Cocona;

DotenvLoader.Load();

var builder = CoconaApp.CreateBuilder();

builder.Services.AddInfrastructure();

var app = builder.Build();

app.AddCommands<GenerateCommands>();
app.AddCommands<ListCommands>();

await app.RunAsync();
