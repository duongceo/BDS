using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MBN.Utils;
using MBN.Utils.Extension;

using HappyRE.Core.Entities;
using MBN.Utils.Caching;
using HappyRE.Core.MapModels.Report;
using HappyRE.Core.MapModels.FrontEnd;
using HappyRE.Core.MapModels;

namespace HappyRE.Web.Controllers
{
    public class BannerController : BaseController
    {
        public BannerController(IUow uow) : base(uow) { }

        /// <summary>
        /// Home Page
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [OutputCache(Duration = Core.Const.CACHE_CLIENT_ONEHOUR)]
        public ActionResult HomeCenter(string code = "")
        {
            var model = _uow.Banner.GetByCode(code);
            if (model == null)
            {
                return Content(string.Empty);
            }
            return PartialView("~/Views/Banner/HomeCenter.cshtml", model);
        }

        /// <summary>
        /// Right Side Bar
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [OutputCache(Duration = Core.Const.CACHE_CLIENT_ONEHOUR)]
        public ActionResult RightBanner(string code = "")
        {
            var model = _uow.Banner.GetByCode(code);

            if (model == null)
            {
                return Content(string.Empty);
            }

            return PartialView("~/Views/Banner/RightBanner.cshtml", model);
        }

        /// <summary>
        /// Main Contain
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [OutputCache(Duration = Core.Const.CACHE_CLIENT_ONEHOUR)]
        public ActionResult MainBanner(string code = "")
        {
            var model = _uow.Banner.GetByCode(code);

            if (model == null)
            {
                return Content(string.Empty);
            }

            return PartialView("~/Views/Banner/MainBanner.cshtml", model);
        }
    }
}