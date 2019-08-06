using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Services
{
    public static class BotService
    {

        public static async Task SetBotStatus(DiscordSocketClient client, string statusText)
        {
            await client.SetGameAsync(statusText);
        }

        //private async Task SetGameStatus(DiscordSocketClient client, Lavalink player)
        //{
        //    return await client.SetGameAsync($"{player.DefaultNode.GetPlayer(new AudioService(player)).currentGuild}"));
        //}


    }
}
