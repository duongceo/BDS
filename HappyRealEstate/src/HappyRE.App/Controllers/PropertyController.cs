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
using HappyRE.Core.Entities.ViewModel;
using Kendo.Mvc.UI;
using log4net;

namespace HappyRE.App.Controllers
{
    [Authorize]
    public class PropertyController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("PropertyController");
        public PropertyController(IUow uow) : base(uow) { }

        [Authorize(Roles = Permission.PROPERTY_VIEW)]
        public ActionResult Index()
        {
            //var totalViewedMobileToday = await _uow.Customer.MobileViewedToday();
            ViewBag.CanViewMobile = User.IsInRole(Permission.CUSTOMER_INFO_VIEW);// (totalViewedMobileToday <= 10 && User.IsInRole(Permission.CUSTOMER_INFO_VIEW)) || User.IsInRole(Permission.PROPERTY_CUSTOMER_INFO_HIDE);
            return View(new PropertyQuery());
        }

        [CompressFilter]
        public ActionResult ListModal()
        {
            return PartialView(new PropertyQuery());
        }

        [CompressFilter]
        public async Task<ActionResult> GetListKeyValue(string search, string id)
        {
            var res = await _uow.Property.SearchForSelect(search, id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [Authorize(Roles = Permission.PROPERTY_VIEW)]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, PropertyQuery model)
        {
            try { 
                model.Page = request.Page;
                model.Limit = request.PageSize;
                var res = await _uow.Property.Search(model);

                this.Log("Property", null, "Search",null);

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

        [Authorize(Roles = Permission.PROPERTY_VIEW)]
        public async Task<ActionResult> Detail(int id)
        {
            var res = await _uow.Property.GetDetail(id);
            if(res!=null) this.Log("Property", id, "Detail", null);
            //var totalViewedMobileToday = await _uow.Customer.MobileViewedToday();
            //ViewBag.CanViewMobile = (totalViewedMobileToday <= 10 && User.IsInRole(Permission.CUSTOMER_INFO_VIEW)) || User.IsInRole(Permission.PROPERTY_CUSTOMER_INFO_HIDE) || res.IsViewedMobileToday == true;
            ViewBag.CanViewMobile = User.IsInRole(Permission.CUSTOMER_INFO_VIEW);
            return View(res);
        }

        [Authorize(Roles = Permission.PROPERTY_CREATE)]
        public ActionResult Create()
        {
            return View(new Property() { PostedBy= this.UserName});
        }

        [Authorize(Roles = "PROPERTY_CREATE,PROPERTY_MODIFY")]
        public async Task<ActionResult> Edit(int id)
        {
            var res = await _uow.Property.GetById(id);
            if (string.IsNullOrEmpty(res.PostedBy)) res.PostedBy = User.Identity.Name;
            ViewBag.selectedStatus = await _uow.SysCode.GetBitMaskByBit(res.StatusId, "PropertyStatusType");
            ViewBag.selectedTypes = await _uow.SysCode.GetBitMaskByBit(res.TypeId, "PropertyType");
            ViewBag.selectedStrongs = await _uow.SysCode.GetBitMaskByBit(res.StrongId, "PropertyStrongType");
            ViewBag.selectedWeaks = await _uow.SysCode.GetBitMaskByBit(res.WeakId, "PropertyWeakType");
            ViewBag.selectedContructs = await _uow.SysCode.GetBitMaskByBit(res.ContructId, "PropertyContructType");
            ViewBag.selectedStructures = await _uow.SysCode.GetBitMaskByBit(res.StructureId, "PropertyStructureType");
            ViewBag.selectedPotentials = await _uow.SysCode.GetBitMaskByBit(res.PotentialId, "PropertyPotentialType");
            ViewBag.selectedUtilities = await _uow.SysCode.GetBitMaskByBit(res.UtilityId, "PropertyUtilityType");
            if (res != null)
            {
                var resImg = await _uow.ImageFile.GetImages(new ImageFileQuery() { TableName="Property", TableKeyId=id});
                res.PropertyImages = resImg.ToList();
            }
            return View("Create", res);
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "PROPERTY_CREATE,PROPERTY_MODIFY")]
        public async Task<JsonResult> _IU(Property data)
        {
            try
            {
                var res = await _uow.Property.IU(data);
                this.Log("Property", data.Id, "IU", null);
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
        [Authorize(Roles = Permission.PROPERTY_VIEW)]
        public async Task<JsonResult> _AddImages(Property data)
        {
            try
            {
                foreach (var item in data.PropertyImages) {
                    await _uow.ImageFile.IU(new ImageFile()
                    {
                        TableName = "Property",
                        TableKeyId = data.Id,
                        Src = item,
                        IsMore = true,
                        GroupCode = DateTime.Now.GetHashCode().ToString("x")
                    });
                }
                return Json(1, JsonRequestBehavior.AllowGet);
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
        public async Task<JsonResult> _TotalViewedMobileToday()
        {
            var res = await _uow.Property.MobileViewedToday();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpPost]
        [Authorize(Roles = Permission.PROPERTY_CUSTOMER_INFO_HIDE)]
        public async Task<JsonResult> _HideMobile(int id,bool isForced)
        {
            try
            {
                var res = await _uow.Property.ForceHideMobile(id, !isForced);
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
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _ShowMobile(int id)
        {
            try
            {
                var Property = await _uow.Property.GetById(id);
                if (User.IsInRole(Permission.CUSTOMER_INFO_VIEW)==false)
                {
                    Response.StatusCode = 400;
                    return Json("Bạn không có quyền xem SĐT", JsonRequestBehavior.AllowGet);
                }
                if (Property.IsForceHiddenPhone == true && User.IsInRole(Permission.PROPERTY_CUSTOMER_INFO_HIDE)==false)
                {
                    Response.StatusCode = 400;
                    return Json("Vui lòng liên hệ Admin", JsonRequestBehavior.AllowGet);
                }
                var res = await _uow.Property.ShowMobile(id, User.IsInRole(Permission.PROPERTY_CUSTOMER_INFO_HIDE));
                this.Log("Property", id, "ShowMobileProperty", null);
                return Json(new {data= res}, JsonRequestBehavior.AllowGet);
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
        [Authorize(Roles = Permission.PROPERTY_HOT)]
        public async Task<JsonResult> _ChangeHot(int id, bool isHot)
        {
            try
            {
                if (this.IsAdmin)
                {
                    var res = await _uow.Property.ChangeHot(id, isHot);
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
        [HttpGet]
        public async Task<JsonResult> _Gets()
        {
            var res = await _uow.Property.Search(new Core.Entities.PropertyQuery() { Page = 1, Limit = 100 });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = Permission.CUSTOMER_INFO_VIEW)]
        [CompressFilter]
        [HttpGet]
        public async Task<JsonResult> _PhoneNumber(int id)
        {
            var res = await _uow.Property.GetPhoneNumber(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]
        public async Task<JsonResult> _IsExistsCode(int id, string code)
        {
            var res = await _uow.Property.IsExistsCode(new Property() {Id=id, Code=code });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]
        public async Task<JsonResult> _IsExistsAddress(int id, string code)
        {
            var res = await _uow.Property.IsExistsAddress(new Property() { Id = id, Code = code });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        [Authorize(Roles = Permission.PROPERTY_DELETE)]
        public async Task<JsonResult> _Delete(int id)
        {
            try { 
                var res = await _uow.Property.Delete(new Property() { Id = id });
                this.Log("Property", id, "Delete", null);
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
        #endregion

        [Authorize(Roles = Permission.PROPERTY_EXPORT)]
        public async Task<FileContentResult> Export(Core.Entities.PropertyQuery model)
        {
            var res = await _uow.Property.Export(model);
            string[] columns = new string[] { "PropertyNumber", "Code", "Commission", "Location", "Type", "Detail", "Status", "Direction", "Office", "RegionCode", "MapCode", "OwnerName", "OwnerPhone", "OwnerPhoneExt", "OwnerNote", "Price", "PriceMatched", "CurrencyType", "CalcMethod", "Width", "Length", "Area", "AreaForBuild", "NumOfBedroom", "NumOfToilet", "NumOfFloor", "StreetWidth", "Source", "Strong", "Weak", "Utility", "Contruct","Structure", "IsHot", "IsVerified", "IsChecked", "Posted", "Created_Date", "Modified_Date" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(res.ToList(), "Bất động sản", true, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, $"BDS_{DateTime.Today.ToString("ddMMyyyy")}.xlsx");
        }

        #region Helper Partial View
        [Authorize(Roles = Permission.PROPERTY_VIEW)]
        public ActionResult PropertyListPartial()
        {
            var pros = _uow.Property.GetAll();
            var model = new Models.PropertyListModel
            {
                Property = pros.ToList()
            };

            return PartialView(model);
        }
        #endregion
    }
}