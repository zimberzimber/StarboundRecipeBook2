using MongoDB.Driver;
using SBRB.Database;
using SBRB.Models;
using SBRB_DatabaseSeeder.Workers;
using System.Threading.Tasks;

namespace SBRB.Seeder
{
    partial class Program
    {
        static DatabaseConnection _db;

        /// <summary>
        /// Get the database connection.
        /// </summary>
        static void GetDatabaseConnection()
            => _db = new DatabaseConnection();

        /// <summary>
        /// Remove all entries for the given mod ID.
        /// </summary>
        /// <param name="modId">The mod whose data should be removed.</param>
        static void RemoveModFromDB(uint modId)
        {
            Logging.Log("Removing old entries for mod {0}...", modId);

            // Remove items who's ID.SourceModId matches the received ID
            var itemFilter = Builders<Item>.Filter.Eq(i => i.ID.SourceModId, modId);
            var itemTask = _db.Items.DeleteManyAsync(itemFilter);

            // Remove recipes who's ID.SourceModId matches the received ID
            var recipeFilter = Builders<Recipe>.Filter.Eq(r => r.ID.SourceModId, modId);
            var recipeTask = _db.Recipes.DeleteManyAsync(recipeFilter);

            // Remove the mod whos SteamId matches the received ID
            var modTask = _db.Mods.DeleteOneAsync(m => m.SteamId == modId);

            // Wait for the removal tasks to complete
            Task.WaitAll(itemTask, recipeTask, modTask);

            Logging.Log("Done removing old entries.");
        }

        /// <summary>
        /// Add the database appropriate data to the database.
        /// </summary>
        static void AddToDatabase()
        {
            Logging.Log("Adding new entries...");

            // Placeholders for insertion tasks
            Task itemTask;
            Task recipeTask;

            // Asynchronously insert the items, or just create a complete task if there are no items to be added.
            if (_DBItems.Count > 0)
                itemTask = _db.Items.InsertManyAsync(_DBItems);
            else
                itemTask = Task.CompletedTask;

            // Asynchronously insert the recipes, or just create a complete task if there are no recipes to be added.
            if (_DBRecipes.Count > 0)
                recipeTask = _db.Recipes.InsertManyAsync(_DBRecipes);
            else
                recipeTask = Task.CompletedTask;

            // Asynchronously insert the mod.
            Task modTask = _db.Mods.InsertOneAsync(_mod);

            // Wait for the insertion tasks to complete.
            Task.WaitAll(itemTask, recipeTask, modTask);

            Logging.Log("Done adding new entries.");
        }
    }
}
