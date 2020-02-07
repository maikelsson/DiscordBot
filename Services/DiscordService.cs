using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;

namespace DiscordBot.Services
{
    public class DiscordService
    {
        private DiscordSocketClient _client;
        private ServiceProvider _services;
        private Lavalink _lavalink;

        public async Task InitializeAsync()
        {
            await LoggingService.WelcomeMessageAsync(); 

            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _lavalink = _services.GetRequiredService<Lavalink>();

            HookEvents();

            await LogClient(_client, TokenType.Bot);
            await _client.StartAsync();
            await _services.GetRequiredService<CommandHandler>().InitializeAsync();
            await Task.Delay(-1);
        }

        private void HookEvents()
        {
            _services.GetRequiredService<CommandService>().Log += LoggingService.OnLog;
            _client.Log += LoggingService.OnLog;
            _lavalink.Log += LoggingService.OnLog;
            _client.Ready += OnReadyAsync;
        }

        private async Task OnReadyAsync()
        {
            try
            {
                var node = await _lavalink.AddNodeAsync(_client);
                node.TrackFinished += _services.GetService<AudioService>().OnFinished;
                node.PlayerUpdated += _services.GetService<AudioService>().OnUpdated;
                await node.ConfigureResuming(true, TimeSpan.FromMinutes(5)); //needs experimenting
            }

            catch
            {
                await EmbedHandler.CreateBasicEmbed("Error", "Wasn't able to add node :(", Color.Purple);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<Lavalink>()
                .AddSingleton<AudioService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        private async Task LogClient(DiscordSocketClient client, TokenType type)
        {
            client = _client;
            await client.LoginAsync(type, Environment.GetEnvironmentVariable("DiscordToken"));
        }

        
    }
}