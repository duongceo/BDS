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
    public class WardController : BaseController
    {
        public WardController(IUow uow) : base(uow) { }

        public async Task<ActionResult> Index(int districtId, int? cityId)
        {
            if (cityId.HasValue==false)
            {
                var dist = await _uow.District.GetById(districtId);
                cityId = dist.CityId;
            }
            return View(new CityQuery() {CityId=cityId, DistrictId=districtId });
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, CityQuery model)
        {
            model.Page = request.Page;
            model.Limit = request.PageSize;
            var res = await _uow.Ward.Search(model);

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
                var res = await _uow.Ward.GetById(id);
                return PartialView(res);
            }
            else
            {
                return PartialView(new Ward());
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var ward = await _uow.Ward.GetById(id);
            if (ward != null)
            {
                await _uow.Ward.Delete(ward);
                var res = await _uow.Ward.Search(new Core.Entities.CityQuery() {CityId=ward.CityId, DistrictId= ward.DistrictId });
                return View("Index", res);
            }
            return null;
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(Ward data)
        {
            var res= await _uow.Ward.IU(data);
            return Json(res,JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]
        //[OutputCache(Duration = 10 * 60, VaryByParam = "cityId;districtId")]
        [AllowAnonymous]
        public async Task<JsonResult> _Gets(int? cityId, int? districtId)
        {
            var res = await _uow.Ward.Search(new Core.Entities.CityQuery() { Page = 1, Limit = 1000, CityId=cityId, DistrictId=districtId });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}