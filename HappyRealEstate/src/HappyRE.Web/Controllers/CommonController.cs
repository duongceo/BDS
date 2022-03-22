
using HappyRE.Web.Helpers;
using HappyRE.Web.Models;
using System.Web.Mvc;
using System.Collections.Generic;

using HappyRE.Core.MapModels.FrontEnd;

using HappyRE.Core.MapModels;
using Mogi.BLL.Utils;
using System;
using MBN.Utils;

namespace HappyRE.Web.Controllers
{
    public class CommonController : BaseController
    {
		public CommonController(IUow uow) : base(uow)
		{
		}

		private bool SendSupportEmail(FeedBackViewModel model)
		{
			try
			{
				var obj = _uow.CMSCategory.Get(Core.Const.MAIL_TEMPLATE_SUPPORT);
				if (obj != null)
				{
					BLL.Utils.EmailUtil.SendMail(
						Core.Const.MAIL_TYPE_SUPPORT,
						BLL.Utils.EmailUtil.emailSupport,
						"Yêu cầu hỗ trợ",
						Core.Const.MAIL_TEMPLATE_SUPPORT,
						obj.Description,
						new Dictionary<string, string>() {
						{ "MOBILE", model.Mobile},
						{ "EMAIL", model.Email},
						{"CONTENT", model.Content }
						});
				}
				else
				{
					string body = string.Format("{0} <br/> {1} <br/> {2}", model.Mobile, model.Email, model.Content);
					EmailUtil.SendMail(EmailUtil.emailSupport, "Yêu cầu hỗ trợ", body);
				}

				return true;
			}
			catch (Exception ex)
			{
				WebLog.Log.Error("Home.Contact", string.Format("Lỗi gửi mail support {0}", ex.Message));
			}

			return false;
		}

		[AllowAnonymous]
		[HttpPost]
		public JsonResult FeedBack(FeedBackViewModel model)
		{
			var rp = new AjaxResponse();
			if (this.GoogleCaptchaValidate(model.Captcha) == true)
			{
				rp.Status = this.SendSupportEmail(model);
				if (rp.Status == true)
				{
					rp.Message = Core.Resources.Message.FeedBack_ThankYou;
					return Json(rp, JsonRequestBehavior.AllowGet);
				}
			}
			rp.Message = Core.Resources.Message.Status_Error;
			return Json(rp, JsonRequestBehavior.AllowGet);
		}

		[OutputCache(Duration = CACHE_ONE_HOUR, Location = System.Web.UI.OutputCacheLocation.Any, VaryByParam = "id")]
		public JsonResult GetMessage(string id = "")
		{
			List<MessageItem> messages = null;

			if (id == "mogipro")
			{
				messages = _uow.InboxMessage.GetLatestForMogiPro();
			}
			else if (id == "member")
			{
				messages = _uow.InboxMessage.GetLatestForMember();
			}
			else
			{
				messages = _uow.InboxMessage.GetLatestForAnonymous();
			}
			return Json(messages, JsonRequestBehavior.AllowGet);
		}
	}
}