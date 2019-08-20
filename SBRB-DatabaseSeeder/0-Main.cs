using Jil;
using SBRB.Models;
using SBRB.Seeder.DeserializedData;
using SBRB_DatabaseSeeder;
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

        public static string modPath;
        static Mod _mod;
        static Logger _logger = Logger.Instance;

        static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                _logger.Log("Program started without any arguements.");
                _logger.Log("Press any key to exit the program.");
                Console.ReadKey();
                return;
            }

            modPath = args[0];
            //modPath = @"D:\Games\steamapps\common\Starbound\mods\_FrackinUniverse-master";

            JSON.SetDefaultOptions(Options.ExcludeNulls);
            string metaString;

            try
            {
                if (File.Exists($"{modPath}\\.metadata"))
                    metaString = File.ReadAllText($"{modPath}\\.metadata");
                else if (File.Exists($"{modPath}\\_metadata"))
                    metaString = File.ReadAllText($"{modPath}\\_metadata");
                else
                {
                    _logger.Log("No metadata file detected.");
                    _logger.Log("Press any key to exit...");
                    Console.ReadKey();
                    return;
                }

                Metadata meta = JSON.Deserialize<Metadata>(metaString);

                if (string.IsNullOrWhiteSpace(meta.steamContentId))
                {
                    if (meta.author == "Chucklefish" && meta.name == "base")
                    {
                        _logger.Log("Base game assets. ID is set to {0}.", BASE_GAME_ASSETS_STEAM_ID);
                        meta.steamContentId = BASE_GAME_ASSETS_STEAM_ID;
                    }
                    else
                    {
                        _logger.Log("No Steam ID detected. Press any key to exit program.");
                        Console.ReadKey();
                        return;
                    }
                }
                else
                    _logger.Log($"Accepted mod with Steam ID {meta.steamContentId}");
                _logger.Log();

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
                _logger.Log("Creating database connection...");
                _logger.Log();
                GetDatabaseConnection();

                _logger.Log("----------------------------------------");
                RemoveModFromDB(_mod.SteamId);
                _logger.Log();

                _logger.Log("----------------------------------------");
                AddToDatabase();
                _logger.Log();

                _logger.Log("----------------------------------------");
                _logger.Log("All done!");
            }
            catch (Exception e)
            {
                _logger.Log("\tAn error has occured:");
                _logger.Log(e.Message);
            }
            _logger.Log("Press any key to exit the program...");
            Console.ReadKey();
        }
    }
}
