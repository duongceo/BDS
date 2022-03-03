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
using HappyRE.Core.Entities.ViewModel;
using Kendo.Mvc.UI;

namespace HappyRE.App.Controllers
{
    public class CustomerInfoController: BaseController
    {
        public CustomerInfoController(IUow uow) : base(uow) { }

        public async Task<ActionResult> Index(string id)
        {
            var summary = await _uow.CustomerInfo.GetSummary(id);
            //var transactions = await _uow.CustomerInfo.GetTransactions(phone);
            var info = await _uow.CustomerInfo.GetByPhone(id);
            var model = new CustomerInfoDetail() { Summary = summary, Personal = info };
            ViewBag.Phone = id;
            if (info != null) this.Log("CustomerInfo", info.Id, "Detail", null);
            return View(model);
        }

        public async Task<ActionResult> Detail(int id)
        {
            var info = await _uow.CustomerInfo.GetById(id);
            if(info==null) return View("Index", new CustomerInfoDetail());
            var summary = await _uow.CustomerInfo.GetSummary(info.Phone);
            var model = new CustomerInfoDetail() { Summary = summary, Personal = info };
            ViewBag.Phone = id;
            //if (info != null) this.Log("CustomerInfo", info.Id, "Detail", null);
            return View("Index",model);
        }

        public async Task<ActionResult> Transaction(string phone)
        {
            var transactions = await _uow.CustomerInfo.GetTransactions(phone);

            return Json(new DataSourceResult()
            {
                Data = transactions,
                Total = transactions.Count()
            });
        }
    }
}