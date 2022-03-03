using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.App.Models
{
    public class LogRequest
    {
        public string User { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string Referer { get; set; }
    }

    public class ErrorModel
    {
        public int StatusCode { get; set; }
    }

    public class Msg
    {
        public string Id { get; set; }
        public string Message { get; set; }
    }

    public class SendToMessenger
    {
        public string Title { get; set; }
        public string ProviderId { get; set; }
        public string UserId { get; set; }
        public string Ref { get; set; }
        public string CtaText { get; set; }
    }

    public class DistrictModel
    {
        public List<Core.Entities.Model.City> City { get; set; }
        public List<Core.Entities.Model.District> District { get; set; }
    }

    public class UserListModel
    {
        public List<Core.Entities.ViewModel.KeyValueModel> Department { get; set; }
        public List<Core.Entities.ViewModel.KeyValueUserModel> User { get; set; }
    }

    public class PropertyListModel
    {
        public List<Core.Entities.ViewModel.KeyValueModel> Property { get; set; }
    }
}