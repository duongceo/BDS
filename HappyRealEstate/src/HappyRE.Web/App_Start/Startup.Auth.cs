using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using HappyRE.Web.Models;
using MBN.Utils;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Cors;
using System.Web.Cors;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HappyRE.Web
{
    public partial class Startup
    {
		public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
		public static OAuthAuthorizationServerOptions OAuthServerOptions { get; private set; }

		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            //app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = WebUtils.AppSettings("Cookie_Auth", ".MOGIAUTH"),
                CookieDomain = MBN.Utils.WebUtils.AppSettings("Cookie_Domain", ".batdongsanhanhphuc.vn"),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                //Provider = 
				//new CookieAuthenticationProvider
				//{
				//	// Enables the application to validate the security stamp when the user logs in.
				//	// This is a security feature which is used when you change a password or add an external login to your account.  
				//	OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
				//		validateInterval: TimeSpan.FromMinutes(30),
				//		regenerateIdentity: (manager, user) =>
				//		{
				//			var u = user;
				//			return user.GenerateUserIdentityAsync(manager);
				//		})
				//}
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(15));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            app.UseFacebookAuthentication(
               appId: WebUtils.AppSettings("FACEBOOK_APP_ID", string.Empty),
               appSecret: WebUtils.AppSettings("FACEBOOK_APP_SECRET", string.Empty));

            var google = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = WebUtils.AppSettings("GOOGLE_CLIENT_ID", string.Empty),
                ClientSecret = WebUtils.AppSettings("GOOGLE_CLIENT_SECRET", string.Empty),
                Provider = new GoogleOAuth2AuthenticationProvider()
            };
            google.Scope.Add("email");
            app.UseGoogleAuthentication(google);

			// Token
			if (OAuthServerOptions == null)
			{
				OAuthServerOptions = new OAuthAuthorizationServerOptions()
				{
					AllowInsecureHttp = true,
					TokenEndpointPath = new PathString("/token"),
					AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),
					AuthorizationCodeExpireTimeSpan = TimeSpan.FromDays(1),
					Provider = new MogiAuthorizationServerProvider(),

				};
			}

			// Token Generation
			app.UseOAuthAuthorizationServer(OAuthServerOptions);
			if (OAuthBearerOptions == null) OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
			app.UseOAuthBearerAuthentication(OAuthBearerOptions);


			// CORS
			//app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
			var policy = new CorsPolicy
			{
				AllowAnyHeader = true,
				AllowAnyMethod = true,
				AllowAnyOrigin = false,
				SupportsCredentials = false,
				PreflightMaxAge = 2592000 // 30 days
			};
			string origins = WebUtils.AppSettings("Cors_Origins", "");
			if (string.IsNullOrEmpty(origins) == false)
			{
				if (origins == "*")
				{
					policy.AllowAnyOrigin = true;
				}
				else
				{
					string[] items = origins.Split(',');
					foreach (var item in items) policy.Origins.Add(item);
				}
			}
			app.UseCors(new CorsOptions { PolicyProvider = new CorsPolicyProvider { PolicyResolver = context => Task.FromResult(policy) } });
		}
    }
}