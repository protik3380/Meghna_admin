//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using EFreshStoreCore.Model.Context;

namespace EFreshStore.Models.Context
{
    using EFreshAdmin.Models.EntityModels;
    using System;
    using System.Collections.Generic;
    
    public partial class MasterDepot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MasterDepot()
        {
            this.Distributors = new HashSet<Distributor>();
            this.MasterDepotProductLines = new HashSet<MasterDepotProductLine>();
            this.Orders = new HashSet<Order>();
            this.MasterDepotDeliveryMen = new HashSet<MasterDepotDeliveryMan>();
            this.ThanaWiseMasterDepots = new HashSet<ThanaWiseMasterDepot>();
        }
    
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter depot name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter contact person name")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Please enter mobile no")]
        [RegularExpression(@"^01[1-9]\d{8}$", ErrorMessage = "Please enter a valid mobile no. Ex. 01xxxxxxxxx")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Distributor> Distributors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MasterDepotProductLine> MasterDepotProductLines { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThanaWiseMasterDepot> ThanaWiseMasterDepots { get; set; }
        public virtual ICollection<MasterDepotDeliveryMan> MasterDepotDeliveryMen { get; set; }
    }
}
