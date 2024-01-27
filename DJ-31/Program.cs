using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DJ_31.Commands;
using DJ_31.Config;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using Lavalink4NET;
using Lavalink4NET.Rest;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Net.NetworkInformation;
using Lavalink4NET.Players.Queued;

namespace DJ_31
{
    internal class Program
    {
        // Variables
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static DiscordClient Client { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static IAudioService AudioService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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

            // Lavalink
            using var serviceProvider = new ServiceCollection()
                .AddSingleton(Client)
                .AddLavalink()
                .ConfigureLavalink(x =>
                {
                    x.Label = "Lavalink";
                    x.BaseAddress = new Uri("http://localhost:433"); 
                    x.Passphrase = "TDLX01"; 
                    x.ResumptionOptions = new LavalinkSessionResumptionOptions(TimeSpan.FromSeconds(60));
                    x.ReadyTimeout = TimeSpan.FromSeconds(15);
                })
                //.AddLogging(x => x.AddConsole())
                .BuildServiceProvider();
            AudioService = serviceProvider.GetRequiredService<IAudioService>();

            // Ready log
            Client.Ready += Client_Ready;

            //Register Commands
            var SlashCommandConfig = Client.UseSlashCommands();
            SlashCommandConfig.RegisterCommands<MusicCommands>();
            SlashCommandConfig.RegisterCommands<InfoCommands>();
            SlashCommandConfig.RegisterCommands<DebugCommands>();

            // Puts bot online
            await Client.ConnectAsync();
            foreach (var hostedService in serviceProvider.GetServices<IHostedService>())
            {
                await hostedService
                    .StartAsync(CancellationToken.None)
                    .ConfigureAwait(false);
            }

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
