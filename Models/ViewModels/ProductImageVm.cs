using EFreshStore.Models.Context;
using System;
using System.Web;

namespace EFreshStore.Models.ViewModels
{
    public class ProductImageVm
    {
        public long Id { get; set; }
        public HttpPostedFileBase ImageLocation { get; set; }
        public Nullable<long> ProductUnitId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public virtual ProductUnit ProductUnit { get; set; }
    }
}