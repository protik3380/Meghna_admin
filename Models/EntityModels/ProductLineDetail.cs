//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace EFreshStore.Models.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductLineDetail
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Please select product line")]
        public Nullable<long> ProductLineId { get; set; }

        [Required(ErrorMessage = "Please select product")]
        public Nullable<long> ProductId { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual ProductLine ProductLine { get; set; }
    }
}
