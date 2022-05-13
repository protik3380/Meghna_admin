using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFreshStore.Models
{
    public class CartView
    {
        public long ProductUnitId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
    }
}