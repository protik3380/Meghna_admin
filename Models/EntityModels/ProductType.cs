using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFreshStore.Models.Context;

namespace EFreshAdmin.Models.EntityModels
{
    public class ProductType
    {
        public ProductType()
        {
            this.Categories = new HashSet<Category>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}