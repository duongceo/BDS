using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HappyRE.App.Infrastructures;
using HappyRE.Core.BLL;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using Kendo.Mvc.UI;
using log4net;

namespace HappyRE.App.Controllers
{
    [Authorize(Roles = Permission.ACCOUNT)]
    public class RoleGroupController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("RoleGroupController");
        public RoleGroupController(IUow uow) : base(uow) { }

        public ActionResult Index()
        {
            return View();
        }

        [CompressFilter]      
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request)
        {
            var res = await _uow.RoleGroup.Search(new Core.Entities.BaseQuery() {Page= request.Page, Limit= request.PageSize});

            return Json(new DataSourceResult()
            {
                Data = res.Item1,
                Total = res.Item2
            });
        }

        public async Task<ActionResult> Detail(int id)
        {
            if (id > 0)
            {
                var res = await _uow.RoleGroup.GetById(id);
                var r = await _uow.Role.GetRolesByParent(res.Roles);
                ViewBag.selectedRoles = r.Select(x => x.Id).ToList();
                return PartialView(res);
            }
            else
            {
                ViewBag.selectedRoles = new List<int>();
                return PartialView(new RoleGroup());
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try { 
                await _uow.RoleGroup.Delete(new RoleGroup() {Id=id },false);
                return Json(new DataSourceResult());
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Warn(ex);
                var r = new DataSourceResult()
                {
                    Errors = ex.Message
                };
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                var r = new DataSourceResult()
                {
                    Errors = "Không thể xóa thông tin này vì đang sử dụng!"
                };
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        #region Json
        [CompressFilter]
        [HttpGet]
        public async Task<JsonResult> _GetRoles()
        {
            var res = await _uow.Role.Search(new Core.Entities.BaseQuery());
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(RoleGroup data)
        {
            try
            {
                var res = await _uow.RoleGroup.IU(data);
                return Json(res, JsonRequestBehavior.AllowGet);
            }catch(BusinessException ex)
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
        public async Task<JsonResult> _Gets()
        {
            var res = await _uow.RoleGroup.Search(new Core.Entities.BaseQuery() { Page = 1, Limit = 100 });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}