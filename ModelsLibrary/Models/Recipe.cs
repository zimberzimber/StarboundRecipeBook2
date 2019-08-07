using MongoDB.Bson.Serialization.Attributes;

namespace SBRB.Models
{
    /// <summary>
    /// Class used to compose a recipes unique ID within the database
    /// </summary>
    public class CompositeRecipeId
    {
        public uint RecipeId { get; set; }
        public uint SourceModId { get; set; }
    }

    /// <summary>
    /// Class for storing a recipes inputs
    /// </summary>
    public class RecipeInputs
    {
        public string ItemName { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// Class for storing recipe data
    /// </summary>
    public class Recipe
    {
        [BsonId]
        public CompositeRecipeId ID { get; set; }

        public string FilePath { get; set; }
        public string[] RecipeGroups { get; set; }

        public string OutputItemName { get; set; }
        public int OutputCount { get; set; }

        public RecipeInputs[] Inputs { get; set; }
    }
}
