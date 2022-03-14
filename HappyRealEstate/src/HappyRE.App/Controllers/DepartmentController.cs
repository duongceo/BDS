using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HappyRE.App.Infrastructures;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;

namespace HappyRE.App.Controllers
{
    [Authorize(Roles = Permission.ACCOUNT)]
    public class DepartmentController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("DepartmentController");
        public DepartmentController(IUow uow) : base(uow) { }

        public ActionResult Index()
        {
            return View(new BaseQuery());
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, BaseQuery model)
        {
            model.Page = request.Page;
            model.Limit = request.PageSize;
            var res = await _uow.Department.Search(model);

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
                var res = await _uow.Department.GetById(id);
                return PartialView(res);
            }
            else
            {
                return PartialView(new Department());
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _uow.Department.Delete(new Department() {Id=id });
                return Json(ModelState.ToDataSourceResult());
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
        [HttpPost]
        public async Task<JsonResult> _IU(Department data)
        {
            try
            {
                var res = await _uow.Department.IU(data);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Warn(ex);
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
        [HttpPost]
        public async Task<JsonResult> _UpdateManager(Department data)
        {
            try
            {
                var res = await _uow.Department.UpdateManager(data);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Warn(ex);
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
            var res = await _uow.Department.Search(new Core.Entities.BaseQuery() { Page = 1, Limit = 100 });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }

        #endregion
        public async Task<FileContentResult> Export(Core.Entities.BaseQuery model)
        {
            var res = await _uow.Department.Export(model);
            string[] columns = new string[] { "Code", "Name", "ManagerName", "Phone", "StartDate", "Note", "Created_Date", "Modified_Date" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(res.ToList(), "Phòng ban", true, columns);
            //this.Log("Department", null, "Export", null);
            return File(filecontent, ExcelExportHelper.ExcelContentType, $"Department_{DateTime.Today.ToString("ddMMyyyy")}.xlsx");
        }

        [AllowAnonymous]
        public ActionResult UserListPartial()
        {
            var depts = _uow.Department.GetAll();
            var users = _uow.UserProfile.GetAll();
            depts.Append(new Core.Entities.ViewModel.KeyValueModel()
            {
                Id = 0,
                Name = "Khác"
            });
            var model = new Models.UserListModel
            {
                Department = depts.ToList(),
                User = users.ToList()
            };

            return PartialView(model);
        }
    }
}