using System.Threading.Tasks;
using Discord;

namespace DiscordBot.Handlers
{
    public static class EmbedHandler
    {
        public static async Task<Embed> CreateBasicEmbed(string title, string description)
        {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title))
                .WithDescription(description)
                .WithColor(Color.Green)
                .WithCurrentTimestamp().Build());

            return embed;
        }

        public static async Task<Embed> CreateErrorEmbed(string title = "Error!", string desc = "Jotain meni pieleen :(")
        {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(desc)
                .WithColor(Color.DarkRed)
                .WithCurrentTimestamp().Build()));

            return embed;
        }
    }
}