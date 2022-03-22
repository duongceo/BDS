using MBN.Utils;


using HappyRE.Core.MapModels;
using HappyRE.Core.Entities;
using HappyRE.Core.Resources;
using HappyRE.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MBN.Utils.Extension;
using HappyRE.Core.MapModels.SEO;
using HappyRE.Core.Utils;
using MBN.Utils.Caching;
using System.Net;
using HappyRE.Core.BLL.Repositories;

namespace HappyRE.Web.Controllers
{
    public class PropertyController : BaseController
    {
        public PropertyController(IUow uow) : base(uow)
        {
            ViewBag.Title = string.Empty;
        }

        public ActionResult Index()
        {
            return View();
        }

		[OutputCache(CacheProfile = "CacheListing")]
		public ActionResult ListBuy(ListFilter filter)
        {
            var f = this.GetSearchFilter(false, filter);
            return this.ListView(f);
        }

		[OutputCache(CacheProfile = "CacheListing")]
		public ActionResult ListRent(ListFilter filter)
        {
            var f = this.GetSearchFilter(true, filter);
            return this.ListView(f);
        }

        public ActionResult MapBuy(ListFilter filter)
        {
            var f = this.GetSearchFilter(false, filter);
            return this.ListView(f);
        }

        private SearchFilter GetSearchFilter(bool rent, Models.ListFilter q)
        {
            SearchFilter filter = new SearchFilter()
            {
                q = q.q,
                Rent = rent,
                ReferTypeId = q.rid,
                ReferId = q.rtid,
                CityId = q.CityId,
                DistrictId = q.DistrictId,
                StreetId = q.StreetId,
                WardId = q.WardId,
                ProjectId = q.ProjectId,
                PlaceId = q.PlaceId,
                PropertyTypeId = q.PropertyTypeId,
                FromPrice = q.fp,
                ToPrice = q.tp,
                FromArea = q.fa,
                ToArea = q.ta,
                FromBedRoom = q.fbr,
                ToBedRoom = q.tbr,
                DirectionId = q.dt,
                LegalId = q.lg,
                Days = q.d,
                Sort = q.s,
                Polyenc = q.polyenc,
                PageIndex = q.cp.GetValueOrDefault(1)
            };

            // PropertyType
            q.PropUrl = (q.PropUrl ?? "");
            if (q.PropUrl != Core.Resources.Message.Routing_PropertyType_All)
            {
                var objProp = _uow.PropertyType.GetByCode(q.PropUrl);
                if (objProp != null)
                {
                    if (objProp.ParentId == 0)
                    {
                        filter.PropertyTypeId = objProp.PropertyTypeId;
                    }
                    else
                    {
                        filter.PropertyTypeId = objProp.ParentId;
                        filter.PropertyStyles = new List<int>() { objProp.PropertyTypeId };
                    }
                }
            }

            // PropertyStyle
            if (filter.PropertyTypeId > 0 && q.psid != null && q.psid.Length > 0 && q.psid.Length < 20)
            {
                filter.PropertyStyles = q.psid.ToList();
            }

            // City & District
            City objCity = _uow.City.GetByCodeUrl(q.CityUrl, q.DistrictUrl);
            if (objCity != null)
            {
                filter.CityId = (objCity.ParentId == 0 ? objCity.CityId : objCity.ParentId);
                filter.DistrictId = (objCity.ParentId == 0 ? 0 : objCity.CityId);
            }

            if (filter.ProjectId > 0)
            {
                filter.ReferId = filter.ProjectId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_PROJECT;
            }
            else if (filter.PlaceId > 0)
            {
                filter.ReferId = filter.PlaceId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_PLACE;
            }
            else if (filter.StreetId > 0)
            {
                filter.ReferId = filter.StreetId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_STREET;
            }
            else if (filter.WardId > 0)
            {
                filter.ReferId = filter.WardId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_WARD;
            }
            else if (filter.DistrictId > 0)
            {
                filter.ReferId = filter.DistrictId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_DISTRICT;
            }
            else if (filter.CityId > 0)
            {
                filter.ReferId = filter.CityId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_CITY;
            }
            var objMap = _uow.Map.GetBy(filter.ReferId, filter.ReferTypeId);
            if (objMap != null)
            {
                filter.Map = objMap.Small();
                if (filter.IsPlace() == true)
                {
                    if (filter.Polyenc == null)
                    {
                        filter.Polyenc = new List<string>();
                    }
                    if (filter.Polyenc.Count == 0)
                    {
                        if (string.IsNullOrEmpty(objMap.Encode) == false)
                        {
                            filter.Polyenc.Add(objMap.Encode);
                        }
                        else
                        {

                        }
                    }
                }
            }

            return filter;
        }

