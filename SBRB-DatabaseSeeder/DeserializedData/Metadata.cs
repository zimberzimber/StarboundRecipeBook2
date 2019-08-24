using SBRB.Models;
using System;

namespace SBRB.Seeder.DeserializedData
{
    /// <summary>Class storing a deserialized .metadata file. Converted into the 'Mod' model.</summary>
    class Metadata
    {
        public string steamContentId;
        public string name;
        public string author;
        public string friendlyName;
        public string description;
        public string link;
        public string version;
        public string tags;
        public string[] includes;
        public string[] requires;

        public Mod ToMod()
        {
            return new Mod
            {
                Author = author,
                FriendlyName = friendlyName,
                InternalName = name,
                LastUpdated = DateTime.Now,
                SteamId = uint.Parse(steamContentId),
                Version = version
            };
        }
    }
}
