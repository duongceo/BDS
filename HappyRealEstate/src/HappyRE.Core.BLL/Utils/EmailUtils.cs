using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using MBN.Utils;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Integration.Gmail;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using RestSharp;
using HappyRE.EmailServices;

namespace HappyRE.Core.BLL.Utils
{
    public class EmailUtils
    {
        static string EMAIL_IMG_BASE_URL = WebUtils.AppSettings("EMAIL_IMG_BASE_URL", "https://static.lenmay.vn/img/mail/");
        static string BASE_URL_IMG_200 = WebUtils.AppSettings("BASE_URL_IMG_200", "https://static.lenmay.vn/img/s200x200/");
        MailAddress _replyMail;
        EmailToModel _emailTo;

        static string PUNNEL_WELCOME_PAGE_ID = WebUtils.AppSettings("PUNNEL_WELCOME_PAGE_ID", "8b574813-9efa-4319-a7d6-c49d23293d6a");
        public EmailUtils(EmailToModel data)
        {
            data.AvatarUrl = BASE_URL_IMG_200 + data.AvatarUrl;
            _emailTo = data;
            _replyMail = new MailAddress("noreply@HappyRE.com", "Punnel Team");
        }

        public EmailUtils()
        {
        }

        string CreateHeader()
        {
            string headerTmp = HappyRE.Core.Entities.Resources.Email.HEADER;
            return CommonUtils.FormatEmailTemplate(
                    headerTmp,
                    new KeyValuePair<string, string>("{#USER_FULLNAME}", _emailTo.FullName),
                    new KeyValuePair<string, string>("{#USER_AVATAR}", _emailTo.AvatarUrl));
        }
        string CreateFooter()
        {
            return HappyRE.Core.Entities.Resources.Email.FOOTER;
        }

        string Build(string icon, string title , string bodyHtml)
        {
            var template = HappyRE.Core.Entities.Resources.Email.CONTAINER;
            string body = CommonUtils.FormatEmailTemplate(
                    template,
                    new KeyValuePair<string, string>("{#ICON_CONTENT}", icon),
                    new KeyValuePair<string, string>("{#TITLE_CONTENT}", title),
                    new KeyValuePair<string, string>("{#BODY_CONTENT}", bodyHtml),
                    new KeyValuePair<string, string>("{#HEADER}", CreateHeader()),
                    new KeyValuePair<string, string>("{#FOOTER}", CreateFooter()));
            return body;
        }

        string GetIconUrl(string fileName)
        {
            return string.Format("{0}{1}", EMAIL_IMG_BASE_URL, fileName);
        }

        #region Send email cases

