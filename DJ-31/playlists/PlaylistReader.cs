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
        public string song { get; set; }
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
            else if (playlist == 1) JSONFile = $"{AppDomain.CurrentDomain.BaseDirectory}PL01.json";
            else if (playlist == 2) JSONFile = $"{AppDomain.CurrentDomain.BaseDirectory}PL02.json";
            else if (playlist == 3) JSONFile = $"{AppDomain.CurrentDomain.BaseDirectory}PL03.json";

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
                Dictionary<string, string> songs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 3!");

                var random = new Random();
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 4!");
                int index = random.Next(1, songs.Count + 1);
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 5!");

                songs.TryGetValue(index.ToString(), out var nextSong);
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 6!");
                this.song = nextSong;
                if (Debug) Console.WriteLine("[DEBUG] Checkpoint 7! (Ende)");
                if (Debug) Console.WriteLine($"[DEBUG] Song: {song}");
            }
        }
    }
}
