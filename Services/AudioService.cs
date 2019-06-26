using Victoria;

namespace DiscordBot.Services
{
    public class AudioService
    {
        private Lavalink _lavalink;

        public AudioService(Lavalink lavalink)
        {
            _lavalink = lavalink;
        }
    }
}