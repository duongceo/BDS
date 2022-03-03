using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Api.Models
{
    public class WpPublishModel
    {
        public string url { get; set; }
        public string title { get; set; }
        public string html { get; set; }
    }

    public class ContactHookModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public string MetaData { get; set; }
        [JsonProperty(PropertyName = "referal")]
        public string Referal { get; set; }
    }

    public class GolbalTemplateRequest
    {
        public List<Guid> ids { get; set; }
    }
}