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
    public class CustomerController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("CustomerController");
        public CustomerController(IUow uow) : base(uow) { }

        [Authorize(Roles = Permission.CUSTOMER_VIEW)]
        public ActionResult Index()
        {
            //var totalViewedMobileToday = await _uow.Customer.MobileViewedToday();
            //ViewBag.CanViewMobile = (totalViewedMobileToday <= 10 && User.IsInRole(Permission.CUSTOMER_INFO_VIEW)) || User.IsInRole(Permission.PROPERTY_CUSTOMER_INFO_HIDE);
            ViewBag.CanViewMobile = User.IsInRole(Permission.CUSTOMER_INFO_VIEW);
            return View(new CustomerQuery());
        }

        [CompressFilter]
        public ActionResult RegionModal()
        {
            return PartialView(new CityQuery());
        }

        [CompressFilter]
        [Authorize(Roles = Permission.CUSTOMER_VIEW)]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, CustomerQuery model)
        {
            try
            {
                model.Page = request.Page;
                model.Limit = request.PageSize;
                var res = await _uow.Customer.Search(model);
                this.Log("Customer", null, "Search", null);
                return Json(new DataSourceResult()
                {
                    Data = res.Item1,
                    Total = res.Item2
                });
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

        [Authorize(Roles = Permission.CUSTOMER_CREATE)]
        public ActionResult Create()
        {
            return View(new Customer());
        }

        [Authorize(Roles = Permission.CUSTOMER_VIEW)]
        public async Task<ActionResult> Detail(int id)
        {
            var res = await _uow.Customer.GetDetail(id);
            //var totalViewedMobileToday = await _uow.Customer.MobileViewedToday();
            //ViewBag.CanViewMobile = (totalViewedMobileToday <= 10 && User.IsInRole(Permission.CUSTOMER_INFO_VIEW)) || User.IsInRole(Permission.PROPERTY_CUSTOMER_INFO_HIDE) || res.IsViewedMobileToday==true;           
            ViewBag.CanViewMobile = User.IsInRole(Permission.CUSTOMER_INFO_VIEW);
            if (res != null) this.Log("Customer", id, "Detail", null);
            return View(res);
        }

        [Authorize(Roles = "CUSTOMER_MODIFY,CUSTOMER_CREATE")]
        public async Task<ActionResult> Edit(int id)
        {
            var res = await _uow.Customer.GetById(id);
            ViewBag.selectedStatus = await _uow.SysCode.GetBitMaskByBit(res.StatusId, "CustomerStatusType");
            ViewBag.selectedDemands = await _uow.SysCode.GetBitMaskByBit(res.DemandId, "CustomerDemandType");
            ViewBag.selectedTargets = await _uow.SysCode.GetBitMaskByBit(res.TargetId, "CustomerTargetType");
            ViewBag.selectedDirections = await _uow.SysCode.GetBitMaskByBit(res.DirectionId??0, "PropertyDirectionType");
            if (res != null)
            {
                var resImg = await _uow.ImageFile.GetImages(new ImageFileQuery() { TableName = "Customer", TableKeyId = id });
                res.Images = resImg.ToList();
            }
            return View("Create", res);
        }

        //public async Task<ActionResult> Delete(int id)
        //{
        //    await _uow.Customer.Delete(new Customer() {Id=id });
        //    var res = await _uow.Customer.Search(new Core.Entities.CustomerQuery());
        //    return View("Index",res);
        //}

        [Authorize(Roles = "CUSTOMER_MODIFY,CUSTOMER_CREATE")]
        public async Task<ActionResult> DeleteRegion(int id)
        {
            await _uow.CustomerRegionTarget.Delete(new CustomerRegionTarget() { Id = id }, false);
            //return View("Index");
            return Json(new DataSourceResult()
            {
            });
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "CUSTOMER_MODIFY,CUSTOMER_CREATE")]
        public async Task<JsonResult> _IU(Customer data)
        {
            try
            {
                data.RegionTarget = data.RegionTargets != null? Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerRegionTarget>>(data.RegionTargets):null;
                var res = await _uow.Customer.IU(data);
                this.Log("Customer", data.Id, "IU", null);
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
        public async Task<JsonResult> _CheckPhoneExists(string phone)
        {
            var res = await _uow.Customer.IsExistByPhone(phone);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]
        public async Task<JsonResult> _TotalViewedMobileToday()
        {
            var res = await _uow.Customer.MobileViewedToday();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpPost]
        [Authorize(Roles = Permission.PROPERTY_CUSTOMER_INFO_HIDE)]
        public async Task<JsonResult> _HideMobile(int id,bool isForced)
        {
            try
            {
                if (this.IsAdmin)
                {
                    var res = await _uow.Customer.ForceHideMobile(id, !isForced);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
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
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _ShowMobile(int id)
        {
            try
            {
                var customer = await _uow.Customer.GetById(id);
                if (User.IsInRole(Permission.CUSTOMER_INFO_VIEW) == false)
                {
                    Response.StatusCode = 400;
                    return Json("Bạn không có quyền xem SĐT", JsonRequestBehavior.AllowGet);
                }

                if (customer.IsForceHiddenPhone == true && User.IsInRole(Permission.CUSTOMER_INFO_ADMIN) == false)
                {
                    Response.StatusCode = 400;
                    return Json("Vui lòng liên hệ Admin", JsonRequestBehavior.AllowGet);
                }
                //var userId = this.GetUserId();
                var res = await _uow.Customer.ShowMobile(id,User.IsInRole(Permission.CUSTOMER_INFO_ADMIN));
                this.Log("Customer", id, "ShowMobile", null);
                return Json(new {data=res}, JsonRequestBehavior.AllowGet);
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
        public async Task<JsonResult> _Gets()
        {
            var res = await _uow.Customer.Search(new Core.Entities.CustomerQuery() { Page = 1, Limit = 100 });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        [Authorize(Roles = Permission.CUSTOMER_DELETE)]
        public async Task<JsonResult> _Delete(int id)
        {
            var res = await _uow.Customer.Delete(new Customer() { Id = id });
            this.Log("Customer", id, "Delete", null);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [Authorize(Roles = Permission.CUSTOMER_EXPORT)]
        public async Task<FileContentResult> Export(Core.Entities.CustomerQuery model)
        {
            var res = await _uow.Customer.Export(model);
            string[] columns = new string[] { "FullName", "Phone","ExtPhone", "Birtday", "ContractName", "SourceName", "Status","Region", "Demand", "Detail", "BudgetFrom", "BudgetTo", "CurrencyType", "MinArea", "MinWidth", "MinLength", "StreetWidth", "CalcMethod", "NumOfFloor", "NumOfRoom", "Note", "PostedBy" , "Created_Date", "Modified_Date" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(res.ToList(), "Khách hàng", true, columns);
            this.Log("Customer", null, "Export", null);
            return File(filecontent, ExcelExportHelper.ExcelContentType, $"Customer_{DateTime.Today.ToString("ddMMyyyy")}.xlsx");
        }
    }
}