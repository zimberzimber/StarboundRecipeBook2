namespace StarboundRecipeBook2.Models
{
    public class RecipeInput
    {
        public int RecipeInputId { get; set; } // PPK
        public int InputCount { get; set; }
        public string InputItemName { get; set; } // FK (Not linking to an item because the target item may be outside of this mod)

        public int SourceModId { get; set; } // PPK

        public int RecipeId { get; set; } // FK
        public virtual Recipe Recipe { get; set; }
    }
}
