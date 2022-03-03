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
    public class HistoryLogController : BaseController
    {
        public HistoryLogController(IUow uow) : base(uow) { }

        public ActionResult Index()
        {
            return View(new HistoryLogQuery());
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, HistoryLogQuery model)
        {
            try
            {
                model.Page = request.Page;
                model.Limit = 20;//request.PageSize;
                var res = await _uow.HistoryLog.Search(model);

                return Json(new DataSourceResult()
                {
                    Data = res.Item1,
                    Total = res.Item2
                });
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

        [CompressFilter]
        public async Task<ActionResult> Detail(int id)
        {
            if (id > 0)
            {
                var res = await _uow.HistoryLog.GetById(id);
                return PartialView(res);
            }
            else
            {
                return PartialView(new HistoryLog());
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            await _uow.HistoryLog.Delete(new HistoryLog() {Id=id },true);
            return View("Index");
        }

        #region Json
        [CompressFilter]
        [HttpPost]
        public async Task<JsonResult> _IU(HistoryLog data)
        {
            var res= await _uow.HistoryLog.IU(data);
            return Json(res,JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpPost]
        [Authorize(Roles = Permission.COMMENT_DELETE)]
        public async Task<JsonResult> _Delete(int id)
        {
            var res=await _uow.HistoryLog.Delete(new HistoryLog() { Id = id }, true);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [CompressFilter]
        [HttpGet]
        public async Task<JsonResult> _Gets(HistoryLogQuery model)
        {
            var res = await _uow.HistoryLog.Search(model);
            return Json(res.Item1, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Track Change
        [CompressFilter]
        public async Task<ActionResult> SearchTrack([DataSourceRequest] DataSourceRequest request, TrackChangeQuery model)
        {
            try
            {
                model.Page = request.Page;
                model.Limit = 20;//request.PageSize;
                var res = await _uow.HistoryLog.SearchTrack(model);

                return Json(new DataSourceResult()
                {
                    Data = res.Item1,
                    Total = res.Item2
                });
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