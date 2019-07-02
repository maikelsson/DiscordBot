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
using Victoria.Entities.Enums;

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
            
            // If something goes wrong, gives error info
            catch(InvalidOperationException ex)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Leave", ex.ToString());
            }
        }

         public async Task<Embed> PlaySongAsync(SocketGuildUser user, string query = "Darude Sandstorm")
        {

            if(user.VoiceChannel == null)
            {
                return await EmbedHandler.CreateErrorEmbed("Play, Music", "You must be in voice channel in order to use this command!");
            }

            else
            {
                try
                {
                    var player = _lavalink.DefaultNode.GetPlayer(user.Guild.Id);
                    LavaTrack track;
                    var search = await _lavalink.DefaultNode.SearchYouTubeAsync(query);

                    if(search.LoadResultType == LoadResultType.NoMatches)
                    {
                        return await EmbedHandler.CreateErrorEmbed("Oh shieet", $"Couldn't find anything in youtube that matches the {query}...");
                    }
                    
                    track = search.Tracks.FirstOrDefault();
                    await player.PlayAsync(track);
                    await player.SetVolumeAsync(100);


                    return await EmbedHandler.CreateBasicEmbed("Music", $"Now Playing: {track.Title}!");
                }   

                catch(Exception ex)
                {
                    return await EmbedHandler.CreateErrorEmbed("Music, Play", $"{ex.ToString()}");
                }
            }
            

        }
    }
}