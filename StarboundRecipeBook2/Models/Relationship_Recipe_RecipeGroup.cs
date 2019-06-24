namespace StarboundRecipeBook2.Models
{
    public class Relationship_Recipe_RecipeGroup
    {
        public int RecipeId { get; set; } // PPK + FK
        public int SourceModId { get; set; } // PPK
        public string RecipeGroupName { get; set; } // PPK + FK

        public virtual Recipe Recipe { get; set; }
        public virtual RecipeGroup RecipeGroup { get; set; }
    }
}
