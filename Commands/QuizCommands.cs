using System.Net.Http.Headers;
using System.Text.Json;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Quiztellation.Quiz.Handlers;
using Quiztellation.Quiz.Questions;
using Quiztellation.Data;
namespace Quiztellation.Commands;

public class QuizCommands : BaseCommandModule
{
    private static async Task<List<Star>> GetStarsByLevel(int level, int count)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri($"https://api.airtable.com/v0/{Environment.GetEnvironmentVariable("ASTRODB_ID")}/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("AIRTABLE_KEY"));
        var streamTask = client.GetStreamAsync($"Stars?fields%5B%5D=names&fields%5B%5D=bayer&fields%5B%5D=level&filterByFormula=%7Blevel%7D%3D{level}");
        var stars = JsonSerializer.Deserialize<Stars>(await streamTask);
        return stars.records.OrderBy(i => Guid.NewGuid()).Take(count).ToList();
    }
    
    [Command("quizstar")]
    [Aliases(new[] {"qs"})]
    [Description("Guess Name of Stars from Bayer Designations")]
    public async Task QuizStar(CommandContext ctx,
            [Description("easy/medium/hard/pitt")] string level = "easy")
    // [Description("Choose Type of Quiz")] string quizType = "star")
    {
        var starsFiltered = new List<Star>();
        switch (level)
        {
            case "easy":
                starsFiltered.AddRange(await GetStarsByLevel(1,10));
                break;
            case "medium":
                foreach (List<Star> result in await Task.WhenAll(GetStarsByLevel(1,5), GetStarsByLevel(2,5)))
                {
                    starsFiltered.AddRange(result);
                }
                break;
            case "hard":
                foreach (List<Star> result in await Task.WhenAll(GetStarsByLevel(1,3), GetStarsByLevel(2,4), GetStarsByLevel(3,3)))
                {
                    starsFiltered.AddRange(result);
                }
                break;
            case "pitt":
                foreach (List<Star> result in await Task.WhenAll(GetStarsByLevel(2,3), GetStarsByLevel(3,4), GetStarsByLevel(4,3)))
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
            quizHandler.AddQuestion(new StarQuestion(star.Fields.Bayer, star.Fields.Names, star.Fields.Level));

        await quizHandler.ProcessQuiz();
    }
}