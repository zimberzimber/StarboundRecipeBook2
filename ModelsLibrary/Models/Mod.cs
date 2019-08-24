using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SBRB.Models
{
    /// <summary>
    /// Class for storing mod meta data
    /// </summary>
    public class Mod
    {
        const string STEAM_PAGE_URL = "https://steamcommunity.com/sharedfiles/filedetails/?id={0}";

        [BsonId]
        public uint SteamId;

        public string FriendlyName;

        public string InternalName;

        public string Author;

        public string Version;

        public DateTime LastUpdated;

        public string SteamPageLink { get { return string.Format(STEAM_PAGE_URL, SteamId); } }
    }
}
