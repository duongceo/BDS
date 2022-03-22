using MBN.Utils;
using MBN.Utils.Extension;


using HappyRE.Core.MapModels;
using HappyRE.Core.MapModels.FrontEnd;
using HappyRE.Core.Entities;
using HappyRE.Web.Helpers;
using HappyRE.Web.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using HappyRE.Core.BLL.Repositories;

namespace HappyRE.Web.Controllers
{
    [Authorize(Roles = "Guess,MogiPro")]
    public class ProfileController : BaseController
    {
        public ProfileController(IUow uow) : base(uow)
        {
            ViewBag.Code = "";
        }

        #region Xem & cập nhật thông tin cá nhân
        [Authorize]
        public ActionResult UserProfile()
        {
            var user = _uow.UserProfile.Get(this.CurrentProfileId);

            if (user.IsMogiProAndApproved() == true)
            {
                return Redirect(Core.Utils.Common.MogiProUrl + "/profile");
            }

            ViewBag.Code = "info";
            return View(new UserProfile());
        }

        public JsonResult GetUserData()
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                rp.Data = this.UserData;
                rp.Status = true;
            }
            catch (Core.BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.GetUserData", ex.Message);
                rp.Message = HappyRE.Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProfile()
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                var user = _uow.UserProfile.Get(this.CurrentProfileId);
                ProfileViewModel model = new ProfileViewModel()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    LogoSquare = user.AvatarUrl(),
                    LoginByOpenId = user.IsLoginByOpenId,
                    CityId = user.CityId,
                    //DistrictId = user.DistrictId,
                    ShortIntro = user.ShortIntro,
                    Introduction = user.Introduction,
                    LevelId = user.LevelId,
                    IsWaitForApprove = user.IsWaitForApprove,
                    IsMogiPro = user.IsMogiPro,
                    IsVerifiedEmail = user.IsVerifiedEmail
                };

