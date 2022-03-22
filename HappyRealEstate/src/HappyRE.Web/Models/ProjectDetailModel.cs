using HappyRE.Core.MapModels.FrontEnd;
using HappyRE.Core.MapModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class ProjectDetailModel
    {
        public ProjectDetailModel()
        {
           // Projects = new List<ProjectItem>();
        }
        public ProjectDetail ProjectDetail { get; set; }
        public List<ProjectItem> Projects { get; set; }
        // Similar Property Buy
        public List<PropertyItem> SaleItems { get; set; }
        // Similar Property rent
        public List<PropertyItem> RentItems { get; set; }

        public List<SampleHouseModel> SampleHouses { get; set; }

        public int TotalRent { get; set; }
        public int TotalSale { get; set; }

        public string[] SaleFavorited { get; set; }
        public string[] RentFavorited { get; set; }
        public bool HasRent()
        {
            return TotalRent > 0;
        }
        public bool HasSale()
        {
            return TotalSale > 0;
        }
    }
    //public class SampleHouseModel
    //{
    //    public SampleHouseModel()
    //    {
    //        Images = new List<SlideImage>();
    //    }
    //    public string Title { get; set; }
    //    public List<SlideImage> Images { get; set; }
    //}

    //public class SlideImage
    //{
    //    public string src { get; set; }
    //    public string h { get; set; }
    //    public string w { get; set; }
    //}
}