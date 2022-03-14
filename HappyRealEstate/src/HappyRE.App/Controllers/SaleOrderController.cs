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
using Kendo.Mvc.UI;
using log4net;

namespace HappyRE.App.Controllers
{
    [Authorize]
    public class SaleOrderController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("SaleOrderController");
        public SaleOrderController(IUow uow) : base(uow) { }

        [Authorize(Roles = Permission.SALEORDER_VIEW)]
        public ActionResult Index()
        {
            return View(new SaleOrderQuery());
        }


        [CompressFilter]
        [Authorize(Roles = Permission.SALEORDER_VIEW)]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, SaleOrderQuery model)
        {
            try { 
            model.Page = request.Page;
            model.Limit = request.PageSize;
            model.UserName = User.Identity.Name;
            if (User.IsInRole(Permission.ADMIN)) model.UserName = "ADMIN";
            var res = await _uow.SaleOrder.Search(model);
            this.Log("SaleOrder", null, "Search", null);
            return Json(new DataSourceResult()
            {
                Data = res.Item1,
                Total = res.Item2
            });
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

        [Authorize(Roles = Permission.SALEORDER_CREATE)]
        public ActionResult Create()
        {
            return View(new SaleOrder());
        }

        public async Task<ActionResult> UpdateCustomer(int id)
        {
            var res = await _uow.SaleOrder.GetById(id);
            return PartialView(res);
        }

        [Authorize(Roles = "SALEORDER_CREATE,SALEORDER_MODIFY")]
        public async Task<ActionResult> Edit(int id)
        {
            var res = await _uow.SaleOrder.GetById(id);
            ViewBag.selectedOwnerTarget = await _uow.SysCode.GetBitMaskByBit(res.OwnerTargetId, "CustomerPotentialType");
            ViewBag.selectedCustomerTarget = await _uow.SysCode.GetBitMaskByBit(res.CustomerTargetId??0, "CustomerPotentialType");

            if (res != null)
            {
                var x= await _uow.ImageFile.GetImages(new ImageFileQuery() { TableName = "SaleOrder_Owner", TableKeyId = id });
                res.OwnerImages = x.ToList();
                var y=await _uow.ImageFile.GetImages(new ImageFileQuery() { TableName = "SaleOrder_Customer", TableKeyId = id });
                res.CustomerImages = y.ToList();
            }
            return View("Create", res);
        }

        [Authorize(Roles = Permission.SALEORDER_VIEW)]
        public async Task<ActionResult> Detail(int id)
        {
            var res = await _uow.SaleOrder.GetDetail(id);
            if (res != null) this.Log("SaleOrder", id, "Detail", null);
            return View(res);
        }


        #region Json
        [CompressFilter]
        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "SALEORDER_CREATE,SALEORDER_MODIFY")]
        public async Task<JsonResult> _IU(SaleOrder data)
        {
            try
            {
                var res = await _uow.SaleOrder.IU(data);
                this.Log("SaleOrder", data.Id, "IU", null);
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
        [ValidateInput(false)]
        public async Task<JsonResult> _UpdateCustomer(SaleOrder data)
        {
            try
            {
                var res = await _uow.SaleOrder.UpdateCustomer(data);
                this.Log("SaleOrder", data.Id, "IU", null);
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


        [HttpDelete]
        [Authorize(Roles = Permission.SALEORDER_DELETE)]
        public async Task<JsonResult> _Delete(int id)
        {
            var res = await _uow.SaleOrder.Delete(new SaleOrder() { Id = id });
            this.Log("SaleOrder", id, "Delete", null);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [Authorize(Roles = Permission.SALEORDER_EXPORT)]
        public async Task<FileContentResult> Export(Core.Entities.SaleOrderQuery model)
        {
            var res = await _uow.SaleOrder.Export(model);
            string[] columns = new string[] { "SaleOrderNumber", "PropertyNumber", "PostedBy", "SellBy", "OrderDate", "TotalAmount", "RewardPoint", "OwnerName", "OwnerPhone", "OwnerBirthday", "OwnerIDNumber", "OwnerAddress", "OwnerTarget", "CustomerName", "CustomerPhone", "CustomerBirthday", "CustomerIDNumber", "CustomerAddress", "CustomerTarget", "Created_Date", "Modified_Date" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(res.ToList(), "Giao dịch BĐS", true, columns);
            this.Log("SaleOrder", null, "Export", null);
            return File(filecontent, ExcelExportHelper.ExcelContentType, $"SaleOrder_{DateTime.Today.ToString("ddMMyyyy")}.xlsx");
        }
    }
}