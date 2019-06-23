using System.Threading.Tasks;
using DiscordBot.Services;

namespace MusicBot
{
    public class Program
    {
        static Task Main(string[] args)
            => new DiscordService().InitializeAsync();
    }
}
