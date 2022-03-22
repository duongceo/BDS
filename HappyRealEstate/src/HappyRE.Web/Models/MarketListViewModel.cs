using HappyRE.Core.MapModels.Report;
using System.Collections.Generic;

namespace HappyRE.Web.Models
{
    public class MarketListViewModel
    {
        public List<DistrictHousePrice> Hanoi { get; set; }
        public List<DistrictHousePrice> HCM { get; set; }
        public bool HasHanoi()
        {
            return this.Hanoi != null && this.Hanoi.Count > 0;
        }
        public bool HasHCM()
        {
            return this.HCM != null && this.HCM.Count > 0;
        }
    }
}