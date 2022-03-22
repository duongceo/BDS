using HappyRE.Core.MapModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Helpers
{
    public class ReCaptchaHelper
    {
        string _secretKey = "6LdIbREUAAAAAFG4MDhnFi6nSTn7ljHjQJxda1IL";
        public ReCaptchaHelper()
        {
        }

        public Response<bool> VerifyResponse(string response)
        {
            Response<bool> res = new Response<bool>();
            try
            {
                var client = new System.Net.WebClient();
                var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", _secretKey, response));
                var obj = Newtonsoft.Json.Linq.JObject.Parse(result);
                res.Data= (bool)obj.SelectToken("success");
                if (!res.Data) res.Errors.Add("Chưa xác thực captcha");
            }
            catch(Exception ex)
            {
                res.Data = false;
                res.Errors.Add(ex.Message);
            }
            return res;
        }
    }
}