        private SearchFilter GetSearchFilter(bool rent, string cityUrl, string districtUrl, string propUrl, int pid, int[] psid, int streetId, int wardId, int projectId, int placeId, int? fp, int? tp, int? fa, int? ta, byte? fbr, byte? tbr, byte? dt, byte? lg, byte? d, string s)
        {
            SearchFilter filter = new SearchFilter() { Rent = rent, StreetId = streetId, WardId = wardId, ProjectId = projectId, PlaceId = placeId };

            // PropertyType
            propUrl = (propUrl ?? "");
            if (propUrl != Core.Resources.Message.Routing_PropertyType_All)
            {
                var objProp = _uow.PropertyType.GetByCode(propUrl);
                if (objProp != null)
                {
                    if (objProp.ParentId == 0)
                    {
                        filter.PropertyTypeId = objProp.PropertyTypeId;
                    }
                    else
                    {
                        filter.PropertyTypeId = objProp.ParentId;
                        filter.PropertyStyles = new List<int>() { objProp.PropertyTypeId };
                    }
                }
            }

            // PropertyStyle
            if (filter.PropertyTypeId > 0 && psid != null && psid.Length > 0 && psid.Length < 20)
            {
                filter.PropertyStyles = psid.ToList();
            }

            // City & District
            City objCity = _uow.City.GetByCodeUrl(cityUrl, districtUrl);
            if (objCity != null)
            {
                filter.CityId = (objCity.ParentId == 0 ? objCity.CityId : objCity.ParentId);
                filter.DistrictId = (objCity.ParentId == 0 ? 0 : objCity.CityId);
            }

            if (filter.ProjectId > 0)
            {
                filter.ReferId = filter.ProjectId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_PROJECT;
            }
            else if (filter.PlaceId > 0)
            {
                filter.ReferId = filter.PlaceId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_PLACE;
            }
            else if (filter.StreetId > 0)
            {
                filter.ReferId = filter.StreetId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_STREET;
            }
            else if (filter.WardId > 0)
            {
                filter.ReferId = filter.WardId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_WARD;
            }
            else if (filter.DistrictId > 0)
            {
                filter.ReferId = filter.DistrictId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_DISTRICT;
            }
            else if (filter.CityId > 0)
            {
                filter.ReferId = filter.CityId;
                filter.ReferTypeId = Const.MAP_REFERTYPE_CITY;
            }
            var objMap = _uow.Map.GetBy(filter.ReferId, filter.ReferTypeId);
            if (objMap != null) filter.Map = objMap.Small();

            return filter;
        }

