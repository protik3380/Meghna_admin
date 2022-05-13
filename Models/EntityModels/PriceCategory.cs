using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFreshAdmin.Models.EntityModels
{
    public class PriceCategory
    {
        public PriceCategory()
        {
            this.LPGComboDiscounts = new HashSet<LPGComboDiscount>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public virtual ICollection<LPGComboDiscount> LPGComboDiscounts { get; set; }
    }
}