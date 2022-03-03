using Facebook;
using log4net;
using MBN.Utils;
using Newtonsoft.Json;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Services
{
    public class SmsService
    {
        private static readonly ILog _log = LogManager.GetLogger("SmsService");
        private static readonly string SMS_PhoneUID = WebUtils.AppSettings("SMS_PhoneUID", "");
        private static readonly string SMS_Pin = WebUtils.AppSettings("SMS_Pin", "");
        private static readonly string SMS_Sim = WebUtils.AppSettings("SMS_Sim", "");

        static readonly string smsSendApi = "http://sms.mos.com.vn/api/services/SendMessageNow?phone_uid={0}&pin={1}&sim={2}&to={3}&sms={4}";
        //static readonly string smsAddDeviceApi = "http://sms.mos.com.vn/api/services/AddPhone?username=lamktvn&password=lamktvn&phonename={0}&pin={1}&sim1={2}&sim2={3}&sim3&sim4";
        //static readonly string smsRemoveDeviceApi = "http://sms.mos.com.vn/api/services/DeletePhone?username=lamktvn&password=lamktvn&phoneUid={0}";
        public SmsService()
        {
        }

        public bool Send(string mobile, string msg)
        {
            if (string.IsNullOrEmpty(mobile) == false)
            {
                string url = string.Format(smsSendApi, SMS_PhoneUID, SMS_Pin, SMS_Sim, mobile, msg);
                WebRequest request = WebRequest.Create(url);
                _log.Warn(url);
                WebResponse response = request.GetResponse();
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    _log.Error(((HttpWebResponse)response).StatusDescription);
                }
            }
            return false;
        }

        public bool Send(string SMS_PhoneUID, string SMS_Pin, string SMS_Sim, string mobile, string msg)
        {
            if (string.IsNullOrEmpty(mobile) == false && string.IsNullOrEmpty(msg) == false)
            {
                if (msg.Length > 160) msg = msg.Substring(0, 159);
                string url = string.Format(smsSendApi, SMS_PhoneUID, SMS_Pin, SMS_Sim, mobile, msg);
                WebRequest request = WebRequest.Create(url);
                _log.Warn(url);
                WebResponse response = request.GetResponse();
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    _log.Error(((HttpWebResponse)response).StatusDescription);
                }
            }
            return false;
        }

        public ApiResponse SendSms(string SMS_PhoneUID, string SMS_Pin, string SMS_Sim, string mobile, string msg)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                if (string.IsNullOrEmpty(mobile) == false && string.IsNullOrEmpty(msg) == false)
                {
                    if (msg.Length > 160) msg = msg.Substring(0, 159);
                    string url = string.Format(smsSendApi, SMS_PhoneUID, SMS_Pin, SMS_Sim, mobile, msg);
                    WebRequest request = WebRequest.Create(url);
                   // _log.Warn(url);
                    WebResponse response = request.GetResponse();
                    res.Code = ((HttpWebResponse)response).StatusCode;
                    if (res.Code != HttpStatusCode.OK)
                    {
                        res.Message = ((HttpWebResponse)response).StatusDescription;
                        _log.Error(((HttpWebResponse)response).StatusDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return res;
        }
        string ProcessSmsContent(string content)
        {
            var nonVietnamese = Utils.CommonUtils.RemoveSign4VietnameseString(content);
            if (content.Length > 300) return nonVietnamese.Substring(0, 300);
            return content;
        }
    }

    #region Facebook Service
    public class FacebookService
    {
        private static readonly ILog _log = LogManager.GetLogger("FacebookService");
        private static readonly string FacebookPage_Id = WebUtils.AppSettings("FacebookPage_Id", "246456482736038");
        private static readonly string FacebookPage_User_Ids = WebUtils.AppSettings("FacebookPage_User_Ids", "2078829535546779");
        private static readonly string fbAccessToken = WebUtils.AppSettings("FacebookPage_AccessToken", "EAAFQibWPIygBAFOaEIuIxJJHSS2J3QGcCkfV8IjSnlvNRVTqbHTx2lWzwBdkjrj4bZBTYaWpS2FhmE5n3ZAZCWhyI2dA2AuqoV2Qqu1nc3VcPm5ZARu9ZCE72ZACzpxaTUjZAWxISacwObJE16QI8BWVC5vSENFgbt6EBZAnMR1AqShI1wLXPC2ZB2CgmtrzuxJEZD");

        public async Task<bool> Send(string fbId, string message)
        {
            try
            {
                return false;
                if (fbId==null || string.IsNullOrEmpty(message)) return true;
                fbId = fbId.Trim();
                if (string.IsNullOrEmpty(fbId) ==true || string.IsNullOrEmpty(message)==true) return true;
                var fb = new FacebookClient(fbAccessToken);
                var msg = new PnFacebookMessage()
                {
                    message = new Message()
                    {
                        text = message
                    },
                    messaging_type = "MESSAGE_TAG",
                    recipient = new Recipient()
                    {
                        id = fbId
                    },
                    tag = "POST_PURCHASE_UPDATE"
                };
                var command = "me/messages";
                dynamic result = await fb.PostTaskAsync(command, msg);
                string id = result.message_id;
                return (id.Length > 0) ? true : false;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                _log.ErrorFormat("{0} , {1}", fbId, message);
                return false;
            }
        }

        public async Task SendToCS(string message)
        {
            return;
            foreach (var fbId in FacebookPage_User_Ids.Split(new char[1] { ',' }))
            {
               await Send(fbId, message);              
            }

           await new ZaloService().SendToCS(message);
        }
    }

    public class Recipient
    {
        public string id { get; set; }
    }

    public class Message
    {
        public string text { get; set; }
    }

    public class PnFacebookMessage
    {
        public Recipient recipient { get; set; }
        public Message message { get; set; }
        public string messaging_type { get; set; }
        public string tag { get; set; }
    }

    #endregion

    #region Zalo Service
    public class ZaloService
    {
        private static readonly ILog _log = LogManager.GetLogger("ZaloService");
        private static readonly string ZaloPage_User_Ids = WebUtils.AppSettings("ZaloPage_User_Ids", "5803407634143730880");
        private static readonly string zlAccessToken = WebUtils.AppSettings("ZaloPage_AccessToken", "vlU0QsS3XoJcy8DYP0U79UduXcfi4U1cuUFYA2zngHggqj8iJNhG8Ch_iMmoLSPnlkAuMr9pmXECzRWO3G7hKC2iddKp0Fn7yxU2OY56Y7xI-8vK14pw6Ccw-nD57BCrjQss9J4xl1AekCTRQJwaIRoWecHz8TOhyP7HEJG5l2dYXlrcEI-DTRgDqsjS98r3dDhD2KfqdIEXsUqmPKoDVhRTo4LmOA9kfkRNJYLKaqBVgi0EBoMGBgYFxJfhPPOrli6IPq1KaMc2oSLxURJ7CdzX7e4c");

        public async Task<bool> Send(string id, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(message)) return true;

                var mes = new ZaloMessage()
                {
                    message = new Message()
                    {
                        text = message
                    },
                    recipient = new Recipient()
                    {
                        user_id = id
                    }
                };

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(mes);
                var stringContent = new System.Net.Http.StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var res = await client.PostAsync($"https://openapi.zalo.me/v2.0/oa/message?access_token={zlAccessToken}", stringContent);
                return (res.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("{0} , {1}", id, message);
                return false;
            }
        }

        public async Task SendToCS(string message)
        {
            foreach (var fbId in ZaloPage_User_Ids.Split(new char[1] { ',' }))
            {
                await Send(fbId, message);
            }
        }

        public class Recipient
        {
            public string user_id { get; set; }
        }

        public class Message
        {
            public string text { get; set; }
        }

        public class ZaloMessage
        {
            public Recipient recipient { get; set; }
            public Message message { get; set; }
        }
    }
    #endregion
}
