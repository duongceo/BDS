﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.Integration.Gmail
{
    [Serializable]
    public class SendMailModel
    {
        public Guid From { get; set; }
        public string ReceiveName { get; set; }
        public string SendTo { get; set; }
        public string SendName { get; set; }
        public string ReplyTo { get; set; }
        public string Title { get; set; }
        public string BodyHtml { get; set; }
        public int? ContactId { get; set; }
        public int Type { get; set; }
    }

    public class SendMailByTemplateModel
    {
        public int TemplateId { get; set; }
        public string LeadIds { get; set; }
    }
}
