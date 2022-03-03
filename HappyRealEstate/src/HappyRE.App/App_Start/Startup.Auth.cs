using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Microsoft.Owin.Security.Facebook;
using HappyRE.App.Providers;
using System.Web.Http;
using HappyRE.Core.Utils;
using Microsoft.AspNet.Identity.Owin;
using System.Net;
using HappyRE.App.Models;
using System.Web.Helpers;
using System.Security.Claims;

namespace HappyRE.App
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions googleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }
        //public void ConfigureOAuth(IAppBuilder app)
        //{
        //    //use a cookie to temporarily store information about a user logging in with a third party login provider
        //    app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
        //    OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

        //    OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
        //    {
        //        AllowInsecureHttp = true,
        //        TokenEndpointPath = new PathString("/token"),
        //        AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
        //        Provider = new SimpleAuthorizationServerProvider()
        //        //RefreshTokenProvider = new SimpleRefreshTokenProvider()
        //    };

        //    // Token Generation
        //    app.UseOAuthAuthorizationServer(OAuthServerOptions);
        //    app.UseOAuthBearerAuthentication(OAuthBearerOptions);

        //    //Configure Google External Login
        //    googleAuthOptions = new GoogleOAuth2AuthenticationOptions()
        //    {
        //        ClientId = ConfigSettings.Get("GOOGLE_CLIENT_ID", ""),
        //        ClientSecret = ConfigSettings.Get("GOOGLE_CLIENT_SECRET", ""),
        //        Provider = new GoogleAuthProvider(),
        //    };
        //    app.UseGoogleAuthentication(googleAuthOptions);

        //    facebookAuthOptions = new FacebookAuthenticationOptions()
        //    {
        //        AppId = ConfigSettings.Get("FACEBOOK_APP_ID", "210785352288225"),
        //        AppSecret = ConfigSettings.Get("FACEBOOK_APP_SECRET", "fea553af537064e49c164a6ad2bfb628"),
        //        Provider = new FacebookAuthProvider()
        //    };
        //    facebookAuthOptions.Scope.Add("email");
        //    app.UseFacebookAuthentication(facebookAuthOptions);
            


        //    //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        //    //app.UseWebApi(new HttpConfiguration());
        //}

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(5),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
                //ExpireTimeSpan= TimeSpan.FromMinutes(1)
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}
