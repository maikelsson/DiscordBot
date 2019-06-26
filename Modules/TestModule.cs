using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Modules
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        public async Task SayAsync(string echo)
        {
            await ReplyAsync(echo);
        }

    }
}