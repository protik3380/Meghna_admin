using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EFreshStore.Models.Context;

namespace EFreshAdmin.Models.EntityModels
{
    public class Coupon
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Please enter coupon code")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Please enter discount percentage")]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public Nullable<decimal> DiscountPercentage { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public Nullable<decimal> MaximumDiscount { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public Nullable<decimal> MinimumOrderAmount { get; set; }
        public Nullable<System.DateTime> Validity { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public Nullable<int> MaximumOrderNo { get; set; }
        public Nullable<long> UserTypeId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public virtual UserType UserType { get; set; }
    }
}