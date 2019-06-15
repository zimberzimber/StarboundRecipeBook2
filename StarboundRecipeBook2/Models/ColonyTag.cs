using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class ColonyTag
    {
        public int ColonyTagId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Relationship_ObjectData_ColonyTag> ObjectDatas { get; set; }
    }
}
