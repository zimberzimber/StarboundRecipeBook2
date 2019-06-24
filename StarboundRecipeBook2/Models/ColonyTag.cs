using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class ColonyTag
    {
        public string ColonyTagName { get; set; } // PK

        public virtual ICollection<Relationship_ObjectData_ColonyTag> ObjectDatas { get; set; }
    }
}
