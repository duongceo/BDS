using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Text.RegularExpressions;
using HappyRE.Web.Helpers;
using HappyRE.Core.Entities;
using HappyRE.Core.MapModels.FrontEnd;
using HappyRE.Core.Utils;
using MBN.Utils;
using MBN.Utils.Extension;
using HappyRE.Core.MapModels;
using HappyRE.Core.Resources;
using System.Collections.Generic;

namespace HappyRE.Web.Controllers
{
    public class AccountController : BaseController
    {
		private static readonly bool ACCOUNTKIT_USE = WebUtils.AppSettings("ACCOUNTKIT_USE", false);
		private static readonly int ACCOUNTKIT_MAX_FAIL = WebUtils.AppSettings("ACCOUNTKIT_MAX_FAIL", 3);

        public AccountController(IUow uow) : base(uow) { }

		#region Đăng nhập bằng số điện thoại
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			if (User.Identity.IsAuthenticated == true)
			{
				int profileId = this.CurrentProfileId;
				var user = _uow.UserProfile.Get(profileId);
				if (user == null || user.ValidUserName(User.Identity.Name) == false)
				{
					this.SignOut();
				}
				else if (user.NeedVerifiedMobile() == true)
				{
					return RedirectToAction("Verified", "Account");
				}
				else
				{
					return RedirectToLocal(returnUrl);
				}
			}

			//ViewBag.ReturnUrl = returnUrl;
			//var openIdInfo = GetOpenIdInfo();
			//Session["Auth_State"] = openIdInfo["STATE"];
			//return View(new LoginViewModel() { RememberMe = true, OpenId = openIdInfo });

			string state = DateTime.Now.GetHashCode().ToString("x");
			var openIdInfo = GetOpenIdInfo();
			Session["Auth_State"] = openIdInfo["STATE"];
			return View(new RegisterModel()
			{
				OpenId = openIdInfo,
				ACK = new AccountKitAuthModel() { CSRF = this.AccountKitCSRF },
				Step = "login",
				ReturnUrl = returnUrl
			});
		}

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(LoginViewModel model)
        {
			AjaxResponse rp = new AjaxResponse() { Message = "" };

			if (ModelState.IsValid == false)
			{
				rp.Message = Message.User_InvalidLogin;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}
			model.RememberMe = true;

            string fileLog = string.Format("LoginFailed_{0}.txt", DateTime.Today.ToString("yyyyMM"));
            try
            {
                // Captcha: Google
                if (this.GoogleValidCaptcha(model.GoogleCaptchaResponse) == false)
                {
					WebLog.Log.Data(DateTime.Now.yyyyMMddHHmmss() + $": IP: {this.ClientIP}, Data: {model.ToJson()}", true, fileLog);
					rp.Message = Message.User_InvalidLogin;
					return Json(rp, JsonRequestBehavior.AllowGet);
				}

                // ValidUser
                var user = _uow.UserProfile.ValidUser(model.Mobile, model.Password);

                // Lưu lịch sử đăng nhập
                _uow.UserProfile.TrackingUserLogin(user, this.ClientIP, Const.PROFILE_PROVIDER_DEFAULT);

                #region Tự động xác thực Mobile khi tạo môi giới từ CPanel
                if (user.IsVerifiedMobile == false && user.IsMogiPro == true && user.IsApproved == true)
                {
                    try
                    {
						if (true == Regex.IsMatch(model.Password, Const.VALIDATION_OTP))
						{
							bool is_verified = _uow.UserProfile.VerifiedMobileByOTP(user.ProfileId, model.Password, user.Mobile);
							if (is_verified == true)
							{
								user = _uow.UserProfile.UpdateVerifiedMobile(user.ProfileId, user.Mobile);
							}
						}
                    }
                    catch { }
                }
                #endregion

                // Đăng nhập
                LoginHelper.LoginOwin(user, model.RememberMe);

                #region Yêu cầu xác thực số điện thoại
                if (user.IsVerifiedMobile == false)
                {
					rp.Data = Url.Action("Verified", "Account");
                }
				#endregion

				#region Chuyển đến MogiPro
				if (user.IsMogiProAndApproved() == true && string.IsNullOrEmpty(model.ReturnUrl) == true)
				{
					rp.Data = Common.MogiProUrl;
				}
				else
				{
					rp.Data = "/";
				}
				#endregion

				rp.Status = true; // Success
			}
            catch (BusinessException ex)
            {
				rp.Message = ex.Message;

				string msg_tracking = string.Format("Pass: {0}, Error: {1}", model.Password, ex.Message);
                BLL.Queues.UserTrackingQueue.TrackingLogin(string.Empty, model.Mobile, this.ClientIP, msg_tracking);
                string msg = string.Format("{0}\t-\tMobile:{1}, Pass:{2}, Error:{3}", DateTime.Now.yyyyMMddHHmmss(), model.Mobile, model.Password, ex.Message);
                WebLog.Log.Data(msg, true, fileLog);
            }
            catch (Exception ex)
            {
				rp.Message = Message.GeneralError;

				string msg_tracking = string.Format("Pass: {0}, Error: {1}", model.Password, ex.Message);
                BLL.Queues.UserTrackingQueue.TrackingLogin(string.Empty, model.Mobile, this.ClientIP, msg_tracking);
                string msg = string.Format("{0}\t-\tMobile:{1}, Pass:{2}, Error:{3}", DateTime.Now.yyyyMMddHHmmss(), model.Mobile, model.Password, ex.Message);
                WebLog.Log.Data(msg, true, fileLog);
            }

			return Json(rp, JsonRequestBehavior.AllowGet);
		}

		private ActionResult ReLogin()
		{
			HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			Session.Abandon();
			return RedirectToAction("Login", "Account");
		}
        #endregion

