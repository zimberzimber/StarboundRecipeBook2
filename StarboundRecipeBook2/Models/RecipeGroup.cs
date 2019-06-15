using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class RecipeGroup
    {
        public int RecipeGroupdId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Relationship_Recipe_RecipeGroup> Recipes { get; set; }
    }
}
