using StarboundRecipeBook2.Models;
using System;

namespace SBRB_DatabaseSeeder.DeserializedData
{
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
                AddedItems = null,
                AddedRecipes = null,
                FriendlyName = friendlyName,
                InternalName = name,
                LastUpdated = DateTime.Now,
                SteamId = int.Parse(steamContentId),
                Version = version,
                Author = author
            };
        }
    }
}
