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
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;

namespace HappyRE.App.Controllers
{
    [Authorize]
    public class CustomerRegionTargetController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("CustomerRegionTargetController");
        public CustomerRegionTargetController(IUow uow) : base(uow) { }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, int customerId)
        {
            var res = await _uow.CustomerRegionTarget.GetByCustomer(customerId);

            return Json(new DataSourceResult()
            {
                Data = res,
            });
        }

        [CompressFilter]
        public async Task<ActionResult> LocationSearch([DataSourceRequest] DataSourceRequest request, CityQuery model)
        {
            model.Page = request.Page;
            model.Limit = request.PageSize;
            var res = await _uow.CustomerRegionTarget.LocationSearch(model);

            return Json(new DataSourceResult()
            {
                Data = res.Item1,
                Total = res.Item2
            });
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(CustomerRegionTarget data)
        {
            try
            {
                var res = await _uow.CustomerRegionTarget.IU(data);
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

    }
}