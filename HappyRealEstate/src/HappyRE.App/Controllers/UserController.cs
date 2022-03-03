using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HappyRE.App.Infrastructures;
using HappyRE.App.Models;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using Kendo.Mvc.UI;
using log4net;
using Microsoft.AspNet.Identity;

namespace HappyRE.App.Controllers
{
    [Authorize(Roles = Permission.ACCOUNT)]
    public class UserController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("UserController");

        internal AuthRepository _repoUser = null;
        public UserController(IUow uow, UserManager<ApplicationUser> userManager) : base(uow) 
        {
            UserManager = userManager;
            _repoUser = new AuthRepository();
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public ActionResult Index(int? departmentId, int? levelId, int? roleGroupId)
        {
            return View(new UserProfileQuery() { DepartmentId_Filter = departmentId, LevelId_Filter = levelId, RoleGroupId_Filter = roleGroupId});
        }

        [CompressFilter]
        [AllowAnonymous]
        public ActionResult ListModal()
        {
            return PartialView(new UserProfileQuery());
        }

        [CompressFilter]
        [AllowAnonymous]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, UserProfileQuery model)
        {
            model.Page = request.Page;
            model.Limit = request.PageSize;
            var res = await _uow.UserProfile.Search(model);

            return Json(new DataSourceResult()
            {
                Data = res.Item1,
                Total = res.Item2
            });
        }

        [CompressFilter]
        [AllowAnonymous]
        public async Task<ActionResult> GetUserOrDepartment(string text)
        {
            var res = await _uow.UserProfile.GetUserOrDepartment(text);
            return Json(res.ToList(), JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [AllowAnonymous]
        public async Task<ActionResult> GetListKeyValue(string text, string id)
        {
            var res = await _uow.UserProfile.GetListKeyValue(text,id);
            return Json(res.ToList(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Detail(int id)
        {
            if (id > 0)
            {
                var res = await _uow.UserProfile.GetById(id);
                return PartialView(res);
            }
            else
            {
                return PartialView(new UserProfile());
            }
        }

        [HttpDelete]
        public async Task<JsonResult> _Delete(int id)
        {
            try
            {
                var res = await _uow.UserProfile.Delete(id);
                if (res >= 0)
                {
                    var user = await _uow.UserProfile.GetById(id);
                    var l = await UserManager.GetLoginsAsync(user.UserId);
                    if (l.Count > 0)
                    {
                        IdentityResult result = await UserManager.RemoveLoginAsync(user.UserId, new UserLoginInfo(l[0].LoginProvider, l[0].ProviderKey));
                    }
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Error(ex);
                Response.StatusCode = 400;
                return Json("Hãy chuyển dữ liệu trước khi xóa nhân viên này vì có dữ liệu liên quan!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                Response.StatusCode = 400;
                return Json("Hãy chuyển dữ liệu trước khi xóa nhân viên này vì có dữ liệu liên quan!", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> _ResetPass(ResetPassViewModel model)
        {
            try
            {
                var user = await _uow.UserProfile.GetById(model.Id);
                await UserManager.RemovePasswordAsync(user.UserId);
                await UserManager.AddPasswordAsync(user.UserId,model.NewPassword);
                return Json(new { message = "Mật khẩu đã được khởi tạo lại!" }, JsonRequestBehavior.AllowGet);
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Error(ex);
                Response.StatusCode = 400;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                Response.StatusCode = 400;
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            
        }
        
        [HttpPost]
        public async Task<JsonResult> _TranferAccount(TranferAccountViewModel model)
        {
            try
            {
                await _uow.UserProfile.TranferAccount(model.FromUser, model.ToUser);
                return Json(new { message = "Đã chuyển giao thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Error(ex);
                Response.StatusCode = 400;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                Response.StatusCode = 400;
                return Json(null, JsonRequestBehavior.AllowGet);
            }         
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(UserProfile data)
        {
            try { 
                var res= await _uow.UserProfile.IU(data);
                if (res >= 0)
                {
                    var user = await _uow.UserProfile.GetById(data.Id);
                    await UserManager.SetLockoutEnabledAsync(user.UserId, data.UserStatus == 1);
                    if (data.UserStatus == 1)
                    {
                        var l = await UserManager.GetLoginsAsync(user.UserId);
                        if (l.Count > 0)
                        {
                            IdentityResult result = await UserManager.RemoveLoginAsync(user.UserId, new UserLoginInfo(l[0].LoginProvider, l[0].ProviderKey));
                        }
                    }
                }
                return Json(res,JsonRequestBehavior.AllowGet);
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Error(ex);
                Response.StatusCode = 400;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                Response.StatusCode = 400;
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [CompressFilter]
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> _Gets(string keyword)
        {
            var res = await _uow.UserProfile.Search(new Core.Entities.UserProfileQuery() { Page = 1, Limit = 100, Keyword= keyword });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public async Task<FileContentResult> Export(Core.Entities.UserProfileQuery model)
        {
            var res = await _uow.UserProfile.Export(model);
            string[] columns = new string[] { "FullName", "UserName", "Email", "Mobile","DepartmentName","LevelName","RoleGroupName","UserStatus","CreatedDate"}; 
            byte[] filecontent = ExcelExportHelper.ExportExcel(res.ToList(), "Người dùng", true, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, $"User_{DateTime.Today.ToString("ddMMyyyy")}.xlsx");
        }

        #region Helper Partial View
        [AllowAnonymous]
        public ActionResult UserListPartial()
        {
            var depts = _uow.Department.GetAll();
            var users = _uow.UserProfile.GetAll();
            depts.Append(new Core.Entities.ViewModel.KeyValueModel()
            {
                Id=0,
                Name="Khác"
            });
            var model = new Models.UserListModel
            {
                Department = depts.ToList(),
                User = users.ToList()
            };

            return PartialView(model);
        }
        #endregion
    }
}