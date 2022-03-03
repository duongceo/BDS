using log4net;
using Newtonsoft.Json;
using HappyRE.Core.Entities;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HappyRE.Core.Entities.Integration.SendGrid;

namespace HappyRE.EmailServices
{
    public class SendGridSvc
    {
        private static readonly ILog _log = LogManager.GetLogger("SendGridSvc");
        private readonly string _BaseApi = "https://api.sendgrid.com/v3";
        private readonly string _apiKey = "";
        public SendGridSvc(string apiKey)
        {
            _apiKey = apiKey;
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
        }

        public ApiResponse Auth()
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var client = new RestClient($"{_BaseApi}/user/username");
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("Authorization", $"Bearer {_apiKey}");
                IRestResponse response = client.Execute(request);
                res.Code = response.StatusCode;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    res.Data = JsonConvert.DeserializeObject<AccountResponse>(response.Content);
                }
                else
                {
                    res.Message = HappyRE.Core.Entities.Resources.Messages.ApiKey_Err;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                res.Message = HappyRE.Core.Entities.Resources.Messages.ApiKey_Err;
            }
            return res;
        }

        ApiResponse GetSenders()
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var client = new RestClient($"{_BaseApi}/verified_senders");
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("Authorization", $"Bearer {_apiKey}");
                IRestResponse response = client.Execute(request);
                res.Code = response.StatusCode;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var r = JsonConvert.DeserializeObject<SenderResponse>(response.Content);
                    res.Data = r.results.FirstOrDefault(x => x.locked == false && x.verified == true);
                }
                else
                {
                    res.Message = HappyRE.Core.Entities.Resources.Messages.ApiKey_Err;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                res.Message = HappyRE.Core.Entities.Resources.Messages.ApiKey_Err;
            }
            return res;
        }
        public async Task<ApiResponse> Send(string subject, string htmlBody, System.Net.Mail.MailAddress to, System.Net.Mail.MailAddress from)
        {
            var senderRes = GetSenders();
            if (senderRes.Code != System.Net.HttpStatusCode.OK) return new ApiResponse();
            if (senderRes.Data == null)
            {
                _log.Error($"Tài khoản SendGrid {from.Address} chưa có sender được xác thực");
                return new ApiResponse();
            }
            var sender = (Sender)senderRes.Data;
            var client = new SendGridClient(_apiKey);
            var fromEmail = new EmailAddress(sender.from_email, from.DisplayName);
            var toEmail = new EmailAddress(to.Address, to.DisplayName);
            var plainTextContent = "";
            var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, plainTextContent, htmlBody);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            return new ApiResponse()
            {
                Code= response.StatusCode,
                Data= null
            };
        }
    }
}
