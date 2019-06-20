using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            //Hide the token somewhere...
            await _client.LoginAsync(TokenType.Bot, "NTUyNzYxNjIwNTUyNjEzODg4.XQs-Zg.i9hZLYFKVLHeJK_k0L-ii3eaAIU");
	        await _client.StartAsync();

	        // Block this task until the program is closed.
	        await Task.Delay(-1);

        }

        private Task Log(LogMessage msg)
        {
            System.Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
