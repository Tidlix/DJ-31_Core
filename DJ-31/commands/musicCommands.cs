using DJ_31.Playlists;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using Lavalink4NET.Rest.Entities.Tracks;

namespace DJ_31
{
    public class MusicCommands : ApplicationCommandModule
    {

        [SlashCommand("Start", "Starte die Musik!")]
        public async Task Start(InteractionContext ctx,
            [Choice("MCDOMELP", 1)]
            [Choice("joni.tv", 2)]
            [Choice("Tidlix", 3)]
            [Option("Playlist", "Wähle eine Playlist aus")] long playlist)
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

            // Set playlist
            var Reader = new playlistReader();
            await Reader.SetPlaylist((int)playlist);

            // Get track
            await Reader.ReadPlaylist((int)playlist, false);

            var Track = await AudioService.Tracks
                .LoadTracksAsync(Reader.song, TrackSearchMode.YouTube);

            // No track error
                #pragma warning disable CS8073 
            if (Track == null)
            {
                var error = new DiscordEmbedBuilder()
                {
                    Title = "Error!",
                    Description = $"Der Song {Reader.song} wurde nicht gefunden! Bitte Kontaktiere Tidlix",
                    Color = DiscordColor.Red
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(error));
                return;
            }
                #pragma warning restore CS8073

            // Play track
            await Player.PlayAsync(Track.ToString());

            // Track ended
            AudioService.TrackEnded += NextTrack;

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "/Play",
                Description = "Der Bot spielt jetzt die ausgewählte Playlist.",
                Color = DiscordColor.Green
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }

        private async Task NextTrack(object sender, Lavalink4NET.Events.Players.TrackEndedEventArgs eventArgs)
        {
            var Reader = new playlistReader();
            await Reader.GetPlaylist();


        }

        [SlashCommand("Stop", "Stoppt die Musik")]
        public async Task Stop(InteractionContext ctx)
        {
            
        }



        [SlashCommand("Pause", "Pausiere, und starte die Musik")]
        public async Task Pause(InteractionContext ctx)
        {
            
        }


        [SlashCommand("Resume", "Spiele die Musik ab, nachdem Sie pausiert wurde!")]
        public async Task Resume(InteractionContext ctx)
        {
                    
        }



        [SlashCommand("Skip", "Gehe zum nächsten Lied")]
        public async Task Skip(InteractionContext ctx)
        {

        }


        [SlashCommand("Playlist", "Verändere die Playlist!")]
        public async Task Playlist(InteractionContext ctx,
            [Choice("MCDOMELP", 1)]
            [Choice("joni.tv", 2)]
            [Choice("Tildix", 3)]
            [Option("Playlist", "Wähle eine Playlist aus")] long playlistL)
        {

        }

        [SlashCommand("Song", "Spiele ein Bestimmtes Lied")]
        public async Task Song(InteractionContext ctx, [Option("Songtitel", "Welches Lied möchtest du spielen?")] string song)
        {

        }

        // Others
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

