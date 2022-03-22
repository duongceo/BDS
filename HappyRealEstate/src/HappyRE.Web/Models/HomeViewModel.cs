using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HappyRE.Core.MapModels;

namespace HappyRE.Web.Models
{
    [Serializable]
    public class HomeViewModel
    {
        public Dictionary<string, string> Msg = new Dictionary<string, string>();
        private Core.MapModels.SearchFilter _filter = new Core.MapModels.SearchFilter();
        public Core.MapModels.SearchFilter Filter
        {
            get { return this._filter; }
            set { this._filter = value; }
        }

        /// <summary>
        /// Lịch sử tìm kiếm
        /// </summary>
        public List<SearchRecent> SearchRecent { get; set; }

        public List<SearchRecent> SearchSave { get; set; }

        /// <summary>
        /// Hướng dẫn
        /// </summary>
        public List<Core.Models.CMS_News> GuideNews { get; set; }

        /// <summary>
        /// Tin tức mới
        /// </summary>
        public List<Core.Models.CMS_News> FocusNews { get; set; }

    }
}