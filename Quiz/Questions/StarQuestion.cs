using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace Quiztellation.Quiz.Questions;

public class StarQuestion : IQuestion
{
    private readonly List<string> _answer;
    private readonly string _question;

    public StarQuestion(string question, List<string> answer, int point)
    {
        _question = question;
        _answer = answer;
        Point = point;
    }

    public int Point { get; private set; }

    public async Task<bool> ProcessQuestion(DiscordClient client, DiscordChannel channel, DiscordUser user)
    {
        var questionEmbed = new DiscordEmbedBuilder
        {
            Title = _question,
            Color = DiscordColor.Azure
        };
        questionEmbed.AddField("Points", $"{Point}");
        questionEmbed.WithFooter("type \"exit\" to end the quiz");
        await channel.SendMessageAsync(questionEmbed);

        var interactivity = client.GetInteractivity();

        while (true)
        {
            var message =
                await interactivity.WaitForMessageAsync(x => x.ChannelId == channel.Id && x.Author.Id == user.Id);

            if (message.TimedOut)
            {
                var timedOutEmbed = new DiscordEmbedBuilder
                {
                    Title = "No interactivity",
                    Color = DiscordColor.Red
                };
                await channel.SendMessageAsync(timedOutEmbed);
                return true;
            }

            var ansStr = "";
            var ansCap = _answer.Select(x => x.ToUpper()).ToList();
            foreach (var ans in _answer) ansStr = ansStr + ans + "/";
            ansStr = ansStr.Remove(ansStr.Length - 1, 1);

            if (string.Equals(message.Result.Content, "exit", StringComparison.CurrentCultureIgnoreCase)) return true;

            if (ansCap.Contains(message.Result.Content.ToUpper()))
            {
                var correctEmbed = new DiscordEmbedBuilder
                {
                    Title = "Correct!",
                    Color = DiscordColor.Green
                };
                correctEmbed.AddField("Answer", $"{ansStr}");
                await channel.SendMessageAsync(correctEmbed);
                return false;
            }

            var incorrectEmbed = new DiscordEmbedBuilder
            {
                Title = "Incorrect!",
                Color = DiscordColor.Red
            };
            incorrectEmbed.AddField("Answer", $"{ansStr}");
            await channel.SendMessageAsync(incorrectEmbed);
            Point = 0;
            return false;
        }
    }
}