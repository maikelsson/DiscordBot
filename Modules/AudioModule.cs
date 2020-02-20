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
        [Alias("Liity", "j")]
        public async Task Join()
        {
            await ReplyAsync("", false, await _service.JoinChannelAsync(
                (SocketGuildUser)Context.User, Context.Channel, Context.Guild.Id));
        }

        [Command("Play")]
        [Alias("Soita", "Soitaparanoid", "paranoid")]
        public async Task Play([Remainder] string search = "Paranoid")
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
        [Alias("Tauko", "paussi")]
        public async Task Pause()
        {
            await ReplyAsync("", false, await _service.PauseOrContinueSongAsync(
                (SocketGuildUser)Context.User));
        }

        [Command("Skip")]
        [Alias("VMP", "toxic")]
        public async Task Skip()
        {
            await ReplyAsync("", false, await _service.SkipSongAsync(Context.Guild.Id));
        }

        [Command("List")]
        [Alias("Lista", "biisit")]
        public async Task List()
        {
            await ReplyAsync("", false, await _service.ListSongsAsync(Context.Guild.Id)); 
        }
    }
}