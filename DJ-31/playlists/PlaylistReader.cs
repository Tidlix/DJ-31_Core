using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DJ_31.Playlists
{
    internal class playlistReader
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string song { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int playlist {  get; set; }

        public async Task ReadPlaylist(int playlist, bool Debug)
        {
            if (Debug)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n[DEBUG] Playlist Reader:");
                Console.ForegroundColor = ConsoleColor.White;
            }

            string JSONFile = "";
            if (playlist == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("JSONReaderError! No Playlist");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            } //Return error
            else if (playlist == 1) JSONFile = $"{AppDomain.CurrentDomain.BaseDirectory}/Playlists/PL01.json";
            else if (playlist == 2) JSONFile = $"{AppDomain.CurrentDomain.BaseDirectory}/Playlists/PL02.json";
            else if (playlist == 3) JSONFile = $"{AppDomain.CurrentDomain.BaseDirectory}/Playlists/PL03.json";

            if (Debug)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[DEBUG] Playlist soll gelesen werden! " +
                    $"\n[DEBUG] PlaylistNr: {playlist}" +
                    $"\n[DEBUG] File: {JSONFile}");
            }
            using (StreamReader sr = new StreamReader(JSONFile.ToString()))
            {
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 1!");
                string json = await sr.ReadToEndAsync();
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 2!");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Dictionary<string, string> songs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 3!");

                var random = new Random();
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 4!");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                int index = random.Next(1, songs.Count + 1);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 5!");

                songs.TryGetValue(index.ToString(), out var nextSong);
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 6!");
#pragma warning disable CS8601 // Possible null reference assignment.
                this.song = nextSong;
#pragma warning restore CS8601 // Possible null reference assignment.
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 7! (Ende)");
                if (Debug) Console.WriteLine($"[DEBUG] Song: {song}");
            }
        }


        public async Task SetPlaylist(int playlist)
        {
            using (StreamWriter sr = new StreamWriter($"{AppDomain.CurrentDomain.BaseDirectory}/Playlists/currentPlaylist.json"))
            {
                await sr.WriteAsync("{" +
                    "\n\"playlist\": " + playlist +
                    "\n}");
            }
        }
        public async Task GetPlaylist()
        {
            using (StreamReader sr = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}/Playlists/currentPlaylist.json"))
            {
                string json = await sr.ReadToEndAsync();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                CurrentPlaylistStructure data = JsonConvert.DeserializeObject<CurrentPlaylistStructure>(json);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                this.playlist = data.playlist;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        internal sealed class CurrentPlaylistStructure
        {
            public int playlist { get; set; }
        }
    }
}
