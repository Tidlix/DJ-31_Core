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
using Lavalink4NET;
using System.Numerics;
using System.Diagnostics.Tracing;

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
            TrackLoadResult Track;
            switch (Reader.playlist)
            {
                case 1: 
                    Track = await AudioService.Tracks
                        .LoadTracksAsync(Reader.song, TrackSearchMode.YouTube);
                    break;
                case 2:
                    Track = await AudioService.Tracks
                        .LoadTracksAsync(Reader.song, TrackSearchMode.YouTube);
                    break;
                case 3:
                    Track = await AudioService.Tracks
                        .LoadTracksAsync(Reader.song, TrackSearchMode.Spotify);
                    break;
            }

            

            // No track error
#pragma warning disable CS8073
            if (Track == null)
            {
                var error = new DiscordEmbedBuilder()
                {
                    Title = "Error!",
                    Description = $"Der Song \n{Reader.song}\n wurde nicht gefunden! Bitte Kontaktiere Tidlix",
                    Color = DiscordColor.Red
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(error));
                return;
            }
#pragma warning restore CS8073


            // Play track
#pragma warning disable CS8604 // Possible null reference argument.
            await Player.PlayAsync(Track.Track);
#pragma warning restore CS8604 // Possible null reference argument.

            // Track ended
            AudioService.TrackEnded += NextTrack;

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "/Start",
                Description = "Der Bot spielt jetzt die ausgewählte Playlist.",
                Color = DiscordColor.Green
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }


        [SlashCommand("Stop", "Stoppt die Musik")]
        public async Task Stop(InteractionContext ctx)
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

            await Player.StopAsync();
            await Player.DisconnectAsync();

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "/Stop",
                Description = "Der Bot wurde gestoppt!",
                Color = DiscordColor.Red
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }



        [SlashCommand("Pause", "Pausiere, und starte die Musik")]
        public async Task Pause(InteractionContext ctx)
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

            await Player.PauseAsync();

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "/Pause",
                Description = "Der Bot wurde Pausiert!",
                Color = DiscordColor.Lilac
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }


        [SlashCommand("Resume", "Spiele die Musik ab, nachdem Sie pausiert wurde!")]
        public async Task Resume(InteractionContext ctx)
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

            await Player.ResumeAsync();

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "/Resume",
                Description = "Der Bot spielt jetzt weiter!",
                Color = DiscordColor.Lilac
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }



        [SlashCommand("Skip", "Gehe zum nächsten Lied")]
        public async Task Skip(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var AudioService = Program.AudioService;
            var Player = GetPlayerAsync(ctx).Result;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var TrackEnd = Player.CurrentTrack.Duration;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            await Player.SeekAsync(TrackEnd);

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "/Skip",
                Description = "Das Lied wurde übersprungen!",
                Color = DiscordColor.Blurple
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }


        [SlashCommand("Playlist", "Verändere die Playlist!")]
        public async Task Playlist(InteractionContext ctx,
            [Choice("MCDOMELP", 1)]
            [Choice("joni.tv", 2)]
            [Choice("Tildix", 3)]
            [Option("Playlist", "Wähle eine Playlist aus")] long playlist)
        {
            var Reader = new playlistReader();
            await Reader.SetPlaylist((int)playlist);

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "/Playlist",
                Description = "Die Playlist wurde geändert, und wird ab dem nächsten Lied abgespielt!",
                Color = DiscordColor.Blurple
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }


        [SlashCommand("Song", "Spiele ein Bestimmtes Lied")]
        public async Task Song(InteractionContext ctx, [Option("Songtitel", "Welches Lied möchtest du spielen?")] string song)
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

            // Check if allready Playing
            bool allreadyPlaying = false;
            if (Player.CurrentTrack == null) allreadyPlaying = false;
            else if (Player.CurrentTrack != null) allreadyPlaying = true;


            TrackLoadResult Track;

            bool isLink = false;
            string searchType = "Namenssuche";

            // Check if Spotify Link
            if (song.Contains("https://open.spotify.com/") || song.Contains("/track/"))
            {
                isLink = true;
                searchType = "Spotify Link";
                Track = await AudioService.Tracks
                .LoadTracksAsync(song, TrackSearchMode.Spotify);
            }
            //Check if Youtube Link
            else if (song.Contains("https://www.youtube.com/watch?v="))
            {
                isLink = true;
                searchType = "Youtube Link";
                Track = await AudioService.Tracks
                .LoadTracksAsync(song, TrackSearchMode.YouTube);
            }
            else if (song.Contains("https://"))
            {
                searchType = "Ungültiger Link!";
            }
            else
            {
                Track = await AudioService.Tracks
                .LoadTracksAsync(song, TrackSearchMode.YouTube);
            }



            // No track error
#pragma warning disable CS8887 // Use of unassigned local variable
            if (Track == TrackLoadResult.CreateEmpty())
#pragma warning disable CS8887 // Use of unassigned local variable
            {
                var error = new DiscordEmbedBuilder()
                {
                    Title = "Error!",
                    Description = $"Die Songsuche ist fehlgeschlagen! " +
                    $"\nFalls der eingegebene Song ein Link war, kontrolliere ob der Link richtig war!" +
                    $"\nFalls der Link richtig war, mache einen Screenshot von dieser Nachricht, und schicke diesen an Tidlix!" +
                    $"\n\nSuche: {song}" +
                    $"\nLink: {isLink}" +
                    $"\nSuchmodus: {searchType}",
                    Color = DiscordColor.Red
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(error));
                return;
            }

            // Play track
#pragma warning disable CS8604 // Possible null reference argument.
            await Player.PlayAsync(Track.Track);
#pragma warning restore CS8604 // Possible null reference argument.

            if (!allreadyPlaying)
            {
                AudioService.TrackEnded += StopAfterSong;
            }

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "/Song",
                Description = "Das Lied wurde in die Warteschlange getan!",
                Color = DiscordColor.Lilac
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
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

        private async Task NextTrack(object sender, Lavalink4NET.Events.Players.TrackEndedEventArgs eventArgs)
        {
            var AudioService = Program.AudioService;
            var Player = eventArgs.Player;

            if (Player.CurrentTrack == null)
            {
                var Reader = new playlistReader();
                await Reader.GetPlaylist();

                bool isPlaying = false;

                while (!isPlaying)
                {
                    // Get track
                    await Reader.ReadPlaylist(Reader.playlist, false);
                    TrackLoadResult Track;
                    switch (Reader.playlist)
                    {
                        case 1:
                            Track = await AudioService.Tracks
                                .LoadTracksAsync(Reader.song, TrackSearchMode.YouTube);
                            break;
                        case 2:
                            Track = await AudioService.Tracks
                                .LoadTracksAsync(Reader.song, TrackSearchMode.YouTube);
                            break;
                        case 3:
                            Track = await AudioService.Tracks
                                .LoadTracksAsync(Reader.song, TrackSearchMode.Spotify);
                            break;
                    }

                    // Play track
#pragma warning disable CS8604 // Possible null reference argument.
                    await Player.PlayAsync(Track.Track);
#pragma warning restore CS8604 // Possible null reference argument.

                    await Task.Delay(100);

                    if (Player.CurrentTrack != null)
                    {
                        isPlaying = true;
                    }
                }
                await Task.Delay(1000);
            }
        }
        private async Task StopAfterSong(object sender, Lavalink4NET.Events.Players.TrackEndedEventArgs eventArgs)
        {
            await eventArgs.Player.DisconnectAsync();
        }


    }
}

