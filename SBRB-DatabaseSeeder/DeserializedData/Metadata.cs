using SBRB.Models;
using System;

namespace SBRB.Seeder.DeserializedData
{
    /// <summary>Class storing a deserialized .metadata file. Converted into the 'Mod' model.</summary>
    class Metadata
    {
        public string steamContentId { get; set; }
        public string name { get; set; }
        public string author { get; set; }
        public string friendlyName { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string version { get; set; }
        public string tags { get; set; }
        public string[] includes { get; set; }
        public string[] requires { get; set; }

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
