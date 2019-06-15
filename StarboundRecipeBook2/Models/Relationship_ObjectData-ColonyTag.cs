namespace StarboundRecipeBook2.Models
{
    public class Relationship_ObjectData_ColonyTag
    {
        public int ObjectDataId { get; set; }
        public int ColonyTagId { get; set; }

        public virtual ObjectData ObjectData { get; set; }
        public virtual ColonyTag ColonyTag { get; set; }
    }
}
