namespace StarboundRecipeBook2.Models
{
    public class Relationship_Recipe_RecipeGroup
    {
        public int RecipeId { get; set; }
        public int RecipeGroupId { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual RecipeGroup RecipeGroup { get; set; }
    }
}
