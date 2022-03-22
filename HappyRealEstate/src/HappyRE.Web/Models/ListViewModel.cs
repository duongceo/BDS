using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HappyRE.Core.MapModels.Search;

namespace HappyRE.Web.Models
{
    [Serializable]
    public class ListViewModel
    {
		public bool IsMobileDevice = false;
		public string Version = string.Empty;

		public Dictionary<string, string> Msg = new Dictionary<string, string>();
        public int Total { get; set; }
        public Core.MapModels.Paging Paging { get; set; }
		public List<PropertyItem> Data { get; set; } = null;
		public List<PropertyItem> TopItems { get; set; } = null;
		public Core.MapModels.SearchFilter Filter { get; set; } = new Core.MapModels.SearchFilter();
		public string ListStyle { get; set; } = "list";
		public string[] Favorites { get; set; } = null;
		public string[] FavoritesTop { get; set; } = null;

		public string AlertSearch = "null";

        public bool IsList()
        {
            return this.ListStyle == "list";
        }
        public bool IsMap()
        {
            return this.ListStyle == "map";
        }

        public string GetResult()
        {
            if (this.Total == 0) return Core.Resources.Message.Listing_Result_NotFound;

            int startIndex = 1 + Math.Max(0, (this.Paging.CurrentPage - 1)) * this.Paging.PageSize;
            int endIndex = Math.Min(this.Total, startIndex + this.Paging.PageSize - 1);
            return string.Format(Core.Resources.Message.Listing_Result,
                startIndex.ToString("N0"), endIndex.ToString("N0"), this.Total.ToString("N0"));
        }

        public string GetResultMap()
        {
            if (this.Total == 0) return Core.Resources.Message.Listing_Result_NotFound;

            int startIndex = 1 + Math.Max(0, (this.Paging.CurrentPage - 1)) * this.Paging.PageSize;
            int endIndex = Math.Min(this.Total, startIndex + this.Paging.PageSize - 1);
            return string.Format(Core.Resources.Message.Listing_ResultMap,
                startIndex.ToString("N0"), endIndex.ToString("N0"), this.Total.ToString("N0"));
        }

		public string GetIds() {
			return string.Join(",", this.Data.Select(c => c.PropertyId).ToList());
		}
    }
}