using DSharpPlus.Entities;
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

        //[SlashCommand("Info", "Erhalte Informationen über das aktuelle Lied!")]
        //public async Task Info(InteractionContext ctx)
        //{
           
        //}
    }
}
