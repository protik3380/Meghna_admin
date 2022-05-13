using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFreshAdmin.Models.ViewModels
{
    public class OrderVsMonthOrYearVm
    {
        public long Month { get; set; }
        public long Year { get; set; }
        public double OrderRate { get; set; }
    }
}