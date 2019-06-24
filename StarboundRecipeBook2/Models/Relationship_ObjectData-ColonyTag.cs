namespace StarboundRecipeBook2.Models
{
    public class Relationship_ObjectData_ColonyTag
    {
        public string ObjectItemName { get; set; } // PPK + FK
        public string ColonyTagName { get; set; } // PPK + FK

        public virtual ObjectData ObjectData { get; set; }
        public virtual ColonyTag ColonyTag { get; set; }
    }
}
