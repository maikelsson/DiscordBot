using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Data;
using DiscordBot.Handlers;
using Victoria;
using Victoria.Entities;

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
        }

        public async Task<Embed> JoinChannelAsync(SocketGuildUser user, string query = null)
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

            var player = _lavalink.DefaultNode.GetPlayer(user.Guild.Id);

            LavaTrack track;
            var search = await _lavalink.DefaultNode.SearchYouTubeAsync(query);
            track = search.Tracks.FirstOrDefault();
            await player.PlayAsync(track);

            return await EmbedHandler.CreateBasicEmbed("Succes", $"Bot joined channel {user.VoiceChannel.Name}!");
        }

        public async Task<Embed> LeaveChannelAsync(SocketGuildUser user)
        {
            var node = _lavalink.DefaultNode;
            if(!node.IsConnected || user.VoiceChannel != null)
            {
                return await EmbedHandler.CreateErrorEmbed();
            }

            await node.DisconnectAsync(user.Guild.Id);

            return await EmbedHandler.CreateBasicEmbed("Leaving..", "Bot has left the channel..");
        }
    }
}