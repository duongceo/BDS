using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class HousingAvgPriceViewModel
    {
        public HousingAvgPriceViewModel()
        {
            Properties = new List<Core.MapModels.Report.HousePrice>();
        }
        public int Total { get; set; }
        public List<Core.MapModels.Report.HousePrice> Properties { get; set; }
    }
}