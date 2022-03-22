using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Mogi.Web.Models
{
    [Serializable]
    public class Paging
    {
        internal int MaxPage = 200;
        public Paging()
        {

        }

        public string Url { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int TotalPaging { get; set; }
        public int CurrentPage { get; set; }

        private bool addParam = true;

        public string RenderHTML(string url, int pageIndex, int pageSize, int total)
        {
            bool add_param = true;


            string[] separateURL = url.Split('?');

            if (separateURL.Length > 1)
            {
                NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(separateURL[1]);
                queryString.Remove("PageIndex");
                queryString.Remove("cp");
                url = separateURL[0] + "?" + queryString.ToString();
            }
            var item = new Paging()
            {
                Url = url,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Total = total,
                addParam = add_param
            };
            return item.RenderHTML();
        }

        public string RenderHTML()
        {
            if (this.Total <= this.PageSize) return string.Empty;

            StringBuilder sb = new StringBuilder();

            this.CurrentPage = Math.Max(1, this.CurrentPage);

            string css = string.Empty;
            string url = this.Url + (this.addParam == false ? "" : (this.Url.IndexOf('?') > 0 ? "&cp=" : "?cp="));
            int totalPage = ((this.Total - (this.Total % this.PageSize)) / this.PageSize) + (this.Total % this.PageSize > 0 ? 1 : 0);
            int deltaPage = 5, hidden_sm = 3, hidden_xs = 2;
            int hidden_sm_total = hidden_sm * 2 + 1;
            int hidden_xs_total = hidden_xs * 2 + 1;
            int displayPage = deltaPage * 2 + (this.CurrentPage <= deltaPage ? -1 : 0);
            int startPage = Math.Max(1, this.CurrentPage - deltaPage);
            int endPage = startPage + displayPage;

            totalPage = Math.Min(totalPage, MaxPage);
            endPage = Math.Min(endPage, totalPage);
            startPage = Math.Max(1, endPage - displayPage);

            sb.Append("<div class=\"paging\"><ul class=\"pagination\">");
            //// first page
            //if (this.CurrentPage > 2)
            //{
            //    sb.AppendFormat("<li><a class=\"{2}\" href=\"{0}{1}\"><i class=\"icon icon-arrow-first-page\"></i></a></li>\r\n", url, 1, css);
            //}

            // prev page
            if (this.CurrentPage > 1)
            {
                sb.AppendFormat("<li><a href=\"{0}{1}\"><i class=\"icon icon-arrow-line-left\"></i></a></li>\r\n", url, this.CurrentPage - 1);
            }
            else
            {
                sb.Append("<li class=\"disabled\"><span><i class=\"icon icon-arrow-line-left\"></i></span></li>\r\n");
            }

            // list page
            for (int i = startPage; i <= endPage; i++)
            {
                int delta = i - this.CurrentPage;
                if (i == this.CurrentPage)
                {
                    sb.AppendFormat("<li class=\"active\"><span>{0}</span></li>\r\n", i);
                }
                else
                {
                    css = string.Empty;
                    css += (delta < -hidden_sm || (delta > hidden_sm && i > hidden_sm_total) ? "hidden-sm" : "");
                    css += (delta < -hidden_xs || (delta > hidden_xs && i > hidden_xs_total) ? " hidden-xs" : "");
                    sb.AppendFormat("<li class=\"{2}\"><a href=\"{0}{1}\">{1}</a></li>\r\n", url, i, css.Trim());
                }
            }

            // next page
            if (this.CurrentPage < endPage)
            {
                sb.AppendFormat("<li><a href=\"{0}{1}\"><i class=\"icon icon-arrow-line-right\"></i></a></li>\r\n", url, this.CurrentPage + 1);
            }
            else
            {
                sb.Append("<li class=\"disabled\"><span><i class=\"icon icon-arrow-line-right\"></i></span></li>\r\n");
            }

            //last page
            //if (this.CurrentPage < (totalPage - 1))
            //{
            //    sb.AppendFormat("<li><a  href=\"{0}{1}\"><i class=\"icon icon-arrow-last-page\"></i></a></li>\r\n", url, totalPage);
            //}

            sb.Append("</ul></div>");

            return sb.ToString();
        }
    }
}