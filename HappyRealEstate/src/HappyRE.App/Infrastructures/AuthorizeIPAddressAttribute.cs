using HappyRE.Core.Utils;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using log4net;
using System;

namespace HappyRE.App
{
    public class AuthorizeIpAddressAttribute : ActionFilterAttribute
    {
        private static readonly ILog _log = LogManager.GetLogger("AuthorizeIpAddressAttribute");
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var roles = ConfigSettings.Get("ADMIN_ROLES", "ADMIN");
                var isAdmin = filterContext.HttpContext.GetOwinContext().Authentication.User.IsInRole(roles);
                if (isAdmin == false)
                {
                    string ipAddress = HttpContext.Current.Request.UserHostAddress;
                    bool isValidIP = false;
                    using (var _ctx = new AuthContext())
                    {
                        isValidIP = _ctx.IpAllowed.Any(x => x.Ip == ipAddress);
                    }
                    if (isValidIP == false)
                    {
                        var ctx = filterContext.HttpContext.GetOwinContext().Authentication;
                        ctx.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        filterContext.Result = new HttpStatusCodeResult(401);
                    }
                }
            }catch(Exception ex)
            {
                _log.Error(ex);
            }
            base.OnActionExecuting(filterContext);
        }

        /// <summary> 
        /// Compares an IP address to list of valid IP addresses attempting to 
        /// find a match 
        /// </summary> 
        /// <param name="ipAddress">String representation of a valid IP Address</param> 
        /// <returns></returns> 
        public static bool IsIpAddressValid(string ipAddress)
        {
            //Split the users IP address into it's 4 octets (Assumes IPv4) 
            string[] incomingOctets = ipAddress.Trim().Split(new char[] { '.' });

            //Get the valid IP addresses from the web.config 
            string addresses = ConfigSettings.Get("AuthorizeIPAddresses", "");
              //Convert.ToString(WebConfig.AuthorizeIPAddresses);

            //Store each valid IP address in a string array 
            string[] validIpAddresses = addresses.Trim().Split(new char[] { ',' });

            //Iterate through each valid IP address 
            foreach (var validIpAddress in validIpAddresses)
            {
                //Return true if valid IP address matches the users 
                if (validIpAddress.Trim() == ipAddress)
                {
                    return true;
                }

                //Split the valid IP address into it's 4 octets 
                string[] validOctets = validIpAddress.Trim().Split(new char[] { '.' });

                bool matches = true;

                //Iterate through each octet 
                for (int index = 0; index < validOctets.Length; index++)
                {
                    //Skip if octet is an asterisk indicating an entire 
                    //subnet range is valid 
                    if (validOctets[index] != "*")
                    {
                        if (validOctets[index] != incomingOctets[index])
                        {
                            matches = false;
                            break; //Break out of loop 
                        }
                    }
                }

                if (matches)
                {
                    return true;
                }
            }

            //Found no matches 
            return false;
        }
    } 

}