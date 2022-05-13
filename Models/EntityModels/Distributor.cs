using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EFreshStore.Models.Context;

namespace EFreshAdmin.Models.EntityModels
{
    public sealed partial class Distributor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Distributor()
        {
            DistributorProductLines = new HashSet<DistributorProductLine>();
        }

        public long Id { get; set; }
        [Required(ErrorMessage = "Please enter name")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Please enter address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter contact person name")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Please enter mobile no")]
        [RegularExpression(@"^01[1-9]\d{8}$", ErrorMessage = "Please enter a valid mobile no. Ex. 01xxxxxxxxx")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select master depot")]
        public long? MasterDepotId { get; set; }

        [Required(ErrorMessage = "Please select thana")]
        public long? ThanaId { get; set; }       
        public MasterDepot MasterDepot { get; set; }
        public Thana Thana { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<DistributorProductLine> DistributorProductLines { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
