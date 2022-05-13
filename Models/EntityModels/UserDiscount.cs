using System;
using System.ComponentModel.DataAnnotations;

namespace EFreshAdmin.Models.EntityModels
{
    public class UserDiscount
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter discount percentage"), Range(0, 100, ErrorMessage = "Max/Min limit exceeded.")]
        public decimal? DiscountPercentage { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
    }
}