        #region ListView
        public ActionResult ListView(Core.MapModels.SearchFilter filter)
        {
            filter.PageSize = 15;

            var cookie = Request.Cookies["view"];
            if (cookie != null && string.IsNullOrEmpty(cookie.Value) == false && cookie.Value == "map")
            {
                return MapView(filter);
            }
            var objUserData = this.UserData;
            ViewBag.Menu_ActiveCode = (filter.Rent ? "menu-rent" : "menu-buy");
            ViewBag.UserData = objUserData;
            ViewBag.IsMogiPro = (objUserData == null ? false : objUserData.IsMogiPro);

            // Tìm kiếm dữ liệu
            var obj = _uow.Property.FrontEnd_Search(filter);
			Models.ListViewModel model = new Models.ListViewModel()
			{
				Version = this.GetVersion()
			};
            if (obj != null)
            {
                model.Total = obj.Response.Total;
                model.Data = obj.Response.Data;
            }
            // Tin TOP
            this.GetTopItem(model, filter);

            // SEO
            this.SEO_Listing(filter);

            // Filter
            model.Filter = filter;

			#region AlertSearch - disabled - 14:34 2019-04-04
			//if (User.Identity.IsAuthenticated == true)
   //         {
   //             var objAlert = _uow.AlertSearch.Get(this.CurrentProfileId, filter);
   //             if (objAlert != null)
   //             {
   //                 model.AlertSearch = Newtonsoft.Json.JsonConvert.SerializeObject(objAlert.ToSmall());
   //             }
   //         }
			#endregion

			// Tạo paging
			this.SetPaging(model, filter);

			//this.SaveResultSeach(model.Data, filter, model.Paging.Url);


			ViewBag.Queries = "?searchkey=" + model.Filter.GetHashKey();

			return View("ListViewv3", model);
        }
        #endregion


        public ActionResult MapView(Core.MapModels.SearchFilter filter)
        {
            var objUserData = this.UserData;
            ViewBag.Menu_ActiveCode = (filter.Rent ? "menu-rent" : "menu-buy");
            ViewBag.UserData = objUserData;
            ViewBag.IsMogiPro = (objUserData == null ? false : objUserData.IsMogiPro);
            ListViewModel model = new ListViewModel()
            {
                Filter = filter
            };

            // Paging
            this.SetPaging(model, filter);
            model.Paging.Total = -1;

            return View("MapView", model);
        }

        public ActionResult MapViewData(ListFilter filter)
        {
            //WebLog.Log.Data(filter.ToJson(), true, "MapViewData.txt");
            var f = this.GetSearchFilter(filter.Rent, filter);
            var json = _uow.Property.FrontEnd_SearchMap(f);
            return Content(json, "application/json");
        }

        public ActionResult ListViewByIds(bool rent, int[] ids, int total = 0, int pageIndex = 1, string url = "")
        {
            if (ids == null || ids.Length == 0 || ids.Length > 50) return null;

            var res = _uow.Property.FrontEnd_GetByIds(rent, ids.ToList());
			ListViewModel model = new ListViewModel
			{
				Total = res.Response.Total,
				Data = res.Response.Data
			};

			this.SetFavorites(model);

            model.Paging = new Paging()
            {
                Total = total,
                CurrentPage = pageIndex,
                PageSize = 10,
                Url = url
            };

            return PartialView("~/Views/Property/Partials/_Map.cshtml", model);
        }

