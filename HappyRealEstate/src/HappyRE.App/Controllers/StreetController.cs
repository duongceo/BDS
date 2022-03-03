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
    public class StreetController : BaseController
    {
        public StreetController(IUow uow) : base(uow) { }

        public async Task<ActionResult> Index(int? districtId, int? cityId)
        {
            if (districtId.HasValue && cityId.HasValue==false)
            {
                var dist = await _uow.District.GetById(districtId.Value);
                cityId = dist.CityId;
            }
            return View(new CityQuery() { DistrictId= districtId, CityId= cityId});
        }

        public async Task<ActionResult> Detail(int id)
        {
            if (id > 0)
            {
                var res = await _uow.Street.GetById(id);
                return PartialView(res);
            }
            else
            {
                return PartialView(new Street());
            }
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, CityQuery model)
        {
            model.Page = request.Page;
            model.Limit = request.PageSize;
            var res = await _uow.Street.Search(model);

            return Json(new DataSourceResult()
            {
                Data = res.Item1,
                Total = res.Item2
            });
        }
        public async Task<ActionResult> Delete(int id)
        {
            await _uow.District.Delete(new District() {Id=id });
            var res = await _uow.Street.Search(new Core.Entities.CityQuery());
            return View("Index",res);
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(Street data)
        {
            var res= await _uow.Street.IU(data);
            return Json(res,JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]
        //[OutputCache(Duration = 10 * 60, VaryByParam = "cityId;districtId")]
        [AllowAnonymous]
        public async Task<JsonResult> _Gets(int? cityId, int? districtId)
        {
            var res = await _uow.Street.Search(new Core.Entities.CityQuery() { Page = 1, Limit = 1000, CityId = cityId, DistrictId = districtId });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}