        #region Đăng nhập bằng External (Facebook, Google)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            Session["FixOwin"] = 0;
			if (Request.IsLocal == false)
			{
				Response.AddHeader("ExternalLogin", provider);
			}

			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            try
            {
                WebLog.Log.Info("AccountController.ExternalLoginCallback:Start");

                var loginInfo = AuthenticationManager.GetExternalLoginInfo();
                if (loginInfo == null)
                {
                    return RedirectToAction("ExternalLoginFailure");
                }

                WebLog.Log.Info("AccountController.ExternalLoginCallback-done:" + loginInfo.Login.LoginProvider);

                string provider = loginInfo.Login.LoginProvider.ToLower();
                string providerKey = loginInfo.Login.ProviderKey;

                var user = _uow.UserProfile.GetUser(provider, providerKey);

                // first login
                if (user == null)
                {
                    // Tạo user profile cho user đăng nhập qua external nếu chưa có 
                    OpenIDUserModel objOpen = new OpenIDUserModel()
                    {
                        FullName = loginInfo.ExternalIdentity.Name,
                        Email = loginInfo.Email,
                        UserName = loginInfo.Login.ProviderKey,
                        Provider = loginInfo.Login.LoginProvider.ToLower()
                    };
                    _uow.UserProfile.AddUserByOpenId(objOpen);
                }

                try
                {
                    user = _uow.UserProfile.ValidUser(provider, providerKey, string.Empty);
                    LoginHelper.LoginOwin(user, true);

                    //Lưu lịch sử đăng nhập
                    _uow.UserProfile.TrackingUserLogin(user, this.ClientIP, loginInfo.Login.ProviderKey);
                    if (user.IsMogiPro == true && user.IsApproved && string.IsNullOrEmpty(returnUrl))
                    {
                        string mogiProUrl = WebUtils.AppSettings("MogiProUrl", "");
                        return Redirect(mogiProUrl);
                    }
                    return RedirectToLocal(returnUrl);
                }
                catch (BusinessException ex)
                {
                    Core.Utils.Common.WriteLog(ex, "AccountController.ExternalLoginCallback:bussiness");
                    if (ex.ErrorCode == (int)Core.LoginStatus.NotVerifiedMobile)
                    {
                        //Chuyển trang yêu cầu xác thực số điện thoại
                        return RedirectToAction("Verified", "Account", null);
                    }
                    return RedirectToAction("ExternalLoginFailure");
                }
            }
            catch (Exception ex)
            {
                Core.Utils.Common.WriteLog(ex, "AccountController.ExternalLoginCallback");
                //WebLog.Log.Error("AccountController.ExternalLoginCallback", ex.Message);
                return RedirectToAction("ExternalLoginFailure");
            }
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        #endregion

        #region Đăng ký

