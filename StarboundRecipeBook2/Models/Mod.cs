using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarboundRecipeBook2.Models
{
    public class Mod
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SteamId { get; set; }
        public string FriendlyName { get; set; }
        public string InternalName { get; set; }
        public string Version { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<Item> AddedItems { get; set; }
        public virtual ICollection<Recipe> AddedRecipes { get; set; }
    }
}
