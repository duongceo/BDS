using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class WardReviewPropertyTypeViewModel
    {
        public string WardName { get; set; }
        public List<HappyRE.Core.MapModels.LinkItem> Sale { get; set; }
        public List<HappyRE.Core.MapModels.LinkItem> Rent { get; set; }
        public string SaleUrl { get; set; }
        public string RentUrl { get; set; }
    }
}