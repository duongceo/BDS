using System;
using System.IO;
using System.Threading;
using System.Net.Mail;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using MimeKit;
using MBN.Utils;
using HappyRE.Core.Entities;
using System.Collections.ObjectModel;
using log4net;

namespace HappyRE.EmailServices
{
    public class GmailSvc
    {
        private static readonly ILog _log = LogManager.GetLogger("GmailSvc");
        static string[] Scopes = { GmailService.Scope.GmailReadonly, GmailService.Scope.GmailSend };

        static string GSUIT_CLIENT_PRIVATEKEY = WebUtils.AppSettings("GSUIT_CLIENT_PRIVATEKEY", "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDqARiLAqZLEdIG\nXeTpT6NSW70VLWG6kazhghQF4ErWntSjodfBzXj8Wke3prAplLYmdudJrROS3k7r\n3EA5w0ZI9kM7N0IwVX5DObVCLC9R3/JwAzTw4IBXUTZzPJhZdbXymjKCXb+sx+DZ\ny3nzZPP3QMJNkCV/IsIRhxk6T2ga89rTCk/gN79q58Rjpz7WlnMk/z7l9qqJKBMr\nayoql9NBSSAwjfv7d8NJt410HlA5oTujFhmEyFuo27QRsHGsM7z/r2L2gkjQx7Qy\nvzDww4NWvNNqHr0iTQBvYzbzyzvnIy/QSL8bAlHRQW8lIAcZQ3gYoh/7SnWcJ8/G\nVNpMLrQZAgMBAAECggEAJ1571Jdjhum+nheZ9a7SWX7ZtwBlms/7eA08dSihLq6W\nM9l8xTxeiIZf3oQKy1QbuSj0DK8zxdflC0WJAK7b2lakRQgs+JjJn2HMHw/z/iBh\nV3sSoxwCO66MLKMbNqHZNLHAobJIFzehmsLjB5IKgaTBi+8ef99HK0/BbNADQdVG\ntC4nmU0icKaiLsQH9Q/6M4u9gemb+cANl5COuNvGC4K4ufi6NjLLnPVsTBaVby6q\nVShOv/lJEz/57pZu9a0XvJ+0t+JGvQSmB1qD/H7lxzdLjlSCqdLshMWvDgPrVQG3\ngFRlLx72DSxKhcbudgm7+AtVUFCfOqVJvrkfYTT9qQKBgQD4KWOgGDLP+7UsVR6e\n7Hn4m2S9BXTy/hDRv03WtypsctPhnczlo2knUmiFp6w8HMeg2ZPK28otLgsEkEbf\n8r/hS9ImiCce8DLyO9nYR4drtwp/I6omAjbl+ceil0MuwTDDlZoU2Nz7JCYEwPyI\nMBo9nynVQumrssYZcr3bUZHJHwKBgQDxZTs9VHhhZH4UPWHhaO28KYeJgCBv0Kae\nKc0IMTLTQB1h8PBC/l3JVuVQS8xcU1z7hE4Rut6sfmmp645PdUij5Lo9IxtwedVH\na6tFRh7U2L28BXGkTij+psdHkFTBTBe+ynMZb8ntCCjXsv/H+mJYGwkvqAjB2U2M\n0BBUgYADxwKBgQDmEcE6UJTzO8II2tVrs4OVF8P6dk+pZCCFxN1OXjwnlQGY3ypy\nD+DSQtWCbCQDCw+MsYsSfFhhMJmoXnZG85AvOJMZhAr45Onmp9Rcblw/YdCKdK8v\nU8g/yCXsOF5hv1wNR+o0v6WH9EiHCcBWJnp9fM1U2Rc4RqLgPv+DkIkfMwKBgQDS\nSD8co/Lh1GrIMtiLRZ6jJQI+03BuNzKN4RgMpN8Di3yNDxgdI/yLIblNA1qYqr37\nFDv10fWcCdr9/IbRzPdkXiGmlGiYyGj0eGSQSg8dl1lg6tUeLkAj5SD/xEkwwZqF\nx1IQvIMIiu0ZSYLrr7/vxE1ySEyooSWfHri7BLyBjQKBgQDhO1HhH2VSYsLzMxov\niU0RFWHvJqhLOSBNdqEqEn24u8LcGI08rV1MPnLHi+jtKdKRFyStYGwQZRDikzph\nYfEbYOFai+Xmc5fj/JhpMhjbboo8pS6/BsXI7Y3KeLavPzEiei88iiXY3L6tui/x\nGwxTHqgGDj9Bf6hSOtbzAD4gCQ==\n-----END PRIVATE KEY-----\n");
        static string GSUIT_CLIENT_ACCOUNT = WebUtils.AppSettings("GSUIT_CLIENT_ACCOUNT", "punnelmail@funnel-test-201618.iam.gserviceaccount.com");
        static string GSUIT_FROM_EMAIL_SYSTEM = WebUtils.AppSettings("GSUIT_FROM_EMAIL_SYSTEM", "alert@HappyRE.com");
        static string GSUIT_LEAD_ALERT = WebUtils.AppSettings("GSUIT_LEAD_ALERT", "alert@HappyRE.com");
        static readonly ReadOnlyCollection<string> SendFromMail = new ReadOnlyCollection<string>(
          new string[] {
            "hi@HappyRE.com",
            "alert@HappyRE.com",
            "noreply@HappyRE.com"
          }
        );
        GmailService service;
        public string UserEmail { get; set; }
        public string FromName { get; set; }

