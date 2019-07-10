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
        public async Task Join()
        {
            await ReplyAsync("", false, await _service.JoinChannelAsync(
                (SocketGuildUser)Context.User, Context.Channel, Context.Guild.Id));
        }

        [Command("Play")]
        public async Task Play([Remainder] string search)
        {
            await ReplyAsync("", false, await _service.PlaySongAsync(
                (SocketGuildUser)Context.User, search));
        }

        [Command("Kick")]
        public async Task Leave()
        {
            await ReplyAsync("", false, await _service.LeaveChannelAsync(
                Context.Guild.Id));
        }
        
    }
}