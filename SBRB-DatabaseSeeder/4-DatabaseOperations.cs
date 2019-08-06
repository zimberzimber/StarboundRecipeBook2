using Microsoft.EntityFrameworkCore;
using StarboundRecipeBook2.Data;
using System.Data.SqlClient;

namespace SBRB_DatabaseSeeder
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

        static void RemoveModFromDB(int modId)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseContext.CONNECTION_STRING))
            using (SqlCommand command = new SqlCommand(string.Format(MOD_REMOVAL_QUERY, modId), connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                    while (reader.Read()) { };
                connection.Close();
            }
        }

        static void AddToDatabase()
        {
            using (var db = new DatabaseContext(new DbContextOptions<DatabaseContext>()))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                //db.Mods.Add(_mod);

                //foreach (var item in _DBItems)
                //{ db.Items.Add(item); }

                //foreach (var item in _DBRecipes)
                //{ db.Recipes.Add(item); }

                var count = db.SaveChanges();
                Log("{0} records saved to database", count);
            }
        }
    }
}
