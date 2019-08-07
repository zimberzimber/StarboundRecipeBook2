using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using SBRB.Database;
using SBRB.Models;
using System;

namespace SBRB.Seeder
{
    partial class Program
    {
        const string MOD_REMOVAL_QUERY = @"delete from Mods where SteamId = {0};
delete from ActiveItemDatas where SourceModId = {0};
delete from ConsumableDatas where SourceModId = {0};
delete from Items where SourceModId = {0};
delete from ObjectDatas where SourceModId = {0};
delete from RecipeInputs where SourceModId = {0};
delete from Recipes where SourceModId = {0};
delete from RecipeUnlocks where UnlockingItemSourceModId = {0};
delete from Relationship_Recipe_RecipeGroup where SourceModId = {0};";

        static DatabaseConnection _db;

        static void GetDatabaseConnection()
        {
            _db = new DatabaseConnection("mongodb://localhost");
        }

        static void RemoveModFromDB(uint modId)
        {
            Console.WriteLine("Removing Items...");
            var itemFilter = Builders<Item>.Filter.Eq(i => i.ID.SourceModId, modId);
            var itemResult = _db.Items.DeleteMany(itemFilter);
            Console.WriteLine(string.Format("Item result: {0}", itemResult.IsAcknowledged ? itemResult.DeletedCount.ToString() : "Didn't work"));

            Console.WriteLine("Removing Recipes...");
            var recipeFilter = Builders<Recipe>.Filter.Eq(r => r.ID.SourceModId, modId);
            var recipeResult = _db.Recipes.DeleteMany(recipeFilter);
            Console.WriteLine(string.Format("Recipe result: {0}", recipeResult.IsAcknowledged ? recipeResult.DeletedCount.ToString() : "Didn't work"));

            Console.WriteLine("Removing Mod...");
            var modResult = _db.Mods.DeleteOne(m => m.SteamId == modId);
            Console.WriteLine(string.Format("Mod result: {0}", modResult.IsAcknowledged ? modResult.DeletedCount.ToString() : "Didn't work"));
        }

        static void AddToDatabase()
        {
            if (_DBItems.Count > 0)
                _db.Items.InsertMany(_DBItems);

            if (_DBRecipes.Count > 0)
                _db.Recipes.InsertMany(_DBRecipes);

            _db.Mods.InsertOne(_mod);
        }
    }
}
