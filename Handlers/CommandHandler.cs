using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly ServiceProvider _services;

        public CommandHandler(ServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commandService = services.GetRequiredService<CommandService>();
            _services = services;

            HookEvents();
        }

        public async Task InstallCommandsAsync()
        {
            await _commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
            services: _services);
        }

        private void HookEvents()
        {
            _commandService.Log += LogAsync;
            _client.MessageReceived += HandleCommandAsync;
        }

        private Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            if (message == null)
            {
                return Task.CompletedTask;
            }

            int argPos = 0;

            //Making sure that prefix is used and command isn't from bot
            if (!(message.HasCharPrefix('!', ref argPos) ||
            message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            {
                return Task.CompletedTask;
            }
            else
            {
                var context = new SocketCommandContext(_client, message);

                var result = _commandService.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);

                //Make the message prettier..
                if (!result.Result.IsSuccess)
                {
                    //var embed = EmbedHandler.CreateErrorEmbed("Error", result.ToString());
                    //context.Channel.SendMessageAsync("", false, embed.Result);
                }

                return result;
            
            }

        }

        private Task LogAsync(LogMessage logMessage)
        {
            System.Console.WriteLine(logMessage.ToString() + "from commandhanler");
            return Task.CompletedTask;
        }
    }
}