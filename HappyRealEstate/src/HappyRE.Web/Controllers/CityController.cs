
using HappyRE.Core.MapModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HappyRE.Web.Controllers
{
    public class CityController : BaseController
    {
        public CityController(IUow uow) : base(uow) { }
		/// <summary>
		/// Lấy danh sách zone bởi cityID
		/// </summary>
		/// <param name="cityId"></param>
		/// <returns></returns>
		[OutputCache(Duration = CACHE_ONE_HOUR)]
		public JsonResult GetZoneByCityId(int cityId = 0)
        {
            ResponseAjax rp = new ResponseAjax();
            try
            {
                var obj = _uow.Zone.GetByCityId(cityId);
                if (obj.Count > 0)
                    rp.Data = obj.OrderByDescending(c => c.Sort).ThenBy(c => c.Name).Select(c => c.Small()).ToList();
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Message = ex.Message;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStreetById(int streetId = 0)
        {
            ResponseAjax rp = new ResponseAjax();
            try
            {
                rp.Data = _uow.Street.Get(streetId);
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Message = ex.Message;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
    }
}