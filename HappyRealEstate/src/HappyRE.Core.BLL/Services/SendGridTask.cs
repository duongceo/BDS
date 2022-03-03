using log4net;
using Newtonsoft.Json;
using HappyRE.Core.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.IntegrationService.SendGrid
{
    public class SendGridService
    {
        private static readonly ILog _log = LogManager.GetLogger("SendGridService");
        public static readonly string BaseApi = "https://api.SendGrid.com/v3/";
        public static readonly string Account_Api = "accounts";
        public static readonly string Campaign_Api = "campaigns";
        public static readonly string Contact_Api = "contacts";
        string _apiKey;
        public SendGridService(string apiKey)
        {
            _apiKey = apiKey;
        }
    }
}
