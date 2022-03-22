using System;
using System.Web.Mvc;
namespace Mogi.Web.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CaptchaValidationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaCheckAttribute"/> class.
        /// </summary>
        public CaptchaValidationAttribute()
            : this("captcha") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaCheckAttribute"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        public CaptchaValidationAttribute(string field)
        {
            Field = field;
        }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>The field.</value>
        public string Field { get; set; }

        /// <summary>
        /// Called when [action executed].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // make sure the captcha valid key is not contained in the route data
            if (filterContext.RouteData.Values.ContainsKey("captchaValid"))
                filterContext.RouteData.Values.Remove("captchaValid");

            // get the guid from the post back
            string guid = filterContext.HttpContext.Request.Form["captcha-guid"];

            // check for the guid because it is required from the rest of the opperation
            if (String.IsNullOrEmpty(guid))
            {
                filterContext.RouteData.Values.Add("captchaValid", false);
                return;
            }

            // get values
            CaptchaImage image = CaptchaImage.GetCachedCaptcha(guid);
            string actualValue = filterContext.HttpContext.Request.Form[Field];
            string expectedValue = image == null ? String.Empty : image.Text;

            // removes the captch from cache so it cannot be used again
            filterContext.HttpContext.Cache.Remove(guid);

            // validate the captch
            if (String.IsNullOrEmpty(actualValue) || String.IsNullOrEmpty(expectedValue) || !String.Equals(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                filterContext.RouteData.Values.Add("captchaValid", false);
                return;
            }

            filterContext.RouteData.Values.Add("captchaValid", true);
        }
        
    }
}