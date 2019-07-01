using System.Drawing;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Handlers;
using Victoria;

namespace DiscordBot.Services
{
    public class AudioService
    {
        private Lavalink _lavalink;

        public AudioService(Lavalink lavalink)
        {
            _lavalink = lavalink;
        }

        public async Task<Embed> JoinOrPlayAsync(SocketGuildUser user)
        {
            if(user.VoiceChannel == null)
            {
                return await EmbedHandler.CreateErrorEmbed();
            }

            return await EmbedHandler.CreateBasicEmbed("Succes", $"Bot joined channel {user.VoiceChannel.Name}!");
        }
    }
}