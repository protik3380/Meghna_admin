//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFreshStore.Models.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductImage
    {
        public long Id { get; set; }
        public string ImageLocation { get; set; }
        public Nullable<long> ProductUnitId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual ProductUnit ProductUnit { get; set; }
    }
}
