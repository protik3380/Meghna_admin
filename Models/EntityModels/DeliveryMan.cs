using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EFreshStore.Models.Context;

namespace EFreshAdmin.Models.EntityModels
{
    public class DeliveryMan
    {
        public DeliveryMan()
        {
            this.MasterDepotDeliveryMen = new HashSet<MasterDepotDeliveryMan>();
            this.AssignOrders = new HashSet<AssignOrder>();
        }

        public long Id { get; set; }
        [Required(ErrorMessage = "Please enter name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter mobile no")]
        [RegularExpression(@"^01[1-9]\d{8}$", ErrorMessage = "Please enter a valid mobile no. Ex. 01xxxxxxxxx")]
        public string MobileNo { get; set; }
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }
        public string Address { get; set; }
        public long? UserId { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid NID")]
        public string NID { get; set; }
        public long? CurrentUserId { get; set; }
        public HttpPostedFileBase ImageLocation { get; set; }
        public string ImageUrl { get; set; }
        public byte[] ImageByte { get; set; }
        [Required(ErrorMessage = "Please select master depot")]
        public long[] MasterDepotIds { get; set; }
        public Nullable<long> ThanaId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public virtual User User { get; set; }
        public virtual Thana Thana { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MasterDepotDeliveryMan> MasterDepotDeliveryMen { get; set; }
        public virtual ICollection<AssignOrder> AssignOrders { get; set; }
    }
}