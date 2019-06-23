using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class DiscordService
    {
        private DiscordSocketClient _client;
        private ServiceProvider _services;
        public async Task InitializeAsync()
        {
            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _client.Log += Log;

            await LogClient(_client, TokenType.Bot);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .BuildServiceProvider();
        }

        private Task Log(LogMessage msg)
        {
            System.Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task LogClient(DiscordSocketClient client, TokenType type)
        {
            client = _client;
            //Hide the token pls
            await client.LoginAsync(type, "NTUyNzYxNjIwNTUyNjEzODg4.XQs-Zg.i9hZLYFKVLHeJK_k0L-ii3eaAIU");
        }
    }
}