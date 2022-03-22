using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using MBN.Utils.Extension;
using HappyRE.Core.MapModels;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace HappyRE.Web.Helpers
{
    public class LoginHelper
    {
        public static void LoginOwin(Core.Models.UserProfile profile, bool rememberMe = true)
        {
            if (profile == null) return;

            if (HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                HttpContext.Current.Request.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.Current.Session.Abandon();
            }

            LoginOwin(ClaimData.MapObject(profile, rememberMe));
        }

        public static void LoginOwin(ClaimData data)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, data.UserId.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, data.UserName));
            claims.Add(new Claim(ClaimTypes.UserData, data.ToJson()));
            if (!string.IsNullOrEmpty(data.Roles))
            {
                foreach (var role in data.Roles.Split(new char[1] { ',' }))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            HttpContext.Current.Request.GetOwinContext().Authentication.SignIn(new Microsoft.Owin.Security.AuthenticationProperties() { IsPersistent = data.RememberMe }, identity);
        }

		public static AuthenticationTicket GetTicket(ClaimData data)
		{
			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.NameIdentifier, data.UserId.ToString()));
			claims.Add(new Claim(ClaimTypes.Name, data.UserName));
			claims.Add(new Claim(ClaimTypes.UserData, data.ToJson()));
			if (string.IsNullOrEmpty(data.Roles) == false)
			{
				foreach (var role in data.Roles.Split(new char[1] { ',' }))
				{
					claims.Add(new Claim(ClaimTypes.Role, role));
				}
			}

			ClaimsIdentity identity = new ClaimsIdentity(claims, Startup.OAuthServerOptions.AuthenticationType);

			Dictionary<string, string> d = new Dictionary<string, string>();
			d.Add("UserId", data.UserId.ToString());
			d.Add("UserName", data.UserName);

			var props = new AuthenticationProperties(d);
			return new AuthenticationTicket(identity, props);
		}

		public static AuthenticationTicket GetTicket(Core.Models.UserProfile profile)
		{
			ClaimData data = ClaimData.MapObject(profile, true);

			return GetTicket(data);
		}

		public static string GetToken(Core.Models.UserProfile profile)
		{
			ClaimData data = ClaimData.MapObject(profile, true);

			return GetToken(data);
		}

		public static string GetToken(ClaimData data)
		{
			//var claims = new List<Claim>();
			//claims.Add(new Claim(ClaimTypes.NameIdentifier, data.UserId.ToString()));
			//claims.Add(new Claim(ClaimTypes.Name, data.UserName));
			//claims.Add(new Claim(ClaimTypes.UserData, data.ToJson()));
			//if (!string.IsNullOrEmpty(data.Roles))
			//{
			//	foreach (var role in data.Roles.Split(new char[1] { ',' }))
			//	{
			//		claims.Add(new Claim(ClaimTypes.Role, role));
			//	}
			//}

			//ClaimsIdentity identity = new ClaimsIdentity(claims, Startup.OAuthServerOptions.AuthenticationType);

			//Dictionary<string, string> d = new Dictionary<string, string>();
			//d.Add("UserId", data.UserId.ToString());
			//d.Add("UserName", data.UserName);

			//var props = new AuthenticationProperties(d);
			//var ticket = new AuthenticationTicket(identity, props);

			AuthenticationTicket ticket = GetTicket(data);
			DateTime currentUtc = DateTime.UtcNow;
			ticket.Properties.IssuedUtc = currentUtc;
			ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromHours(1));
			return "bearer " + Startup.OAuthServerOptions.AccessTokenFormat.Protect(ticket);
		}

        public static Guid GetUserId()
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated == true)
            {
                return new Guid(identity.GetUserId());
            }
            return Guid.Empty;
        }

        public static ClaimData GetUserData()
        {
            string key = "MOGI:ClaimData";
            ClaimData res = (ClaimData)HttpContext.Current.Items[key];
			if (res != null) return res;

			if (res == null)
            {
                var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                if (identity.IsAuthenticated == true)
                {
                    Claim item = identity.Claims.FirstOrDefault(w => w.Type == ClaimTypes.UserData);
                    if (item != null)
                    {
                        res = item.Value.FromJson<ClaimData>();
                    }
                }
            }

			if (res != null && res.IsExpired(60) == true)
			{
				// reload data from profile
				Core.Interfaces.IUow uow = Core.ObjectFactory.GetInstance<Core.Interfaces.IUow>();
				var profile = uow.UserProfile.Get(res.ProfileId);
				if (profile == null)
				{
					LogOff();
					return res;
				}

				res = ClaimData.MapObject(profile, res.RememberMe);
				res.LastChecked = DateTime.Now;

				// Re-Login
				LoginOwin(res);
			}

            HttpContext.Current.Items[key] = res;

            return res;
        }     

        public static int GetUserProfileId()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated == true)
            {
                Claim item = identity.Claims.FirstOrDefault(w => w.Type == ClaimTypes.UserData);
                if (item != null)
                {
                    return item.Value.FromJson<ClaimData>().ProfileId;
                }
            }

            return 0;
        }

        public static void LogOff()
        {
            try
            {
                var current = HttpContext.Current;
                current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                current.Session.Abandon();
            }
            catch (Exception)
            {
            }
        }

		public static string MogiJS()
		{
			bool is_auth = HttpContext.Current.User.Identity.IsAuthenticated;
			string auth = (is_auth ? "true" : "false");
			string thirdparty = (Core.Utils.Common.JavaScriptThirdparty ? "true" : "false");
			string member = "";
			string res = string.Empty;
			if (is_auth == true)
			{
				var u = GetUserData();
				if (u != null)
				{
					member = (u.IsMogiPro ? "mogipro" : "member");
					// dataLayer = [{'userID': '12483'}]
					res = "dataLayer = [{'userID': 'agent." + u.ProfileId + (string.IsNullOrEmpty(u.Mobile) == true ? "" : ".m" + u.Mobile) + "'}];";
				}
			}
			return $"var MOGI={{IsAuth:{auth},Thirdparty:{thirdparty},Member:'{member}'}};" + res;
		}
	}
}