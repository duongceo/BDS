using log4net;
using Newtonsoft.Json;
using Punnel.Core.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Punnel.IntegrationService.SendGrid
{
    public class SendGridTask
    {
        private static readonly ILog _log = LogManager.GetLogger("SendGridTask");
        public static readonly string BaseApi = "https://api.SendGrid.com/v3/";
        public static readonly string Account_Api = "accounts";
        public static readonly string Campaign_Api = "campaigns";
        public static readonly string Contact_Api = "contacts";
        string _apiKey;
        public SendGridTask(string apiKey)
        {
            _apiKey = apiKey;
        }
    }
}
