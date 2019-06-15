using System.Collections.Generic;

namespace StarboundRecipeBook2.Models
{
    public class ObjectData
    {
        public int ObjectDataId { get; set; }
        public string Race { get; set; }
        public bool Printable { get; set; }

        public virtual Item Item { get; set; }
        public virtual ICollection<Relationship_ObjectData_ColonyTag> ColonyTags { get; set; }
    }
}
