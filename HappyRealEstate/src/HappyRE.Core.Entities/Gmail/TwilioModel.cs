using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.Integration.Twilio
{
    public class TwilioAccountModel
    {
        public string AccSid { get; set; }
        public string AuthToken { get; set; }
        public string FriendlyName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
