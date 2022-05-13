using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFreshAdmin.Models
{
    public class OrderGrowthRateVM
    {
        public DateTime? PriorFromDate { get; set; }
        public DateTime? PriorToDate { get; set; }
        public DateTime? CurrentFromDate { get; set; }
        public DateTime? CurrentToDate { get; set; }
        public double? OrderGrowthRate { get; set; }
    }
}