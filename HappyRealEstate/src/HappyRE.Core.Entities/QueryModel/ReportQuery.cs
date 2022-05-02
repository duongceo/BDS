using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.QueryModel
{
    public class ReportQuery
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class ReportSaleOrderDailyQuery: ReportQuery
    {
        public int? OwnerTargetId { get; set; }
        public int? CustomerTargetId { get; set; }
        public string PostedBy { get; set; }
        public string SellBy { get; set; }
        public string Unit { get; set; }
    }

    public class ReportPropertyDailyQuery : ReportQuery
    {
        public int? TypeId { get; set; }
        public int? StatusId { get; set; }
        public string IsChecked { get; set; }
        public string UserName { get; set; }
        public string Unit { get; set; }
    }
}
