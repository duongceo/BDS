using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MBN.Utils;

namespace HappyRE.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly string lang = WebUtils.AppSettings("LANG", "vi-VN");
        private static readonly System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(lang);

        protected void Application_Start()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

			//PreSendRequestHeaders += Application_PreSendRequestHeaders;
			MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier = System.Security.Claims.ClaimTypes.NameIdentifier;

            App_Start.MogiInit.InitApp();

            ClientDataTypeModelValidatorProvider.ResourceClassKey = "HappyRE.Core.Resources.Validation";
            DefaultModelBinder.ResourceClassKey = "HappyRE.Core.Resources.Validation";
            ModelBinders.Binders.DefaultBinder = new Helpers.TrimModelBinder();

            // AntiXss - HtmlEncode
            Mogi.BLL.Utils.HtmlUtils.AntiXss_MarkAsSafe();
        }
        protected void Application_Error()
        {
            Exception ex = Server.GetLastError();

            string fileName = string.Format("global_{0}.txt", DateTime.Today.ToString("yyyyMMdd"));
            string url = HttpContext.Current.Request.Url.PathAndQuery;
            MBN.Utils.WebLog.Log.Error("Application_Error-" + url, ex, fileName);

            if (ex is HttpAntiForgeryException)
            {
                Response.Clear();
                Server.ClearError(); //make sure you log the exception first
                Response.Redirect("~/", true);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			var resp = HttpContext.Current.Response;
			if (resp.StatusCode == 302)
			{
				if (resp.Headers["ExternalLogin"] != null)
				{
					string url = resp.Headers["Location"];
					url = url.Replace("redirect_uri=http%", "redirect_uri=https%");
					resp.Headers["Location"] = url;
				}
			}

			
		}

		//protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
		//{
		//	var resp = HttpContext.Current.Response;
		//	resp.Headers.Remove("Vary");
		//}
	}
}
