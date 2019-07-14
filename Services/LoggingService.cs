using System.Threading.Tasks;
using Discord;

namespace DiscordBot.Services
{
    public static class LoggingService
    {
        public static Task OnLog(LogMessage msg)
        {
            System.Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}