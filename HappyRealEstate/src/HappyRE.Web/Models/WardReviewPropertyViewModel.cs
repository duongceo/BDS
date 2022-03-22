using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class WardReviewPropertyViewModel
    {
        public string WardName { get; set; }
        public List<HappyRE.Core.MapModels.Search.PropertyItem> Sale { get; set; }
        public List<HappyRE.Core.MapModels.Search.PropertyItem> Rent { get; set; }
    }
}