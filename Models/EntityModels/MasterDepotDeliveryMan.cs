using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFreshStore.Models.Context;

namespace EFreshAdmin.Models.EntityModels
{
    public class MasterDepotDeliveryMan
    {
        public long Id { get; set; }
        public Nullable<long> DeliveryManId { get; set; }
        public Nullable<long> MasterDepotId { get; set; }

        public virtual DeliveryMan DeliveryMan { get; set; }
        public virtual MasterDepot MasterDepot { get; set; }
    }
}