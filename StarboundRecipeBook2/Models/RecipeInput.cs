namespace StarboundRecipeBook2.Models
{
    public class RecipeInput
    {
        public int RecipeInputId { get; set; } // PPK
        public int SourceModId { get; set; } // PPK + FK

        public string InputItemName { get; set; } // (Not linking to an item because the target item may be outside of this mod)
        public int InputCount { get; set; }

        public int RecipeId { get; set; } // FK
        public virtual Recipe Recipe { get; set; }
    }
}
