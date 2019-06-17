using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarboundRecipeBook2.Models
{
    public class ConsumeableData
    {
        public int ConsumeableDataId { get; set; }
        public int FoodValue { get; set; }
        public int ItemId { get; set; }

        public virtual Item Item { get; set; }
        //public virtual ICollection<StatusEffect> StatusEffects { get; set; }
        //public virtual ICollection<IDKTBH> AppliedEffects { get; set; }
    }
}
