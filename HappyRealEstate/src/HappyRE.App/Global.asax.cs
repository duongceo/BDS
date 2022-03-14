using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using MBN.Utils;

namespace HappyRE.App
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly ILog _log = LogManager.GetLogger("WebApiApplication");
        private static readonly string lang = "vi-VN";//WebUtils.AppSettings("LANG", "en-US");
        private static readonly System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(lang);
        protected void Application_Start()
         {
            culture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            MvcHandler.DisableMvcResponseHeader = true;
            
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //Hangfire.RecurringJob.AddOrUpdate(19872.ToString(), () => (), Hangfire.Cron.Daily(9, 0));
        }

        protected void Application_Error()
        {
            Exception ex = Server.GetLastError();
            if (ex is HttpAntiForgeryException)
            {
                _log.Error(ex);
                Response.Clear();
                Server.ClearError(); //make sure you log the exception first
                Response.Redirect("~/", true);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            culture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
