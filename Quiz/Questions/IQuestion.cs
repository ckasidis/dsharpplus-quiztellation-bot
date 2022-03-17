using DSharpPlus;
using DSharpPlus.Entities;

namespace Quiztellation.Quiz.Questions;

public interface IQuestion
{
    int Point { get; }

    Task<bool> ProcessQuestion(DiscordClient client, DiscordChannel channel, DiscordUser user);
}