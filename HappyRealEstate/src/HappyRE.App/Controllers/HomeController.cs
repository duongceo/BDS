using Newtonsoft.Json;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.BLL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HappyRE.App.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IUow uow) : base(uow) { }
        public ActionResult Index()
        {
            return Redirect("/notification");
            return View();
        }

        //[OutputCache(Duration = 3600, VaryByParam = "*")]
        public ActionResult FAQ(int from, int to)        
        {
            from = 5054;
            to = 5054;
            //await _uow.Property.TranferImages(from, to);
            return View();
        }

        public ActionResult ShowSizeBoard()
        {
            return View();
        }

        //[OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = VaryParam.USER)]
        public ActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> _TranferImage(int from, int to)
        {
            await _uow.Property.TranferImages(from, to);
            return Json(0, JsonRequestBehavior.AllowGet);
        }

    }
}
