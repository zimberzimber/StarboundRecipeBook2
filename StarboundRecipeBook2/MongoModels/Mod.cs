using MongoDB.Bson.Serialization.Attributes;
using System;

namespace StarboundRecipeBook2.MongoModels
{
    /// <summary>
    /// Class for storing mod meta data
    /// </summary>
    public class Mod
    {
        const string STEAM_PAGE_URL = "https://steamcommunity.com/sharedfiles/filedetails/?id={0}";

        [BsonId]
        public uint SteamId { get; set; }

        public string FriendlyName { get; set; }

        public string InternalName { get; set; }

        public string Author { get; set; }

        public string Version { get; set; }

        public DateTime LastUpdated { get; set; }

        public string SteamPageLink { get { return string.Format(STEAM_PAGE_URL, SteamId); } }
    }
}
