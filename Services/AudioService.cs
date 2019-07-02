using System;
using System.Collections.Concurrent;
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

        public async Task<Embed> JoinChannelAsync(SocketGuildUser user, IChannel channel, ulong id)
        {
            if(user.VoiceChannel == null)
            {
                return await EmbedHandler.CreateErrorEmbed("Error", $"You must be in a voicechannel sir {user.Nickname}!");
            }

            await _lavalink.DefaultNode.ConnectAsync(user.VoiceChannel);
            Options.TryAdd(user.Guild.Id, new AudioOptions
            {
                Summoner = user
            });                

            return await EmbedHandler.CreateBasicEmbed("Succes", $"Bot joined channel {user.VoiceChannel.Name}!");
        }

        public async Task<Embed> LeaveChannelAsync(ulong guildID)
        {
            try
            {
                var player = _lavalink.DefaultNode.GetPlayer(guildID);

                if(player.IsPlaying)
                {
                    await player.StopAsync();
                }

                //Leave voice channel
                var channelName = player.VoiceChannel.Name;
                await _lavalink.DefaultNode.DisconnectAsync(guildID);
                return await EmbedHandler.CreateBasicEmbed($"Leaving channel: {channelName}", "Invite me again sometime :)");
            }
            
            catch(InvalidOperationException ex)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Leave", ex.ToString());
            }
        }

        /* public async Task<Embed> PlaySongAsync(SocketGuildUser user, string query = null)
        {
            var player = _lavalink.DefaultNode.GetPlayer(user.Guild.Id);

            LavaTrack track;
            var search = await _lavalink.DefaultNode.SearchYouTubeAsync(query);
            track = search.Tracks.FirstOrDefault();
            await player.PlayAsync(track);

        }*/
    }
}