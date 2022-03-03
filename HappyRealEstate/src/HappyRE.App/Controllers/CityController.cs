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
    [Authorize]
    public class CityController : BaseController
    {
        public CityController(IUow uow) : base(uow) { }

        [Authorize(Roles = Permission.SYS_ADMIN)]
        public ActionResult Index()
        {
            var u = User.IsInRole("ADMIN");
            return View(new BaseQuery());
        }

        [CompressFilter]
        [Authorize(Roles = Permission.SYS_ADMIN)]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, CityQuery model)
        {
            model.Page = request.Page;
            model.Limit = request.PageSize;
            var res = await _uow.City.Search(model);

            return Json(new DataSourceResult()
            {
                Data = res.Item1,
                Total = res.Item2
            });
        }

        [Authorize(Roles = Permission.SYS_ADMIN)]
        public async Task<ActionResult> Detail(int id)
        {
            if (id > 0)
            {
                var res = await _uow.City.GetById(id);
                return PartialView(res);
            }
            else
            {
                return PartialView(new City());
            }
        }

        [Authorize(Roles = Permission.SYS_ADMIN)]
        public async Task<ActionResult> Delete(int id)
        {
            await _uow.City.Delete(new City() {Id=id });
            var res = await _uow.City.Search(new Core.Entities.CityQuery());
            return View("Index",res);
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        [Authorize(Roles = Permission.SYS_ADMIN)]
        public async Task<JsonResult> _IU(City data)
        {
            var res= await _uow.City.IU(data);
            return Json(res,JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]
        //[OutputCache(Duration = 60 * 60)]
        public async Task<JsonResult> _Gets()
        {
            var res = await _uow.City.Search(new Core.Entities.CityQuery() { Page = 1, Limit = 1000 });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}