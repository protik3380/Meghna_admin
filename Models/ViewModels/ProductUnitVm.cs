using EFreshStore.Models.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using EFreshStoreCore.Model.Context;

namespace EFreshStore.Models.ViewModels
{
    public class ProductUnitVm
    {
        public ProductUnitVm()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.ProductImages = new HashSet<ProductImageVm>();
            this.ProductUnitPrices = new HashSet<ProductUnitPrice>();
            this.ImageBytes = new List<byte[]>();
        }

        public long Id { get; set; }
        [Required]
        public Nullable<long> ProductId { get; set; }
        [Required]
        public string StockKeepingUnit { get; set; }
        public HttpPostedFileBase ImageLocation { get; set; }
        [Required]
        public string CartonSize { get; set; }
        [Required]
        public string CartonSizeUnit { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid float Number")]
        public Nullable<decimal> DistributorPricePerCarton { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid float Number")]
        public Nullable<decimal> TradePricePerCarton { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid float Number")]
        public Nullable<decimal> MaximumRetailPrice { get; set; }
        public bool Question { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
       
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Product Product { get; set; }        
        public virtual ICollection<ProductImageVm> ProductImages { get; set; }        
        public virtual ICollection<ProductUnitPrice> ProductUnitPrices { get; set; }

        public ICollection<byte[]> ImageBytes { get; set; }
    }
}