using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarboundRecipeBook2.Models
{
    public class RecipeGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string RecipeGroupName { get; set; } // PK

        public virtual ICollection<Relationship_Recipe_RecipeGroup> Recipes { get; set; }
    }
}
