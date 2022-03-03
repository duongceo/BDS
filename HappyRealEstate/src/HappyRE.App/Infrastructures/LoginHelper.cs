using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using MBN.Utils.Extension;
using Microsoft.Owin.Security;
using System.Web.Security;
using HappyRE.App.Models;

namespace HappyRE.App
{
    public class LoginHelper
    {
        public static void LoginOwin(string userId, string userName, int profileId = 0, string mobile = "", string email = "")
        {
            LoginOwin(new ClaimData() { UserId = userId, UserName = userName, ProfileId = profileId, Mobile = mobile, Email = email });
        }

        public static void LoginOwin(ClaimData data)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, data.UserId.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, data.UserName));
            claims.Add(new Claim(ClaimTypes.UserData, data.ToJson()));

            //var roles = Roles.GetRolesForUser(data.UserName);
            //foreach (string item in roles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, item));
            //}
            var id = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            HttpContext.Current.Request.GetOwinContext().Authentication.SignIn(id);
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(id), new AuthenticationProperties() { IsPersistent = true });
        }
        /// <summary>
        /// User có nằm trong role này không
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool HasRole(string role)
        {
            return  Roles.IsUserInRole(role);
        }
        /// <summary>
        /// quyền administrator
        /// </summary>
        /// <returns></returns>
        public static bool IsAdmin()
        {
            return Roles.IsUserInRole("ADMIN");
        }
        /// <summary>
        /// quyền mogisystem
        /// </summary>
        /// <returns></returns>
        public static bool IsAdminSystem()
        {           
             return Roles.IsUserInRole("ADMIN_SYSTEM");
        }
        public static string GetUserId()
        {
            var identity = HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated == true)
            {
                return identity.GetUserId();
            }
            return null;
        }

        public static ClaimData GetUserData()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated == true)
            {
                Claim item = identity.Claims.FirstOrDefault(w => w.Type == ClaimTypes.UserData);
                if (item != null)
                {
                    return item.Value.FromJson<ClaimData>();
                }
            }

            return null;
        }

        /// <summary>
        /// Có được phép làm gì không
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool AllowAccess(string code)
        {
            return true;
            //var q = GetPermission(code);
            //return (q == null ? false : q.Access);
        }
    }
}