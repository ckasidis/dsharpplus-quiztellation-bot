using System.Net.Http.Headers;
using System.Text.Json;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Quiztellation.Data;
namespace Quiztellation.Commands;

public class LearnCommands : BaseCommandModule
{
    private static async Task<List<Star>> GetStarsByCon(string con)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri($"https://api.airtable.com/v0/{Environment.GetEnvironmentVariable("ASTRODB_ID")}/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("AIRTABLE_KEY"));
        var streamTask = client.GetStreamAsync($"Stars?fields%5B%5D=names&fields%5B%5D=bayer&fields%5B%5D=level" + // Project Names, Bayer, Level
                                               $"&filterByFormula=OR(%0ALOWER(CONCATENATE(%7BconAbbreviation%7D%2C%22%22))%3D%22{con.ToLower()}%22%2C" + // User Inputs Abbreviation
                                               $"%0ALOWER(CONCATENATE(%7BconName%7D%2C%22%22))%3D%22{con.ToLower()}%22%2C" + // User Inputs Name 
                                               $"%0ALOWER(CONCATENATE(%7BconGenitive%7D%2C%22%22))%3D%22{con.ToLower()}%22%0A)"); // User Inputs Genitive
        var stars = JsonSerializer.Deserialize<Stars>(await streamTask);
        return stars.records;
    }

    [Command("learnstar")]
    [Aliases(new[] { "ls" })]
    [Description("Learn Stars in each Constellation")]
    public async Task LearnStar(CommandContext ctx,
            [Description("Enter Abbreviation of Constellation")] string con = "and")
    // [Description("Choose Type of Quiz")] string quizType = "star")
    {
        var starsInCon = await GetStarsByCon(con);
        var starsInConEmbed = new DiscordEmbedBuilder
        {
            Title = $"Result for constellation \"{con}\"",
            Description = "This constellation has the following stars",
            Color = DiscordColor.Azure
        };

        foreach (Star star in starsInCon)
        {
            starsInConEmbed.AddField(star.Fields.Bayer, $"{star.Fields.Names}");
        }
        await ctx.Channel.SendMessageAsync(starsInConEmbed);

        // var errorEmbed = new DiscordEmbedBuilder
        // {
        //     Title = "Please Enter a Valid Constellation",
        //     Description = "e.g. UMa, CVn, Ori",
        //     Color = DiscordColor.Red
        // };
        // await ctx.Channel.SendMessageAsync(errorEmbed);
    }
}