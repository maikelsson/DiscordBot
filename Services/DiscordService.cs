using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Services
{
    public class DiscordService
    {
        private DiscordSocketClient _client;
        private ServiceProvider _services;
        private Lavalink _lavalink;
        private LoggingService _log;

        public async Task InitializeAsync()
        {
            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _lavalink = _services.GetRequiredService<Lavalink>();
            _log = _services.GetRequiredService<LoggingService>();

            HookEvents();

            await LogClient(_client, TokenType.Bot);
            await _client.StartAsync();
            await _services.GetRequiredService<CommandHandler>().InitializeAsync();  
            await Task.Delay(-1);
        }

        private void HookEvents()
        {
            _services.GetRequiredService<CommandService>().Log += _log.OnLog;
            _client.Log += _log.OnLog;
            _lavalink.Log += _log.OnLog;
            _client.Ready += OnReadyAsync;          
        }

        private async Task OnReadyAsync()
        {
            try
            {
                var node = await _lavalink.AddNodeAsync(_client);
                node.TrackFinished += _services.GetService<AudioService>().OnFinished;
                await _client.SetGameAsync("Music!");
            }

            catch
            {
                await EmbedHandler.CreateBasicEmbed("Error", "Wasn't able to add node :(");
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<Lavalink>()
                .AddSingleton<AudioService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        private async Task LogClient(DiscordSocketClient client, TokenType type)
        {
            client = _client;
            //Hide the token pls
            await client.LoginAsync(type, "NTUyNzYxNjIwNTUyNjEzODg4.XQs-Zg.i9hZLYFKVLHeJK_k0L-ii3eaAIU");
        }
    }
}