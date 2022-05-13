using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFreshAdmin.Models.CustomValidator;
using EFreshAdmin.Models.EntityModels;
using EFreshStore.Models.Context;

namespace EFreshAdmin.Models
{
    public class SalesByProductVM
    {
        public List<Brand> Brands { get; set; }
        public List<Category> Categories { get; set; }
        public List<ProductType> ProductTypes { get; set; }
        public long[] BrandIds { get; set; }
        public long[] CategoryIds { get; set; }
        public long[] ProductTypeIds { get; set; }
        //[DateGreaterThan("ToDate")]
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ProductName { get; set; }
        public string ProductUnit { get; set; }
        public int TotalProducts { get; set; }
        public double Price { get; set; }
        public long TotalOrders { get; set; }
        public string ProductTypeName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
    }
}