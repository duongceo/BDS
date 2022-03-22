using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HappyRE.Core.Entities;
using HappyRE.Core.MapModels;
using HappyRE.Web.Models;

using HappyRE.Core.MapModels.Search;
using HappyRE.Core.Resources;
using System.ComponentModel;
using MBN.Utils;
using HappyRE.Web.Helpers;
using HappyRE.Core.MapModels.FrontEnd;

namespace HappyRE.Web.Controllers
{
    public class AgentController : BaseController
    {

        public AgentController(IUow uow) : base(uow)
        {
            ViewBag.Menu_ActiveCode = "menu-agent";
        }

		/// <summary>
		/// GET: Agent
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rent"></param>
		/// <param name="cp"></param>
		/// <returns></returns>
		//[OutputCache(CacheProfile = "AgentDetail")]
		public ActionResult Detail(int id = 0, bool rent = false, int cp = 1)
        {
            var model = new AgentViewModel();

            model.Agent = _uow.UserProfile.FrontEnd_GetAgent(id);

            if (model.Agent == null || model.Agent.IsLocked == true)
            {
                return RedirectToAction("List");
            }

            var agentObj = model.Agent;
            var url = _uow.UserProfile.FriendlyUrl_Agent(agentObj.ProfileId, agentObj.Mobile, agentObj.Code);

            #region Redirect to correct
            if (model.Agent.Code != null)
            {
                var requestUrl = this.Request.Url.AbsolutePath;
                if (!requestUrl.EndsWith(url))
                {
                    return Redirect(url);
                }
            }

            #endregion

            #region User Properties
            var filter = new Core.MapModels.SearchFilter();
            var rentPageIndex = (rent == true) ? cp : 1;
            var buyPageIndex = (rent == false) ? cp : 1;

            var rentProperties = _uow.Property.FrontEnd_GetByProfile(true, id, rentPageIndex);
            var rentUrl = this.Url.Action("Detail", "Agent", new { id = id, rent = true, pageIndex = cp }, this.Request.Url.Scheme);
            if (rentProperties != null)
            {
                model.RentProperties = rentProperties.Response.Data;
                model.RentPaging = this.GetPaging(rentUrl, rentPageIndex, rentProperties.Response.Total);
            }

            var buyProperties = _uow.Property.FrontEnd_GetByProfile(false, id, buyPageIndex);
            var buyUrl = this.Url.Action("Detail", "Agent", new { id = id, rent = false, pageIndex = cp }, this.Request.Url.Scheme); ;
            if (rentProperties != null)
            {
                model.SaleProperties = buyProperties.Response.Data;
                model.SalePaging = this.GetPaging(buyUrl, buyPageIndex, buyProperties.Response.Total);
            }

            if (rentProperties.Response.Total > 0 && buyProperties.Response.Total > 0)
            {
                ViewBag.IsRent = rent;
            }
            else
            {
                ViewBag.IsRent = rentProperties.Response.Total == 0 ? false : true;
            }

            ViewBag.SaleIds = string.Join(",", model.SaleProperties.Select(c => c.PropertyId).ToList());
            ViewBag.RentIds = string.Join(",", model.RentProperties.Select(c => c.PropertyId).ToList());
            ViewBag.RentItems = rentProperties.Response.Total;
            ViewBag.BuyItems = buyProperties.Response.Total;
            ViewBag.TotalItems = buyProperties.Response.Total + rentProperties.Response.Total;
            #endregion

            #region Message Box disable by cuong.phung 09-04-2019
            //var userInbox = new UserInboxViewModel
            //{
            //    ProfileId = model.Agent.ProfileId,
            //    PropertyId = 0,
            //    SenderMessage = Core.Resources.Message.Property_Broker_Message,
            //    AgentCerNo = model.Agent.CertNo,
            //    AgentName = model.Agent.Name,
            //    AgentMobile = model.Agent.Mobile,
            //    Avatar = model.Agent.ProfilePicture,
            //};

            //if (this.IsAuthenticated())
            //{
            //    userInbox.SenderEmail = this.UserData.Email;
            //    userInbox.SenderMobile = this.UserData.Mobile;
            //    userInbox.SenderName = this.UserData.LastName + " " + this.UserData.FirstName;

            //}

            //model.LeftContactForm = new AgentMessageViewModel
            //{
            //    UserInbox = userInbox,
            //    Setting = new AgentMessageSetting
            //    {
            //        FormName = "LeftContactForm",
            //        IsShowAgent = true,
            //        IsShowType = true
            //    }
            //};
            #endregion

            #region SEO
            ViewBag.OG_Title = ViewBag.Title;
            ViewBag.OG_Description = agentObj.ShortIntro;
            ViewBag.OG_Image = agentObj.ProfilePicture;
            ViewBag.Canonical = Core.Utils.Common.HomeUrl + url;
			if (model.Agent.IsHidden == true)
			{
				ViewBag.Robots = "noindex";
			}
            this.SEO_Detail(agentObj);
            #endregion

            return View(model);
        }

