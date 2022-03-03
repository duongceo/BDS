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

        [OutputCache(Duration = 3600, VaryByParam = "*")]
        public ActionResult FAQ()
        {
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

    }
}
