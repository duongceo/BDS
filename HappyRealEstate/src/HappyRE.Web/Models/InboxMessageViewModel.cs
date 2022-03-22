using HappyRE.Core.MapModels;
using HappyRE.Core.MapModels.FrontEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class InboxMessageViewModel
    {
        public List<MessageItem> Items { get; set; }
        public Paging Paging { get; set; }
    }
}