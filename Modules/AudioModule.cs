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
        
        [Command("Pause")]
        public async Task Pause()
        {
            await ReplyAsync("", false, await _service.PauseOrContinueSongAsync(
                (SocketGuildUser)Context.User));
        }

        [Command("Skip")]
        public async Task Skip()
        {
            await ReplyAsync("", false, await _service.SkipSongAsync(Context.Guild.Id));
        }
    }
}