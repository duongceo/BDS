using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HappyRE.App.Infrastructures;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.Entities.Model;
using Kendo.Mvc.UI;

namespace HappyRE.App.Controllers
{
    [Authorize]
    public class UserLogController : BaseController
    {
        public UserLogController(IUow uow) : base(uow) { }

        public ActionResult Index()
        {
            return View(new Core.Entities.HistoryLogQuery());
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, Core.Entities.HistoryLogQuery data)
        {
            if (string.IsNullOrEmpty(data.CreatedBy) == false)
            {
                data.CreatedBy = data.CreatedBy.Substring(data.CreatedBy.IndexOf('(')+1, data.CreatedBy.IndexOf(')')- data.CreatedBy.IndexOf('(')-1);
            }
            data.Page = request.Page;
            data.Limit = request.PageSize;
            var res = await _uow.HistoryLog.SearchTrackingLog(data);

            return Json(new DataSourceResult()
            {
                Data = res.Item1,
                Total = res.Item2
            });
        }

        [CompressFilter]
        public async Task<ActionResult> SearchDetail([DataSourceRequest] DataSourceRequest request, Core.Entities.HistoryLogQuery data)
        {
            data.Page = request.Page;
            data.Limit = request.PageSize;
            var res = await _uow.HistoryLog.SearchTrackingLogUserDetail(data);

            return Json(new DataSourceResult()
            {
                Data = res.Item1,
                Total = res.Item2
            });
        }

        public ActionResult Detail(string id)
        {
            return View(new Core.Entities.HistoryLogQuery()
            {
                CreatedBy= id
            });
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(HistoryLog data)
        {
            try
            {
                data.CreatedBy = User.Identity.Name;
                var res = await _uow.HistoryLog.AddTrackingLog(data);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                Response.StatusCode = 400;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}