        public void SubcriblePage(string name,string email)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email)) return;
                var client = new RestClient("https://api.HappyRE.com/api/subcrible");
                var request = new RestRequest(Method.POST);
                request.AddHeader("postman-token", "ff7f9915-07da-6414-014a-7342ee5b52ab");
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");

                var content = $"id={Guid.Parse(PUNNEL_WELCOME_PAGE_ID)}&dataForm[0].name=name&dataForm[0].value={name}&dataForm[1].name=email&dataForm[1].value={email}&dataForm[2].name=url_page&dataForm[2].value=https://HappyRE.com/punnel-advisor/";
                request.AddParameter("application/x-www-form-urlencoded", content, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
            }catch(Exception ex)
            {
            }
        }

        public void SendWelcome()
        {
            string template = HappyRE.Core.Entities.Resources.Email.WELCOME;
            string title = $"Chào mừng {_emailTo.FullName} đến với Punnel";
            string iconType = GetIconUrl("welcome-icon.png");
            var bodyHtml = CommonUtils.FormatEmailTemplate(
                    template,
                   new KeyValuePair<string, string>("{#FULLNAME}", _emailTo.FullName));

            var html = Build(iconType, title, bodyHtml);
            GmailSvc svc = new GmailSvc(EmailSendType.System_NoReply);
            svc.SendMail(title, html, new MailAddress(_emailTo.Email, _emailTo.FullName), _replyMail);

            BackgroundJob.Enqueue(() => SubcriblePage(_emailTo.FullName, _emailTo.Email));
        }

        public void SendWelcomeWidthVerifyEmail(string urlVerify)
        {
            string template = HappyRE.Core.Entities.Resources.Email.WELCOME_WITH_VERIFY_EMAIL;
            string title = $"Chào mừng {_emailTo.FullName} đến với Punnel";
            string iconType = GetIconUrl("welcome-icon.png");
            var bodyHtml = CommonUtils.FormatEmailTemplate(
                    template,
                   new KeyValuePair<string, string>("{#FULLNAME}", _emailTo.FullName),
                   new KeyValuePair<string, string>("{#LINK-URL}", urlVerify),
                    new KeyValuePair<string, string>("{#LINK}", "Xác thực email"));

            var html = Build(iconType, title, bodyHtml);
            GmailSvc svc = new GmailSvc(EmailSendType.System_NoReply);
            _replyMail = new MailAddress("hi@HappyRE.com", "Punnel Team");
            svc.SendMail(title, html, new MailAddress(_emailTo.Email, _emailTo.FullName), _replyMail);

            BackgroundJob.Enqueue(()=>SubcriblePage(_emailTo.FullName, _emailTo.Email));
        }

        public void SendVerifyEmail(string urlVerify)
        {
            string template = HappyRE.Core.Entities.Resources.Email.VERIFY_EMAIL;
            string title = $"Xác thực email của {_emailTo.FullName} trên Punnel";
            string iconType = GetIconUrl("verify-icon.png");
            var bodyHtml = CommonUtils.FormatEmailTemplate(
                    template,
                   new KeyValuePair<string, string>("{#LINK-URL}", urlVerify),
                    new KeyValuePair<string, string>("{#LINK}", "Xác thực email"));

            var html = Build(iconType, title, bodyHtml);
            GmailSvc svc = new GmailSvc(EmailSendType.System_NoReply);
            svc.SendMail(title, html, new MailAddress(_emailTo.Email, _emailTo.FullName), _replyMail);
        }
        public void SendResetPassword(string urlReset)
        {
            string template = HappyRE.Core.Entities.Resources.Email.RESET_PASS;
            string title = "Yêu cầu tạo lại mật khẩu mới";
            string iconType = GetIconUrl("forgetpass-icon.png");
            var bodyHtml = CommonUtils.FormatEmailTemplate(
                    template,
                   new KeyValuePair<string, string>("{#LINK-URL}", urlReset),
                    new KeyValuePair<string, string>("{#LINK}", urlReset));

            var html = Build(iconType, title, bodyHtml);
            GmailSvc svc = new GmailSvc(EmailSendType.System_NoReply);
            svc.SendMail(title, html, new MailAddress(_emailTo.Email, _emailTo.FullName), _replyMail);
        }

        public void SendNotify_AutoReplyError(string content)
        {
            string template = HappyRE.Core.Entities.Resources.Email.NOTIFY_INTEGRATIONAPP_ERROR;
            string title = "Tài khoản tích hợp Auto Responder không hoạt động trên Punnel";
            string iconType = GetIconUrl("verify-icon.png");
            var bodyHtml = CommonUtils.FormatEmailTemplate(
                    template,
                   new KeyValuePair<string, string>("{#CONTENT}", content));

            var html = Build(iconType, title, bodyHtml);
            GmailSvc svc = new GmailSvc(EmailSendType.Alert);
            svc.SendMail(title, html, new MailAddress(_emailTo.Email, _emailTo.FullName), _replyMail);
        }
        #endregion

        #region Notify to user
        /// <summary>
        /// Thông báo tài khoản Punnel của bạn sắp hết hạn trải nghiệm
        /// </summary>
        /// <param name="expiredDate"></param>
        public void SendUpgradeAccount(DateTime expiredDate)
        {
            if (expiredDate < DateTime.Now || expiredDate> DateTime.Now.AddDays(30)) return;
            string template = HappyRE.Core.Entities.Resources.Email.NOTIFY_FREEMEMBER_EXPIRED;
            string title = $"Tài khoản của {_emailTo.FullName} trên Punnel sắp hết hạn trải nghiệm";
            string iconType = GetIconUrl("alert.png");
            var bodyHtml = CommonUtils.FormatEmailTemplate(
                    template,
                   new KeyValuePair<string, string>("{#FULLNAME}", _emailTo.FullName),
                   new KeyValuePair<string, string>("{#DATE}", expiredDate.ToString("dd/MM/yyyy"))
                   );

            var html = Build(iconType, title, bodyHtml);
            GmailSvc svc = new GmailSvc(EmailSendType.System_NoReply);
            svc.SendMail(title, html, new MailAddress(_emailTo.Email, _emailTo.FullName), _replyMail);
        }

        /// <summary>
        /// Thông báo thời hạn sử dụng dịch vụ Punnel sắp hết
        /// </summary>
        /// <param name="expiredDate"></param>
        public void SendExpiredAccount(DateTime expiredDate)
        {
            if (expiredDate < DateTime.Now || expiredDate > DateTime.Now.AddDays(30)) return;
            string template = HappyRE.Core.Entities.Resources.Email.NOTIFY_EXPIRED;
            string title = "Thông báo thời hạn sử dụng dịch vụ Punnel sắp hết";
            string iconType = GetIconUrl("alert.png");
            var bodyHtml = CommonUtils.FormatEmailTemplate(
                    template,
                   new KeyValuePair<string, string>("{#FULLNAME}", _emailTo.FullName),
                   new KeyValuePair<string, string>("{#DATE}", expiredDate.ToString("dd/MM/yyyy"))
                   );

            var html = Build(iconType, title, bodyHtml);
            GmailSvc svc = new GmailSvc(EmailSendType.System_NoReply);
            svc.SendMail(title, html, new MailAddress(_emailTo.Email, _emailTo.FullName), _replyMail);
        }

        public void SendUpgradeSuccess(string ServiceName, int month, DateTime expiredDate)
        {
            if (expiredDate <= DateTime.Now) return;
            string template = HappyRE.Core.Entities.Resources.Email.NOTIFY_PAYMENT_SUCCESS;
            string title = "Gia hạn sử dụng dịch vụ Punnel thành công";
            string iconType = GetIconUrl("alert.png");
            var bodyHtml = CommonUtils.FormatEmailTemplate(
                    template,
                   new KeyValuePair<string, string>("{#FULLNAME}", _emailTo.FullName),
                   new KeyValuePair<string, string>("{#SERVICE}", ServiceName),
                   new KeyValuePair<string, string>("{#MONTH}", month.ToString()),
                   new KeyValuePair<string, string>("{#EXPIRED_DATE}", expiredDate.ToString("dd/MM/yyyy"))
                   );

            var html = Build(iconType, title, bodyHtml);
            GmailSvc svc = new GmailSvc(EmailSendType.System_NoReply);
            svc.SendMail(title, html, new MailAddress(_emailTo.Email, _emailTo.FullName), _replyMail);
        }
        #endregion

        #region Notify new lead
        /// <summary>
        /// Gửi thông báo khách mới cho user
        /// </summary>
        /// <param name="linkPage"></param>
        /// <param name="htmlInfo"></param>
        public void SendAlertNewLead(string linkPage, string htmlInfo)
        {
            string template = HappyRE.Core.Entities.Resources.Email.NOTIFY_NEW_LEAD;
            string title = CommonUtils.FormatEmailTemplate("Bạn có khách đăng kí mới trên trang {#LINK}",
                   new KeyValuePair<string, string>("{#LINK}", linkPage));
            string iconType = GetIconUrl("alert.png");
            var bodyHtml = CommonUtils.FormatEmailTemplate(
                    template,
                   new KeyValuePair<string, string>("{#NAME}", _emailTo.FullName),
                   new KeyValuePair<string, string>("{#LINK}", linkPage),
                   new KeyValuePair<string, string>("{#CUSTOMER_INFO}", htmlInfo));

            var html = Build(iconType, title, bodyHtml);
            GmailSvc svc = new GmailSvc(EmailSendType.Alert,"Punnel");
            svc.SendMail(title, html, new MailAddress(_emailTo.Email, _emailTo.FullName), _replyMail);
        }
        #endregion
    }
}
