using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFreshAdmin.Models.ViewModels
{
    public class SalesOverTimeVm
    {
        public long ReportType { get; set; }
        public long FromMonth { get; set; }
        public long ToMonth { get; set; }
        public long Year { get; set; }
        public long FromYear { get; set; }
        public long ToYear { get; set; }
    }
}