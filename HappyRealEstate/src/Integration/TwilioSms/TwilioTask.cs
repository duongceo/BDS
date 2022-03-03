using log4net;
using Punnel.Core.Entities;
using Punnel.Core.Entities.Integration.Twilio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Punnel.IntegrationService.TwilioSms
{
    public class TwilioTask
    {
        private static readonly ILog _log = LogManager.GetLogger("TwilioTask");
        string _accountSid, _authToken;
        //const string accountSid = "AC53331bbc9dcae863be677c7080caa9ea";
        //const string authToken = "your_auth_token";
        public TwilioTask(string accountSid, string authToken)
        {
            _accountSid = accountSid;
            _authToken = authToken;
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
        }

        public ApiResponse Auth()
        {
            ApiResponse res = new ApiResponse();
            try
            {
                if(string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken))
                {
                    res.Message = Punnel.Core.Entities.Resources.Messages.ApiKey_Err;
                    return res;
                }
                TwilioClient.Init(_accountSid, _authToken);
                var incomingPhoneNumbers = IncomingPhoneNumberResource.GetPage($"https://api.twilio.com/2010-04-01/Accounts/{_accountSid}/IncomingPhoneNumbers/.json?PageSize=10&Page=0", TwilioClient.GetRestClient());
                if (incomingPhoneNumbers.Records.Count > 0)
                {
                    res.Code = System.Net.HttpStatusCode.OK;
                    res.Data = new TwilioAccountModel()
                    {
                        AccSid = _accountSid,
                        AuthToken = _authToken,
                        PhoneNumber = incomingPhoneNumbers.Records[0].PhoneNumber.ToString(),
                        FriendlyName = incomingPhoneNumbers.Records[0].FriendlyName
                    };
                }
                else
                {
                    res.Message = "Tài khoản chưa được tạo số điện thoại gửi đi";
                }
                
            }catch(Exception ex)
            {
                _log.Error(ex);
                res.Message = Punnel.Core.Entities.Resources.Messages.ApiKey_Err;
            }
            return res;
        }

        public ApiResponse Send(string from_mobile,string to_mobile, string content)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                if (!to_mobile.StartsWith("+84")) to_mobile = $"+84{to_mobile.Substring(1, to_mobile.Length - 1)}";
                TwilioClient.Init(_accountSid, _authToken);
                var message = MessageResource.Create(
                body: content,
                from: new Twilio.Types.PhoneNumber(from_mobile),
                to: new Twilio.Types.PhoneNumber(to_mobile));
                var is_send= message.Status == MessageResource.StatusEnum.Failed ? false : true;
                if (is_send)
                {
                    res.Code = System.Net.HttpStatusCode.OK;
                    res.Data = is_send;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return res;
        }

        public bool SendSms(string to_mobile,string content)
        {
            try
            {
                if (!to_mobile.StartsWith("+84")) to_mobile = $"+84{to_mobile.Substring(1, to_mobile.Length - 1)}";
                TwilioClient.Init(_accountSid, _authToken);
                var message = MessageResource.Create(
                body: content,
                from: new Twilio.Types.PhoneNumber("+12075077338"),
                to: new Twilio.Types.PhoneNumber(to_mobile));
                return message.Status == MessageResource.StatusEnum.Failed ? false : true;
            }catch(Exception ex)
            {
                _log.Error(ex);
                return false;
            }
        }
    }
}
