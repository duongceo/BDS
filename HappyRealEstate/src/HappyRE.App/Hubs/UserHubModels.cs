using HappyRE.Core.Utils.Helpers;
using System;
using System.Collections.Generic;

namespace HappyRE.App
{
    public class UserHubModels
    {
        public string UserName { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }

    public class NotificationResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TimeDisplay => this.CreatedDate.ToFriendlyDate();
    }
}