namespace StarboundRecipeBook2.Models
{
    public class RecipeInput
    {
        public int RecipeInputId { get; set; }
        public int InputCount { get; set; }
        public int InputItemId { get; set; }
        public int RecipeId { get; set; }

        public virtual Item InputItem { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
