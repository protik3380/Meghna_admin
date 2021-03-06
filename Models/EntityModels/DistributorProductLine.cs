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
    using EFreshAdmin.Models.EntityModels;
    using System;
    using System.Collections.Generic;
    
    public partial class DistributorProductLine
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Please select distributor")]
        public Nullable<long> DistributorId { get; set; }

        [Required(ErrorMessage = "Please enter product line")]
        public Nullable<long> ProductLineId { get; set; }
    
        public virtual Distributor Distributor { get; set; }
        public virtual ProductLine ProductLine { get; set; }
    }
}
