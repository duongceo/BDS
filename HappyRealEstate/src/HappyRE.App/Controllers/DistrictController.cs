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
    [Authorize(Roles = Permission.SYS_ADMIN)]
    public class DistrictController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("DistrictController");
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
            try { 
                await _uow.District.Delete(new District() { Id = id });
                return Json(new DataSourceResult());
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
        public async Task<JsonResult> _IU(District data)
        {
            try { 
                var res= await _uow.District.IU(data);
                return Json(res,JsonRequestBehavior.AllowGet);
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