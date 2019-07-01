using System.Threading.Tasks;
using Discord;

namespace DiscordBot.Services
{

    //Put this to discordservice, this feels off
    public class LoggingService
    {
        public Task OnLog(LogMessage msg)
        {
            System.Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}