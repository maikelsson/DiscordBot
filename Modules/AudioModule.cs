using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;

namespace DiscordBot.Modules
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        public AudioService _service { get; set; }

        [Command("Join")]
        public async Task JoinChannelAsync()
        {
            await ReplyAsync("", false, await _service.JoinChannelAsync(
                (SocketGuildUser)Context.User));
        }

        [Command("Kick")]
        public async Task LeaveChannelAsync()
        {
            await ReplyAsync("", false, await _service.LeaveChannelAsync(
                (SocketGuildUser)Context.User));
        }
        
    }
}