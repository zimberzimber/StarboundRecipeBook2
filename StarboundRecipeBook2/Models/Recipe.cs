using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public int OutputCount { get; set; }
        public int OutputItemId { get; set; }
        public int SourceModId { get; set; }
        public string FilePath { get; set; }

        public virtual Item OutputItem { get; set; }
        public virtual Mod SourceMod { get; set; }

        public virtual ICollection<RecipeInput> RecipeInputs { get; set; }
        public virtual ICollection<Relationship_Recipe_RecipeGroup> RecipeGroups { get; set; }
    }
}
