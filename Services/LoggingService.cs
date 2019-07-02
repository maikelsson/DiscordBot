using System.Threading.Tasks;
using Discord;

namespace DiscordBot.Services
{
    public class LoggingService
    {
        public Task OnLog(LogMessage msg)
        {
            System.Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}