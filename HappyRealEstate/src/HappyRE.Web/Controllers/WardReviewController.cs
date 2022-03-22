
using HappyRE.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HappyRE.Core.Resources;
using HappyRE.Core.Utils;
using MBN.Utils.Extension;
using HappyRE.Core.BLL.Repositories;

namespace HappyRE.Web.Controllers
{
    public class WardReviewController : BaseController
    {
        public WardReviewController(IUow uow) : base(uow)
        {
        }

        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult City()
        {
            var model = _uow.WardReview.GetCity();

            // SEO
            ViewBag.Title = "Review khu vực";
            ViewBag.Description = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_Desc, "khu vực");
            ViewBag.Canonical = Common.HomeUrl + Message.Routing_WardReview;

            return View(model);
        }

        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult Wards(int did = 0)
        {
            var model = _uow.WardReview.GetWard(did);
            ViewBag.DistrictId = did;

            var district = _uow.City.Get(did);
            ViewBag.DistrictName = (district != null) ? district.Name : "";

            // SEO
            if (district != null)
            {
                ViewBag.Title = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_Title, district.FullName);
                ViewBag.Description = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_Desc, district.FullName);
                ViewBag.Canonical = Common.HomeUrl + string.Format(Message.Routing_WardReview_WardListing, district.CodeUrl, district.CityId);
                ViewBag.GeoPlacename = district.FullName;
                ViewBag.DCTitle = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_DCTitle, district.FullName);
            }

            ViewBag.GeoRegion = (district.ParentId == 24) ? "VN-HN" : "VN-SG";

            var map = _uow.Map.GetBy(did, HappyRE.Core.Const.MAP_REFERTYPE_DISTRICT);
            if (map != null && !string.IsNullOrEmpty(map.Location))
            {
                string[] separators = { ",", " " };
                var location = map.Location.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (location.Length >= 2)
                {
                    var latlong = location[0] + "," + location[1];
                    ViewBag.GeoPosition = latlong;
                    ViewBag.ICBM = latlong;
                }
            }

            return View(model);
        }

        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult Districts(int cid = 0)
        {
            var model = _uow.WardReview.GetDistrict(cid);
            ViewBag.CityId = cid;

            var city = _uow.City.Get(cid);
            ViewBag.CityName = (city != null) ? city.Name : "";

            // SEO
            if (city != null)
            {
                ViewBag.Title = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_Title, city.FullName);
                ViewBag.Description = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_Desc, city.FullName);
                ViewBag.Canonical = Common.HomeUrl + string.Format(Message.Routing_WardReview_DistrictListing, city.CodeUrl, city.CityId);
                ViewBag.GeoPlacename = city.FullName;
                ViewBag.DCTitle = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_DCTitle, city.FullName);
            }

            ViewBag.GeoRegion = (cid == 24) ? "VN-HN" : "VN-SG";

            var map = _uow.Map.GetBy(cid, HappyRE.Core.Const.MAP_REFERTYPE_CITY);
            if (map != null && !string.IsNullOrEmpty(map.Location))
            {
                string[] separators = { ",", " " };
                var location = map.Location.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (location.Length >= 2)
                {
                    var latlong = location[0] + "," + location[1];
                    ViewBag.GeoPosition = latlong;
                    ViewBag.ICBM = latlong;
                }
            }

            return View(model);
        }

        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult Ward(int id = 0)
        {
            WardReviewViewModel model = new WardReviewViewModel();

            var wardReview = _uow.WardReview.Get(id);
            if (wardReview == null)
            {
                return RedirectToAction("Wards");
            }
            model.WardReview = wardReview;

            var places = _uow.MogiReport.GetWardPlaceSummarize(id);
            model.WardPlaces = places;

            var ward = _uow.Ward.Get(id);
            model.WardName = ward.Name;

            var codeUrl = _uow.Property.FriendlyUrl_Ward(id);
            var district = _uow.City.Get(ward.DistrictId);
            var city = _uow.City.Get(district.ParentId);

            model.WardId = id;
            model.CityId = city.CityId;
            model.DistrictId = district.CityId;

            model.SaleUrl = _uow.Property.FriendlyUrl(false, codeUrl, city, district);
            model.RentUrl = _uow.Property.FriendlyUrl(true, codeUrl, city, district);

            var schools = _uow.MogiReport.GetWardSchools(id);
            model.Schools = schools;

            // Map Url
            // https://cdn.batdongsanhanhphuc.vn/upload/gmap/30/372/ward-11006.png
            var objServer = _uow.MediaServer.Get(6);
            string applicationPath = "";
            if (objServer != null) applicationPath = objServer.ApplicationPath;
            model.MapUrl = applicationPath + string.Format("{0}/{1}/ward-{2}.png", city.CityId, district.CityId, id);

            // SEO
            var locationTitle = ward.Name + ", " + district.FullName;
            ViewBag.Title = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_Title, locationTitle);
            ViewBag.Description = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_Desc, locationTitle);
            ViewBag.Canonical = Common.HomeUrl + string.Format(Message.Routing_WardReview_WardListing, district.CodeUrl, district.CityId);
            ViewBag.GeoPlacename = locationTitle;
            ViewBag.GeoRegion = (district.ParentId == 24) ? "VN-HN" : "VN-SG";
            ViewBag.DCTitle = string.Format(HappyRE.Web.Resources.Message.ReviewWard_SEO_DCTitle, locationTitle);

            var map = _uow.Map.GetBy(id, HappyRE.Core.Const.MAP_REFERTYPE_WARD);
            if (map != null && !string.IsNullOrEmpty(map.Location))
            {
				char[] separators = new char[] { ',', ' ' };
                var location = map.Location.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (location.Length >= 2)
                {
                    var latlong = location[0] + "," + location[1];
                    ViewBag.GeoPosition = latlong;
                    ViewBag.ICBM = latlong;
                }
            }

			//ViewBag.MAPJSON = map.ToJson();

            return View(model);
        }
    }
}