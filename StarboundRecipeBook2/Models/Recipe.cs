using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; } // PPK
        public int OutputCount { get; set; }
        public string FilePath { get; set; }

        public string OutputItemName { get; set; } // FK
        public virtual Item OutputItem { get; set; }

        public int SourceModId { get; set; } // PPK + FK
        public virtual Mod SourceMod { get; set; }

        public virtual ICollection<RecipeInput> RecipeInputs { get; set; }
        public virtual ICollection<Relationship_Recipe_RecipeGroup> RecipeGroups { get; set; }
    }
}
