using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFreshAdmin.Models.ViewModels
{
    public class AnalysisReportParams
    {
        public long[] BrandIds { get; set; }
        public long[] CategoryIds { get; set; }
        public long[] ProductTypeIds { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}