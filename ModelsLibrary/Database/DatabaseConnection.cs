using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using SBRB.Models;

namespace SBRB.Database
{
    public class DatabaseConnection
    {
        // TO DO: Move this to a separate file
        const string CONNECTION_STRING = "mongodb://localhost";

        // Database connection config file
        const string CONNECTION_FILE_PATH = "DatabaseConnection.config";

        // Default millisecond time for a pings timeout
        const int DEFAULT_PING_TIMEOUT = 3000;

        // Store the database reference for pinging
        IMongoDatabase _database;

        // Properties containing the collections
        public IMongoCollection<Mod> Mods { get; private set; }
        public IMongoCollection<Item> Items { get; private set; }
        public IMongoCollection<Recipe> Recipes { get; private set; }

        // Singleton defenition
        static object _lock = new object();
        static DatabaseConnection _instance = null;
        public static DatabaseConnection Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DatabaseConnection();
                }
                return _instance;
            }
        }

        // Private ctor for singleton definition
        DatabaseConnection()
        {
            // Register a convention to ignore nulls
            var pack = new ConventionPack { new IgnoreIfNullConvention(true) };
            ConventionRegistry.Register("remove nulls", pack, t => true);

            // connect to the database
            var client = new MongoClient(CONNECTION_STRING);
            _database = client.GetDatabase("test");

            // Bind the collections
            Mods = _database.GetCollection<Mod>("mods");
            Items = _database.GetCollection<Item>("items");
            Recipes = _database.GetCollection<Recipe>("recipes");
        }

        /// <summary>
        /// Ping the database, checking whether a connection is established.
        /// </summary>
        /// <param name="timeout">Milliseconds to wait for an answer.</param>
        /// <returns>Whether the ping was succesful.</returns>
        public bool Ping(int timeout = DEFAULT_PING_TIMEOUT)
            => _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(timeout);
    }
}