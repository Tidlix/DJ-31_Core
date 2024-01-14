using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DJ_31.Commands;
using DJ_31.Config;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace DJ_31
{
    internal class Program
    {
        // Variables
        public static DiscordClient Client { get; set; }
        public bool isOnline { get; set; } = true;

        static async Task Main(string[] args)
        {
            // Configurations
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var DcConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };
            Client = new DiscordClient(DcConfig);

            // Ready log
            Client.Ready += Client_Ready;

            //Register Commands
            var SlashCommandConfig = Client.UseSlashCommands();
            SlashCommandConfig.RegisterCommands<MusicCommands>();
            SlashCommandConfig.RegisterCommands<InfoCommands>();
            SlashCommandConfig.RegisterCommands<DebugCommands>();

            // Lavalink 
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "oce-lavalink.lexnet.cc",
                Port = 443,
                Secured = true,
            };

            var LavalinkConfig = new LavalinkConfiguration()
            {
                Password = "lexn3tl@val!nk",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint,
            };

            var lavalink = Client.UseLavalink();

            // Puts bot online
            await Client.ConnectAsync();
            await lavalink.ConnectAsync(LavalinkConfig);


            await Task.Delay(-1);
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ONLINE] Bot is now Online");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[HINWEIS!] Sobald diese Konsole geschlossen wird, fährt der Bot herunter!");
            Console.ForegroundColor = ConsoleColor.White;
            return Task.CompletedTask;
        }
    }
}
