using Dapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.App.Model
{
    public class UserType
    {
        //public int GetUserType()
        //{
        //    if (HttpContext.Current.Session["UserType"] == null)
        //    {
        //        using (var cnn = SqlHelper.OpenConnection())
        //        {
        //            var user = cnn.Query<ApplicationUser>("select * from AspNetUsers where UserName = @username", new { username = HttpContext.Current.User.Identity.GetUserName() }).FirstOrDefault();
        //            if (user != null)
        //            {
        //                HttpContext.Current.Session["UserType"] = user.UserType;
        //            }
        //        }
        //    }

        //    var userType = 0;
        //    int.TryParse(HttpContext.Current.Session["UserType"].ToString(), out userType);
        //    return userType;

        //}
    }
}