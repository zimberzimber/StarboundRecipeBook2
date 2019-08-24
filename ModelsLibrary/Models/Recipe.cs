using MongoDB.Bson.Serialization.Attributes;

namespace SBRB.Models
{
    /// <summary>
    /// Class used to compose a recipes unique ID within the database
    /// </summary>
    public class CompositeRecipeId:ModDependant
    {
        public uint RecipeId;
    }

    /// <summary>
    /// Class for storing a recipes inputs
    /// </summary>
    public class RecipeInputs
    {
        public string ItemName;
        public int Count;
    }

    /// <summary>
    /// Class for storing recipe data
    /// </summary>
    public class Recipe
    {
        [BsonId]
        public CompositeRecipeId ID;

        public string FilePath;
        public string[] RecipeGroups;

        public string OutputItemName;
        public int OutputCount;

        public RecipeInputs[] Inputs;
    }
}