        public JsonResult ListingByIds(bool rent, int[] ids, int total = 0, int pageIndex = 1, string url = "")
        {
            if (ids == null || ids.Length == 0 || ids.Length > 50)
            {
                throw new Exception("ids max 50!");
            }

            var res = _uow.Property.FrontEnd_GetByIds(rent, ids.ToList());
			ListViewModel model = new ListViewModel
			{
				Total = res.Response.Total,
				Data = res.Response.Data
			};

			this.SetFavorites(model);

            model.Paging = new Paging()
            {
                Total = total,
                CurrentPage = pageIndex,
                PageSize = 10,
                Url = url
            };
            var data = new
            {
                Paging = new { Total = total, CurrentPage = pageIndex, PageSize = 10, Url = url },
                model.Total,
                Data = model.Data.Select(c => new
                {
                    c.PropertyId,
                    c.Url,
                    c.Title,
                    c.Address,
                    c.Summary,
                    c.BedRooms,
                    c.BathRooms,
                    Area = Convert.ToInt64(c.Area),
                    CoverImage = c.GetCoverImageUrl(),
                    c.TotalImage,
                    c.StickerName,
                    VIP = c.IsVip(),
                    UP = c.IsUP(),
                    c.PriceView,
                    c.PublishView,
                    AvatarUrl = c.AvatarUrl(),
                    c.User_FullName,
                    c.User_Mobile,
                    MobileView = c.MobileView(3)
                }).ToList()
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListingPopup(string section, List<int> listing_id)
        {
            bool rent = (section.ToLower() == "to-rent");

            var res = _uow.Property.FrontEnd_GetByIds(rent, listing_id);
			ListViewModel model = new ListViewModel
			{
				Total = res.Response.Total,
				Data = res.Response.Data
			};

			this.SetFavorites(model);

            return PartialView("~/Views/Property/Partials/_Map.cshtml", model);
        }

        /// <summary>
        /// Danh sách tin TOP (Paging)
        /// quy.vu - 14:18 2017-05-03
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rent"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult TopService(int id, bool rent = false, int pageIndex = 1, int pageSize = 10)
        {
            var model = _uow.Property.FrontEnd_ServiceTop(id, rent, pageIndex, pageSize);
            return View(model);
        }

        /// <summary>
        /// Lấy danh sách tin đã lưu
        /// </summary>
        /// <param name="model"></param>
        private void SetFavorites(Models.ListViewModel model)
        {
            List<string> lst = new List<string>();
            if (model.Data != null)
            {
                lst = model.Data.Select(s => s.PropertyId.ToString()).ToList();
            }
            if (model.TopItems != null)
            {
                lst.AddRange(model.TopItems.Select(s => s.PropertyId.ToString()));
            }
            if (lst.Count == 0)
            {
                model.Favorites = new List<string>().ToArray();
                return;
            }


            if (this.IsAuthenticated())
            {
                model.Favorites = _uow.Favorite.GetByProperties(lst.ToArray(), this.UserData.ProfileId);
            }
            else
            {
                model.Favorites = _uow.Favorite.GetByProperties(lst.ToArray(), this.ClientId());
            }
        }

        /// <summary>
        /// Lấy danh sách tin TOP
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filter"></param>
        private void GetTopItem(Models.ListViewModel model, Core.MapModels.SearchFilter filter)
        {
            int cityId = 0;
            if (filter.DistrictId > 0) cityId = filter.DistrictId;
            else if (filter.CityId > 0) cityId = filter.CityId;
            if (cityId == 0) return;
            var objTop = _uow.Property.FrontEnd_ServiceTop(cityId, filter.Rent, 1, 6);
            model.TopItems = objTop.Data;
        }

        protected void SEO_Listing(Core.MapModels.SearchFilter filter)
        {
            try
            {
                var seo = _uow.SEO_Page.SEO_Listing(filter);
                //seo.Canonical = BLL.Utils.Common.Url_List(query, false).Replace("http:", "").Replace("https:", "");

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
                WebLog.Log.Error("PropertyController.SEO_Listing", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Set paging cho listing
        /// </summary>
        /// <param name="model"></param>
        private void SetPaging(Models.ListViewModel model, Core.MapModels.SearchFilter filter)
        {
			Core.MapModels.Paging p = new Core.MapModels.Paging
			{
				Total = model.Total,
				CurrentPage = filter.PageIndex,
				PageSize = filter.PageSize,
				Url = Core.Utils.Common.HomeUrl + Request.RawUrl
			};
			model.Paging = p;
        }


		#region Next/Prev
		/// <summary>
		/// quy.vu - 15:08 2018-04-26
		/// quy.vu  - 10:02 2019-04-04: Không còn sử dụng do Cloudflare
		/// </summary>
		/// <param name="data"></param>
		/// <param name="filter"></param>
		/// <param name="listUrl"></param>
		private void SaveResultSeach_BAK(List<Core.MapModels.Search.PropertyItem> data, SearchFilter filter, string listUrl = "")
		{
			string clientId = this.ClientId().ToString();
			string filterKey = "MOGI_LISTCONTROLLER_FILTER_" + clientId;
			string resultKey = "MOGI_LISTCONTROLLER_RESULT_" + clientId;

			ResultSearchCached obj = null;

			// Filter
			if (filter != null)
			{
				obj = new ResultSearchCached()
				{
					Filter = filter,
					ListUrl = listUrl
				};
				obj.MemoryCache(filterKey, 60);
				// obj.Filter.Fields = "id,cid,did,wid,sid,pjid";
			}

			// Data
			obj = new Models.ResultSearchCached()
			{
				Filter = filter,
				ListUrl = listUrl
			};

            obj.Items = (from c in data
                         select new ItemResult
                         {
                             Id = c.PropertyId,
                             Name = c.Title,
                             Url = c.Url
                         }).ToList();
            obj.ToJson().MemoryCache(resultKey, 60);
        }

		/// <summary>
		/// quy.vu - 15:08 2018-04-26
		/// quy.vu  - 10:02 2019-04-04: Không còn sử dụng do Cloudflare
		/// </summary>
		/// <returns></returns>
		private ResultSearchCached GetResultSearch_BAK()
        {
            string clientId = this.ClientId().ToString();
            //string filterKey = "MOGI_LISTCONTROLLER_FILTER_" + clientId;
            string resultKey = "MOGI_LISTCONTROLLER_RESULT_" + clientId;

            return resultKey.GetCache<string>().FromJson<ResultSearchCached>();
        }

		/// <summary>
		/// quy.vu - 15:08 2018-04-26
		/// quy.vu  - 10:02 2019-04-04: Không còn sử dụng do Cloudflare
		/// </summary>
		/// <param name="propertyId"></param>
		/// <returns></returns>
		public string[] GetNextPrevResult_BAK(int propertyId)
        {
            string[] res = new string[3] { "", "", "" };

            ResultSearchCached obj = this.GetResultSearch_BAK();
            if (obj == null) return res;

            ItemResult[] items = obj.GetNexAndPrev(propertyId);
            if (items == null || items.Length < 2) return res;


            res[2] = obj.ListUrl;
            if (items[0] != null) res[0] = items[0].Url;
            if (items[1] != null) res[1] = items[1].Url;

            return res;
        }
		#endregion

		#region Detail
		[OutputCache(CacheProfile = "CacheDetail")]
		public ActionResult Detail(int id = 0)
		{
			if (id <= 0)
			{
				return RedirectToAction("Index", "Home");
			}

			// Lấy thông tin chi tiết tin
			var data = _uow.Property.GetDetail(id);
			if (data == null)
			{
				// Tin hết hạn
				if (_uow.Property.FrontEnd_IsExpired(id) == true)
				{
					return new HttpStatusCodeResult(HttpStatusCode.Gone); // 410
				}
				return new HttpStatusCodeResult(HttpStatusCode.NotFound); // 404
			}

			// Lấy bất động sản tương tự
			//data.SimilarProperty = _uow.Property.GetSimilarProperty(data);
			var similarQuery = SolrSearch.Models.PropertySimilarQuery.MapObject(data);
			string similarUrl = Url.Action("Property_Similar", "Template").ToLower() + "?" + similarQuery.ToQueryString();
			if (true == string.IsNullOrEmpty(data.SearchUrl))
			{
				var objCity = _uow.City.Get(data.Property.CityId);
				var objDistrict = _uow.City.Get(data.Property.DistrictId);
				var objPropType = _uow.PropertyType.Get(data.Property.SubPropertyTypeId);
				data.SearchUrl = _uow.Property.FriendlyUrl(data.Property.IsRent, "", objCity, objDistrict, objPropType);
			}
			var model = new PropertyDetailViewModel()
			{
				Property = data.Property,
				SimilarUrl = similarUrl,
				SearchUrl = data.SearchUrl
			};

			// Thông tin liên hệ
			model.UserInbox = new UserInboxViewModel
            {
                IsVerifiedIDCard = model.Property.IsVerifiedIDCard,
                ProfileId = model.Property.ProfileId,
                PropertyId = model.Property.PropertyId,
                SenderMessage = Message.Property_Broker_Message,
                AgentCerNo = model.Property.CertNo,
                AgentCode = model.Property.UserCode,
                AgentName = model.Property.FullName,
                AgentMobile = model.Property.Mobile,
                Avatar = model.Property.Avatar,
                AgentUrl = _uow.UserProfile.FriendlyUrl_Agent(model.Property.ProfileId, model.Property.Mobile, model.Property.UserCode, model.Property.FullName),
                UserTypeId = model.Property.UserTypeId,
                IsHidden = model.Property.IsAgentExpired,
                ContactMobile = model.Property.ContactMobile,
                ContactName = Server.HtmlEncode(model.Property.ContactName),
				SenderName = string.Empty,
				SenderMobile = string.Empty,
				SenderEmail = string.Empty,
				JoinedDate = model.Property.JoinedDate
            };

            // Meta SEO
            if (data.Meta == null)
            {
                data.Meta = new SEO_MetaPage();
            }

            var seo = _uow.SEO_Page.SEO_Detail(data);
            var prop = data.Property;
            var meta = data.Meta;
            ViewBag.Canonical = data.Meta.Canonical;
            ViewBag.Title = (string.IsNullOrEmpty(seo.Title) ? prop.Title : seo.Title);
            ViewBag.Description = (string.IsNullOrEmpty(seo.Description) ? prop.Summary : seo.Description);
            ViewBag.OG_Title = prop.Title;
            ViewBag.OG_Description = ViewBag.Description;
            ViewBag.OG_Image = data.Meta.OG_Image;

            //string[] urls = this.GetNextPrevResult(id);
            //model.PrevPropertyResultUrl = urls[0];
            //model.NextPropertyResultUrl = urls[1];
            //model.PropertySearchResultUrl = urls[2];

            return View("Detail", model);
        }

		[OutputCache(Duration = Const.CACHE_CLIENT_ONEMONTH)]
		public ActionResult Go(int id = 0)
		{
			if (id <= 0)
			{
				return RedirectToAction("Index", "Home");
			}

			// Lấy thông tin chi tiết tin
			var data = _uow.Property.GetDetail(id);
			if (data == null)
			{
				// Tin hết hạn
				if (_uow.Property.FrontEnd_IsExpired(id) == true)
				{
					return new HttpStatusCodeResult(HttpStatusCode.Gone); // 410
				}
				return new HttpStatusCodeResult(HttpStatusCode.NotFound); // 404
			}

			return RedirectPermanent(data.Meta.Canonical);

			//return RedirectToAction("Detail", new { id });
		}

		public JsonResult GetNext(bool rent, int did, int psid, int pindex)
		{
			if (psid <= 0 || did <= 0)
			{
				return Json(0, JsonRequestBehavior.AllowGet);
			}

			var resp = _uow.Property.FrontEnd_GetNext(rent, did, psid, pindex, 2);
			if (resp.Response.Total == 0)
			{
				return Json(0, JsonRequestBehavior.AllowGet);
			}
			var data = resp.Response.Data.Select(s => new { s.PropertyId, s.Url }).ToList();
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region SendMail
		public ActionResult ListMail(List<int> ids, string userName, int pid, int aid, string alertTitle, int frequency, bool isRent = false, int total = 0)
        {
            //var data = _uow.Property.FrontEnd_GetByIds(false, new List<int> { 2089072, 2088994 });
            //userName = "Cuong Phung";
            //alertTitle = "Thuê Mặt bằng, Cửa hàng, Shop, Quận Hoàn Kiếm, Hà Nội, 10 Triệu - 50 Triệu ";
            //frequency = 10;
            //aid = 1036;

            var data = _uow.Property.FrontEnd_GetByIds(isRent, ids);
            var properties = data.Response.Data;
			var model = new SearchAlertViewModel
			{
				Alerts = properties,
				UserName = userName,
				Title = alertTitle,
				Frequency = frequency
			};
			var alert = _uow.AlertSearch.Get(aid);
            var filter = alert.GetSearchFilter();

            ViewBag.Total = Math.Max(total, model.Alerts.Count);

            //filter.PageSize = 1;
            //filter.Days = alert.GetDays();
            //var result = _uow.Property.FrontEnd_Search(filter);
            //ViewBag.Total = Math.Max(total, model.Alerts.Count);

            ViewBag.SearchUrl = Common.HomeUrl + _uow.Property.GetPropertySearchUrl(filter);
            ViewBag.UnsubscribeUrl = Common.HomeUrl + _uow.UserProfile.UnsubscribeUrl_SearchAlert(pid, aid, true);
            ViewBag.ChangeUrl = Common.HomeUrl + _uow.UserProfile.UnsubscribeUrl_SearchAlert(pid, aid, false);
            ViewBag.Footer = BLL.Utils.EmailUtil.MailFooter;


            return View(model);
        }
        public ActionResult SendMail()
        {

            //string body = System.IO.File.ReadAllText(@"D:\Project\Mogi\svn\trunk\Implementation\Src\Mogi\HappyRE.Web\Views\Property\ListMail.cshtml");

            //Helpers.EmailHelper.SendMail("chicuongphung@gmail.com", "Mogi - Notify", body);
            //Helpers.EmailHelper.SendMail("cuong.phung@muaban.net", "Mogi - Notify", body);
            //Helpers.EmailHelper.SendMail("chi_cuong2903@yahoo.com", "Mogi - Notify", body);

            return Content("OK");
        }
		#endregion

        #region Student Rent Housse
        public ActionResult StudentRentHouse()
        {
            // seo
            var seo = _uow.SEO_Page.SEO_StudentRentHouse();
            ViewBag.TitlePage = seo.TitlePage;
            ViewBag.Title = seo.Title;
            ViewBag.Description = seo.Description;
            ViewBag.PageDescription = seo.PageDescription;
            ViewBag.Keywords = seo.MetaKeywords;
            ViewBag.Footer = seo.Footer;
            ViewBag.Canonical = HappyRE.Core.Utils.Common.HomeUrl + Message.Routing_StudentRentHouse;

            return View();
        }
        #endregion

        #region Industrial Park Rent House
        public ActionResult IndustrialParkRentHouse()
        {
            // seo
            var seo = _uow.SEO_Page.SEO_IndustrialParkRentHouse();
            ViewBag.TitlePage = seo.TitlePage;
            ViewBag.Title = seo.Title;
            ViewBag.Description = seo.Description;
            ViewBag.PageDescription = seo.PageDescription;
            ViewBag.Keywords = seo.MetaKeywords;
            ViewBag.Footer = seo.Footer;
            ViewBag.Canonical = HappyRE.Core.Utils.Common.HomeUrl + Message.Routing_IndustrialParkRentHouse;

            return View();
        }
        #endregion

        #region Ajax
        public JsonResult ReportMessages()
        {
            var rp = new AjaxResponse();

            try
            {
                var messages = _uow.Feedback.GetFeedbackMessages();
                rp.Data = messages;
                rp.Status = true;
            }
            catch (Exception)
            {
                rp.Status = false;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ReportAbuse(Feedback model)
        {
            var rp = new AjaxResponse();

            try
            {
                var res = this.GoogleCaptchaValidate(model.Captcha);
                if (!res)
                {
                    rp.Status = false;
                    rp.Message = Core.Resources.Message.Captcha_Error;
                }
                else
                {
                    model.IP = this.ClientIP;
                    model.ClientId = this.ClientId();
                    model.ProfileId = (this.IsAuthenticated()) ? this.UserData.ProfileId : 0;
                    model.Created = DateTime.Now;
                    model.ProcessedDate = null;
                    var isReported = _uow.Feedback.IsReported(model.PropertyId, model.ProfileId, model.ClientId);
                    if (!isReported)
                    {
                        _uow.Feedback.Add(model);
                        _uow.Commit();
                        rp.Data = model;
                    }
                    rp.Status = true;
                }
            }
            catch (Exception)
            {
                rp.Status = false;
            }


            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankInterestRates()
        {
            var rp = new AjaxResponse();
            try
            {

                var data = _uow.Loan.GetLoanForDetailPage();
                rp.Data = data;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Message = ex.Message;
                rp.Status = true;

            };
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}