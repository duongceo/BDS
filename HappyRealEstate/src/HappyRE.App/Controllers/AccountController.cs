using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using HappyRE.App.Models;
using HappyRE.App.Infrastructures;
using System.Net;
using HappyRE.Core.BLL.Repositories;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Security;
using HappyRE.Core.Utils;

namespace HappyRE.App.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private static readonly bool IP_RULE_ENABLE = bool.Parse(ConfigSettings.Get("IP_RULE_ENABLE", "false"));

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController(IUow uow, ApplicationUserManager userManager, ApplicationSignInManager signInManager) : base(uow) {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var isAdmin = await _uow.UserProfile.IsAdmin(model.UserName);

            if ((isAdmin == false || User.IsInRole(Core.Entities.Permission.IP_ACCESS)==false) && IP_RULE_ENABLE==true)
            {
                var ip = Request.UserHostAddress;
                IPAddress address = IPAddress.Parse(ip);
                var mac = NetworkHelper.GetMacAddress(address);

                string clientIp = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                   Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();


                var valid = _uow.IpAlloweds.IsValidIp(clientIp);
                if (!valid)
                {
                    ModelState.AddModelError("", "Bạn đang đăng nhập trên thiết bị lạ (" + clientIp + ")");
                    return View(model);
                }
            }

            var isLockedOut = await _uow.UserProfile.IsLockedOut(model.UserName);
            if (isLockedOut)
            {
                ModelState.AddModelError("", "Tài khoản này đã ngưng hoạt động");
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    //var user = await _uow.UserProfile.GetByUserName(model.UserName);
                    //var k = User.IsInRole("ADMIN");
                    //var u = UserManager.IsInRole("7a5d2d39-0d39-452c-9e6b-d7bc6010341e", "ADMIN");
                    //if (k == false)
                    //{
                    //    UserManager.AddToRole(user.UserId, "ADMIN");
                    //}
                    //k = User.IsInRole("ADMIN");
                    //LoginHelper.LoginOwin(user.UserId, model.UserName, user.Id, user.Mobile, user.Email);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "Tài khoản này đã ngưng hoạt động");
                    return View(model);
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Thông tin đăng nhập không hợp lệ");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [Authorize]
        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            var model = new ManageUserViewModel()
            {
            };
            return View("_ChangePasswordPartial", model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {                  
                    await _uow.UserProfile.IU(new Core.Entities.Model.UserProfile()
                    {
                        UserId = user.Id,
                        UserName = model.UserName,
                        Email = model.Email,
                        Mobile = model.Mobile,
                        FullName = model.FullName,
                        DepartmentId = model.DepartmentId,
                        LevelId = model.LevelId,
                        RoleGroupId = model.RoleGroupId,
                        Avatar = model.Avatar,
                        UserStatus = 0
                    });
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "User");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = true;
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("_ChangePasswordPartial");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                        //return RedirectToAction("_ChangePasswordPartial", new { Message ="Đổi mật khẩu thành công!" });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            return View("_ChangePasswordPartial", model);
        }

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Mật khẩu đã được thay đổi thành công."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "Đã có lỗi khi thực hiện, vui lòng liên hệ quản trị."
                : "";
            ViewBag.HasLocalPassword = true;
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
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
        #endregion
    }

    //    public class AccountController : BaseController
    //    {
    //        internal AuthRepository _repoUser = null;

    //        public AccountController(IUow uow, UserManager<ApplicationUser> userManager) : base(uow)
    //        {
    //            _repoUser = new AuthRepository();
    //            UserManager = userManager;
    //        }
    //        public AccountController()
    //            : this(null,new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
    //        {
    //        }

    //        //public AccountController(UserManager<ApplicationUser> userManager)
    //        //{
    //        //    UserManager = userManager;
    //        //}  

    //        public UserManager<ApplicationUser> UserManager { get; private set; }

    //        //
    //        // GET: /Account/Login
    //        [AllowAnonymous]
    //        public ActionResult Login(string returnUrl)
    //        {            
    //            ViewBag.ReturnUrl = returnUrl;
    //            return View();
    //        }

    //        //
    //        // POST: /Account/Login
    //        [HttpPost]
    //        [AllowAnonymous]
    //        [ValidateAntiForgeryToken]
    //        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    //        {
    //            if (ModelState.IsValid)
    //            {
    //                var isAdmin = await _uow.UserProfile.IsAdmin(model.UserName);
    //                if (isAdmin==false)
    //                {
    //                    var ip = Request.UserHostAddress;
    //                    IPAddress address = IPAddress.Parse(ip);
    //                    var mac = NetworkHelper.GetMacAddress(address);

    //                    var valid = _uow.IpAlloweds.IsValidIp(mac.ToString());
    //                    if (!valid)
    //                    {
    //                        ModelState.AddModelError("", "Ban đang đăng nhập trên thiết bị lạ (" + mac + ")");
    //                        return View(model);
    //                    }
    //                }

    //                var user = await UserManager.FindAsync(model.UserName, model.Password);
    //                if (user != null)
    //                {
    //                    var isLocked = await UserManager.GetLockoutEnabledAsync(user.Id);
    //                    if (isLocked == true)
    //                    {
    //                        ModelState.AddModelError("", "Tài khoản đăng nhập không đúng hoặc bị khóa!");
    //                        return View(model);
    //                    }
    //                    //Clear session
    //                    HttpContext.Session.Clear();
    //                    HttpContext.Session.Abandon();

    //                    await SignInAsync(user, model.RememberMe);
    //                    //await UserManager.AddToRoleAsync(user.Id,"ADMIN");
    //                    return RedirectToLocal(returnUrl);
    //                }
    //                else
    //                {
    //                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
    //                }
    //            }

    //            // If we got this far, something failed, redisplay form
    //            return View(model);
    //        }

    //        //
    //        // GET: /Account/Register     
    //        [AllowAnonymous] 
    //        public ActionResult Register()
    //        {
    //            return View();
    //        }

    //        //
    //        // POST: /Account/Register
    //        [HttpPost]        
    //        [ValidateAntiForgeryToken]
    //        public async Task<ActionResult> Register(RegisterViewModel model)
    //        {
    //            if (ModelState.IsValid)
    //            {
    //                var user = new ApplicationUser() { UserName = model.UserName };
    //                var result = await UserManager.CreateAsync(user, model.Password);
    //                if (result.Succeeded)
    //                {
    //                    var _user = await UserManager.FindByNameAsync(model.UserName);
    //                    await _uow.UserProfile.IU(new Core.Entities.Model.UserProfile()
    //                    {
    //                       UserId= _user.Id,
    //                       UserName= model.UserName,
    //                       Email= model.Email,
    //                       Mobile= model.Mobile,
    //                       FullName= model.FullName,
    //                       DepartmentId= model.DepartmentId,
    //                       LevelId= model.LevelId,
    //                       RoleGroupId= model.RoleGroupId,
    //                       Avatar= model.Avatar,
    //                       UserStatus= 0
    //                    });
    //                    //await SignInAsync(user, isPersistent: false);
    //                    return RedirectToAction("Index", "User");
    //                }
    //                else
    //                {
    //                    AddErrors(result);
    //                }
    //            }

    //            // If we got this far, something failed, redisplay form
    //            return View(model);
    //        }

    //        //
    //        // POST: /Account/Disassociate
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
    //        {
    //            ManageMessageId? message = null;
    //            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
    //            if (result.Succeeded)
    //            {
    //                message = ManageMessageId.RemoveLoginSuccess;
    //            }
    //            else
    //            {
    //                message = ManageMessageId.Error;
    //            }
    //            return RedirectToAction("Manage", new { Message = message });
    //        }

    //        //
    //        // GET: /Account/Manage
    //        public ActionResult Manage(ManageMessageId? message)
    //        {
    //            ViewBag.StatusMessage =
    //                message == ManageMessageId.ChangePasswordSuccess ? "Mật khẩu đã được thay đổi thành công."
    //                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
    //                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
    //                : message == ManageMessageId.Error ? "Đã có lỗi khi thực hiện, vui lòng liên hệ quản trị."
    //                : "";
    //            ViewBag.HasLocalPassword = HasPassword();
    //            ViewBag.ReturnUrl = Url.Action("Manage");
    //            return View();
    //        }

    //        //
    //        // POST: /Account/Manage
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public async Task<ActionResult> Manage(ManageUserViewModel model)
    //        {
    //            bool hasPassword = HasPassword();
    //            ViewBag.HasLocalPassword = hasPassword;
    //            ViewBag.ReturnUrl = Url.Action("Manage");
    //            if (hasPassword)
    //            {
    //                if (ModelState.IsValid)
    //                {
    //                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
    //                    if (result.Succeeded)
    //                    {
    //                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
    //                    }
    //                    else
    //                    {
    //                        AddErrors(result);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                // User does not have a password so remove any validation errors caused by a missing OldPassword field
    //                ModelState state = ModelState["OldPassword"];
    //                if (state != null)
    //                {
    //                    state.Errors.Clear();
    //                }

    //                if (ModelState.IsValid)
    //                {
    //                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
    //                    if (result.Succeeded)
    //                    {
    //                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
    //                    }
    //                    else
    //                    {
    //                        AddErrors(result);
    //                    }
    //                }
    //            }

    //            // If we got this far, something failed, redisplay form
    //            return View(model);
    //        }

    //        //
    //        // POST: /Account/ExternalLogin
    //        [HttpPost]
    ////        [AllowAnonymous]
    //        [ValidateAntiForgeryToken]
    //        public ActionResult ExternalLogin(string provider, string returnUrl)
    //        {
    //            // Request a redirect to the external login provider
    //            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
    //        }

    //        //
    //        // GET: /Account/ExternalLoginCallback
    //        [AllowAnonymous]
    //        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
    //        {
    //            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
    //            if (loginInfo == null)
    //            {
    //                return RedirectToAction("Login");
    //            }

    //            // Sign in the user with this external login provider if the user already has a login
    //            var user = await UserManager.FindAsync(loginInfo.Login);
    //            if (user != null)
    //            {
    //                await SignInAsync(user, isPersistent: false);
    //                return RedirectToLocal(returnUrl);
    //            }
    //            else
    //            {
    //                // If the user does not have an account, then prompt the user to create an account
    //                ViewBag.ReturnUrl = returnUrl;
    //                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
    //                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
    //            }
    //        }

    //        //
    //        // POST: /Account/LinkLogin
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public ActionResult LinkLogin(string provider)
    //        {
    //            // Request a redirect to the external login provider to link a login for the current user
    //            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
    //        }

    //        //
    //        // GET: /Account/LinkLoginCallback
    //        public async Task<ActionResult> LinkLoginCallback()
    //        {
    //            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
    //            if (loginInfo == null)
    //            {
    //                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
    //            }
    //            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
    //            if (result.Succeeded)
    //            {
    //                return RedirectToAction("Manage");
    //            }
    //            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
    //        }

    //        //
    //        // POST: /Account/ExternalLoginConfirmation
    //        [HttpPost]
    ////        [AllowAnonymous]
    //        [ValidateAntiForgeryToken]
    //        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
    //        {
    //            if (User.Identity.IsAuthenticated)
    //            {
    //                return RedirectToAction("Manage");
    //            }

    //            if (ModelState.IsValid)
    //            {
    //                // Get the information about the user from the external login provider
    //                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
    //                if (info == null)
    //                {
    //                    return View("ExternalLoginFailure");
    //                }
    //                var user = new ApplicationUser() { UserName = model.UserName };
    //                var result = await UserManager.CreateAsync(user);
    //                if (result.Succeeded)
    //                {
    //                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
    //                    if (result.Succeeded)
    //                    {
    //                        await SignInAsync(user, isPersistent: false);
    //                        return RedirectToLocal(returnUrl);
    //                    }
    //                }
    //                AddErrors(result);
    //            }

    //            ViewBag.ReturnUrl = returnUrl;
    //            return View(model);
    //        }

    //        //
    //        // POST: /Account/LogOff
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public ActionResult LogOff()
    //        {
    //            //Clear session
    //            HttpContext.Session.Clear();
    //            HttpContext.Session.Abandon();

    //            //MvcApplication.Token = Guid.NewGuid().ToString();
    //            AuthenticationManager.SignOut();
    //            return RedirectToAction("Index", "Home");
    //        }

    //        //
    //        // GET: /Account/ExternalLoginFailure
    //        [AllowAnonymous]
    //        public ActionResult ExternalLoginFailure()
    //        {
    //            return View();
    //        }

    //        [ChildActionOnly]
    //        public ActionResult RemoveAccountList()
    //        {
    //            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
    //            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
    //            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
    //        }

    //        protected override void Dispose(bool disposing)
    //        {
    //            if (disposing && UserManager != null)
    //            {
    //                UserManager.Dispose();
    //                UserManager = null;
    //            }
    //            base.Dispose(disposing);
    //        }

    //        #region Helpers
    //        // Used for XSRF protection when adding external logins
    //        private const string XsrfKey = "XsrfId";

    //        private IAuthenticationManager AuthenticationManager
    //        {
    //            get
    //            {
    //                return HttpContext.GetOwinContext().Authentication;
    //            }
    //        }

    //        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
    //        {
    //            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
    //            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
    //            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
    //        }

    //        private void AddErrors(IdentityResult result)
    //        {
    //            foreach (var error in result.Errors) 
    //            {
    //                ModelState.AddModelError("", error);
    //            }
    //        }

    //private bool HasPassword()
    //{
    //    var user = UserManager.FindById(User.Identity.GetUserId());
    //    if (user != null)
    //    {
    //        return user.PasswordHash != null;
    //    }
    //    return false;
    //}

    public enum ManageMessageId
    {
        ChangePasswordSuccess,
        SetPasswordSuccess,
        RemoveLoginSuccess,
        Error
    }

    //        private ActionResult RedirectToLocal(string returnUrl)
    //        {
    //            if (Url.IsLocalUrl(returnUrl))
    //            {
    //                return Redirect(returnUrl);
    //            }
    //            else
    //            {
    //                return RedirectToAction("Index", "Home");
    //            }
    //        }

    //        private class ChallengeResult : HttpUnauthorizedResult
    //        {
    //            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
    //            {
    //            }

    //            public ChallengeResult(string provider, string redirectUri, string userId)
    //            {
    //                LoginProvider = provider;
    //                RedirectUri = redirectUri;
    //                UserId = userId;
    //            }

    //            public string LoginProvider { get; set; }
    //            public string RedirectUri { get; set; }
    //            public string UserId { get; set; }

    //            public override void ExecuteResult(ControllerContext context)
    //            {
    //                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
    //                if (UserId != null)
    //                {
    //                    properties.Dictionary[XsrfKey] = UserId;
    //                }
    //                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
    //            }
    //        }
    //        #endregion
    //    }


}