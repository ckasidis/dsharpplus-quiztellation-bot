using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Quiztellation.Commands;

namespace Quiztellation;

public class Bot
{
    private readonly DiscordClient _client;
    private readonly CommandsNextExtension _commands;

    public Bot()
    {
        _client = new DiscordClient(new DiscordConfiguration
        {
            Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
            TokenType = TokenType.Bot,
            AutoReconnect = true
        });

        _commands = _client.UseCommandsNext(new CommandsNextConfiguration
        {
            StringPrefixes = new[] { Environment.GetEnvironmentVariable("BOT_PREFIX") },
            EnableMentionPrefix = true,
            EnableDms = false
        });
        _commands.RegisterCommands<QuizCommands>();
        _commands.RegisterCommands<LearnCommands>();

        _client.UseInteractivity(new InteractivityConfiguration
        {
            Timeout = TimeSpan.FromMinutes(2)
        });
    }

    public async Task RunAsync()
    {
        _client.Ready += onReady;

        await _client.ConnectAsync();
        await Task.Delay(-1);
    }

    private Task onReady(DiscordClient client, ReadyEventArgs e)
    {
        Console.WriteLine("Ready");
        return Task.CompletedTask;
    }
}