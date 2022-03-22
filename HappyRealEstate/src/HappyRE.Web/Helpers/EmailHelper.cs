using MBN.Utils;
using Mogi.Core;
using Mogi.Core.Models;
using Mogi.Core.Utils;
using System;
using System.Collections;
using System.Web;
using System.Web.Security;
using MBN.Security.BLL.SqlServer;
using System.Collections.Generic;

namespace Mogi.Web.Helpers
{
    public class EmailHelper
    {
        private static Hashtable _mailTemplate = new Hashtable();
        private static readonly string rootUrl = WebUtils.AppSettings("RootUrl");
        private static readonly string helpUrl = WebUtils.AppSettings("HelpUrl");
        private static readonly string domain = WebUtils.AppSettings("DOMAIN_NAME");
        private static readonly bool queue = WebUtils.AppSettings("MAIL_QUEUE", false);
        private static readonly string mailUrl = WebUtils.AppSettings("MAIL_URL", string.Empty);
        public static readonly string emailSupport = WebUtils.AppSettings("EMAIL_SUPPORT");

        #region Send mail
        public static void SendMail(string emailto, string subject, string body)
        {
            SendMail(emailto, subject, body, Const.MAIL_TYPE_SUPPORT);
        }
        public static void SendMail(string emailto, string subject, string body, string emailType)
        {
            if (Common.IsEmail(emailto) == false)
            {
                WebLog.Log.Error("EmailHelper.SendMail", string.Format("Email {0} Không đúng định dạng", emailto));
                return;
            }

            var objMail = SqlEmail.Instance.AddConfirm(
                   queue,
                   emailType,
                   Guid.Empty,
                   emailto,
                   subject,
                   body,
                   0);

            if (queue == false && objMail != null)
            {
                Mail.SendAsync(Const.MAIL_SUPPORT, objMail.Email, objMail.Subject, objMail.Body);
            }
        }
        public static void AddMailConfirm(string confirmType, Guid verification, string email, string subject, string body, int expiredHours)
        {
            if (Common.IsEmail(email) == false)
            {
                WebLog.Log.Error("EmailHelper.SendMail", string.Format("Email {0} Không đúng định dạng", email));
                return;
            }

            var objMail = SqlEmail.Instance.AddConfirm(queue, confirmType, verification, email, subject, body, expiredHours);
            if (queue == false && objMail != null)
            {
                Mail.SendAsync(Core.Const.MAIL_SUPPORT, objMail.Email, objMail.Subject, objMail.Body);
            }
        }
        public static void SendMail(string emailType, string emailTo, string subject, string templateKey, string template, Dictionary<string,string> data)
        {
            if (string.IsNullOrEmpty(emailType))
            {
                emailType = Const.MAIL_TYPE_SUPPORT;
            }

            string[] p = new string[data.Count * 2];
            int index = 0;
            foreach (string key in data.Keys)
            {
                p[index++] = "{#" + key + "}";
                p[index++] = data[key];
            }

            // Lấy mẫu gửi nếu trống
            if (string.IsNullOrEmpty(template) && string.IsNullOrEmpty(templateKey) == false)
            {
                templateKey = GetTemplate(templateKey);
            }

            // Tạo cacheKey nếu trống
            if (string.IsNullOrEmpty(templateKey))
            {
                templateKey = MBN.Utils.Security.Encryption.SHA1(template);
            }
            string body = WebUtils.FormatTemplate(
                template,
                templateKey,
                true, p);

            SendMail(emailTo, subject, body, emailType);
        }

        private static string GetTemplate(string code)
        {
            code = (code ?? "").ToLower();
            if (_mailTemplate.ContainsKey(code))
            {
                return (string)_mailTemplate[code];
            }
            Core.Interfaces.IUow uow = Core.ObjectFactory.GetInstance<Core.Interfaces.IUow>();
            var obj = uow.CMSCategory.Get(code);
            if (obj == null)
            {
                WebLog.Log.Error("EmailHelper.GetTemplate:", "Không tìm thấy mẫu gửi email - " + code);
                return string.Empty;
            }

            _mailTemplate[code] = obj.Description;

            return obj.Description;
        }
        #endregion

        #region Custom
        /// <summary>
        /// Xác thực email
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="urlActivation"></param>
        /// <param name="returnUrl"></param>
        public static void SendVerifyEmail(UserProfile profile, string urlActivation, string returnUrl)
        {
            try
            {
                if (profile == null || Common.IsEmail(profile.Email) == false)
                {
                    return;
                }

                string email = profile.Email;
                string name = profile.FullName;
                Guid key = Guid.Empty;
                long _checksum = 0;
                SqlEmail.GetKey(out key, out _checksum);

                /*Tạo mẫu email kích hoạt & gửi cho người sử dụng.*/
                string subject = string.Format("[{0}] Thu xac thuc tai khoan email", domain);
                string url = string.Format("{0}{1}?pid={2}&key={3}&cs={4}&returnUrl={5}",
                    rootUrl,
                    urlActivation,
                    profile.ProfileId,
                    key,
                    _checksum,
                    HttpUtility.UrlEncode(returnUrl)
                    );

                string template = GetTemplate(Const.MAIL_TEMPLATE_VERIFY);
                string body = WebUtils.FormatTemplate(
                    template,
                    Const.MAIL_TEMPLATE_VERIFY,
                    true,
                    "{#URL}", mailUrl,
                    "{#NAME}", name,
                    "{#URL_ACTIVATE}", url,
                    "{#DOMAIN}", domain,
                    "{#EXPIRED}", DateTime.Now.AddHours(Const.MAIL_CONFIRM_EXPIRED).ToString("HH:mm dd/MM/yyyy"),
                    "{#EMAIL_SUPPORT}", emailSupport,
                    "{#URL_HELP}", helpUrl);

                // Send Mail
                AddMailConfirm(Const.MAIL_TYPE_VERIFY, key, email, subject, body, Const.MAIL_CONFIRM_EXPIRED);
            }
            catch (Exception ex)
            {
                WebLog.Log.Error(string.Format("EmailHelper.SendVerifyEmail(email = {0})", profile.Email), ex.Message);
            }
        }
        #endregion
    }
}