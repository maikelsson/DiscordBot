using System;
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
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _commandService = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();            
            _services = services;

            HookEvents();
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
            services: _services);
        }

        private void HookEvents()
        {
            _commandService.CommandExecuted += CommandExecutedAsync;
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

                System.Console.WriteLine("Message from commandHandler");
                return result;
            
            }

        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, 
        ICommandContext context, IResult result)
        {
            if(!command.IsSpecified) return;
            if(result.IsSuccess) return;

            await context.Channel.SendMessageAsync("Jotakin meni pieleen...");
        }

        private Task LogAsync(LogMessage logMessage)
        {
            System.Console.WriteLine(logMessage.ToString() + "from commandhanler");
            return Task.CompletedTask;
        }
    }
}