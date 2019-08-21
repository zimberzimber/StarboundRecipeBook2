using Jil;
using SBRB.Database;
using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB_DatabaseSeeder.Workers;
using System;
using System.IO;

// NOTE:
// Certain edge cases may be discovered and not resolved yet.
// Look for them via Ctrl + F 'EDGE CASE' through the entire project

// Add currency

// To tell whether a currency is used in a crafting recipe, first check if a currency exists with that name, and only then check items
namespace SBRB.Seeder
{
    partial class Program
    {
        const string BASE_GAME_ASSETS_STEAM_ID = "0";

        // Reference to class handling database connection
        static DatabaseConnection _db = DatabaseConnection.Instance;

        public static string modPath;
        static Mod _mod;
        static Logger _logger = Logger.Instance;

        static void Main(string[] args)
        {
#if DEBUG
            _logger.Log("Debug mode. Setting Frackin' Universe as target.");
            args = new string[] { @"D:\Games\steamapps\common\Starbound\mods\_FrackinUniverse-master" };
#endif

            // Ping the database to check whethers there's a connection to it before doing any work
            _logger.Log("Pinging database for connection...");
            if (!_db.Ping())
            {
                _logger.Log("ERROR: Could not connect to database.");
                ExitPrompt();
                return;
            }

            if (args == null || args.Length < 1)
            {
                _logger.Log("ERROR: Program started without any arguements.");
                ExitPrompt();
                return;
            }

            // Set the default de/serialization to ignore nulls
            JSON.SetDefaultOptions(Options.ExcludeNulls);

            // Get the mod path from the arguements
            modPath = args[0];

            // Reference to metastring
            string metaString;

            try
            {
                // Look for the metadata file. It can be either _metadata or .metadata
                if (File.Exists($"{modPath}\\.metadata"))
                    metaString = File.ReadAllText($"{modPath}\\.metadata");
                else if (File.Exists($"{modPath}\\_metadata"))
                    metaString = File.ReadAllText($"{modPath}\\_metadata");
                else
                {
                    // Exit if it wasan't found.
                    _logger.Log("ERROR: No metadata file detected.");
                    ExitPrompt();
                    return;
                }
                _logger("Connected to database...");

                // Deserialize the metadata
                Metadata meta = JSON.Deserialize<Metadata>(metaString);

                // Find the Steam ID
                if (string.IsNullOrWhiteSpace(meta.steamContentId))
                {
                    if (meta.author == "Chucklefish" && meta.name == "base")
                    {
                        // Base game assets dont have a steam ID, but has other specific data.
                        _logger.Log("Base game assets. ID is set to {0}.", BASE_GAME_ASSETS_STEAM_ID);
                        meta.steamContentId = BASE_GAME_ASSETS_STEAM_ID;
                    }
                    else
                    {
                        // Exit if there isn't a steam ID. We only work with Steam mods.
                        _logger.Log("ERROR: No Steam ID detected.");
                        ExitPrompt();
                        return;
                    }
                }
                else
                    _logger.Log($"Accepted mod with Steam ID {meta.steamContentId}");
                _logger.Log();

                // Convert the metadata into a mod data class
                _mod = meta.ToMod();

                _logger.Log("----------------------------------------");
                _logger.Log("Scanning and sorting mod files...");
                _logger.Log();
                ScanFiles(modPath);

                _logger.Log("----------------------------------------");
                _logger.Log("Building item and recipe lists...");
                _logger.Log();
                BuildQueues();

                _logger.Log("----------------------------------------");
                _logger.Log("Converting to DB models...");
                _logger.Log();
                ConvertToDBData();

                // Print warnings. Continue if there are none, await player input if there are any.
                _logger.Log("----------------------------------------");
                bool hasWarnings = _logger.PrintWarnings();
                if (hasWarnings)
                {
                    _logger.Log("Warnings present. Press any key to continue...");
                    Console.ReadKey();
                }
                else
                    _logger.Log("No warnings, proceeding...");
                _logger.Log();


                _logger.Log("----------------------------------------");
                // Ping the database again to ensur the connection is still alive.
                _logger.Log("Pinging database for connection again...");
                if (!_db.Ping())
                {
                    _logger.Log("ERROR: Lost connection to database.");
                    ExitPrompt();
                    return;
                }
                _logger.Log("Still connected to database...");
                _logger.Log();

                _logger.Log("----------------------------------------");
                _logger.Log("Removing old mod database entires for Steam ID: {0}", _mod.SteamId);
                RemoveModFromDB(_mod.SteamId);
                _logger.Log();

                _logger.Log("----------------------------------------");
                _logger.Log("Adding new entries to database...");
                AddToDatabase();
                _logger.Log();

                _logger.Log("----------------------------------------");
                _logger.Log("All done!");
            }
            catch (Exception e)
            {
                _logger.Log("\tAn exception has occured:");
                _logger.Log(e.Message);
            }

            ExitPrompt();
        }

        /// <summary>
        /// Because writing these two lines everywhere is annoying :^)
        /// </summary>
        static void ExitPrompt()
        {
            Console.WriteLine("Press any key to exit the program...");
            Console.ReadKey();
        }
    }
}