		[OutputCache(CacheProfile = "AgentListing")]
		public ActionResult List(FrontEndAgentQuery query)
        {
            var model = new AgentListViewModel();

            if (string.IsNullOrEmpty(query.Keyword) == true)
            {
                query.Keyword = WebUtils.GetQuery("q", "");
            }

            var results = _uow.UserProfile.FrontEnd_Search(query);
            var requestUrl = _uow.UserProfile.FriendlyUrl_AgentListing(query);
            model.Query = query;
            model.Query.Keyword = GetAgentKeyWord(query);
            model.Query.Url = (this.Request.Url.Segments.Count() == 3) ? this.Request.Url.Segments.Last() : "";
            model.Agents = results.Response.Data;
            model.Paging = this.GetPaging(requestUrl, query.cp, results.Response.Total, query.PageSize);


			#region // Get user's total posts - disabled by quy.vu - 15:12 2018-11-09
			/*
			var stats = _uow.Property.FrontEnd_GetTotalByProfile(model.Agents.Select(c => c.ProfileId).ToList());
            foreach (var item in model.Agents)
            {
                if (stats.ContainsKey(item.ProfileId) == false)
                {
                    item.TotalSale = item.TotalRent = 0;
                    continue;
                }
                var obj = (Core.MapModels.Search.AgentStatistic)stats[item.ProfileId];
                item.TotalSale = obj.TotalSale;
                item.TotalRent = obj.TotalRent;
            }
			*/
			#endregion

			// seo
			ViewBag.Canonical = Core.Utils.Common.HomeUrl + requestUrl;
            this.SEO_Listing(query.Keyword);

            return View(model);
        }


        #region Ajax
        /// <summary>
        /// Send Message to Agent
        /// cuong.phung
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendMessageToAgent(UserInboxViewModel model)
        {
            var rp = new AjaxResponse();

            try
            {
                var res = this.GoogleCaptchaValidate(model.Captcha);
                if (!res)
                {
                    rp.Status = false;
                    rp.Message = Core.Resources.Message.Captcha_Error;
                }
                else
                {
                    var message = model.ToUserInbox();
                    // reset default value
                    message.LanguageId = Core.Const.LANG_ID;
                    message.Created = DateTime.Now;
                    message.SentDate = DateTime.Now;
                    message.Updated = DateTime.Now;
                    message.ReadDate = null;
                    message.IsRead = false;
                    message.IsDeletedd = false;
                    message.PropertyUrl = (message.PropertyUrl == null) ? "" : message.PropertyUrl;
                    if (model.InboxType == 0)
                    {
                        message.InboxTypeId = (int)UserInboxType.CONTACT;
                    }

                    _uow.UserInbox.AddMessage(message);
                    rp.Status = true;
                }
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = Core.Resources.Message.Status_Error;
                WebLog.Log.Error("Agent.SendMessageToAgent", string.Format("Lỗi gửi tin nhan cho moi giới {0}", ex.StackTrace));
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private
        /// <summary>
        /// Get Paging for List Object
        /// </summary>
        /// <param name="model"></param>
        private Paging GetPaging(string pageUrl, int pageIndex, int totalPage, int pageSize = 10)
        {
            Paging paging = new Paging();
            paging.Total = totalPage;
            paging.CurrentPage = pageIndex;
            paging.PageSize = pageSize;
            paging.Url = pageUrl;
            return paging;
        }

        /// <summary>
        /// Find Favorite Properties on property list
        /// </summary>
        /// <param name="propertyIds">list of property Id</param>
        /// <returns></returns>
        private string[] FindFavoriteProperties(string[] propertyIds)
        {
            string[] favorites = null;

            if (this.IsAuthenticated())
            {
                var profileId = this.UserData.ProfileId;
                favorites = _uow.Favorite.GetByProperties(propertyIds, profileId);
            }
            else
            {
                favorites = _uow.Favorite.GetByProperties(propertyIds, this.ClientId());
            }
            return favorites;
        }

        private void SEO_Listing(string keyword)
        {
            try
            {
                var seo = _uow.SEO_Page.SEO_AgentListing(keyword);
                ViewBag.TitlePage = seo.TitlePage;
                ViewBag.Title = seo.Title;
                ViewBag.Description = seo.Description;
                ViewBag.PageDescription = seo.PageDescription;
                ViewBag.Keywords = seo.MetaKeywords;
                ViewBag.Footer = seo.Footer;
                // ViewBag.Canonical = seo.Alternate;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("AgentController.SEO_Listing", ex);
                //throw ex;
            }
        }

        private void SEO_Detail(AgentDisplay agent)
        {
            try
            {
                var seo = _uow.SEO_Page.SEO_AgentDetail(agent);
                ViewBag.TitlePage = seo.TitlePage;
                ViewBag.Title = seo.Title;
                ViewBag.Description = seo.Description;
                ViewBag.PageDescription = seo.PageDescription;
                ViewBag.Keywords = seo.MetaKeywords;
                ViewBag.Footer = seo.Footer;
                //ViewBag.Canonical = seo.Alternate;

            }
            catch (Exception ex)
            {
                WebLog.Log.Error("AgentController.SEO_Detail", ex);
                //throw ex;
            }
        }

        private string GetAgentKeyWord(FrontEndAgentQuery filter)
        {
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                return filter.Keyword;
            }
            // search by agent name
            if (filter.AgentId > 0)
            {
                var objP = _uow.UserProfile.Get(filter.AgentId);
                if (objP != null)
                {
                    return objP.FullName;
                }
                return string.Empty;
            }

            var ReferId = 0;
            var ReferTypeId = 0;

            // City & District
            if (filter.CityId > 0)
            {
                ReferId = filter.CityId;
                City objCity = _uow.City.Get(filter.CityId);
                if (objCity != null)
                {
                    filter.CityId = (objCity.ParentId == 0 ? objCity.CityId : objCity.ParentId);
                    ReferTypeId = (objCity.ParentId == 0) ? Const.MAP_REFERTYPE_CITY : Const.MAP_REFERTYPE_DISTRICT;
                }
            }

            var objMap = _uow.Map.GetBy(ReferId, ReferTypeId);
            if (objMap != null)
            {
                return objMap.Name;
            }
            return string.Empty;
        }
        #endregion
    }
}