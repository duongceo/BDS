using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HappyRE.Core.MapModels.WardReview;
namespace HappyRE.Web.Models
{
    public class WardRelationViewModel
    {
        public List<WardRelation> Wards { get; set; }
        public string DistrictName { get; set; }
        public string WardReviewUrl { get; set; }
    }
}