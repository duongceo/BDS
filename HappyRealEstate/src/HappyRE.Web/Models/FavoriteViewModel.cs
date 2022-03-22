using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class FavoriteViewModel
    {
        public FavoriteViewModel()
        {
            RentProperties = new List<Core.MapModels.Search.PropertyItem>();
            BuyProperties = new List<Core.MapModels.Search.PropertyItem>();
        }
        public List<Core.MapModels.Search.PropertyItem> RentProperties;
        public List<Core.MapModels.Search.PropertyItem> BuyProperties;
        public Core.MapModels.Paging RentPaging { get; set; }
        public Core.MapModels.Paging BuyPaging { get; set; }
    }
}