using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.Integration.SendGrid
{
    public class AccountResponse
    {
        public string user_id { get; set; }
        public string username { get; set; }
    }

    public class Sender
    {
        public int id { get; set; }
        public string nickname { get; set; }
        public string from_email { get; set; }
        public string from_name { get; set; }
        public string reply_to { get; set; }
        public string reply_to_name { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public bool verified { get; set; }
        public bool locked { get; set; }
    }

    public class SenderResponse
    {
        public List<Sender> results { get; set; }
    }
}
