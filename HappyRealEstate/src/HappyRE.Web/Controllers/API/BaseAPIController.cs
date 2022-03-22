using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using MBN.Utils;

using HappyRE.Core.MapModels;

namespace HappyRE.Web.Controllers.API
{
    public class BaseAPIController : ApiController
    {
        protected IUow _uow;
        protected Guid? _CurrentUserId = null;
        protected ClaimData _UserData = null;

        public BaseAPIController(IUow uow)
        {
            _uow = uow;
        }

        #region User
        internal bool IsAuthenticated()
        {
            return System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        }

		internal string ClientIP()
		{
			return BLL.Utils.HttpUtils.GetRequestIP();
		}

        internal Guid ClientId()
        {
            return Http.Cookies.UniqueClient(string.Empty);
        }

        internal Guid CurrentUserId
        {
            get { if (_CurrentUserId.HasValue == false) _CurrentUserId = Helpers.LoginHelper.GetUserId(); return _CurrentUserId.Value; }
        }

        internal ClaimData UserData
        {
            get { return _UserData ?? (_UserData = Helpers.LoginHelper.GetUserData()); }
        }

        #endregion


        /// <summary>
        /// Đưa ra một lỗi ứng dụng
        /// </summary>
        /// <param name="code"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorMsg"></param>
        public HttpResponseException Error(HttpStatusCode code, string errorCode, string errorMsg)
        {
            return new HttpResponseException(Request.CreateResponse(code,
                                                                   new Dictionary<string, string>() { 
                                                                   { "Message", errorMsg},
                                                                   { "ErrorCode", errorCode}
                                                                   }));
        }

        /// <summary>
        /// Lấy một thông tin lỗi từ modelstate
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        protected string ModelStateToErrorMessage(ModelStateDictionary modelState)
        {
            var errors = modelState.Values
                  .SelectMany(o => o.Errors)
                  .Where(e => string.IsNullOrEmpty(e.ErrorMessage) == false)
                  .Select(e => e.ErrorMessage).ToList();
            return string.Join("<br/>", errors);
        }

        internal string GetError(Exception ex){
            string res = string.Empty;
            while (ex != null)
            {
                res += "\r\n" + ex.Message;
                ex = ex.InnerException;
            }
            return res;
        }

        #region Log
        internal void WriteError(string function, Exception ex)
        {
            WebLog.Log.Error(function, ex);
        }
        #endregion
    }

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class WebAPIOutputCacheAttribute : ActionFilterAttribute
	{
		private static readonly Dictionary<string, OutputCacheProfile> CacheProfiles = new Dictionary<string, OutputCacheProfile>();
		private static readonly OutputCacheProfileCollection OutputCacheProfiles = ((OutputCacheSettingsSection)System.Configuration.ConfigurationManager.GetSection("system.web/caching/outputCacheSettings")).OutputCacheProfiles;

		public string CacheProfile { get; set; }
		public int Duration { get; set; }
		public int SharedMaxAge { get; set; }
		public bool MustRevalidate { get; set; } = false;
		public bool Publish { get; set; } = true;
		
		public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
		{
			await ApplyCacheHeaderAsync(actionExecutedContext);
		}

		private async Task ApplyCacheHeaderAsync(HttpActionExecutedContext actionExecutedContext)
		{
			if (this.Duration == 0 && string.IsNullOrEmpty(this.CacheProfile) == true)
			{
				await Task.FromResult(0);
				return;
			}

			TimeSpan? t = null;
			DateTime lastModified = DateTime.Now.ToUniversalTime();
			CacheControlHeaderValue cachecontrol = null;
			if (this.Duration > 0)
			{
				cachecontrol = new CacheControlHeaderValue()
				{
					MaxAge = TimeSpan.FromSeconds(this.Duration),
					SharedMaxAge = (SharedMaxAge == 0 ? t : TimeSpan.FromSeconds(this.SharedMaxAge)),
					MustRevalidate = this.MustRevalidate,
					Private = !this.Publish,
					Public = this.Publish
				};
			}
			else if (string.IsNullOrEmpty(CacheProfile) == false)
			{
				var profile = this.GetCacheProfile(this.CacheProfile);
				if (profile.Enabled == false)
				{
					await Task.FromResult(0);
					return;
				}

				Duration = profile.Duration;
				SharedMaxAge = Math.Min(profile.Duration, SharedMaxAge);
				cachecontrol = new CacheControlHeaderValue()
				{
					MaxAge = TimeSpan.FromSeconds(Duration),
					SharedMaxAge = (SharedMaxAge == 0 ? t : TimeSpan.FromSeconds(SharedMaxAge)),
					Public = true
				};
			}
			
			var response = actionExecutedContext.ActionContext.Response;
			response.Headers.CacheControl = cachecontrol;
			response.Content.Headers.LastModified = new DateTimeOffset(lastModified);
			if (response.Content != null)
			{
				response.Content.Headers.Expires = new DateTimeOffset(lastModified.AddSeconds(this.Duration));
			}
		}

		private OutputCacheProfile GetCacheProfile(string name)
		{
			if (CacheProfiles.ContainsKey(name)) return CacheProfiles[name];

			var item = OutputCacheProfiles.Get(name);
			CacheProfiles[name] = item;

			return item;
		}
	}
}
