using HappyRE.Core.MapModels.WardReview;
using HappyRE.Core.Entities;
using System.Collections.Generic;

namespace HappyRE.Web.Models
{
    public class WardReviewViewModel
    {
        public string WardName { get; set; }
        public WardReview WardReview { get; set; }
        public List<WardPlaceSummarize> WardPlaces { get; set; }
        public List<WardPlaceRating> Schools { get; set; }
        public string SaleUrl { get; set; }
        public string RentUrl { get; set; }
        public int CityId { get; set; }
        public int WardId { get; set; }
        public int DistrictId { get; set; }
        public string MapUrl { get; set; }
    }
}