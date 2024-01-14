using DJ_31.Playlists;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DJ_31
{
    public class MusicCommands : ApplicationCommandModule
    {
        public int currentPlaylist { get; set; }

        [SlashCommand("Start", "Starte die Musik!")]
        public async Task Start(InteractionContext ctx,
            [Choice("MCDOMELP", 1)]
            [Choice("joni.tv", 2)]
            [Choice("Tidlix", 3)]
            [Option("Playlist", "Wähle eine Playlist aus")] long playlistL)
        {
            await ctx.DeferAsync();
            var lavalinkInstance = ctx.Client.GetLavalink();

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(ctx.Member.VoiceState.Channel);

            var connection = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            int playlist = (int)playlistL;

            //ERRORS:    
            // No Channel found (Error 01)
            if (ctx.Member.VoiceState == null)
            {
                await ctx.DeferAsync();
                var noChannelResponse = new DiscordEmbedBuilder()
                {
                    Title = "Error 01",
                    Description = "Beitritt fehlgeschlagen!" +
                    "\n Kein Voicechannel gefunden!" +
                    "\n\n Bitte trete einem Voicechannel bei!",
                    Color = DiscordColor.Red,
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noChannelResponse));
                return;
            }
            // no lavalinkInstance (Error 02)
            if (lavalinkInstance == null)
            {
                await ctx.DeferAsync();
                var noIstanceResponse = new DiscordEmbedBuilder()
                {
                    Title = "Error 02",
                    Description = "Beitritt fehlgeschlagen!" +
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
                    Description = "Beitritt fehlgeschlagen!" +
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
                    Description = "Beitritt fehlgeschlagen!" +
                    "\n Keine Connection gefunden!" +
                    "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                    Color = DiscordColor.Red,
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noIstanceNodeResponse));
                return;
            }
            // allready Playing (Error 05)
            if (connection.CurrentState.CurrentTrack != null)
            {
                await ctx.DeferAsync();
                var allreadyPlayingRespose = new DiscordEmbedBuilder()
                {
                    Title = "Error 05",
                    Description = "Beitritt fehlgeschlagen!" +
                    "\n Der Bot spielt bereits Musik!" +
                    "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                    Color = DiscordColor.Red,
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(allreadyPlayingRespose));
                return;
            }

            // set playlist
            this.currentPlaylist = playlist;

            // get first Song
            var Reader = new playlistReader();
            await Reader.ReadPlaylist(currentPlaylist, false);
            var query = await node.Rest.GetTracksAsync(Reader.song);
            var track = query.Tracks.First();
            // playSong
            await connection.PlayAsync(track);

            // loop
            connection.PlaybackFinished += Connection_PlaybackFinished;

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "Fertig!",
                Description = "Der Bot spielt jetzt Musik!",
                Color = DiscordColor.Green
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }
        private async Task Connection_PlaybackFinished(LavalinkGuildConnection sender, DSharpPlus.Lavalink.EventArgs.TrackFinishEventArgs args)
        {
            TimeSpan time = TimeSpan.FromSeconds(5);
            await Task.Delay(1);
            if (sender.CurrentState.PlaybackPosition <= time) return;
            var Reader = new playlistReader();
            await Reader.ReadPlaylist(currentPlaylist, false);

            var query = await sender.Node.Rest.GetTracksAsync(Reader.song);
            var track = query.Tracks.First();

            await sender.PlayAsync(track);
            return;
        }


        [SlashCommand("Stop", "Stoppt die Musik")]
        public async Task Stop(InteractionContext ctx)
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
                    Description = "Beitritt fehlgeschlagen!" +
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
                    Description = "Beitritt fehlgeschlagen!" +
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
                    Description = "Beitritt fehlgeschlagen!" +
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
                Title = "Fertig!",
                Description = "Die Musik wurde gestoppt, und der Bot hat den Channel verlassen!",
                Color = DiscordColor.Red
            };
            await connection.StopAsync();
            await connection.DisconnectAsync();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }



        [SlashCommand("Pause", "Pausiere, und starte die Musik")]
        public async Task Pause(InteractionContext ctx)
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
                    Description = "Beitritt fehlgeschlagen!" +
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
                    Description = "Beitritt fehlgeschlagen!" +
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
                    Description = "Beitritt fehlgeschlagen!" +
                    "\n Keine Connection gefunden!" +
                    "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                    Color = DiscordColor.Red,
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noIstanceNodeResponse));
                return;
            }

            await ctx.DeferAsync();
            var pauseResponse = new DiscordEmbedBuilder()
            {
                Title = "Fertig!",
                Description = "Der Bot ist jetzt Pausiert!",
                Color = DiscordColor.Orange
            };
            await connection.PauseAsync();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(pauseResponse));
        }


        [SlashCommand("Resume", "Spiele die Musik ab, nachdem Sie pausiert wurde!")]
        public async Task Resume(InteractionContext ctx)
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
                    Description = "Beitritt fehlgeschlagen!" +
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
                    Description = "Beitritt fehlgeschlagen!" +
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
                    Description = "Beitritt fehlgeschlagen!" +
                    "\n Keine Connection gefunden!" +
                    "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                    Color = DiscordColor.Red,
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noIstanceNodeResponse));
                return;
            }

            await ctx.DeferAsync();
            var resumeResponse = new DiscordEmbedBuilder()
            {
                Title = "Fertig!",
                Description = "Die Musik spielt jetzt weiter!",
                Color = DiscordColor.Green
            };
            await connection.ResumeAsync();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(resumeResponse));
        }



        [SlashCommand("Skip", "Gehe zum nächsten Lied")]
        public async Task Skip(InteractionContext ctx)
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
                    Description = "Befehl fehlgeschlagen!" +
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
                    Description = "Befehl fehlgeschlagen!" +
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
                    Description = "Befehl fehlgeschlagen!" +
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
                Title = "Fertig!",
                Description = "Das Lied wurde übersprungen!",
                Color = DiscordColor.Blurple
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));

            var Reader = new playlistReader();
            await Reader.ReadPlaylist(currentPlaylist, false);

            var query = await node.Rest.GetTracksAsync(Reader.song);
            var track = query.Tracks.First();

            await connection.PlayAsync(track);

        }


        [SlashCommand("Playlist", "Verändere die Playlist!")]
        public async Task Playlist(InteractionContext ctx,
            [Choice("MCDOMELP", 1)]
            [Choice("joni.tv", 2)]
            [Choice("Tildix", 3)]
            [Option("Playlist", "Wähle eine Playlist aus")] long playlistL)
        {
            await ctx.DeferAsync();

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
                    Description = "Befehl fehlgeschlagen!" +
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
                    Description = "Befehl fehlgeschlagen!" +
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
                    Description = "Befehl fehlgeschlagen!" +
                    "\n Keine Connection gefunden!" +
                    "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                    Color = DiscordColor.Red,
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noIstanceNodeResponse));
                return;
            }

            // Set Playlist
            int playlist = (int)playlistL;
            this.currentPlaylist = playlist;

            // New Song
            var Reader = new playlistReader();
            await Reader.ReadPlaylist(currentPlaylist, false);
            var query = await node.Rest.GetTracksAsync(Reader.song);
            var track = query.Tracks.First();

            await connection.PlayAsync(track);

            // Response
            var response = new DiscordEmbedBuilder()
            {
                Title = "Fertig!",
                Description = "Die Playlist wurde geändert!",
                Color = DiscordColor.Aquamarine
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
        }

        [SlashCommand("Song", "Spiele ein Bestimmtes Lied")]
        public async Task Song(InteractionContext ctx, [Option("Songtitel", "Welches Lied möchtest du spielen?")] string song)
        {
            await ctx.DeferAsync();

            var lavalinkInstance = ctx.Client.GetLavalink();
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var connection = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (connection == null)
            {
                await node.ConnectAsync(ctx.Member.VoiceState.Channel);
                connection = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

                //ERRORS:    
                // No Channel found (Error 01)
                if (ctx.Member.VoiceState == null)
                {
                    await ctx.DeferAsync();
                    var noChannelResponse = new DiscordEmbedBuilder()
                    {
                        Title = "Error 01",
                        Description = "Beitritt fehlgeschlagen!" +
                        "\n Kein Voicechannel gefunden!" +
                        "\n\n Bitte trete einem Voicechannel bei!",
                        Color = DiscordColor.Red,
                    };

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noChannelResponse));
                    return;
                }
                // no lavalinkInstance (Error 02)
                if (lavalinkInstance == null)
                {
                    await ctx.DeferAsync();
                    var noIstanceResponse = new DiscordEmbedBuilder()
                    {
                        Title = "Error 02",
                        Description = "Beitritt fehlgeschlagen!" +
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
                        Description = "Beitritt fehlgeschlagen!" +
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
                        Description = "Beitritt fehlgeschlagen!" +
                        "\n Keine Connection gefunden!" +
                        "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                        Color = DiscordColor.Red,
                    };

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noIstanceNodeResponse));
                    return;
                }

                var query = await node.Rest.GetTracksAsync(song);
                var track = query.Tracks.First();

                await connection.PlayAsync(track);

                connection.PlaybackFinished += Connection_PlaybackFinished_END;

                var response = new DiscordEmbedBuilder()
                {
                    Title = "Fertig!",
                    Description = "Dein Lied wird jetzt abgespielt! " +
                    "\n\n(Der Bot verlässt den Channel nach diesem Lied!)",
                    Color = DiscordColor.DarkGreen
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
            }
            else
            {
                //ERRORS:    
                // No Channel found (Error 01)
                if (ctx.Member.VoiceState == null)
                {
                    await ctx.DeferAsync();
                    var noChannelResponse = new DiscordEmbedBuilder()
                    {
                        Title = "Error 01",
                        Description = "Beitritt fehlgeschlagen!" +
                        "\n Kein Voicechannel gefunden!" +
                        "\n\n Bitte trete einem Voicechannel bei!",
                        Color = DiscordColor.Red,
                    };

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noChannelResponse));
                    return;
                }
                // no lavalinkInstance (Error 02)
                if (lavalinkInstance == null)
                {
                    await ctx.DeferAsync();
                    var noIstanceResponse = new DiscordEmbedBuilder()
                    {
                        Title = "Error 02",
                        Description = "Beitritt fehlgeschlagen!" +
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
                        Description = "Beitritt fehlgeschlagen!" +
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
                        Description = "Beitritt fehlgeschlagen!" +
                        "\n Keine Connection gefunden!" +
                        "\n\n Bitte versuche es später erneut! Sollte der Fehler weiterhin bestehen, kontaktiere Artzt31!",
                        Color = DiscordColor.Red,
                    };

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(noIstanceNodeResponse));
                    return;
                }

                var query = await node.Rest.GetTracksAsync(song);
                var track = query.Tracks.First();

                await connection.PlayAsync(track);

                var response = new DiscordEmbedBuilder()
                {
                    Title = "Fertig!",
                    Description = "Dein Lied wird jetzt abgespielt! " +
                    "\n\n(Der Bot spielt nach diesem Lied die aktuelle Playlist weiter!)",
                    Color = DiscordColor.DarkGreen
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(response));
            }


        }
        public async Task Connection_PlaybackFinished_END(LavalinkGuildConnection sender, DSharpPlus.Lavalink.EventArgs.TrackFinishEventArgs args)
        {
            await sender.StopAsync();
            await sender.DisconnectAsync();
            return;
        }
    }
}