                rp.Data = model;
                rp.Status = true;
            }
            catch (Core.BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.GetProfile", ex.Message);
                rp.Message = HappyRE.Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateProfile(ProfileViewModel model)
        {

            AjaxResponse rp = new AjaxResponse();
            string returnUrl = string.Empty;
            try
            {
                _uow.UserProfile.Mogi_UpdateProfile(this.CurrentProfileId, model.Mobile, model.Email, model.FirstName, model.LastName, 0, null);
                rp.Message = Core.Resources.Message.Updated_Success;
                rp.Status = true;
                rp.Data = new { data = model, returnUrl = returnUrl };
            }
            catch (Core.BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.UpdateProfile", ex.Message);
                rp.Message = HappyRE.Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Đổi mật khẩu
        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.Code = "changepassword";
            return View(new ProfileChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdatePassword(ProfileChangePasswordViewModel model)
        {
            AjaxResponse rp = new AjaxResponse();
			try
			{
				// valid old password
				var user = _uow.UserProfile.ValidUser(User.Identity.Name, model.OldPassword);
				if (user != null)
				{
					// reset
					_uow.UserProfile.ResetPassword(User.Identity.Name, model.Password);
					rp.Data = model;
					rp.Status = true;
					rp.Message = Core.Resources.Message.Updated_Success;
				}
				else
				{
					rp.Message = HappyRE.Core.Resources.Message.GeneralError;
				}
			}
			catch (Core.BusinessException ex)
			{
				if (ex.ErrorCode == (int)Core.LoginStatus.Invalid)
				{
					rp.Message = HappyRE.Core.Resources.Message.UserProfile_InvalidPassword;
				}
				else
				{
					rp.Message = ex.Message;
				}
			}
			catch (Exception ex)
			{
				WebLog.Log.Error("ProfileController.UpdatePassword", ex.Message);
				rp.Message = HappyRE.Core.Resources.Message.GeneralError;
			}

            return Json(rp, JsonRequestBehavior.AllowGet);
        }
		#endregion

		#region Đăng ký MogiPro
		private ActionResult ReLogin()
		{
			HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			Session.Abandon();
			return RedirectToAction("Login", "Account");
		}

		public ActionResult MogiPro()
        {
			// Tìm routing xóa function này 
            return RedirectToAction("MogiPro", "Account", new { });
        }

        public ActionResult Posting()
        {
            ViewBag.Code = "mogipro";
            var user = _uow.UserProfile.Get(this.CurrentProfileId);
            // not found
            if (user == null)
            {
				return ReLogin();
            }

            // re-login
            if (user.IsMogiProAndApproved() == true && this.UserData.IsMogiPro == false)
            {
				return ReLogin();
			}

			// Verify Mobile
			if (user.NeedVerifiedMobile() == true)
			{
				return RedirectToAction("Verified", "Account", new { returnUrl = Url.Action("Posting", "Profile") });
			}

			// MogiPro: Nâng cấp tài khoản
			if (user.IsMogiPro == false)
			{
				return RedirectToAction("MogiPro", "Profile");
			}
			else if (user.IsMogiProAndApproved() == true)
            {
				// Mogipro - Posting
				string url = WebUtils.AppSettings("MogiProUrl_Posting", string.Empty);
                if (url == string.Empty)
                {
                    url = WebUtils.AppSettings("MogiProUrl", string.Empty);
                    url += (url.EndsWith("/") ? "" : "/") + "property/posting/0";
                }
                return Redirect(url);
            }

            return RedirectToAction("Verified", "Account", new { returnUrl = Url.Action("Posting", "Profile") });
		}
        #endregion

        #region Xác thực SĐT
        public ActionResult Verified()
        {
            var user = _uow.UserProfile.Get(this.CurrentProfileId);
            if (user.IsVerifiedMobile)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new OTPConfirmViewModel() { Mobile = user.Mobile });
        }

        [HttpPost]
        public ActionResult Verified(OTPConfirmViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            try
            {
                _uow.UserProfile.VerifiedMobileByOTP(this.UserData.ProfileId, model.Code);

                return RedirectToAction("MogiPro");
            }
            catch (Core.BusinessException ex)
            {
                //if (ex.Message == Core.Resources.Message.ConfirmMobile_IsExpired || ex.Message == Core.Resources.Message.ConfirmMobile_IsVerified)
                //{
                //    _uow.ConfirmMobile.CreatedVerifiedCode(user.ProfileId, user.Mobile);
                //}
                ModelState.AddModelError("", ex.Message);
                WebLog.Log.ErrorFormat("ProfileController.Verified: ProfileId:{0}, Code: {1}, Error: {2}", this.UserData.ProfileId.ToString(), model.Code, ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", HappyRE.Core.Resources.Message.GeneralError);
                WebLog.Log.ErrorFormat("ProfileController.Verified: ProfileId:{0}, Code: {1}, Error: {2}", this.UserData.ProfileId.ToString(), model.Code, ex.Message);
            }

            return View(model);
        }
        #endregion

        #region Email
        [HttpPost]
        public JsonResult SendVerifyEmail()
        {
            var user = _uow.UserProfile.Get(LoginHelper.GetUserProfileId());
            AjaxResponse rp = new AjaxResponse();
            if (!user.IsVerifiedEmail && !string.IsNullOrEmpty(user.Email))
            {
                //send mail verify                
                try
                {
                    //send mail verify 
                    _uow.UserProfile.SendVerifyEmail(user.ProfileId);
                    rp.Status = true;
                    rp.Message = Core.Resources.Message.Send_EmailVerify_Success;
                }
                catch (Core.BusinessException ex)
                {
                    rp.Message = ex.Message;
                }
                catch (Exception ex)
                {
                    WebLog.Log.Error("ProfileController.SendVerifyEmail", ex.Message);
                    rp.Message = HappyRE.Core.Resources.Message.GeneralError;
                }
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateEmail(Core.MapModels.FrontEnd.ProfileViewModel model)
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                _uow.UserProfile.ChangeEmail(this.UserData.ProfileId, model.Email);
                rp.Status = true;
                rp.Message = $"{Core.Resources.Message.Updated_Success}. {Core.Resources.Message.Send_EmailVerify_Success}";
            }
            catch (Core.BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.UpdateEmail", ex.Message);
                rp.Message = HappyRE.Core.Resources.Message.GeneralError;
            }

            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerifyEmail()
        {
            return View(new HappyRE.Core.MapModels.MogiPro.ProfileViewModel());
        }

        #endregion

        #region Lịch sử tìm kiếm
        public ActionResult Index()
        {
            ViewBag.Code = "alertsearch";
            return View(new UserProfile());
        }

        //public JsonResult GetReceiveEmailType()
        //{
        //    AjaxResponse rp = new AjaxResponse();
        //    try
        //    {
        //        var objSysTable = _uow.SysTable.GetByCode(Core.Const.SYSTABLE_RE_FILTER_ALERT);
        //        var lst = _uow.SysCode.GetBySysTableId(objSysTable.SysTableId);
        //        rp.Data = lst;
        //        rp.Status = true;
        //    }
        //    catch (BusinessException ex)
        //    {
        //        rp.Message = ex.Message;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log.Error("ProfileController.GetReceiveEmailType", ex.Message);
        //        rp.Message = Core.Resources.Message.GeneralError;
        //    }
        //    return Json(rp, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetAlertSearchs(Core.MapModels.AlertSearchQuery query)
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                int total = 0;
                int totalBuy = 0;
                int totalForent = 0;
                query.ProfileId = this.CurrentProfileId;
                var obj = _uow.AlertSearch.GetAlertSearchs(query, out total, out totalBuy, out totalForent);
                rp.Data = new { Data = obj, Total = total, TotalBuy = totalBuy, TotalForent = totalForent };
                rp.Status = true;
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.GetAlertSearchs", ex.Message);
                rp.Message = Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAlertSearchLatest()
        {
            AjaxResponse rp = new AjaxResponse() { Status = false };
            if (User.Identity.IsAuthenticated == false) return Json(rp, JsonRequestBehavior.AllowGet);

            try
            {
                int profileId = this.CurrentProfileId;
                List<AlertSearch> lst = _uow.AlertSearch.GetAlertSearchLatest(profileId);
                var data = (from t in lst
                            select t.ToRecent());
                rp.Data = data;
                rp.Status = true;
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.GetAlertSearchLatest", ex.Message);
                rp.Message = Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAlertSearch(AlertSearchUpdateModel data)
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
				if (data.ReceiveEmailType <= 0)
				{
					data.ReceiveEmailType = (int)ReceiveEmailType.None;
				}
				data.ProfileId = this.CurrentProfileId;
                data.ClientId = this.ClientId();
                _uow.AlertSearch.ChangeReceiveEmailType(data);
                rp.Data = data;
                rp.Status = true;
            }

            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.UpdateAlertSearch", ex.Message);
                rp.Message = Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAlertSearch(int alertSearchId)
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                int profileId = this.CurrentProfileId;
                _uow.AlertSearch.Delete(alertSearchId, profileId);
                rp.Message = Core.Resources.Message.Deleted_Success;
                rp.Status = true;
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.DeleteAlertSearch", ex.Message);
                rp.Message = Core.Resources.Message.GeneralError;
            }

            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRecentSearch(SearchRecentViewModel model, int ReceiveEmailType)
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                var filter = new SearchFilter()
                {
                    CityId = model.Filter.cid,
                    Days = model.Filter.d,
                    DirectionId = model.Filter.dt,
                    DistrictId = model.Filter.did,
                    FromArea = model.Filter.fa,
                    ToArea = model.Filter.ta,
                    FromBedRoom = model.Filter.fbr,
                    ToBedRoom = model.Filter.tbr,
                    FromPrice = model.Filter.fp,
                    ToPrice = model.Filter.tp,
                    LegalId = model.Filter.lg,
                    Map = model.Filter.Map,
                    Location = model.Filter.l,
                    PropertyTypeId = model.Filter.pid,
                    ProjectId = model.Filter.pjid,
                    PlaceId = model.Filter.plid,
                    Radius = model.Filter.r,
                    PropertyStyles = model.Filter.psid,
                    WardId = model.Filter.wid,
                    Polyenc = model.Filter.poly,
                    StreetId = model.Filter.sid,
                    Sort = model.Filter.s,
                    ReferId = model.Filter.rid,
                    MapId = model.Filter.id,
                    ReferTypeId = model.Filter.rtid,
                    Rent = model.Filter.Rent,
                    q = model.Filter.q
                };
                _uow.AlertSearch.IU(filter, this.CurrentProfileId, ReceiveEmailType);
                _uow.AlertSearch.RemoveRecentSearch(this.ClientId(), model.HashKey);
                rp.Data = model;
                rp.Status = true;
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.SaveRecentSearch", ex.Message);
                rp.Message = Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAlertSearch(string json, int ReceiveEmailType)
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                SearchFilter filter = json.FromJson<SearchFilter>();
                _uow.AlertSearch.IU(filter, this.CurrentProfileId, ReceiveEmailType);
                rp.Status = true;
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.SaveAlertSearch", ex.Message);
                rp.Message = Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetRecentSearchs()
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                var obj = _uow.AlertSearch.GetRecentSearch(this.ClientId());
                rp.Data = obj;
                rp.Status = true;
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.GetRecentSearchs", ex.Message);
                rp.Message = Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ClearRecentSearch()
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                _uow.AlertSearch.ClearRecentSearch(this.ClientId());
                rp.Status = true;
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("ProfileController.ClearRecentSearch", ex.Message);
                rp.Message = Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region FavoriteList
        [AllowAnonymous]
        public ActionResult FavoriteList(bool rent = false, int cp = 1)
        {
            ViewBag.Code = "favorite";
            ViewBag.IsAuthenticated = false;
            var model = new FavoriteViewModel();
            int profileId = 0;
            Guid? clientId = null;
            if (this.IsAuthenticated() == true)
            {
                var claim = this.UserData;
                profileId = claim.ProfileId;
                ViewBag.IsAuthenticated = true;
            }
            else
            {
                clientId = this.ClientId();
            }

            var rentPageIndex = 1;
            var buyPageIndex = 1;
            if (rent)
            {
                rentPageIndex = cp;
            }
            else
            {
                buyPageIndex = cp;
            }

            int rentTotal = 0;
            var favoriteRentItems = _uow.Favorite.GetList(out rentTotal, cp, 10, true, profileId, clientId);

            var rentUrl = this.Url.Action("FavoriteList", "Profile", new { rent = true, pageIndex = cp }, this.Request.Url.Scheme); ;
            if (favoriteRentItems != null)
            {
                model.RentProperties = favoriteRentItems.Response.Data;
                model.RentPaging = this.GetPaging(rentUrl, rentPageIndex, rentTotal);
            }

            int buyTotal = 0;
            var favoriteBuyItems = _uow.Favorite.GetList(out buyTotal, cp, 10, false, profileId, clientId);
            var buyUrl = this.Url.Action("FavoriteList", "Profile", new { rent = false, pageIndex = cp }, this.Request.Url.Scheme); ;
            if (favoriteBuyItems != null)
            {
                model.BuyProperties = favoriteBuyItems.Response.Data;
                model.BuyPaging = this.GetPaging(buyUrl, buyPageIndex, buyTotal);
            }

            ViewBag.IsRent = rent;
            ViewBag.RentItems = rentTotal;
            ViewBag.BuyItems = buyTotal;

            return View(model);
        }
		#endregion

		#region Info
		/*
		[AllowAnonymous]
		public JsonResult GetInfo()
		{
			int ProfileId = 0;
			Guid? ClientId = null;
			string FirstName = "", Avatar = "";
			if (User.Identity.IsAuthenticated == true)
			{
				var claim = this.UserData;
				if (claim != null)
				{
					FirstName = (claim.FirstName ?? "");
					Avatar = (string.IsNullOrEmpty(claim.Avatar) == false ? claim.Avatar : Core.Utils.Common.AvatarLoginUrl);
					ProfileId = claim.ProfileId;
				}
			}
			if (ProfileId == 0)
			{
				ClientId = this.ClientId();
			}
			int TotalFavorite = _uow.Favorite.GetTotal(ProfileId, ClientId);
			var res = new { ClientId, ProfileId, FirstName, Avatar, TotalFavorite };

			return Json(res, JsonRequestBehavior.AllowGet);
		}

		[AllowAnonymous]
		[OutputCache(Duration = CACHE_ONE_HOUR, Location = OutputCacheLocation.Any, VaryByParam = "id")]
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

		[AllowAnonymous]
		[OutputCache(Duration = CACHE_TEN_MINUTE, Location = OutputCacheLocation.Client)]
		public JsonResult GetInbox()
		{
			if (User.Identity.IsAuthenticated == false)
			{
				return Json(null, JsonRequestBehavior.AllowGet);
			}
			var res = _uow.UserInbox.GetLastestMessages(this.UserData.ProfileId, 3);

			return Json(res, JsonRequestBehavior.AllowGet);
		}
		*/
		#endregion

		#region Search Alert
		[AllowAnonymous]
        public ActionResult UnsubcribeSearchAlert(string pid, string aid, string sig, bool u = false)
        {
            var isAuth = Core.Utils.Common.VerifySHA1(sig, new string[] { pid, aid });
            if (isAuth)
            {
                var alert = _uow.AlertSearch.Get(int.Parse(aid));
                if (alert != null)
                {
                    ViewBag.AlertTitle = alert.Title;
                    ViewBag.AlertType = (u == true ? (int)ReceiveEmailType.None : alert.ReceiveEmailType);
                    ViewBag.Unsubscribe = u;
                    return View();
                }

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult ConfirmUnsubcribeSearchAlert(int pid, int aid, string sig, int t)
        {
            AjaxResponse rp = new AjaxResponse();
            var isAuth = Core.Utils.Common.VerifySHA1(sig, new string[] { pid.ToString(), aid.ToString() });
            if (isAuth)
            {
                if (t == 0) t = (int)ReceiveEmailType.None;
                var data = new AlertSearchUpdateModel { AlertSearchId = aid, ProfileId = pid, ReceiveEmailType = t };
                _uow.AlertSearch.ChangeReceiveEmailType(data);
            }

            return Json(rp, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Inbox Message

        [AllowAnonymous]
        public ActionResult InboxMessage(int p = 1)
        {
            ViewBag.IsAuthenticated = false;
            int profileId = 0;
            Guid? clientId = null;
            List<MessageItem> items = null;
            var total = 0;
            if (this.IsAuthenticated() == true)
            {
                var claim = this.UserData;
                profileId = claim.ProfileId;
                ViewBag.IsAuthenticated = true;
                if (claim.IsMogiPro)
                {
                    items = _uow.InboxMessage.GetMessageForMogiPro(out total, p);

                }
                else
                {
                    items = _uow.InboxMessage.GetMessageForMember(out total, p);
                }
            }
            else
            {
                clientId = this.ClientId();
                items = _uow.InboxMessage.GetMessageForAnonymous(out total, p);

            }
            var requestUrl = this.Request.Url.ToString();
            requestUrl = requestUrl.Split('?')[0];
            InboxMessageViewModel model = new InboxMessageViewModel();
            model.Items = items;
            model.Paging = new Paging
            {
                CurrentPage = p,
                PageSize = 20,
                Total = total,
                Url = requestUrl
            };

            return View(model);
        }

        [AllowAnonymous]
        public JsonResult MemberInboxMessage(int p = 1)
        {

            AjaxResponse rp = new AjaxResponse();
            int profileId = 0;
            Guid? clientId = null;
            List<MessageItem> items = null;
            var total = 0;
            if (this.IsAuthenticated() == true)
            {
                var claim = this.UserData;
                profileId = claim.ProfileId;
                ViewBag.IsAuthenticated = true;
                if (claim.IsMogiPro)
                {
                    items = _uow.InboxMessage.GetMessageForMogiPro(out total, p);

                }
                else
                {
                    items = _uow.InboxMessage.GetMessageForMember(out total, p);
                }
            }
            else
            {
                clientId = this.ClientId();
                items = _uow.InboxMessage.GetMessageForAnonymous(out total, p);

            }
            //var requestUrl = this.Request.Url.ToString();
            //requestUrl = requestUrl.Split('?')[0];
            //InboxMessageViewModel model = new InboxMessageViewModel();
            //model.Items = items;
            //model.Paging = new Paging
            //{
            //    CurrentPage = p,
            //    PageSize = 20,
            //    Total = total,
            //    Url = requestUrl
            //};
            rp.Data = new
            {
                Items = items,
                Total = total
            };
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpGet]
        public bool ReadMessage(int id)
        {
            if (id == 0) return true;
            ViewBag.IsAuthenticated = false;

            if (this.IsAuthenticated() == true)
            {
                var claim = this.UserData;
                var profileId = claim.ProfileId;
                var item = _uow.UserMessage.GetUserMessage(id, profileId);
                if (item != null)
                {
                    return true;
                }

                var um = new UserMessage
                {
                    MessageId = id,
                    ProfileId = profileId,
                    ReadDate = DateTime.Now


                };

                _uow.UserMessage.Add(um);
                _uow.Commit();
            }


            return true;
        }
        #endregion

        #region Private
        /// <summary>
        /// Get Paging for List Object
        /// </summary>
        /// <param name="model"></param>
        private Paging GetPaging(string pageUrl, int pageIndex, int totalPage, int pageSize = 10)
        {
            Core.MapModels.Paging paging = new Core.MapModels.Paging();
            paging.Total = totalPage;
            paging.CurrentPage = pageIndex;
            paging.PageSize = pageSize;
            paging.Url = pageUrl;
            return paging;
        }
        #endregion
    }
}