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

        //To keep track of the current guildID
        public ulong currentGuild { get; private set; }

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

            currentGuild = id;

            await LoggingService.LogInformationAsync("Node", $"Bot connected to voice channel: {user.VoiceChannel.Name} + {currentGuild}");
            
            return await EmbedHandler.CreateBasicEmbed("Success", $"AmarilloBot joined channel {user.VoiceChannel.Name}!");
        }

        public async Task<Embed> LeaveChannelAsync(ulong guildID)
        {
            try
            {
                var player = _lavalink.DefaultNode.GetPlayer(guildID);

                if(player.IsPlaying || player.IsPaused)
                    await player.StopAsync();

                //Leave voice channel
                var channelName = player.VoiceChannel.Name;
                await _lavalink.DefaultNode.DisconnectAsync(guildID);
                await LoggingService.LogInformationAsync("Node", $"Bot disconnected from voice channel: {player.VoiceChannel.Name} + {currentGuild}");
                return await EmbedHandler.CreateBasicEmbed($"Leaving channel: {channelName}", "Invite me again sometime :)");
            }

            // If something goes wrong, gives error info
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Leave", ex.ToString());
            }

        }

         public async Task<Embed> PlaySongAsync(SocketGuildUser user, string query)
        {

            if(user.VoiceChannel == null)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Play", "You must be in voice channel in order to use this command!");
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
                        return await EmbedHandler.CreateErrorEmbed("Sad face", $"Couldn't find anything in youtube that matches the {query}...");
                    }
                    
                    track = search.Tracks.FirstOrDefault();

                    if(player.CurrentTrack != null && player.IsPlaying || player.IsPaused)
                    {
                        player.Queue.Enqueue(track);
                        return await EmbedHandler.CreateBasicEmbed("Music, Play", $"Added song {track.Title} to queue \nPosition: {player.Queue.Count}\nDuration: {track.Length}");
                    }

                    // Was not playing anything, so we play requested track
                    await player.PlayAsync(track);
                    await player.SetVolumeAsync(100);
                    await LoggingService.LogInformationAsync("Node", $"Now playing {track.Title}, {track.Uri}");
                    return await EmbedHandler.CreateBasicEmbed("Music, Play", $"Now Playing: {track.Title} \nDuration: {track.Length}");

                }   

                catch(Exception ex)
                {
                    return await EmbedHandler.CreateErrorEmbed("Music, Play", $"{ex.ToString()}");
                }
            }
          
        }

        public async Task<Embed> PauseOrContinueSongAsync(SocketGuildUser user)
        {

            if (user.VoiceChannel == null)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Pause", "Must be in voice channel in order to use this command!");
            }

            else
            {
                try
                {
                    var player = _lavalink.DefaultNode.GetPlayer(user.Guild.Id);

                    //Continues song
                    if (player.IsPaused)
                    {
                        await player.PauseAsync();
                        return await EmbedHandler.CreateBasicEmbed("Music, Pause", $"Continued: {player.CurrentTrack.Title}\nPosition: {player.CurrentTrack.Position.ToString(@"hh\:mm\:ss")}");
                    }

                    //Pause song
                    await player.PauseAsync();

                    return await EmbedHandler.CreateBasicEmbed("Music, Pause", $"Paused: {player.CurrentTrack.Title}\nPosition: {player.CurrentTrack.Position.ToString(@"hh\:mm\:ss")}");
                }

                catch(Exception ex)
                {
                    return await EmbedHandler.CreateErrorEmbed("Music, Pause", $"{ex.ToString()}");
                }
            }
        }

        //show next song
        public async Task<Embed> SkipSongAsync(ulong guildId)
        {
            try
            {
                var player = _lavalink.DefaultNode.GetPlayer(guildId);

                if(player == null)
                {
                    return await EmbedHandler.CreateErrorEmbed("Music, List", "Could not aqcuire player. Are you using the bot right now?");
                }

                if(player.Queue.Count < 1)
                {
                    return await EmbedHandler.CreateErrorEmbed("Music, List", "Unable to skip a track as there is none in queue");
                }

                else
                {
                    try
                    {
                        var currentTrack = player.CurrentTrack;
                        var nextTrack = player.Queue.Items.ElementAt(0);

                        //Since SkipAsync() isn't working, this seems to be only option to skip track in version 3.0..
                        await player.SeekAsync(currentTrack.Length);
                        return await EmbedHandler.CreateBasicEmbed("Music, Skip", $"Succesfully skipped {currentTrack.Title},\nNext track: {nextTrack.Title}\nDuration: {nextTrack.Length}");
                    }
                    catch(Exception ex)
                    {
                        return await EmbedHandler.CreateErrorEmbed("Error, Skip", $"{ex.ToString()}");
                    }
                }
            }
            catch(Exception ex)
            {
                return await EmbedHandler.CreateErrorEmbed("Error, Skip", $"{ex.ToString()}");
            }
        }

        public async Task OnFinished(LavaPlayer player, LavaTrack track, TrackReason reason)
        {
            if(reason is TrackReason.LoadFailed || reason is TrackReason.Cleanup)
            {
                return;
            }

            player.Queue.TryDequeue(out LavaTrack nextTrack);

            if(nextTrack == null)
            {
                await player.StopAsync();
                await player.TextChannel.SendMessageAsync("", false, await EmbedHandler.CreateBasicEmbed("Music", $"Paused, no songs in queue"));
            }

            else
            {
                await player.PlayAsync(nextTrack);
                await player.TextChannel.SendMessageAsync("", false, await EmbedHandler.CreateBasicEmbed("Now Playing", $"{nextTrack.Title}"));
            }
        }

        public async Task OnUpdated(LavaPlayer player, LavaTrack track, TimeSpan timeSpan)
        {
            await PeriodicCheckAsync(TimeSpan.FromSeconds(5));
            await LoggingService.LogInformationAsync("OnUpdated", $"We here + Last time updated: {player.LastUpdate} + {DateTime.Now}");
        }

        //Can be used to run methods by defined interval
        public Task PeriodicCheckAsync(TimeSpan interval)
        {
            while (true)
            {
                CheckForPlayerLastUpdate();
                Task.Delay(interval);
            }
        }

        private async Task CheckForPlayerLastUpdate()
        {
            var player = _lavalink.DefaultNode.GetPlayer(currentGuild);

            if (player == null)
            {
                LoggingService.LogCriticalAsync("CheckUpdate method", "player == null");
            }


            else
            {

                TimeSpan timeSpan = DateTime.Now.Subtract(player.LastUpdate.Date);

                LoggingService.LogInformationAsync("timespan", $"timespan: {timeSpan}");

                if (timeSpan < TimeSpan.FromMinutes(1))
                {
                    _lavalink.DefaultNode.DisconnectAsync(currentGuild);
                }
            }

        }

        //Blueprint for something useful maybe..
        public async Task OnUserConnectedOrDisconnected(LavaPlayer _player)
        {
            var player = _player;
            if(player.VoiceChannel == null)
            {
                return;
            }

            else
            {
                try
                {
                    await player.StopAsync();
                }
                catch(Exception ex)
                {
                    await LoggingService.LogCriticalAsync("LavaPlayer", "Something went wrong.. OnUserConnectedOrDisconnected", ex);
                }
            }
        }

    }
}