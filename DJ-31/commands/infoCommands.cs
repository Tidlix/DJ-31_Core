using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
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
            var lavalinkInstance = ctx.Client.GetLavalink();
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var connection = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            //ERRORS:    
            // no lavalinkInstance (Error 02)
            if (lavalinkInstance == null)
            {
                await ctx.DeferAsync();
                var noIstanceResponse = new DiscordEmbedBuilder()
                {
                    Title = "Error 02",
                    Description = "Fehlgeschlagen!" +
                    "\n Kein Lavalink gefunden!" +
                    "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                    Color = DiscordColor.Red,
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noIstanceResponse));
                return;
            }
            // no lavalinkNodes (Error 03)
            if (lavalinkInstance.ConnectedNodes == null)
            {
                await ctx.DeferAsync();
                var noIstanceNodeResponse = new DiscordEmbedBuilder()
                {
                    Title = "Error 03",
                    Description = "Fehlgeschlagen!" +
                    "\n Keine Lavalink node gefunden!" +
                    "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                    Color = DiscordColor.Red,
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noIstanceNodeResponse));
                return;
            }
            // no Connection (Error 04)
            if (connection == null)
            {
                await ctx.DeferAsync();
                var noIstanceNodeResponse = new DiscordEmbedBuilder()
                {
                    Title = "Error 04",
                    Description = "Fehlgeschlagen!" +
                    "\n Keine Connection gefunden!" +
                    "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                    Color = DiscordColor.Red,
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noIstanceNodeResponse));
                return;
            }

            await ctx.DeferAsync();
            var response = new DiscordEmbedBuilder()
            {
                Title = "Info",
                Description = $"Titel: {connection.CurrentState.CurrentTrack.Title}" +
                $"\n Author: {connection.CurrentState.CurrentTrack.Author}" +
                $"\n Länge: {connection.CurrentState.CurrentTrack.Length} (Stunden:Minuten:Sekunden)" +
                $"\n Link: {connection.CurrentState.CurrentTrack.Uri}",
                Color = DiscordColor.Aquamarine
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }
    }
}
