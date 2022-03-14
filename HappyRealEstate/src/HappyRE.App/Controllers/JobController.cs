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
using log4net;

namespace HappyRE.App.Controllers
{
    public class JobController: BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("JobController");
        public JobController(IUow uow) : base(uow) { }

    // GET: Job
        public ActionResult AlertBirthday()
        {
            Hangfire.RecurringJob.AddOrUpdate("AlertBirthday", () => (AlertBirthdays()), Hangfire.Cron.Daily(1,0));
            return View();
        }


        [NonAction]
        public async Task AlertBirthdays()
        {
            try
            {
                var customers = await _uow.CustomerInfo.GetAlertBirtdayCustomers();
                var admins = await _uow.UserProfile.GetUsersInRole(Permission.ADMIN);
                NotificationHub objNotifHub = new NotificationHub();
                var data = new Notification()
                {
                    Title = "Báo sinh nhật",
                    SendTos = admins.ToList()
                };

                foreach (var c in customers)
                {
                    data.Details = $"3 ngày nữa là sinh nhật của khách hàng: {c.FullName} , sđt {c.Phone}";
                    data.DetailsURL = $"/Customerinfo?id={c.Phone}";
                    data.SentTo= String.Join(";", data.SendTos);
                    var r= await  _uow.Notification.IU(data);
                    if(r>0) objNotifHub.SendNotificationToList(data, false);
                }
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Warn(ex);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        [HttpGet]
        public async Task<JsonResult> AlertGoodProperty(int propertyId, string code)
        {
            try {
                var data = new Core.Entities.Model.Notification()
                {
                    Title = "Bất động sản tốt",
                    DetailsURL = $"/property/detail/{propertyId}",
                    Type = 5,
                    Code = propertyId.ToString()
                };
                var r = await _uow.Notification.IU(data);
                if (r > 0)  new NotificationHub().SendNotificationToList(data, true);
                return Json("", JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public async Task<JsonResult> AlertNewSO(int soId, string sellBy)
        {
            try
            {
                var sentTos = new List<string>();
                sentTos.Add(sellBy);
                var data = new Core.Entities.Model.Notification()
                {
                    Title = "Giao dịch BĐS mới của bạn",
                    DetailsURL = $"/saleOrder/detail/{soId}",
                    Type = 6,
                    SendTos= sentTos,
                    SentTo = sellBy,
                    Code = $"{soId.ToString()}_{sellBy}" 
                };
                var r = await _uow.Notification.IU(data);
                if (r > 0) new NotificationHub().SendNotification(data, sellBy);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Error(ex);
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


        [HttpGet]
        public async Task<JsonResult> AlertNewSOToAdmin(int soId)
        {
            try
            {
                var admins = await _uow.UserProfile.GetUsersInRole(Permission.ADMIN);
                var data = new Core.Entities.Model.Notification()
                {
                    Title = "Giao dịch BĐS mới cập nhật thông tin",
                    DetailsURL = $"/saleOrder/detail/{soId}",
                    Type = 7,
                    SendTos= admins.ToList(),
                    Code = soId.ToString()
                };
                data.SentTo = String.Join(";", data.SendTos);
                var r = await _uow.Notification.IU(data);
                if (r > 0)
                {                   
                    NotificationHub objNotifHub = new NotificationHub();
                    objNotifHub.SendNotificationToList(data, false);
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Error(ex);
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