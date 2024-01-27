using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DJ_31.Commands
{
    public class InfoCommands : ApplicationCommandModule
    {
        [SlashCommand("Help", "Zeige alle Befehle an")]
        public async Task Help(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var response = new DiscordEmbedBuilder()
            {
                Title = "/Help",
                Description = "Hier sind alle Befehle, für den Musikbot: " +
                "\n 1. /Help => Zeigt diese übersicht" +
                "\n 2. /Start => Startet den Bot" +
                "\n 3. /Stop => Stoppt den Musik bot" +
                "\n 4. /Skip => Überspringt den aktuellen Song" +
                "\n 5. /Pause => Pausiert die Musik" +
                "\n 6. /Resume => Startet die Musik, nachdem sie Pausiert wurde" +
                "\n 7. /Info => Zeigt Informationen über den Momentanen Song" +
                "\n 8. /Playlist => Ändere die Momentane Playlist" +
                "\n 9. /Song => Spielt sofort ein bestimmtes Lied ab" +
                "\n\n Außerdem gibt es die Debug Commands, die sind allerdings nur für Entwicker freigeschalten!",
                Color = DiscordColor.Black
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }

        [SlashCommand("Info", "Erhalte Informationen über das aktuelle Lied!")]
        public async Task Info(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var AudioService = Program.AudioService;
            var Player = GetPlayerAsync(ctx).Result;

            // No player error
            if (Player == null)
            {
                await ctx.DeleteResponseAsync();
                return;
            }

            // Not Playling
            if (Player.CurrentTrack == null)
            {
                var Error = new DiscordEmbedBuilder()
                {
                    Title = "Error",
                    Description = "Der Bot spielt momentan keine Musik!",
                    Color = DiscordColor.Red
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(Error));
                return;
            }

            var response = new DiscordEmbedBuilder()
            {
                Title = "Info",
                Description = $"Aktuelles Lied: {Player.CurrentTrack.Title}" +
                $"\nAuthor: {Player.CurrentTrack.Author}" +
                $"\nLänge des Liedes: {Player.CurrentTrack.Duration}" +
                $"\nLink: {Player.CurrentTrack.Uri}",
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }


        private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(InteractionContext ctx, bool connectToVoiceChannel = true)
        {
            var AudioService = Program.AudioService;

            var channelBehavior = connectToVoiceChannel
                ? PlayerChannelBehavior.Join
                : PlayerChannelBehavior.None;

            var playerOptions = new QueuedLavalinkPlayerOptions { };
            var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior);

            var result = await AudioService.Players
                .RetrieveAsync(guildId: ctx.Guild.Id, memberVoiceChannel: ctx.Member.VoiceState.Channel.Id, playerFactory: PlayerFactory.Queued, options: Options.Create(options: playerOptions), retrieveOptions)
                .ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                var errorMessage = result.Status switch
                {
                    PlayerRetrieveStatus.UserNotInVoiceChannel => "Du musst in einem Sprachkanal sein!",
                    PlayerRetrieveStatus.BotNotConnected => "Der Bot ist nicht verbunden!",
                    _ => "Unbekannter fehler!",
                };

                await ctx.Channel.SendMessageAsync(errorMessage);
                return null;
            }

            return result.Player;
        }
    }
}
