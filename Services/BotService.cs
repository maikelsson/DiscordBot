using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Services
{
    public class BotService
    {
        private DiscordSocketClient _client;
        private Lavalink _lavalink;

        public BotService(DiscordSocketClient client, Lavalink lavalink)
        {
            _client = client;
            _lavalink = lavalink;
        }

        //private async Task SetGameStatus(DiscordSocketClient client, Lavalink player)
        //{
        //    return await client.SetGameAsync($"{player.DefaultNode.GetPlayer(new AudioService(player)).currentGuild}"));
        //}


    }
}
