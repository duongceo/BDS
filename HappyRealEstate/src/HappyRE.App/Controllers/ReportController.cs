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
using HappyRE.Core.Entities.QueryModel;
using Kendo.Mvc.UI;
using log4net;

namespace HappyRE.App.Controllers
{
    [Authorize]
    [CompressFilter]
    public class ReportController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("ReportController");
        public ReportController(IUow uow) : base(uow) { }
        // GET: Report
        public ActionResult Index()
        {
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var q = new ReportQuery()
            {
                FromDate = firstDayOfMonth,
                ToDate = lastDayOfMonth
            };
            return View(q);
        }

        public ActionResult Property()
        {
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var q = new ReportPropertyDailyQuery()
            {
                FromDate = firstDayOfMonth,
                ToDate = lastDayOfMonth,
                Unit="d"
            };
            return View(q);
        }

        public ActionResult SaleOrder()
        {
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var q = new ReportSaleOrderDailyQuery()
            {
                FromDate = firstDayOfMonth,
                ToDate = lastDayOfMonth,
                Unit = "d"
            };
            return View(q);
        }


        [HttpPost]
        public async Task<ActionResult> Summary(ReportQuery model)
        {
            try
            {
                var res = await _uow.Report.Summary(model);
                return Json(new { data=res});
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

        public async Task<ActionResult> TopProductAdd([DataSourceRequest] DataSourceRequest request, ReportQuery model)
        {
            try
            {
                var res = await _uow.Report.TopUserPropertyAdd(model);

                return Json(new DataSourceResult()
                {
                    Data = res,
                    Total = res.Count()
                });
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

        public async Task<ActionResult> TopViewMobile([DataSourceRequest] DataSourceRequest request, ReportQuery model)
        {
            try
            {
                var res = await _uow.Report.TopUserPropertyViewMobile(model);

                return Json(new DataSourceResult()
                {
                    Data = res,
                    Total = res.Count()
                });
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

        public async Task<ActionResult> TopHighPerformance([DataSourceRequest] DataSourceRequest request, ReportQuery model)
        {
            try
            {
                var res = await _uow.Report.TopUserHighPerformance(model);

                return Json(new DataSourceResult()
                {
                    Data = res,
                    Total = res.Count()
                });
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

        public async Task<ActionResult> TopLowPerformance([DataSourceRequest] DataSourceRequest request, ReportQuery model)
        {
            try
            {
                var res = await _uow.Report.TopUserLowPerformance(model);

                return Json(new DataSourceResult()
                {
                    Data = res,
                    Total = res.Count()
                });
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

        public async Task<ActionResult> PropertyDailyChart([DataSourceRequest] DataSourceRequest request, ReportPropertyDailyQuery model)
        {
            try
            {
                var res = await _uow.Report.PropertyDaily(model);
                return Json(res);
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

        public async Task<ActionResult> SaleOrderDailyChart([DataSourceRequest] DataSourceRequest request, ReportSaleOrderDailyQuery model)
        {
            try
            {
                var res = await _uow.Report.SaleOrderDaily(model);
                return Json(res);
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

        public async Task<ActionResult> PropertyDaily([DataSourceRequest] DataSourceRequest request, ReportPropertyDailyQuery model)
        {
            try
            {
                var res = await _uow.Report.PropertyDaily(model);
                return Json(new DataSourceResult()
                {
                    Data = res,
                    Total = res.Count()
                });
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

        public async Task<ActionResult> SaleOrderDaily([DataSourceRequest] DataSourceRequest request, ReportSaleOrderDailyQuery model)
        {
            try
            {
                var res = await _uow.Report.SaleOrderDaily(model);
                return Json(new DataSourceResult()
                {
                    Data = res,
                    Total = res.Count()
                });
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
    }
}