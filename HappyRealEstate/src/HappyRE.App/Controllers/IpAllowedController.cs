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
    [Authorize(Roles = Permission.ACCOUNT)]
    public class IpAllowedController : BaseController
    {
        public IpAllowedController(IUow uow) : base(uow) { }

        public ActionResult Index()
        {
            return View();
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request)
        {
            var res = await _uow.IpAlloweds.Search(new Core.Entities.BaseQuery() {Page= request.Page, Limit= request.PageSize});

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
                var res = await _uow.IpAlloweds.GetById(id);
                return PartialView(res);
            }
            else
            {
                return PartialView(new IpAlloweds());
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            await _uow.IpAlloweds.Delete(new IpAlloweds() {Id=id });
            return Json(new DataSourceResult());
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(IpAlloweds data)
        {
            var res= await _uow.IpAlloweds.IU(data);
            return Json(res,JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> _Gets()
        {
            var res = await _uow.IpAlloweds.Search(new Core.Entities.BaseQuery() { Page = 1, Limit = 1000 });
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}