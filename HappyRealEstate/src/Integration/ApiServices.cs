using log4net;
using Newtonsoft.Json;
using Punnel.Core.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationServices
{
    public class ApiHookServices
    {
        private static readonly ILog _log = LogManager.GetLogger("ApiHookServices");
        string _apiKey, _hookUrl;
        public ApiHookServices(string hookUrl, string apiKey="")
        {
            _apiKey = apiKey;
            _hookUrl = hookUrl;
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
        }

        public ApiResponse Auth()
        {
            var ex_contact = new ContactHookModel()
            {
                Name = "User Test",
                Email = "test@gmail.com",
                Phone = "090000000"
            };
            return Send(ex_contact);
        }

        public ApiResponse Send(ContactHookModel model)
        {
            ApiResponse res = new ApiResponse();
            res.Data = false;
            try
            {
                var client = new RestClient(_hookUrl);
                var request = new RestRequest(Method.POST);
                request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                res.Code = response.StatusCode;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    res.Data = true;
                }
                else
                {
                    res.Code = response.StatusCode;
                    res.Message = Punnel.Core.Entities.Resources.Messages.ApiUrl_Err;
                    _log.Error(response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                res.Message = Punnel.Core.Entities.Resources.Messages.ApiUrl_Err;
                _log.Error(ex);
            }
            return res;
        }
    }

    public class ContactHookModel
    {
        [JsonProperty(PropertyName ="name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }
        
        [JsonProperty(PropertyName = "source")]
        public string Referal { get; set; }
        [JsonProperty(PropertyName = "ip")]
        public string IpAddress { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public string MetaData { get; set; }

    }
}
