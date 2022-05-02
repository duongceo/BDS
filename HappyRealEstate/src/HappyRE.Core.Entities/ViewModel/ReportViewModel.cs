using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.ViewModel
{
    public class ReportSummaryViewModel
    {
        public int TotalProperty { get; set; }
        public int TotalCustomer { get; set; }
        public int TotalSaleOrder { get; set; }
        public int TotalUser { get; set; }

    }

    public class ReportTopUserViewModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Mobile { get; set; }
        public int Value { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }

    }

    public class ReportDailyViewModel
    {
        public string DateKey { get; set; }
        public int Total { get; set; }

    }
}
