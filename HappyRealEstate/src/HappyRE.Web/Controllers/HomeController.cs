using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.SessionState;

using MBN.Utils.Extension;
using HappyRE.Web.Models;
using HappyRE.Core.MapModels;
using Mogi.BLL.Utils;
using MBN.Utils;
using MBN.Utils.Caching;
using HappyRE.Core.Entities;
using HappyRE.Web.Helpers;
using HappyRE.Core.MapModels.Report;
using HappyRE.Core.BLL.Repositories;

namespace HappyRE.Web.Controllers
{
	[SessionState(SessionStateBehavior.Disabled)]
    public class HomeController : BaseController
    {
        public HomeController(IUow uow) : base(uow) { }
        public HomeController() : base(null) { }

        public void CreateResource()
        {
            _uow.City.ClearCacheAll();
            App_Start.MogiInit.InitApp();
        }
        private DateTime landingDate = new DateTime(2017, 8, 30);

        public ActionResult LandingPageEN()
        {
            string cacheExiredDate = "expired-landingpage";
            // DateTime expired = DateTime.Now;

            ///var result = CacheManager.CacheClient.GetValue<DateTime>(cacheExiredDate);
            DateTime? expired = cacheExiredDate.GetCache<DateTime?>();//  CacheManager.CacheClient.GetValue<DateTime>(cacheExiredDate);
            if (expired == null)
            {
                expired = this.landingDate;
                if (expired.Value.Date < DateTime.Today) expired = DateTime.Today.AddDays(10);
            }
            expired.MemoryCache(cacheExiredDate);
            ViewBag.Expired = expired.Value.ToString("yyyy-MM-dd");
            ViewBag.Menu = _uow.CMSCategory.GetByGroupId(Core.Const.CMS_GROUP_MENU);
            return View();
        }
        public ActionResult LandingPage()
        {
            try
            {
                string cacheExiredDate = "expired-landingpage";
                // DateTime expired = DateTime.Now;

                ///var result = CacheManager.CacheClient.GetValue<DateTime>(cacheExiredDate);
                DateTime? expired = cacheExiredDate.GetCache<DateTime?>();//  CacheManager.CacheClient.GetValue<DateTime>(cacheExiredDate);
                if (expired == null)
                {
                    expired = this.landingDate;
                    if (expired.Value.Date < DateTime.Today) expired = DateTime.Today.AddDays(10);
                }
                expired.MemoryCache(cacheExiredDate);
                ViewBag.Expired = expired.Value.ToString("yyyy-MM-dd");
                ViewBag.Menu = _uow.CMSCategory.GetByGroupId(Core.Const.CMS_GROUP_MENU);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return View();
        }

        public ActionResult LandingPage3()
        {
            return View();
        }

        public ActionResult LandingPage4()
        {
            return View();
        }

		[OutputCache(CacheProfile = "CacheHome")]
        public ActionResult Index()
        {
			//if (this.IsAuthenticated() == true)
			//{
			//	var user = this.UserData;
			//	if (user == null)
			//	{
			//		HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			//		Session.Abandon();
			//	}
			//	else if (user.IsNeedVerifiedMobile() == true)
			//	{
			//		var profile = _uow.UserProfile.GetUser(Core.Const.PROFILE_PROVIDER_DEFAULT, User.Identity.Name);
			//		if (profile != null && user.IsVerifiedMobile == false)
			//		{
			//			return RedirectToAction("Verified", "Account");
			//		}
			//	}
			//}



			//ViewBag.IsMobileDevice = this.IsMobileDevice();
			//ViewBag.IsAuthenticated = this.IsAuthenticated();

			this.SEO_Home();
			return View("Index");
        }

        protected void SEO_Home()
        {
            try
            {
                var seo = _uow.SEO_Page.SEO_Home();
                ViewBag.TitlePage = seo.TitlePage;
                ViewBag.Title = seo.Title;
                ViewBag.Description = seo.Description;
                ViewBag.PageDescription = seo.PageDescription;
                ViewBag.Keywords = seo.MetaKeywords;
                ViewBag.Footer = seo.Footer;
                ViewBag.Canonical = seo.Canonical;
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("HomeController.SEO_Home", ex);
                throw ex;
            }
        }

        public ActionResult About()
        {
            ViewBag.Data = "";
            ViewBag.Message = "Your application description page.";

            //Mogi.SolrSearch.Models.PropertyQuery q = new SolrSearch.Models.PropertyQuery()
            //{
            //    CityId = 30,
            //    FromArea = 10,
            //    ToArea = 0,
            //    FromPrice = 0,
            //    ToPrice = 9000000,
            //    Indoor = new List<byte>() { 1, 2, 3 }
            //};
            //Mogi.SolrSearch.PropertyProvider provider = new Mogi.SolrSearch.PropertyProvider("http://192.168.2.252:8080/solr/mogi.ban");
            //string resp = provider.QueryAsJson(q);
            //var resp = provider.Query<Core.MapModels.Search.PropertyResult>(q);

            //ViewBag.Data = resp;

            //string keyword = MBN.Utils.WebUtils.GetQuery("q", "");
            //ViewBag.Data = _uow.Map.Suggest(keyword);

            return View();
        }

        public ActionResult Image360()
        {
            ViewBag.Data = "";
            ViewBag.Message = "Your application description page.";
            return View();
        }

		public ActionResult UnSubscribe(string act="")
		{
			if (act == Core.Const.UNSUBSCRIBE_ACTION_LEAD)
			{
				return this.UnSubscribeLead();
			}

            return RedirectToAction("PageNotFound");
        }

        public ActionResult UnSubscribeLead()
        {
            string k = WebUtils.GetQuery("k", "");
            int id = WebUtils.GetQuery("id", 0);
            if (id <= 0 || string.IsNullOrEmpty(k) == true)
            {
                return RedirectToAction("PageNotFound");
            }
            Guid leadKey;
            if (Guid.TryParse(k, out leadKey) == false)
            {
                return RedirectToAction("PageNotFound");
            }


            var lead = _uow.Lead.GetBy(id, leadKey);
            if (lead == null)
            {
                return RedirectToAction("PageNotFound");
            }

            if (lead.UnSubscribe == false)
            {
                _uow.Lead.UnSubscribeLead(lead.LeadId);
                ViewBag.Text = string.Format("Email {0} của bạn đã được cập nhật không nhận thư mời!", lead.Email);
                return View();
            }

            return RedirectToAction("PageNotFound");
        }


        #region Move
        public ActionResult Move(string newUrl)
        {
            if (string.IsNullOrEmpty(newUrl) == true)
            {
                newUrl = "/";
            }

            return this.RedirectPermanent(newUrl);
        }

        public ActionResult PageNotFound(int errorCode = 404)
        {
            ViewBag.Title = $"{errorCode} Error | batdongsanhanhphuc.vn";
            Response.StatusCode = errorCode;
			Models.ListViewModel model = new Models.ListViewModel()
			{
				Version = this.GetVersion(),
				Data = new List<Core.MapModels.Search.PropertyItem>()
			};
			return View(model);
        }
        #endregion

        #region BuyingGuide
        public ActionResult BuyingGuide()
        {
            return View("BuyingGuide");
        }

        #endregion

        #region Test
        public dynamic test_destination_point()
        {
            Core.MapModels.Maps.LatLon source = new Core.MapModels.Maps.LatLon()
            {
                Lat = 10.77546545,
                Lon = 106.69840599999998
            };

            List<Core.MapModels.Maps.LatLon> res = new List<Core.MapModels.Maps.LatLon>();
            Core.MapModels.Maps.LatLon item;
            item = SolrSearch.GoogleMapUtils.DestinationPoint(source, 1000, 90);
            res.Add(item);
            item = SolrSearch.GoogleMapUtils.DestinationPoint(source, 1000, -90);
            res.Add(item);
            return res.ToJson();
        }

        [AllowAnonymous]
        public dynamic test_homedy_crawl(int start = 0, int end = 0)
        {
            BLL.Proxy.HomedyCrawlService service = new BLL.Proxy.HomedyCrawlService();

            return service.CrawlData(_uow, start, end);
        }

        [AllowAnonymous]
        public void test_alertsearch_get_total(string fileName)
        {
            var alert = _uow.AlertSearch.Get(1179);
            var filter = alert.GetSearchFilter();
            filter.PageSize = 1;
            filter.Days = 3;
            _uow.Property.FrontEnd_Search(filter);
        }


        [AllowAnonymous]
        public string GetBase64StringForImage(string fileName)
        {
            if (fileName.Contains("\\") || fileName.Contains("/")) return string.Empty;

            fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "content\\images\\icons\\" + fileName);

            byte[] imageBytes = System.IO.File.ReadAllBytes(fileName);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        public ActionResult Test2()
        {
            _uow.UserProfile.Mogi_UploadAvatarFromUrl(897, "http://cafefcdn.com/zoom/370_230/2018/7/25/photo1532554822089-153255482208915448333.jpg", this.CurrentUserId);
            return View();
        }

        [AllowAnonymous]
        public ActionResult Test()
        {
            ViewBag.Data = "Data";
            ViewBag.Data1 = "Data1";
            //ViewBag.Data =  _uow.CMS.GetNewsFocus().ToJson();

            //BLL.Queues.TopServiceTask t = new BLL.Queues.TopServiceTask();
            //t.Execute();

            //var res = _uow.Property.FrontEnd_SearchSimilar(false, 30, 363, 3);
            //var res1 = _uow.Property.FrontEnd_ServiceTop(30);
            //ViewBag.Data = res.ToJson();
            //ViewBag.Data1 = res1.ToJson();

            //BLL.Utils.MBCoreAPI.SendBrandName("0918425068", "Hello", 0, string.Empty);

            //string s = string.Empty;
            //Core.Models.CaptchaState obj = _uow.Captcha.GetState("0918425068", Core.Const.CAPTCHA_REGISTER);
            //s += "<br>" + (obj.Show ? "show:ok" : "show:no");
            //s += "<br>" + (obj.Lockout ? "lockout:ok" : "lockout:no");
            //for (int i = 0; i < 10; i++)
            //{
            //    _uow.Captcha.Audit("0918425068", Core.Const.CAPTCHA_REGISTER);
            //    obj = _uow.Captcha.GetState("0918425068", Core.Const.CAPTCHA_REGISTER);
            //    s += "<br>" + i.ToString() + ". " + (obj.Show ? "show:ok" : "show:no");
            //    s += "<br>" + i.ToString() + ". " + (obj.Lockout ? "lockout:ok" : "lockout:no");
            //}
            //ViewBag.Data = s;
            //ViewBag.Data1 = s;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Test_Price(string code = "VCH-200", int days = 1, int quantity = 1)
        {
            List<Core.MapModels.ItemPrice> items = new List<Core.MapModels.ItemPrice>();
            items.Add(new Core.MapModels.ItemPrice()
            {
                Code = code,
                Days = days,
                Quantity = quantity
            });
            var res = BLL.Utils.MBCoreAPI.GetPrice(items);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Test_Favorite()
        {
            int total = 0;
            var res = _uow.Favorite.GetList(out total, 1, 10, false, 51);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Test_LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public ActionResult Test_SendMail()
        {

            var obj = _uow.CMSCategory.Get("mail-support");
            if (obj != null)
            {
                EmailUtil.SendMail("mogi_support", "vuvanquy@gmail.com", "Please support me", "mogi_support", obj.Description,
                    new Dictionary<string, string>() {
                        { "NAME", "VU VAN QUY"},
                        { "EMAIL", "quy.vu@muaban.net"},
                        {"CONTENT","Yêu cầu hỗ trợ đăng tin trên batdongsanhanhphuc.vn" }
                    });
            }
            else
            {
                EmailUtil.SendMail("vuvanquy@gmail.com", "TEST", "SUPPORT ME PLEASE");

            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Test_Map(int referId, int referTypeId)
        {
            var obj = _uow.Map.GetBy(referId, referTypeId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }



        [AllowAnonymous]
        public ActionResult Test_SearchMap()
        {
            var filter = new Core.MapModels.SearchFilter()
            {
                MapId = 731,
                ReferId = 30,
                ReferTypeId = 2,
                CityId = 30
            };
            string json = _uow.Property.FrontEnd_SearchMap(filter);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Test_Map_RepairEncode(int mapId = 0)
        {
            int total = 0;
            int lastId = -1;
            while (mapId < 100000)
            {
                total += 100;
                lastId = _uow.Map.Repair_Encode(mapId);
                if (lastId == mapId)
                {
                    break;
                }
                mapId = lastId;
            }
            return Json(new { Total = total, MapId = mapId }, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult Test_Project_Repair(int projectId = 0, int total = 0)
        {
            if (total == 0) total = 100;

            projectId = _uow.Project.RepairImage(projectId, total);

            return Json(new { Total = total, ProjectId = projectId }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Test_Org_Repair(int orgId = 0, int total = 0)
        {
            if (total == 0) total = 100;

            orgId = _uow.Org.RepairImage(orgId, total);

            return Json(new { Total = total, ProjectId = orgId }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Test_Encrypt()
        {
            string v = "Vu Van Quy";
            string v1 = _uow.UserWebsite.Encrypt(v);
            string v2 = _uow.UserWebsite.Decrypt(v1);

            return Json(new { v, v1, v2 }, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult Test_SearchAlert()
        {
            try
            {
                var obj = new Mogi.BLL.Queues.AlertSearchTask();
                obj.Execute();
                return Json(new { status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        [AllowAnonymous]
        public ActionResult Test_SendSearchAlert()
        {
            try
            {
                var obj = new Mogi.BLL.Queues.AlertSearchSendTask();
                obj.Execute();
                return Json(new { status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        [AllowAnonymous]
        public ActionResult Test_Captcha(string code, string mobile = "")
        {
            string key_audit = _uow.Captcha.GetKeyAudit(mobile, code, this.ClientIP);
            string key_lockout = _uow.Captcha.GetKeyLockout(mobile, code, this.ClientIP);
            var audit = key_audit.StringGetAs<CaptchaAudit>();

            var state = _uow.Captcha.GetState(mobile, code, this.ClientIP);

            var res = new
            {
                ClientIP = this.ClientIP,
                State = state,
                Audit = audit
            };



            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test_ImageToBase64(string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }

            string base64String = "";
            string path = System.IO.Path.GetFileName(fileName);
            path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "content/images/" + fileName);
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                using (System.IO.MemoryStream m = new System.IO.MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    base64String = Convert.ToBase64String(imageBytes);
                }
            }

            return Json(base64String, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Test_Property_RepairMedia(int propertyId, int top = 10, int total = 1)
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stop.txt");
            while (total > 0)
            {
                propertyId = _uow.Property.Repair_GetList(propertyId, top);
                total--;
                if (System.IO.File.Exists(fileName))
                {
                    break;
                }
                System.Threading.Thread.Sleep(10);
            }

            return Json(propertyId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test_Property_RepairMediaEmpty()
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RepairMediaEmpty.txt");
            if (System.IO.File.Exists(fileName) == false)
            {
                return Json("NotFound:" + fileName, JsonRequestBehavior.AllowGet);
            }
            string[] items = System.IO.File.ReadAllText(fileName).Split(',', ';');
            List<int> lst = new List<int>();
            foreach (string item in items)
            {
                if (string.IsNullOrEmpty(item)) continue;
                lst.Add(int.Parse(item));
            }
            if (lst.Count > 0)
            {
                _uow.Property.Repair_GetList(lst);
            }

            return Json("Total:" + lst.Count, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test_City_Language(int cityId = 0)
        {

            var item = _uow.City.Get(cityId);
            //if (item != null)
            //{
            //    string[] names = item.Name.Split('-');


            //    item.Name = names[0] + "-" + DateTime.Now.ToString("HH:mm:ss");

            //    _uow.City.Update(item);
            //    _uow.City.Commit();

            //    item.Code = DateTime.Now.ToString("HH:mm:ss");
            //    //_uow.City.Update(item);
            //    //_uow.City.Commit();
            //}
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test_Translation(int translateId = 0, int languageId = 2)
        {
            _uow.TranslateConfig.Import(translateId, languageId);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Test_MarketPrice(int sid = 0, int ptid = 0)
        {

            var k = sid.ToString() + "_" + ptid.ToString();
            var x = Core.Const.REDIS_PROPERTY_HOUSEPRICE.HashGet<MarketPrices>(k);

            // _uow.TranslateConfig.Import(translateId, languageId);

            return Json(x, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Test_PushCacheMarketPrice()
        {

            var x = new BLL.Queues.AvgPriceTask();
            x.Execute();
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test_Project_Repair_Images(int projectId = 0, int top = 1, bool next = false)
        {

            projectId = _uow.Project.Repair_Images(projectId, top);
            while (next == true && projectId != -1)
            {
                WebLog.Log.Info("Test_Project_Repair_Images-ProjectId:" + projectId);
                projectId = _uow.Project.Repair_Images(projectId, top);
            }

            return Json("Next:" + projectId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test_Project_Repair_Images_Content(int projectId = 0, int top = 1, bool next = false)
        {

            projectId = _uow.Project.Repair_Images(projectId, top);
            while (next == true && projectId != -1)
            {
                projectId = _uow.Project.Repair_Images(projectId, top);
            }

            return Json("Next:" + projectId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test_Repair_Media_Agent(int mediaId = 0, int top = 1, bool next = false)
        {

            mediaId = _uow.Media.Repair_Media_Agent(mediaId, top);
            while (next == true && mediaId != -1)
            {
                mediaId = _uow.Media.Repair_Media_Agent(mediaId, top);
            }

            return Json("Next:" + mediaId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test_Homedy()
        {
            new BLL.Queues.HomedyCrawlTask().Execute();
            return null;
        }

        public dynamic Test_Cache_Street(int districtId = 0)
        {

            string json = _uow.Street.GetAll().ToJson();
            WebLog.Log.Data(json, false, "street.json");

            if (districtId > 0)
            {
                return Json(_uow.Street.API_GetByDistrictId(districtId), JsonRequestBehavior.AllowGet);
            }

            return "DONE";
        }

        public dynamic Test_Call_API()
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.BaseAddress = new Uri("https://api.muaban.net/");
            return client.GetAsync("/user/getauth?authKey=0918425068").Result;

            //return "DONE";
        }

        public dynamic test_wikimapia_gadm_lookup_ward(int id = 0, int districtId = 0)
        {
            string s = "";
            List<int> districts = null;
            var lst = _uow.Map.wikimapia_gadm_getlist(3, id, districtId);
            foreach (var item in lst)
            {
                var polygons = item.Encode.Split(';').ToList();
                districts = new List<int>() { item.DistrictId };
                var data = Mogi.SolrSearch.MarkerProvider.Query("", null, districts, polygons, 1);
                if (data == null) continue;
                if (data.Response.Total > 0)
                {
                    s += "\r\nWardId: " + item.WardId + " - ";
                    foreach (var d in data.Response.Data)
                    {
                        s += d.ReferId;
                    }
                }
            }

            return "DONE - " + DateTime.Now.yyyyMMddHHmmss() + s;
        }

        public ActionResult TestLeadInvitation()
        {
            try
            {
                var obj = new Mogi.BLL.Queues.InvitingLeadTask();
                obj.Execute();
                return Json(new { status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = ex.Message, trace = ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult TestLeadUpgradedQueue(int id, Guid k, int pid, string mobile)
        {
            try
            {
                var obj = new Mogi.BLL.Queues.LeadUpgradingCompletedQueue(id, k, pid, mobile);
                obj.Execute();
                return Json(new { status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = ex.Message, trace = ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult test_convert_zone(int fromLevelId = 2, int toLevelId = 4, int days = 60)
        {
            var res = _uow.Level.ConvertZone(fromLevelId, toLevelId, days);
            return Json(new { Status = "OK", Data = res }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public dynamic Repair_UserZone_StartDate()
        {
            //_uow.UserZone.Repair_StartDate();
            return "DONE";
        }

        public dynamic Repair_UserOrder_ReferId(int fromOrderId = 0, int top = 1)
        {
            var res = _uow.UserOrder.Repair_ReferId(fromOrderId, top);
            return "DONE: " + DateTime.Today.yyyyMMddHHmmss() + ": " + res;
        }
    }
}