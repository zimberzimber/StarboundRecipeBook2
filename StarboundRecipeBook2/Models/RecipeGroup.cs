using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class RecipeGroup
    {
        public string RecipeGroupName { get; set; } // PK

        public virtual ICollection<Relationship_Recipe_RecipeGroup> Recipes { get; set; }
    }
}
