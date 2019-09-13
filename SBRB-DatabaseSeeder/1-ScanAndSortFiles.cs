using SBRB.Seeder.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SBRB.Seeder
{
    partial class Program
    {
        // Array telling the program which extensions are to be considered as items
        static readonly string[] ACCEPTABLE_ITEM_EXTENSIONS = new string[]
        {   ".item", ".object", ".activeitem", ".legs", ".chest", ".head",
            ".back", ".consumable", ".beamaxe", ".flashlight", ".miningtool",
            ".harvestingtool", ".painttool", ".wiretool", ".inspectiontool",
            ".tillingtool", ".augment", ".currency", ".instrument", ".liquid",
            ".matitem", ".throwitem" };

        // String telling the program which extension is to be considered as a recipe
        const string RECIPE_FILE_EXTENSION = ".recipe";

        // String telling the program which extension is to be considered as a patch
        //const string PATCH_FILE_EXTENSION = ".patch";
        const string PATCH_FILE_EXTENSION = "?"; // <- Files can't have a question mark in their name :^)

        // Queues containing discovered items and recipes
        static ConcurrentQueue<string> _itemFiles = new ConcurrentQueue<string>();
        static ConcurrentQueue<string> _recipeFiles = new ConcurrentQueue<string>();
        static ConcurrentQueue<string> _patchFiles = new ConcurrentQueue<string>();

        /// <summary>
        /// Recursive function iterating through files and folders within the given directory.
        /// Folders have this method applied to them.
        /// Files have the SortFile method aplied to them.
        /// </summary>
        /// <param name="directory">The directory to iterate over</param>
        static void ScanFiles(string directory)
        {
            // Get directories and files within the current directory
            string[] directories = Directory.GetDirectories(directory);
            string[] files = Directory.GetFiles(directory);

            // Create a list containing all the tasks initialized within this stack.
            List<Task> tasks = new List<Task>();

            // Create, index, and start tasks to run the same method on all the subdirectories
            foreach (var dir in directories)
            {
                var task = new Task(() => ScanFiles(dir));
                tasks.Add(task);
                task.Start();
            }

            // Create, index, and start tasks to sort files within the current directory
            foreach (var file in files)
            {
                var task = new Task(() => SortFile(file));
                tasks.Add(task);
                task.Start();
            }

            // Wait for the tasks to complete
            Task.WaitAll(tasks.ToArray());

            var z = 5;
        }

        /// <summary>
        /// Method receiving a file, and adding it into an item or recipe queue should it fit the requirements.
        /// </summary>
        /// <param name="file">Path to file</param>
        static void SortFile(string file)
        {
            // Get the files extension
            string extension = Path.GetExtension(file);

            // Check whether the files extension falls under a recipe or item category, and add them into the appropriate queue.
            // If the extension matches to that of a patch file, check if its subextension also matches the above criteria.
            // Do nothing if its an unaccaptable extension.
            if (extension.Equals(RECIPE_FILE_EXTENSION))
            {
                _recipeFiles.Enqueue(file);
                _logger.Log("Found recipe file:\t{0}", file.TrimPath(modPath));
            }
            else if (extension.Equals(PATCH_FILE_EXTENSION))
            {
                // Get the sub extansion
                string fileName = Path.GetFileNameWithoutExtension(file);
                string subExtension = Path.GetExtension(fileName);

                if (subExtension.Equals(RECIPE_FILE_EXTENSION) || ACCEPTABLE_ITEM_EXTENSIONS.Contains(subExtension))
                {
                    _patchFiles.Enqueue(file);
                    _logger.Log("Found patch file:\t{0}", file.TrimPath(modPath));
                }
            }
            else if (ACCEPTABLE_ITEM_EXTENSIONS.Contains(extension))
            {
                _itemFiles.Enqueue(file);
                _logger.Log("Found item file:\t{0}", file.TrimPath(modPath));
            }
        }
    }
}
