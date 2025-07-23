using ai_documentation_cli.Commands;
using Cocona;
using kvandijk.Common.Extensions;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddChatCompletions();

var app = builder.Build();

app.AddCommands<GenerateCommands>();
app.AddCommands<ListCommands>();

await app.RunAsync();