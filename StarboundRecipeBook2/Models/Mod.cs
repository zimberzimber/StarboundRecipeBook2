using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarboundRecipeBook2.Models
{
    public class Mod
    {
        const string STEAM_PAGE_URL = "https://steamcommunity.com/sharedfiles/filedetails/?id={0}";

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SteamId { get; set; } // PK
        public string FriendlyName { get; set; }
        public string InternalName { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<Item> AddedItems { get; set; }
        public virtual ICollection<Recipe> AddedRecipes { get; set; }

        public string SteamPageLink
        {
            get
            {
                return string.Format(STEAM_PAGE_URL, SteamId);
            }
        }
    }
}
