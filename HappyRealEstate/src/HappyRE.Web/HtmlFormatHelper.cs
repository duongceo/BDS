using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Text;
using HappyRE.Core.MapModels;

namespace HappyRE.Web
{
    public static class HtmlFormatHelper
    {
        public static string Pagination(this HtmlHelper helper, Paging item)
        {
            if (item.Total <= item.PageSize) return string.Empty;
            //return item.RenderHTML(item.Url, item.CurrentPage, item.PageSize, item.Total);
            return item.RenderHTML();
        }
    }
}