using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Quiztellation.Data;

namespace Quiztellation.Commands;

public class LearnCommands : BaseCommandModule
{
    [Command("learnstar")]
    [Aliases(new[] { "ls" })]
    [Description("Learn Stars in each Constellation")]
    public async Task QuizStar(CommandContext ctx,
            [Description("Enter Abbreviation of Constellation")] string con = "And")
    // [Description("Choose Type of Quiz")] string quizType = "star")
    {
        var starsInCon = await StarData.GetStarsByCon(con);
        var starsInConEmbed = new DiscordEmbedBuilder
        {
            Title = con,
            Description = "This constellation has the following stars",
            Color = DiscordColor.Azure
        };
        foreach (Star star in starsInCon)
        {
            var namesStr = "";
            foreach (var ans in star.Names) namesStr = namesStr + ans + "/";
            namesStr = namesStr.Remove(namesStr.Length - 1, 1);
            starsInConEmbed.AddField($"{star.Bayer} {star.Con}", $"{namesStr}");
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