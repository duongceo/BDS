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

namespace HappyRE.App.Controllers
{
    public class JobController: BaseController
    {
        public JobController(IUow uow) : base(uow) { }

    // GET: Job
        public ActionResult AlertBirthday()
        {
            //var client = new HttpClient();
            //await client.GetAsync($"https://api.punnel.com/api/schedule/send-auto-responder?leadId={leadId}&templateId={templateId}");
            Hangfire.RecurringJob.AddOrUpdate("AlertBirthday", () => (AlertBirthdays()), Hangfire.Cron.Daily(2,46));
            return View();
        }


        [NonAction]
        public async Task AlertBirthdays()
        {
            var customers = await _uow.CustomerInfo.GetAlertBirtdayCustomers();
            var admins = await _uow.UserProfile.GetUsersInRole("ADMIN");
            NotificationHub objNotifHub = new NotificationHub();
            var data = new Notification()
            {
                Title = "Báo sinh nhật",
                SendTos = admins.ToList()
            };

            foreach (var c in customers)
            {
                data.Details = $"Hôm nay là sinh nhật của khách hàng: {c.FullName} , sđt {c.Phone}";
                data.DetailsURL = $"/Customerinfo?id={c.Phone}";
                data.SentTo= String.Join(";", data.SendTos);
                var r= await  _uow.Notification.IU(data);
                if(r>0) objNotifHub.SendNotificationToList(data, false);
            }
        }
    }
}