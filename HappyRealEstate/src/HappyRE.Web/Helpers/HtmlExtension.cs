using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace System.Web.Mvc.Html
{
    public static partial class Extensions
    {
        public static MvcHtmlString DisplayPlaceHolderFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var result = html.DisplayNameFor(expression).ToHtmlString();
            return new MvcHtmlString(System.Web.HttpUtility.HtmlDecode(result.ToString()));
        }
    }
}