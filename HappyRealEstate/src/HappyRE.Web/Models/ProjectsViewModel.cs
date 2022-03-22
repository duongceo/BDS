using HappyRE.Core.MapModels;
using HappyRE.Core.MapModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class ProjectsViewModel
    {
        public ProjectsViewModel()
        {
            Projects = new List<ProjectItem>();
        }

        public FrontEndProjectQuery Query { get; set; }
        public List<ProjectItem> Projects { get; set; }
        public string Keyword { get; set; }
        public Paging Paging { get; set; }
        public string GetResult()
        {
            if (this.Paging.Total == 0) return Core.Resources.Message.Listing_Result_NotFound;

            int startIndex = 1 + Math.Max(0, (this.Paging.CurrentPage - 1)) * this.Paging.PageSize;
            int endIndex = Math.Min(this.Paging.Total, startIndex + this.Paging.PageSize - 1);
            return string.Format(Core.Resources.Message.Listing_Result,
                startIndex.ToString("N0"), endIndex.ToString("N0"), this.Paging.Total.ToString("N0"));
        }
    }
}