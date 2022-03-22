
using HappyRE.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HappyRE.Core.Resources;

namespace HappyRE.Web.Models
{
    [Serializable]
    public class AgentMessageViewModel
    {
        public AgentMessageViewModel()
        {

        }
        public AgentMessageSetting Setting { get; set; }
        public UserInboxViewModel UserInbox { get; set; }
    }

    public class AgentMessageSetting
    {
        public AgentMessageSetting()
        {
            IsShowAgent = true;
        }
        public string FormName { get; set; }
        public bool IsShowAgent { get; set; }
        public bool IsShowType { get; set; }
        public UserInboxType InboxType { get; private set; }
    }

    public class UserInboxViewModel
    {
        public int ProfileId { get; set; }
        public int PropertyId { get; set; }
        public string Avatar { get; set; }
        public string AgentUrl { get; set; }
        public string AgentCode { get; set; }
        public string AgentName { get; set; }
        public string AgentMobile { get; set; }
        public string ContactName { get; set; }
        public string ContactMobile { get; set; }
        public string SenderName { get; set; }
        public string AgentCerNo { get; set; }
        public string SenderMobile { get; set; }
        public string SenderEmail { get; set; }
        public string SenderMessage { get; set; }
        public int InboxType { get; set; }
        public string Guid { get; set; }
        public string Captcha { get; set; }
        public int UserTypeId { get; set; }
        public bool IsHidden { get; set; }
        public bool IsVerifiedIDCard { get; set; }
        public DateTime JoinedDate { get; set; }

        public bool HasCertNo()
        {
            return string.IsNullOrEmpty(this.AgentCerNo) == false;
        }

        public string GetAvatarUrl()
        {
            if (string.IsNullOrEmpty(this.Avatar))
            {
                return Core.Utils.Common.AvatarUrl;
            }
            return this.Avatar.Replace("thumb-avatar", "thumb-avatar");
        }

        public UserInbox ToUserInbox()
        {
            return new UserInbox
            {
                Body = SenderMessage,
                Email = SenderEmail,
                PropertyId = PropertyId,
                Sender = SenderName,
                ProfileId = ProfileId,
                Mobile = SenderMobile,
                InboxTypeId = InboxType
            };
        }

        public bool IsContractorPost()
        {
            return (!string.IsNullOrEmpty(this.ContactName) && !string.IsNullOrEmpty(this.ContactMobile));
        }

		public string JoineDateToString()
		{
			string res = string.Empty;
			if (JoinedDate == DateTime.MinValue) return res;

			return Core.Utils.Common.JoineDateToString(this.JoinedDate);

			//var ts = DateTime.Now - JoinedDate;
			//long days = Convert.ToInt64(ts.TotalDays);
			//bool greater_year = false;
			//if (days > 365)
			//{
			//	res = string.Format(Message.Agent_JoinedDate_Year, (days / 365).ToString("N0"));
			//	days = days % 365;
			//	greater_year = true;
			//}
			//if (days > 30)
			//{
			//	res += (res == "" ? "" : " ") + string.Format(Message.Agent_JoinedDate_Month, (days / 30).ToString("N0"));
			//	days = days % 30;
			//}
			//if (days > 0 && greater_year == false)
			//{
			//	res += (res == "" ? "" : " ") + string.Format(Message.Agent_JoinedDate_Day, days.ToString("N0"));
			//}
			//return res;
		}
    }
}