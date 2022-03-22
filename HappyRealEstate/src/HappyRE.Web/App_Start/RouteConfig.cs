using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HappyRE.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{service}.asmx/{*pathInfo}");
            routes.IgnoreRoute("content/{*pathInfo}");
            RouteTable.Routes.Ignore("{*allstatic}", new { allstatic = @".*\.(jpg|gif|png|js|css|txt)" });

			//routes.MapMvcAttributeRoutes();

            Helpers.RouteConfig.Instance.RegisterRoutes(routes);
        }
    }
}
