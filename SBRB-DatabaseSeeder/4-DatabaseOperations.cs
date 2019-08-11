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

        static void GetDatabaseConnection()
            => _db = new DatabaseConnection();

        static void RemoveModFromDB(uint modId)
        {
            Logging.Log("Removing old entries for mod {0}...", modId);

            var itemFilter = Builders<Item>.Filter.Eq(i => i.ID.SourceModId, modId);
            var itemTask = _db.Items.DeleteManyAsync(itemFilter);

            var recipeFilter = Builders<Recipe>.Filter.Eq(r => r.ID.SourceModId, modId);
            var recipeTask = _db.Recipes.DeleteManyAsync(recipeFilter);

            var modTask = _db.Mods.DeleteOneAsync(m => m.SteamId == modId);

            Task.WaitAll(itemTask, recipeTask, modTask);

            Logging.Log("Done removing old entries.");
        }

        static void AddToDatabase()
        {
            Logging.Log("Adding new entries...");

            Task itemTask;
            Task recipeTask;

            if (_DBItems.Count > 0)
                itemTask = _db.Items.InsertManyAsync(_DBItems);
            else
                itemTask = Task.CompletedTask;

            if (_DBRecipes.Count > 0)
                recipeTask = _db.Recipes.InsertManyAsync(_DBRecipes);
            else
                recipeTask = Task.CompletedTask;

            Task modTask = _db.Mods.InsertOneAsync(_mod);

            Task.WaitAll(itemTask, recipeTask, modTask);

            Logging.Log("Done adding new entries.");
        }
    }
}