        [AllowAnonymous]
        public ActionResult Register(string returnUrl = "")
        {
			if (User.Identity.IsAuthenticated == true)
			{
				int profileId = this.CurrentProfileId;
				var user = _uow.UserProfile.Get(profileId);
				if (user == null || user.ValidUserName(User.Identity.Name) == false)
				{
					this.SignOut();
				}
				else if (user.NeedVerifiedMobile() == true)
				{
					return RedirectToAction("Verified", "Account");
				}
				else
				{
					return RedirectToLocal(returnUrl);
				}
			}

			string state = DateTime.Now.GetHashCode().ToString("x");
            var openIdInfo = GetOpenIdInfo();
            Session["Auth_State"] = openIdInfo["STATE"];
            return View("Login", new RegisterModel()
            {
                OpenId= openIdInfo,
                ACK = new AccountKitAuthModel() { CSRF = this.AccountKitCSRF },
				Step = "register",
				ReturnUrl = returnUrl
			});
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Register(RegisterModel model)
        {
			string cookie = "";
			if (Request.UrlReferrer != null) cookie = "Refer= " + Request.UrlReferrer.ToString();
			if (Request.Cookies != null)
			{
				foreach (var k in Request.Cookies.AllKeys)
				{
					var c = Request.Cookies[k];
					if (c == null || string.IsNullOrEmpty(c.Value)) continue;
					cookie += $"; {k} = {c.Value}";
				}
			}
			// Kiểm tra nếu không còn sử dụng thì xóa
				WebLog.Log.Data(DateTime.Now.yyyyMMddHHmmss() + "\tUrl:" + Request.Url.AbsoluteUri + "\tData:" + model.ToJson() + "\rCookie: " + cookie, true, $"Register_{DateTime.Today.ToString("yyyyMM")}.txt");




			AjaxResponse rp = new AjaxResponse();
            rp.Message = string.Empty;
            //if (model.Password != model.ConfirmPassword)
            //{
            //    rp.Message = Message.ConfirmPassword_NotMatch;
            //}

            if (ModelState.IsValid == false)
            {
                return Json(rp, JsonRequestBehavior.AllowGet);
            }
			if (true)
			{
				rp.Message = Message.GeneralError;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}

			/*
			try
            {
                if (model.IsAnonymous() == true)
                {
                    model.Email = null;
                }

                // Add User
                _uow.UserProfile.AddUser(model);

                // Get User
                var user = _uow.UserProfile.GetUserByMobile(model.Mobile);

                _uow.UserProfile.UpdateVerifiedMobile(user.ProfileId, user.Mobile);

                // UserZone: Thêm khu vực đăng tin
                if (model.ZoneId > 0)
                {
                    _uow.UserZone.AddZoneForNewRegister(user.ProfileId, model.ZoneId);
                }

                // Get User: Login
                user = _uow.UserProfile.GetUserByMobile(model.Mobile);
                LoginHelper.LoginOwin(user, true);

                rp.Status = true;
                rp.Data = new
                {
                    data = model,
                    result = new
                    {
                        Title = user.IsMogiPro ? Message.Account_MogiThanks : Message.Account_RegiterSuccessFull,
                        Msg = user.IsMogiPro ? string.Format(Message.Account_RegiterMogiProSuccessFull, DateTime.Today.AddDays(30).ToString("dd/MM/yyyy")) : Message.Account_Welcome,
                        ReturnUrl = user.IsMogiPro ? Url.Action("MogiPro", "Account") : Url.Action("Index", "Home")
                    }
                };
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
                rp.Data = new { data = model, returnUrl = string.Empty };
            }
            catch (Exception ex)
            {
                MBN.Utils.WebLog.Log.Error("AccountController.Register", ex);
                rp.Message = Message.GeneralError;
                rp.Data = new { data = model, returnUrl = string.Empty };
            }

            return Json(rp, JsonRequestBehavior.AllowGet);
			*/
        }

		private AjaxResponse RegisterVerifiedUser(RegisterModel model)
		{
			AjaxResponse rp = new AjaxResponse() { Message = "" };

			UserProfile user = null;
			bool is_authenticated = User.Identity.IsAuthenticated;
			int profileId = is_authenticated == false ? 0 : this.CurrentProfileId;
			string step = "start";

			WebLog.Log.Data(DateTime.Now.yyyyMMddHHmmss() + $"\tProfileId: {profileId}" + model.ToJson(), true, $"RegisterVerifiedUser_{DateTime.Today.ToString("yyyyMM")}.txt");

			if (is_authenticated == true)
			{
				step = "Get by profileId: " + profileId;
				user = _uow.UserProfile.Get(profileId);
			}
			else
			{
				step = "Get By Mobile";
				user = _uow.UserProfile.GetByMobile(model.Mobile);
			}

			// Đăng ký mới
			if (user == null)
			{
				// Tạo mới phải có mật khấu
				if (true == string.IsNullOrEmpty(model.Password))
				{
					throw new BusinessException(string.Format(Validation.PaswordInvalid, Model.UserProfile_Password));
				}
				
				step = "AddUser";

				#region  AddUser
				try
				{
					model.LevelId = Const.LEVEL_ANONYMOUS; // default

					if (_uow.UserProfile.CheckMobileUsed(model.Mobile, 0) == false)
					{
						_uow.UserProfile.AddUser(model, null);
					}
					else if (string.IsNullOrEmpty(model.Password) == false)
					{
						_uow.UserProfile.ResetPassword(model.Mobile, model.Password);
					}
				}
				catch (BusinessException ex)
				{
					throw new BusinessException(ex.Message);
				}
				catch (Exception ex)
				{
					WebLog.Log.Error("AccountController.RegisterVerifiedUser: " + model.ToJson(), ex);
					throw new BusinessException(Message.GeneralError);
				}
				#endregion

				step = "GetInfo";
				// GetInfo
				user = _uow.UserProfile.GetByMobile(model.Mobile);
			}

			// Cập nhật Họ & Tên
			_uow.UserProfile.Mogi_UpdateFullName(user.ProfileId, model.FirstName, model.LastName);

			// Update: Xác thực số điện thoại
			if (user.NeedVerifiedMobile() == true)
			{
				step = "UpdateVerifiedMobile";
				user = _uow.UserProfile.UpdateVerifiedMobile(user.ProfileId, model.Mobile);
			}

			// UserZone: Thêm khu vực đăng tin
			if (model.ZoneId > 0)
			{
				step = "AddZoneForNewRegister";
				_uow.UserZone.AddZoneForNewRegister(user.ProfileId, model.ZoneId);
			}

			// Nếu user đăng kí là CTV Sinh viên => kết nối đăng tin MBN
			if (model.UserTypeId == (int)Core.UserType.Contractor)
			{
				step = "ConnectMBN";
				_uow.UserWebsite.ConnectMBN(user.Mobile);
			}

			// Remove cache
			_uow.UserProfile.RemoveCache(user.ProfileId);

			step = "Login";
			// Login, ReLogin
			user = _uow.UserProfile.Get(user.ProfileId);
			LoginHelper.LoginOwin(user, true);

			rp.Status = true;
			rp.Data = new
			{
				data = model,
				result = new
				{
					Title = user.IsMogiPro ? Message.Account_MogiThanks : Message.Account_RegiterSuccessFull,
					Msg = user.IsMogiPro ? string.Format(Message.Account_RegiterMogiProSuccessFull, Common.LimitTrial, user.LevelExpired.Value.ToShortDateString()) : Message.Account_Welcome,
					ReturnUrl = user.IsMogiPro ? Url.Action("MogiPro", "Account") : Url.Action("Index", "Home")
				}
			};

			return rp;
		}

        [AllowAnonymous]
        [HttpPost]
        public JsonResult ValidateMobile(string mobile)
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                _uow.UserProfile.ValidateMobile(mobile);
                rp.Status = true;

            }
            catch (BusinessException ex)
            {
                rp.Message = System.Text.RegularExpressions.Regex.Replace(ex.Message, "<.*?>", String.Empty);
            }
            catch (Exception)
            {
                rp.Message = Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        #endregion

  //      #region Nâng cấp MogiPro   
  //      [Authorize]
  //      public ActionResult MogiPro()
  //      {
		//	var user = _uow.UserProfile.Get(this.CurrentProfileId);

  //          // not found
  //          if (user == null)
  //          {
		//		return this.ReLogin();
  //          }

  //          // ReLogin
  //          if (user.IsMogiProAndApproved() == true && this.UserData.IsMogiPro == false)
  //          {
		//		return this.ReLogin();
		//	}

  //          // MogiPro
  //          if (user.IsMogiProAndApproved() == true)
  //          {
  //              return Redirect(WebUtils.AppSettings("MogiProUrl", string.Empty));
  //          }

		//	// Verify Mobile
		//	if (user.NeedVerifiedMobile() == true)
		//	{
		//		return RedirectToAction("Verified", "Account");
		//	}

		//	RegisterModel model = new RegisterModel(user);

		//	var cms = _uow.CMSCategory.GetByCodeUrl(Const.CMS_GROUP_WIDGET, "register-mogipro-agreement");
		//	if (cms != null)
		//	{
		//		model.Agreement = cms.Description;
		//	}
		//	model.ACK = new AccountKitAuthModel() { CSRF = this.AccountKitCSRF };

		//	return View(model);
  //      }

		//[Authorize]
		//[HttpPost]
		//public JsonResult RegisterMogiPro(ProfileViewModel model)
  //      {
  //          AjaxResponse rp = new AjaxResponse();
		//	if (model.ZoneId <= 0)
		//	{
		//		rp.Status = false;
		//		rp.Message = Message.Register_Invalid + $"({Model.UserProfile_ZoneId})";
		//		return Json(rp, JsonRequestBehavior.AllowGet);
		//	}

  //          string returnUrl = string.Empty;
  //          try
  //          {
  //              var user = _uow.UserProfile.Get(this.CurrentProfileId);

  //              // Không tìm thấy thông tin
  //              if (user == null)
  //              {
  //                  LoginHelper.LogOff();
  //                  rp.Status = false;
  //                  rp.Message = Message.UserProfile_NotFound;
  //                  return Json(rp, JsonRequestBehavior.AllowGet);
  //              }

  //              // Đã đăng ký MogiPro trước rồi
  //              if (user.IsMogiPro == true)
  //              {
  //                  rp.Status = true;
  //                  rp.Message = Message.Msg_Register_MogiPro_Exists;
  //                  // Tạo tài khoản MBPAY khi đăng ký mogipro
  //                  if (user.AllowCreatedMBPayAccount() == true)
  //                  {
  //                      _uow.UserProfile.CreateMBPayAccount(user.ProfileId);
  //                  }
		//			rp.Data = new
		//			{
		//				Title = Message.Account_MogiThanks,
		//				Msg = string.Format(Message.Account_RegiterMogiProSuccessFull, Common.LimitTrial, user.LevelExpired.Value.ToShortDateString()),
		//				ReturnUrl = user.IsMogiProAndApproved() ? WebUtils.AppSettings("MogiProUrl", string.Empty) : Url.Action("Index", "Home")
		//			};
		//			return Json(rp, JsonRequestBehavior.AllowGet);
  //              }

		//		// Cập nhật e-mail
		//		if (string.IsNullOrEmpty(model.Email) == false)
		//		{
		//			_uow.UserProfile.Mogi_UpdateEmail(user.ProfileId, model.Email);
		//		}

  //              // Nâng cấp MogiPro
  //              _uow.UserProfile.UpgradeMogiProTrial(user.ProfileId);

  //              // UserZone: Thêm khu vực đăng tin
  //              if (model.ZoneId > 0)
  //              {
  //                  _uow.UserZone.AddZoneForNewRegister(user.ProfileId, model.ZoneId);
  //              }

		//		// Gửi sms thông báo chờ duyệt
		//		string msg = string.Format(SMS.SMS_MogiPro_Register, user.Mobile);
		//		_uow.ConfirmMobile.SendAlertSMS(user.ProfileId, user.Mobile, msg);

		//		// Reload 
  //              user = _uow.UserProfile.Get(user.ProfileId);
				
		//		// Tạo tài khoản MBPAY khi đăng ký mogipro
		//		if (user.AllowCreatedMBPayAccount() == true)
  //              {
  //                  _uow.UserProfile.CreateMBPayAccount(user.ProfileId);
  //              }

  //              rp.Status = true;
  //              rp.Message = Message.UserProfile_Register_MogiPro;
  //              rp.Data = new
  //              {
  //                  Title = Message.Account_MogiThanks,
  //                  Msg = string.Format(Message.Account_RegiterMogiProSuccessFull, Common.LimitTrial, user.LevelExpired.Value.ToShortDateString()),
  //                  ReturnUrl = user.IsMogiProAndApproved() ? WebUtils.AppSettings("MogiProUrl", string.Empty) : Url.Action("Index", "Home")
  //              };

  //              // Đăng nhập lại
  //              LoginHelper.LoginOwin(user, true);
  //          }
  //          catch (BusinessException ex)
  //          {
  //              rp.Message = ex.Message;
  //          }
  //          catch (Exception ex)
  //          {
  //              WebLog.Log.Error("ProfileController.UpdateProfile", ex.Message);
  //              rp.Message = Message.GeneralError;
  //          }
  //          return Json(rp, JsonRequestBehavior.AllowGet);
  //      }
		//#endregion

		#region Xác thực số điện thoại (OTP)

		[HttpPost]
		public JsonResult SendSMS(OTPSendModel model)
		{
			AjaxResponse rp = new AjaxResponse() { Message = "" };
			if (ModelState.IsValid == false)
			{
				rp.Status = false;
				rp.Message = Message.Msg_InvalidInfo;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}

			if (string.IsNullOrEmpty(model.Mobile))
			{
				rp.Status = false;
				rp.Message = Message.Msg_InvalidMobile;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}

			#region Captcha Google
			if (this.GoogleValidCaptcha(model.GoogleCaptchaResponse) == false)
			{
				rp.Message = Message.GeneralError;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}
			#endregion

			string msg = "";
			string step = "start";
			string mobile = model.Mobile;
			string command = model.Command.ToLower();
			string[] commands = new string[] { "resetpassword", "register", "verified" };
			if (Array.IndexOf(commands, command) == -1)
			{
				rp.Message = Message.GeneralError;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}

			UserProfile profile = null;
			try
			{
				#region Reset Password
				if (command == "resetpassword")
				{
					// Đã đăng nhập
					if (User.Identity.IsAuthenticated == true)
					{
						rp.Data = new { ReturnUrl = "/" };
						rp.Status = false;
						return Json(rp, JsonRequestBehavior.AllowGet);
					}

					profile = _uow.UserProfile.GetByMobile(mobile);

					// Send SMS
					if (profile != null)
					{
						_uow.ConfirmMobile.CreatedVerifiedCode(profile.ProfileId, profile.Mobile);
					}

					rp.Message = string.Format(Message.Msg_Confirm_Mobile_Notice, mobile);
					rp.Status = true;

					return Json(rp, JsonRequestBehavior.AllowGet);
				}
				#endregion

				#region Register
				if (command == "register")
				{
					int profileId = 0;
					// Nếu chưa đăng nhập thì kiểm tra số điện thoại
					if (User.Identity.IsAuthenticated == false)
					{
						profile = _uow.UserProfile.GetUserByMobile(mobile);
						if (profile != null)
						{
							// Số điện thoại đã được sử dụng
							if (profile.IsVerifiedMobile == true)
							{
								rp.Status = false;
								rp.Message = Message.UserProfile_Mobile_IsExists;
								return Json(rp, JsonRequestBehavior.AllowGet);
							}

							profileId = profile.ProfileId;
						}
					}
					else
					{
						profileId = this.CurrentProfileId;
					}

					_uow.ConfirmMobile.CreatedVerifiedCode(profileId, mobile);

					rp.Data = new { ReturnUrl = Url.Action("Verified", "Account") };
					rp.Message = string.Format(Message.Msg_Confirm_Mobile_Notice, mobile);
					rp.Status = true;

					return Json(rp, JsonRequestBehavior.AllowGet);
				}
				#endregion

			}
			catch (BusinessException ex)
			{
				rp.Message = ex.Message;
				msg = $"AccountController.SendSMS - Step: {step}, data: {model.ToJson()}";
			}
			catch (Exception ex)
			{
				rp.Message = Message.GeneralError;
				msg = $"AccountController.SendSMS - Step: {step}, data: {model.ToJson()}";
				WebLog.Log.Error(msg, ex);
			}

			rp.Message = Message.GeneralError;
			return Json(rp, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// review: quy.vu - 15:39 2019-03-26
		/// </summary>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		[Authorize]
		public ActionResult Verified(string returnUrl)
		{
			var user = _uow.UserProfile.Get(this.CurrentProfileId);
			if (user == null || user.NeedVerifiedMobile() == false)
			{
				return RedirectToLocal(""); // Đã xác thực
			}

			var model = new OTPConfirmViewModel()
			{
				Mobile = user.Mobile,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Facebook = ACCOUNTKIT_USE,
				ACK = new AccountKitAuthModel() { CSRF = this.AccountKitCSRF },
				ReturnUrl = returnUrl
			};
			return View(model);
		}

        [HttpPost]
        public JsonResult Verified(OTPConfirmViewModel model)
        {
            AjaxResponse rp = new AjaxResponse() { Message = "" };
			if (ModelState.IsValid == false)
			{
				rp.Status = false;
				rp.Message = Message.Register_Invalid;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}

			if (string.IsNullOrEmpty(model.Code))
			{
				rp.Status = false;
				rp.Message = Message.ConfirmMobile_InvalidCode;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}


			#region Captcha Google
			if (this.GoogleValidCaptcha(model.GoogleCaptchaResponse) == false)
            {
                rp.Message = Message.GeneralError;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}
			#endregion

			string msg = "";
			string step = "start";

            try
            {
				bool is_authenticated = User.Identity.IsAuthenticated;
				int profileId = is_authenticated == false ? 0 : this.CurrentProfileId;

				step = "VerifiedMobileByOTP";
				var is_verified = _uow.UserProfile.VerifiedMobileByOTP(profileId, model.Code, model.Mobile);
				if (is_verified == false)
				{
					rp.Status = false;
					rp.Message = Message.ConfirmMobile_InvalidCode;
					return Json(rp, JsonRequestBehavior.AllowGet);
				}

				// Register
				rp = this.RegisterVerifiedUser(new RegisterModel(model));

                return Json(rp, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
				msg = $"AccountController.Verified - Step: {step}, data: {model.ToJson()}";
            }
            catch (Exception ex)
            {
				rp.Message = Message.GeneralError;
				msg = $"AccountController.Verified - Step: {step}, data: {model.ToJson()}";
				WebLog.Log.Error(msg, ex);
			}

			return Json(rp, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult Welcome()
        {
            return View();
        }
		#endregion

		//#region AccountKit
		//private IDictionary<string,string> AccountKitValid(string code, string csrfNonce) {
		//	string csrf = this.AccountKitCSRF;
			
		//	if (string.IsNullOrWhiteSpace(code) == true ||
		//		string.IsNullOrWhiteSpace(csrf) == true ||
		//		string.IsNullOrWhiteSpace(csrfNonce) == true ||
		//		csrfNonce != csrf)
		//	{
		//		throw new BusinessException(Message.ConfirmMobile_AccountKit_Failed);
		//	}

		//	BLL.Proxy.AccountKitService ack = new BLL.Proxy.AccountKitService();
		//	var accessToken = ack.QueryAccessToken(code);
		//	// accessToken
		//	if (string.IsNullOrWhiteSpace(accessToken) == true)
		//	{
		//		throw new BusinessException(Message.ConfirmMobile_AccountKit_Failed);
		//	}

		//	var userData = ack.GetUserData(accessToken);
			
		//	// error
		//	if (userData.ContainsKey("error") && !string.IsNullOrWhiteSpace(userData["error"]))
		//	{
		//		throw new BusinessException(Message.ConfirmMobile_AccountKit_Failed);
		//	}

		//	return userData;
		//} 


  //      [HttpPost]
  //      public ActionResult AccountKitLoginSuccess(ACKResponseModel model)
  //      {
		//	AjaxResponse rp = new AjaxResponse() { Status = false, Message = string.Empty };
		//	if (ModelState.IsValid == false)
		//	{
		//		rp.Status = false;
		//		rp.Message = Message.Register_Invalid;
		//		return Json(rp, JsonRequestBehavior.AllowGet);
		//	}
		//	string msg = "";
		//	string step = "start";
  //          try
  //          {
		//		#region Move to AccountKitValid
		//		/*
  //              string csrf = this.AccountKitCSRF;
		//		// check data
  //              if (string.IsNullOrWhiteSpace(model.Code) == true ||
  //                  string.IsNullOrWhiteSpace(csrf) == true ||
  //                  string.IsNullOrWhiteSpace(model.Csrf_Nonce) == true ||
  //                  model.Csrf_Nonce != csrf)
  //              {
		//			rp.Status = false;
		//			rp.Message = Message.ConfirmMobile_AccountKit_Failed;
		//			return Json(rp, JsonRequestBehavior.AllowGet);
		//		}

		//		step = "AccountKitService - AccessToken";
		//		BLL.Proxy.AccountKitService ack = new BLL.Proxy.AccountKitService();
		//		var accessToken = ack.QueryAccessToken(model.Code);
		//		// accessToken
		//		if (string.IsNullOrWhiteSpace(accessToken) == true)
		//		{
		//			rp.Status = false;
		//			rp.Message = Message.ConfirmMobile_AccountKit_Failed;
		//			return Json(rp, JsonRequestBehavior.AllowGet);
		//		}

		//		step = "AccountKitService - UserData";
		//		var userData = ack.GetUserData(accessToken);
		//		// error
		//		if (userData.ContainsKey("error") && !string.IsNullOrWhiteSpace(userData["error"]))
		//		{
		//			WebLog.Log.Error($"AccountController.AccountKitLoginSuccess: {model.Mobile}", userData["error"]);

		//			rp.Status = false;
		//			rp.Message = Message.ConfirmMobile_AccountKit_Failed;
		//			return Json(rp, JsonRequestBehavior.AllowGet);
		//		}
		//		*/
		//		#endregion

		//		step = "AccountKitService - Valid";
		//		var userData = AccountKitValid(model.Code, model.Csrf_Nonce);

		//		step = "AccountKitService - Check Phone";
		//		// Số điện thoại xác thực phải là số đăng ký
		//		if (userData["phone"] != model.Mobile)
		//		{
		//			rp.Status = false;
		//			rp.Message = Message.VerifyMobile_ACK_Wrong_Number;
		//			return Json(rp, JsonRequestBehavior.AllowGet);
		//		}


		//		rp.Status = true;
		//		rp = this.RegisterVerifiedUser(new RegisterModel(model));
		//	}
  //          catch (BusinessException ex)
  //          {
  //              rp.Message = ex.Message;
		//		msg = $"AccountController.AccountKitLoginSuccess - Step: {step}, data: {model.ToJson()}";
		//		WebLog.Log.Error(msg, ex);
		//	}
  //          catch (Exception ex)
  //          {
		//		rp.Message = Message.GeneralError;
		//		msg = $"AccountController.AccountKitLoginSuccess - Step: {step}, data: {model.ToJson()}";
		//		WebLog.Log.Error(msg, ex);
  //          }

  //          return Json(rp, JsonRequestBehavior.AllowGet);
  //      }

  //      [HttpPost]
  //      public ActionResult AccountKitLoginFailed(string mobile, string command = "")
  //      {
  //          AjaxResponse rp = new AjaxResponse();
  //          rp.Message = string.Empty;

		//	if (command == "resetpassword")
		//	{
		//		// Đã đăng nhập
		//		if (User.Identity.IsAuthenticated == true)
		//		{
		//			rp.Data = new { ReturnUrl = "/" };
		//			rp.Status = false;
		//			return Json(rp, JsonRequestBehavior.AllowGet);
		//		}

		//		var profile = _uow.UserProfile.GetByMobile(mobile);

		//		// SendSMS
		//		if (profile != null)
		//		{
		//			_uow.ConfirmMobile.CreatedVerifiedCode(profile.ProfileId, profile.Mobile);
		//		}

		//		rp.Message = string.Format(Message.Msg_Confirm_Mobile_Notice, mobile);
		//		rp.Status = true;

		//		return Json(rp, JsonRequestBehavior.AllowGet);
		//	}


		//	try
		//	{
		//		bool allow = _uow.UserProfile.AllowFacebookOTP(mobile, true, ACCOUNTKIT_MAX_FAIL);
		//		if (allow == false)
		//		{
		//			int profileId = 0;
		//			// Nếu chưa đăng nhập thì kiểm tra số điện thoại
		//			if (User.Identity.IsAuthenticated == false)
		//			{
		//				var profile = _uow.UserProfile.GetUserByMobile(mobile);
		//				if (profile != null)
		//				{
		//					// Số điện thoại đã được sử dụng
		//					if (profile.IsVerifiedMobile == true)
		//					{
		//						rp.Status = false;
		//						rp.Message = Message.UserProfile_Mobile_IsExists;
		//						return Json(rp, JsonRequestBehavior.AllowGet);
		//					}

		//					profileId = profile.ProfileId;
		//				}
		//			}
		//			else
		//			{
		//				profileId = this.CurrentProfileId;
		//			}

		//			_uow.ConfirmMobile.CreatedVerifiedCode(profileId, mobile);

		//			rp.Data = new { ReturnUrl = Url.Action("Verified", "Account") };
		//			rp.Message = string.Format(Message.Msg_Confirm_Mobile_Notice, mobile);
		//			rp.Status = true;
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		rp.Message = Message.GeneralError;
		//		WebLog.Log.Error($"AccountController.AccountKitLoginFailed: {mobile} ", ex.Message);
		//	}

  //          return Json(rp, JsonRequestBehavior.AllowGet);
  //      }
  //      #endregion

        #region Xác thực email
        [AllowAnonymous]
        public ActionResult VerifyEmail(string pid = "", string key = "", int cs = 0, string email = "", string sig = "")
        {
            string title = Message.Verify_Email_Title;
            string msg = Message.Verify_Email_Success;
            try
            {
                int profileId = int.Parse(pid);
                bool success = _uow.UserProfile.VerifiedEmail(profileId, key, cs, email, sig);
            }
            catch (Core.BusinessException ex)
            {
                msg = ex.Message;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("AccountController.VerifyEmail", ex.Message);
                msg = Message.GeneralError;
            }
            return View("VerifyEmail", new Notification
            {
                Title = title,
                Description = msg
            });
        }
        #endregion

        #region Quên mật khẩu
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
			string state = DateTime.Now.GetHashCode().ToString("x");
			var openIdInfo = GetOpenIdInfo();
			Session["Auth_State"] = openIdInfo["STATE"];

			var model = new ForgotPasswordViewModel()
			{
				OpenId = openIdInfo,
				ACK = new AccountKitAuthModel() { CSRF = this.AccountKitCSRF }
			};
			return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPasswordConfirmation_BAK(OTPConfirmViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

			string fileLog = string.Format("ForgotPassword_{0}.txt", DateTime.Today.ToString("yyyyMM"));
			try
            {
				// Captcha: Google
				if (this.GoogleValidCaptcha(string.Empty) == false)
				{
					WebLog.Log.Data(DateTime.Now.yyyyMMddHHmmss() + "\t ForgotPasswordConfirmation.GoogleValidCaptcha IP: " + this.ClientIP + ", Data: " + model.ToJson(), true, fileLog);
					ModelState.AddModelError("", Message.User_InvalidLogin);
					return View(model);
				}

				// Kiểm tra OTP hợp lệ
				var user = _uow.UserProfile.GetByMobile(model.Mobile);
                if (user == null)
                {
					ModelState.AddModelError("", Message.UserProfile_NotFound);
					return View(model);
                }

                int code = 0;
                if (int.TryParse(model.Code, out code) == false)
                {
                    ModelState.AddModelError("", Message.ConfirmMobile_InvalidCode);
                    return View(model);
                }

				// Xác thực OTP
				if (_uow.ConfirmMobile.VerifiedMobile(user.Mobile, code, user.ProfileId) == false)
				{
					throw new Core.BusinessException(Message.ConfirmMobile_InvalidCode);
				}

				// Nếu số điện thoại chưa được xác thực thì thực hiện xác thực
				if (user.IsVerifiedMobile == false)
				{
					_uow.UserProfile.UpdateVerifiedMobile(user.ProfileId, user.Mobile);
				}

				Session["OTP"] = new OTPSession() { ProfileId = user.ProfileId, Mobile = user.Mobile, Code = model.Code, Tick = DateTime.Now.Ticks };

				return RedirectToAction("ResetPassword");
            }
            catch (BusinessException ex)
            {
				WebLog.Log.Data(DateTime.Now.yyyyMMddHHmmss() + "\t ForgotPasswordConfirmation IP: " + this.ClientIP + ", Data: " + model.ToJson() + ", error: " + ex.Message, true, fileLog);
				ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
				WebLog.Log.Data(DateTime.Now.yyyyMMddHHmmss() + "\t ForgotPasswordConfirmation IP: " + this.ClientIP + ", Data: " + model.ToJson() + ", error: " + ex.Message, true, fileLog);
				ModelState.AddModelError("", Message.GeneralError);
            }

			return View(model);
		}
		#endregion

		#region Đổi mật khẩu mới
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(ForgotPasswordViewModel model)
        {
			AjaxResponse rp = new AjaxResponse() { Status = false, Message = "" };
			string msg = "";
			string step = "start";
			UserProfile profile = null;

			if (ModelState.IsValid == false || User.Identity.IsAuthenticated == true)
			{
				rp.Message = Message.Msg_InvalidInfo;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}


			try
			{
				if (string.IsNullOrEmpty(model.Code))
				{
					rp.Message = Message.ConfirmMobile_InvalidCode;
					return Json(rp, JsonRequestBehavior.AllowGet);
				}

				step = "Captcha";
				// Captcha Google
				if (this.GoogleValidCaptcha(model.GoogleCaptchaResponse) == false)
				{
					rp.Message = Message.GeneralError;
					return Json(rp, JsonRequestBehavior.AllowGet);
				}

				step = "GetByMobile";
				profile = _uow.UserProfile.GetByMobile(model.Mobile);
				if (profile == null)
				{
					rp.Message = Message.UserProfile_NotFound;
					return Json(rp, JsonRequestBehavior.AllowGet);
				}

				step = "VerifiedMobileByOTP";
				// OTP
				var is_verified = _uow.ConfirmMobile.VerifiedMobile(profile.Mobile, int.Parse(model.Code), profile.ProfileId);
				if (is_verified == false)
				{
					rp.Message = Message.ConfirmMobile_InvalidCode;
					return Json(rp, JsonRequestBehavior.AllowGet);
				}

				step = "ResetPassword";
				_uow.UserProfile.ResetPassword(model.Mobile, model.Password);

				step = "UpdateVerifiedMobile";
				// Nếu số điện thoại chưa được xác thực thì thực hiện xác thực
				if (profile.IsVerifiedMobile == false)
				{
					_uow.UserProfile.UpdateVerifiedMobile(profile.ProfileId, profile.Mobile);
				}

				rp.Status = true;
				rp.Message = Message.Msg_ResetPassword_Success;
				return Json(rp, JsonRequestBehavior.AllowGet);
			}
			catch (BusinessException ex)
			{
				rp.Message = ex.Message;
				msg = $"AccountController.ResetPassword - Step: {step}, data: {model.ToJson()}";
			}
			catch (Exception ex)
			{
				rp.Message = Message.GeneralError;
				msg = $"AccountController.ResetPassword - Step: {step}, data: {model.ToJson()}";
				WebLog.Log.Error(msg, ex);
			}
			return Json(rp, JsonRequestBehavior.AllowGet);
		}
        #endregion

        #region Thoát
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Session.Abandon();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                MBN.Utils.WebLog.Log.Error("AccountController.LogOff", ex);
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        private string AccountKitCSRF
        {
            get
            {
                string v = (string)Session["AccountKitCSRF"];
                if (string.IsNullOrEmpty(v) == true)
                {
                    v = Guid.NewGuid().ToString();
                    Session["AccountKitCSRF"] = v;
                }
                return v;
            }
            set { Session["AccountKitCSRF"] = value; }
        }
        Dictionary<string,string> GetOpenIdInfo()
        {
            var openId = new Dictionary<string, string>();
            openId.Add("STATE", DateTime.Now.GetHashCode().ToString("x"));
            openId.Add("ZALO_APP_ID", WebUtils.AppSettings("ZALO_APP_ID", ""));
            openId.Add("ZALO_APP_VERSION", WebUtils.AppSettings("ZALO_APP_VERSION", ""));
            openId.Add("ZALO_APP_CALLBACK", WebUtils.AppSettings("ZALO_APP_CALLBACK", ""));
            return openId;
        }
        #endregion

        #region Google Captcha
        private bool GoogleValidCaptcha(string captcha)
        {
            if (string.IsNullOrEmpty(captcha) == true)
            {
                captcha = Request["g-recaptcha-response"];
            }
            string secretKey = HappyRE.Core.Utils.Common.GOOGLE_CAPTCHA_SECRETKEY;
            string clientIP = this.ClientIP;
            string url = string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}&remoteip={2}", secretKey, captcha, clientIP);
            var client = new System.Net.WebClient();
            var resp = client.DownloadString(url);
            var data = resp.FromJson<Core.MapModels.FrontEnd.GoogleCaptchaResponse>();
            if (data == null || data.Success == false)
            {
                return false;
            }
            return true;
        }
        #endregion

  //      #region Đăng ký Cộng tác viên là Sinh viên
  //      public ActionResult AffilateRegister(string data, string sig)
  //      {
  //          //teo.nguyen@muaban.net|1122334444|1990-01-01|7
  //          if (User.Identity.IsAuthenticated) return RedirectToLocal("");
  //          AffiliateProfileModel model = new AffiliateProfileModel();

  //          //TEST_DATA
  //          //model.Mobile = "01234567890";
  //          //model.FirstName = "Nguyễn Văn";
  //          //model.LastName = "Tèo";
  //          //model.Email = "teo.nguyen@muaban.net";
  //          //model.IdCard = "1122334444";
  //          //model.Birthday = "1990-01-01";
  //          //model.ZoneId = "7";

  //          //var d = model.EnCode();
  //          //var s = model.GetSignature();

  //          ViewBag.IsErr = true;
  //          try
  //          {
  //              model.Decode(data);
  //              var isValidSig = model.VerifySignature(sig);

  //              if (isValidSig)
  //              {
  //                  _uow.UserProfile.ValidateProfile(model);

  //                  //check birthday
  //                  var birthday = DateTime.ParseExact(model.Birthday, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
  //                  int zoneId = int.Parse(model.ZoneId);
  //                  ViewBag.IsErr = false;

  //                  return View(new RegisterModel()
  //                  {
  //                      Mobile = model.Mobile,
  //                      FirstName = model.FirstName,
  //                      LastName = model.LastName,
  //                      Email = model.Email,
  //                      Birthday = birthday,
  //                      IDCard = model.IdCard,
  //                      ZoneId = zoneId,
  //                      Avatar = model.Avatar,
  //                      IDCard1 = model.IDCard1,
  //                      IDCard2 = model.IDCard2,
  //                      UserTypeId = 72,
  //                      LevelId = 2,
  //                      ACK = new AccountKitAuthModel() { CSRF = this.AccountKitCSRF }
  //                  });
  //              }
  //              else
  //              {
  //                  MBN.Utils.WebLog.Log.Error("AccountController.AffilateRegister", "Chữ ký không hợp lệ");
  //                  ViewBag.Message = "Chữ ký không hợp lệ";
  //              }
  //          }
  //          catch (BusinessException ex)
  //          {
  //              MBN.Utils.WebLog.Log.Error("AccountController.AffilateRegister", ex);
  //              ViewBag.Message = ex.Message;
  //          }
  //          catch (Exception ex)
  //          {
  //              MBN.Utils.WebLog.Log.Error("AccountController.AffilateRegister", ex);
  //              ViewBag.Message = Message.Affilate_Register_NotValid;
  //          }

  //          return View(new RegisterModel()
  //          {
  //              ACK = new AccountKitAuthModel() { CSRF = this.AccountKitCSRF }
  //          });
  //      }
		//#endregion

		//#region ZaloLogin
		//public ActionResult ZaloLoginCallback(string sid, string uid, string code, string state, string scope)
		//{
		//	if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(code))
		//	{
		//		return RedirectToAction("ExternalLoginFailure");
		//	}
		//	string step = "start";
		//	UserProfile user = null;
		//	try
		//	{
		//		user = _uow.UserProfile.GetUser(Core.Const.PROFILE_PROVIDER_ZALO, uid);

		//		// Kết nối với Zalo
		//		if (User.Identity.IsAuthenticated == true && state == "mogipro" && user == null)
		//		{
		//			try
		//			{
		//				step = "Mogi_UpdateSocialId";
		//				_uow.UserProfile.Mogi_UpdateSocialId(this.CurrentProfileId, Core.Const.PROFILE_PROVIDER_ZALO, uid);
		//			}
		//			catch (Exception ex)
		//			{
		//				WebLog.Log.Error("ZaloLoginCallback.Mogi_UpdateSocialId", ex);
		//			}

		//			return Redirect(WebUtils.AppSettings("MogiProUrl", ""));
		//		}

		//		step = "Validate state";
		//		// Validate state
		//		if (Session["Auth_State"] == null || state != Session["Auth_State"].ToString())
		//		{
		//			return RedirectToAction("ExternalLoginFailure");
		//		}

		//		step = "Zalo GetProfile";
		//		if (user == null)
		//		{
		//			var profile = new BLL.Utils.ZaloUtils().GetProfile(code);

		//			OpenIDUserModel objOpenId = new OpenIDUserModel()
		//			{
		//				FullName = profile.name,
		//				UserName = profile.id,
		//				Provider = Core.Const.PROFILE_PROVIDER_ZALO
		//			};

		//			step = "AddUserByOpenId";
		//			_uow.UserProfile.AddUserByOpenId(objOpenId);
		//		}


		//		step = "ValidUser";
		//		user = _uow.UserProfile.ValidUser(Const.PROFILE_PROVIDER_ZALO, uid, string.Empty);

		//		step = "LoginOwin";
		//		LoginHelper.LoginOwin(user, true);

		//		step = "TrackingUserLogin";
		//		// Lưu lịch sử đăng nhập
		//		_uow.UserProfile.TrackingUserLogin(user, this.ClientIP, Const.PROFILE_PROVIDER_ZALO);

		//		if (user.IsMogiPro == true && user.IsApproved)
		//		{
		//			string mogiProUrl = WebUtils.AppSettings("MogiProUrl", "");
		//			return Redirect(mogiProUrl);
		//		}

		//		return RedirectToLocal("/");
		//	}
		//	catch (BusinessException ex)
		//	{
		//		WebLog.Log.Error($"ZaloLoginCallback.ValidUser-Step: {step}", ex);

		//		if (ex.ErrorCode == (int)Core.LoginStatus.NotVerifiedMobile)
		//		{
		//			// Chuyển trang yêu cầu xác thực số điện thoại
		//			return RedirectToAction("Verified", "Account", null);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		WebLog.Log.Error("ZaloLoginCallback.ValidUser", ex);
		//	}

		//	return RedirectToAction("ExternalLoginFailure");
		//}
  //      #endregion

    //    #region Đăng kí Email Referal
    //    [HttpPost]
    //    public ActionResult CheckReferal(AccountReferalModel model)
    //    {
    //        AjaxResponse rp = new AjaxResponse();
    //        rp.Message = string.Empty;
    //        try
    //        {
				//if (model.LeadId > 0)
				//{
				//	var res = _uow.Lead.GetBy(model.LeadId, model.LeadKey.GetValueOrDefault(Guid.Empty));
				//	if (res != null)
				//	{
				//		var x = _uow.UserProfile.GetByMobile(res.Mobile);
				//		if (x == null)
				//		{
				//			rp.Status = true;
				//			rp.Data = new { FullName = res.Name, Mobile = res.Mobile };
				//		}
				//		else
				//		{
				//			rp.Status = true;
				//			rp.Data = new { FullName = res.Name, Mobile = "" };
				//		}
				//	}
				//}
    //        }
    //        catch (Exception ex)
    //        {
    //            rp.Message = ex.Message;
    //        }
    //        return Json(rp, JsonRequestBehavior.AllowGet);
    //    }
    //    #endregion
    }
}