        public GmailSvc(EmailSendType sendType, string sendFromName ="")
        {
            switch (sendType)
            {
                case EmailSendType.System_NoReply:
                    UserEmail = GSUIT_FROM_EMAIL_SYSTEM;
                    FromName = "Punnel";
                    break;
                case EmailSendType.Lead_AutoReply:
                    UserEmail = GSUIT_LEAD_ALERT;
                    FromName = sendFromName;
                    break;
                case EmailSendType.Alert:
                    UserEmail = GSUIT_LEAD_ALERT;
                    FromName = sendFromName;
                    break;
            }
            Credential();
        }

        public void Credential()
        {
            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(GSUIT_CLIENT_ACCOUNT)
                {
                    User = UserEmail,
                    Scopes = Scopes
                }.FromPrivateKey(GSUIT_CLIENT_PRIVATEKEY));

            if (credential.RequestAccessTokenAsync(CancellationToken.None).Result)
            {
                service = new GmailService(
                    new Google.Apis.Services.BaseClientService.Initializer()
                    {
                        ApplicationName = "Punnel",
                        HttpClientInitializer = credential
                    }
                );
            }
        }

        public ApiResponse SendMail(string subject , string htmlBody, MailAddress to, MailAddress replyTo)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var message = new MimeMessage();
                message.ReplyTo.Add(new MailboxAddress(replyTo.DisplayName, replyTo.Address));
                message.To.Add(new MailboxAddress(to.DisplayName, to.Address));
                message.Subject = subject;
                message.From.Add(new MailboxAddress(FromName, UserEmail));
                var builder = new BodyBuilder();
                builder.HtmlBody = htmlBody;
                message.Body = builder.ToMessageBody();

                Message gmail_msg = new Message();
                gmail_msg.Raw = Base64UrlEncode(message);
                var result = service.Users.Messages.Send(gmail_msg, "me").Execute();
                res.Code = System.Net.HttpStatusCode.OK;
            }
            catch(Exception ex) {
                res.Message = ex.Message;
                _log.Error(ex);
            }
            return res;
        }

        private static string Base64UrlEncode(MimeMessage message)
        {
            using (var stream = new MemoryStream())
            {
                message.WriteTo(stream);
                return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Replace("=", "");
            }
        }

    }
}
