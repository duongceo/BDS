using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HappyRE.Core.MapModels;
using HappyRE.Core.MapModels.FrontEnd;
namespace HappyRE.Web.Models
{
    public class AgentListViewModel
    {
        public AgentListViewModel()
        {
            Agents = new List<AgentDisplay>();
        }

        public string Keyword { get; set; }
        public List<AgentDisplay> Agents { get; set; }
        public FrontEndAgentQuery Query { get; set; }
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