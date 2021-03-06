//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using EFreshAdmin.Models.CustomValidator;
using EFreshStore.Models.Context;

namespace EFreshStoreCore.Model.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductDiscount
    {
        public long Id { get; set; }

        //[Required(ErrorMessage = "Please select product unit")]
        public Nullable<long> ProductUnitId { get; set; }

        //[Required(ErrorMessage = "Please enter discount percentage"), Range(0, 100, ErrorMessage = "Max/Min limit exceeded.")]
        public Nullable<decimal> DiscountPercentage { get; set; }
        //[Required(ErrorMessage = "Please enter end date")]
        //[DateGreaterThan("ActiveDate")]
        public Nullable<System.DateTime> Validity { get; set; }

        //[Required(ErrorMessage = "Please enter start date")]
        //[GreaterThan("StartDate")]
        public Nullable<System.DateTime> ActiveDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual ProductUnit ProductUnit { get; set; }
        //IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        //{
        //    List<ValidationResult> res = new List<ValidationResult>();
        //    if (ActiveDate > Validity)
        //    {
        //        ValidationResult dateError = new ValidationResult(errorMessage: "End Date must be greater than Start Date",
        //                               memberNames: new[] { "Validity" });
        //        res.Add(dateError);
        //    }
        //    return res;
        //}
    }
}
