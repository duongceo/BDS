using log4net;
using Newtonsoft.Json;
using Punnel.Core.Entities;
using Punnel.Core.Entities.Integration.OmiCall;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Punnel.IntegrationService.OmiCall
{
    public class OmiCallTask
    {
        private static readonly ILog _log = LogManager.GetLogger("OmiCallTask");
        public static string BaseApi = "https://public-v1-stg.omicall.com";
        public static readonly string Account_Api = "users";
        public static readonly string Campaign_Api = "list_list";
        public static readonly string Contact_Api = "contact_add";
        string _apiKey="", _accessToken="";
        public OmiCallTask(string apiKey)
        {
            _apiKey = apiKey;
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
        }

        ApiResponse GetAccessToken()
        {
            ApiResponse res = new ApiResponse();
            try
            {
                var client = new RestClient($"{BaseApi}/api/auth?apiKey={_apiKey}");
                var request = new RestRequest(Method.GET);
                request.AddHeader("apiKey", _apiKey);
                IRestResponse response = client.Execute(request);
                res.Code = response.StatusCode;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var r = JsonConvert.DeserializeObject<Core.Entities.Integration.OmiCall.AuthResponse>(response.Content);
                    _accessToken = r.payload.access_token;
                    if (_accessToken == null)
                    {
                        res.Code = System.Net.HttpStatusCode.Unauthorized;
                        res.Message = Punnel.Core.Entities.Resources.Messages.ApiKey_Err;
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = Punnel.Core.Entities.Resources.Messages.ApiKey_Err;
            }
            return res;
        }

        public ApiResponse Auth()
        {
            ApiResponse res = new ApiResponse();
            try
            {
                GetAccessToken();
                var client = new RestClient($"{BaseApi}/api/tenant/detail");
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", $"Bearer {_accessToken}");
                var response = client.Execute(request);
                res.Code = response.StatusCode;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var ra = JsonConvert.DeserializeObject<Core.Entities.Integration.OmiCall.AccountResponse>(response.Content);
                    res.Data = ra;
                }
                else
                {
                    _log.Error(response.StatusDescription);
                    _log.Error(response.ErrorMessage);
                    res.Message = Punnel.Core.Entities.Resources.Messages.ApiKey_Err;
                }
            }
            catch (Exception ex)
            {
                res.Message = Punnel.Core.Entities.Resources.Messages.ApiKey_Err;
            }
            return res;
        }

        #region Contact
        public ApiResponse AddContact(ContactRequest contact_request)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                GetAccessToken();
                var client = new RestClient($"{BaseApi}/api/contacts/add");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", $"Bearer {_accessToken}");
                request.AddParameter("application/json", JsonConvert.SerializeObject(contact_request), ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                res.Code = response.StatusCode;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    res.Code = System.Net.HttpStatusCode.OK;
                    var contactRes = JsonConvert.DeserializeObject<ContactResponse>(response.Content);
                }
                else
                {
                    res.Code = response.StatusCode;
                    res.Message = response.Content;
                    _log.Error(response);
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
            }
            return res;
        }
        #endregion
    }
}
