
using System;
using System.ComponentModel.DataAnnotations;
using EFreshStore.Models.Context;
namespace EFreshAdmin.Models.EntityModels
{
    public class Customer
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }
        public string MobileNo { get; set; }
        [Required]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter delivery address 1")]
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public long? UserId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public virtual User User { get; set; }
    }
}
