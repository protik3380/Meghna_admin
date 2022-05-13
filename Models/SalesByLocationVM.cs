using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFreshAdmin.Models.EntityModels;
using EFreshStore.Models.Context;

namespace EFreshAdmin.Models
{
    public class SalesByLocationVM
    {
        public List<Brand> Brands { get; set; }
        public List<Category> Categories { get; set; }
        public List<ProductType> ProductTypes { get; set; }
        public List<MasterDepot> MasterDepots { get; set; }
        public List<Thana> Thanas { get; set; }
        public List<District> Districts { get; set; }
        public long[] BrandIds { get; set; }
        public long[] CategoryIds { get; set; }
        public long[] ProductTypeIds { get; set; }
        public long[] MasterDepotIds { get; set; }
        public long[] ThanaIds { get; set; }
        public long[] DistrictIds { get; set; }
        public long? UserId { get; set; }
        //[DateGreaterThan("ToDate")]
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string OrderNo { get; set; }
        public long OrderId { get; set; }
        public string CustomerName { get; set; }
        public double TotalPrice { get; set; }
        public string Thana { get; set; }
        public string District { get; set; }
        public DateTime OrderDate { get; set; }
        public string MasterDepotName { get; set; }
    }
}