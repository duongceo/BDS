using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class SearchAlertViewModel
    {
        public List<HappyRE.Core.MapModels.Search.PropertyItem> Alerts { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public int Frequency { get; set; }
    }
}