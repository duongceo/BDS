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

namespace HappyRE.App.Controllers
{
    [Authorize(Roles = Permission.SYS_ADMIN)]
    public class SysCodeController : BaseController
    {
        public SysCodeController(IUow uow) : base(uow) { }

        public ActionResult ContractType()
        { 
            return View("Index",new SysCodeTable() {Id="ContractType", Name="Loại hợp đồng"});
        }
        public ActionResult PropertyType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyType", Name = "Loại bất động sản" });
        }
        public ActionResult PropertyStatusType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyStatusType", Name = "Trạng thái BĐS" });
        }
        public ActionResult PropertyLegalType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyLegalType", Name = "Pháp lý" });
        }
        public ActionResult PropertyUtilityType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyUtilityType", Name = "Tiện ích" });
        }
        public ActionResult PropertySourceType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertySourceType", Name = "Loại nguồn BĐS" });
        }
        public ActionResult PropertyStrongType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyStrongType", Name = "Đặc điểm tốt" });
        }
        public ActionResult PropertyWeakType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyWeakType", Name = "Đặc điểm xấu" });
        }
        public ActionResult PropertyContructType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyContructType", Name = "Mức độ xây dựng" });
        }
        public ActionResult PropertyStructureType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyStructureType", Name = "Kết cấu xây dựng" });
        }
        public ActionResult PropertyPotentialType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyPotentialType", Name = "Tiềm năng BĐS" });
        }
        public ActionResult PropertyDirectionType()
        {
            return View("Index", new SysCodeTable() { Id = "PropertyDirectionType", Name = "Hướng nhà" });
        }

        public ActionResult CustomerStatusType()
        {
            return View("Index", new SysCodeTable() { Id = "CustomerStatusType", Name = "Trạng thái khách hàng" });
        }
        public ActionResult CustomerDemandType()
        {
            return View("Index", new SysCodeTable() { Id = "CustomerDemandType", Name = "Nhu cầu khách hàng" });
        }
        public ActionResult CustomerTargetType()
        {
            return View("Index", new SysCodeTable() { Id = "CustomerTargetType", Name = "Mục đích khách hàng" });
        }
        public ActionResult CustomerSourceType()
        {
            return View("Index", new SysCodeTable() { Id = "CustomerSourceType", Name = "Nguồn khách hàng" });
        }
        public ActionResult UserLevel()
        {
            return View("Index", new SysCodeTable() { Id = "UserLevel", Name = "Chức vụ" });
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request,string tableId)
        {
            var res = await _uow.SysCode.Search(new Core.Entities.SysCodeQuery() {Page= request.Page, Limit= request.PageSize, TableId= tableId});

            return Json(new DataSourceResult()
            {
                Data = res.Item1,
                Total = res.Item2
            });
        }

        public async Task<ActionResult> Detail(int id, string tableId)
        {
            if (id > 0)
            {
                var res = await _uow.SysCode.GetById(id);
                return PartialView(res);
            }
            else
            {
                return PartialView(new SysCode() { TableId= tableId});
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var sysCode = await _uow.SysCode.GetById(id);
            var q = new Core.Entities.SysCodeQuery();
            if (sysCode != null)
            {
                await _uow.SysCode.Delete(new SysCode() { Id = id });
                q.TableId = sysCode.TableId;
                var res = await _uow.SysCode.Search(new Core.Entities.SysCodeQuery());
                return View("Index", res);
            }
            return null;
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(SysCode data)
        {
            var res= await _uow.SysCode.IU(data);
            return Json(res,JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]       
        [AllowAnonymous]
        [OutputCache(Duration = 10 * 60, VaryByParam = "tableId")]
        public async Task<JsonResult> _Gets(string tableId)
        {
            var res = await _uow.SysCode.Search(new Core.Entities.SysCodeQuery() { Page = 1, Limit = 100, TableId=tableId });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}