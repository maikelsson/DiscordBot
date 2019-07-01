using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Data;
using DiscordBot.Handlers;
using Victoria;

namespace DiscordBot.Services
{
    public class AudioService
    {
        private Lavalink _lavalink;

        private readonly Lazy<ConcurrentDictionary<ulong, AudioOptions>> _lazyOptions
            = new Lazy<ConcurrentDictionary<ulong, AudioOptions>>();

        private ConcurrentDictionary<ulong, AudioOptions> Options
            => _lazyOptions.Value;

        public AudioService(Lavalink lavalink)
        {
            _lavalink = lavalink;
            _lavalink.Log += OnLog;
        }

        private Task OnLog(LogMessage message)
        {
            Console.WriteLine($"{message}");
            return Task.CompletedTask;
        }

        public async Task<Embed> JoinOrPlayAsync(SocketGuildUser user, string query = null)
        {
            if(user.VoiceChannel == null)
            {
                return await EmbedHandler.CreateErrorEmbed();
            }

            if(query == null)
            {
                await _lavalink.DefaultNode.ConnectAsync(user.VoiceChannel);
                Options.TryAdd(user.Guild.Id, new AudioOptions
                {
                    Summoner = user
                });

                
            }

            return await EmbedHandler.CreateBasicEmbed("Succes", $"Bot joined channel {user.VoiceChannel.Name}!");
        }
    }
}