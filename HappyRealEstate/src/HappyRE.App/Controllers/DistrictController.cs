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
    public class DistrictController : BaseController
    {
        public DistrictController(IUow uow) : base(uow) { }

        public ActionResult Index(int cityId)
        {
            return View(new CityQuery() { CityId= cityId});
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, CityQuery model)
        {
            model.Page = request.Page;
            model.Limit = request.PageSize;
            var res = await _uow.District.Search(model);

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
                var res = await _uow.District.GetById(id);
                return PartialView(res);
            }
            else
            {
                return PartialView(new District());
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var dist = await _uow.District.GetById(id);
            if (dist != null)
            {
                await _uow.District.Delete(dist);
                var res = await _uow.District.Search(new Core.Entities.CityQuery() { CityId=dist.CityId});
                return View("Index", res);
            }
            return null;
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(District data)
        {
            var res= await _uow.District.IU(data);
            return Json(res,JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]
        //[OutputCache(Duration = 10 * 60, VaryByParam = "cityId")]
        [AllowAnonymous]
        public async Task<JsonResult> _Gets(int? cityId)
        {
            var res = await _uow.District.Search(new Core.Entities.CityQuery() { Page = 1, Limit = 1000, CityId=cityId });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Helper Partial View
        [AllowAnonymous]
        public ActionResult DistrictPartial()
        {
            var cities = _uow.City.GetAll();
            var districts = _uow.District.GetAll();

            var model = new Models.DistrictModel
            {
                City = cities.ToList(),
                District = districts.ToList()
            };

            return PartialView(model);
        }
        #endregion
    }
}