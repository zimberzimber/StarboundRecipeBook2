namespace StarboundRecipeBook2.Models
{
    public class RecipeInput
    {
        public int RecipeInputId { get; set; } // PPK
        public int InputCount { get; set; }

        public int SourceModId { get; set; } // PPK

        public string InputItemName { get; set; } // FK
        public virtual Item InputItem { get; set; }

        public int RecipeId { get; set; } // FK
        public virtual Recipe Recipe { get; set; }
    }
}
