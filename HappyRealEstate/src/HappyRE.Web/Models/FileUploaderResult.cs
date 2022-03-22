using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HappyRE.Web.Models
{
    /// <summary>
    /// Kết quả trả về sau khi login
    /// </summary>
    public class FileUploaderResult : ActionResult
    {
        public const string ResponseContentType = "text/plain";

        private readonly bool _success;
        private readonly string _error;
        private readonly bool? _preventRetry;
        private readonly JObject _otherData;

        public bool Success { get { return _success; } }
        public string Error { get { return _error; } }
        public JObject Data { get { return _otherData; } }

        public FileUploaderResult(bool success, object otherData = null, string error = null, bool? preventRetry = null)
        {
            _success = success;
            _error = error;
            _preventRetry = preventRetry;

            if (otherData != null)
                _otherData = JObject.FromObject(otherData);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = ResponseContentType;

            response.Write(BuildResponse());
        }

        public string BuildResponse()
        {
            var response = _otherData ?? new JObject();
            response["success"] = _success;

            if (!string.IsNullOrWhiteSpace(_error))
                response["error"] = _error;

            if (_preventRetry.HasValue)
                response["preventRetry"] = _preventRetry.Value;

            return response.ToString();
        }
    }
}