using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using SBRB.Models;

namespace SBRB.Database
{
    public class DatabaseConnection
    {
        public IMongoCollection<Mod> Mods { get; set; }
        public IMongoCollection<Item> Items { get; set; }
        public IMongoCollection<Recipe> Recipes { get; set; }

        public DatabaseConnection(string connectionString)
        {
            // Register a convention to ignore nulls
            var pack = new ConventionPack { new IgnoreIfNullConvention(true) };
            ConventionRegistry.Register("remove nulls", pack, t => true);

            // connect to the database
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("test");

            // Bind the collections
            Mods = database.GetCollection<Mod>("mods");
            Items = database.GetCollection<Item>("items");
            Recipes = database.GetCollection<Recipe>("recipes");
        }
    }
}
