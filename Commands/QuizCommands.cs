using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Quiztellation.Quiz.Handlers;
using Quiztellation.Quiz.Questions;
using Quiztellation.Data;

namespace Quiztellation.Commands;

public class QuizCommands : BaseCommandModule
{
    [Command("quizstar")]
    [Aliases(new[] { "qs" })]
    [Description("Guess Name of Stars from Bayer Designations")]
    public async Task QuizStar(CommandContext ctx,
            [Description("easy/medium/hard/pitt")] string level = "easy")
    // [Description("Choose Type of Quiz")] string quizType = "star")
    {
        var starsFiltered = new List<Star>();
        switch (level)
        {
            case "easy":
                starsFiltered.AddRange(await StarData.GetStarsByLevel(1, 10));
                break;
            case "medium":
                foreach (List<Star> result in await Task.WhenAll(StarData.GetStarsByLevel(1, 5), StarData.GetStarsByLevel(2, 5)))
                {
                    starsFiltered.AddRange(result);
                }
                break;
            case "hard":
                foreach (List<Star> result in await Task.WhenAll(StarData.GetStarsByLevel(1, 3), StarData.GetStarsByLevel(2, 4), StarData.GetStarsByLevel(3, 3)))
                {
                    starsFiltered.AddRange(result);
                }
                break;
            case "pitt":
                foreach (List<Star> result in await Task.WhenAll(StarData.GetStarsByLevel(2, 3), StarData.GetStarsByLevel(3, 4), StarData.GetStarsByLevel(4, 3)))
                {
                    starsFiltered.AddRange(result);
                }
                break;
            default:
                var errorEmbed = new DiscordEmbedBuilder
                {
                    Title = "Invalid Input",
                    Description = "Please enter quiz easy/medium/hard/pitt",
                    Color = DiscordColor.Red
                };
                await ctx.Channel.SendMessageAsync(errorEmbed);
                return;
        }

        starsFiltered = starsFiltered.OrderBy(i => Guid.NewGuid()).ToList();
        var quizHandler = new QuizHandler(ctx.Client, ctx.Channel, ctx.User);
        foreach (var star in starsFiltered)
            quizHandler.AddQuestion(new StarQuestion($"{star.Bayer} {star.Con}", star.Names, star.Level));

        await quizHandler.ProcessQuiz();
    }
}