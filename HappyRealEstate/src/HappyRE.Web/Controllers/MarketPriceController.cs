using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using HappyRE.Core.MapModels.Report;
using HappyRE.Web.Models;
using HappyRE.Core.MapModels;
using HappyRE.Core.Resources;
using HappyRE.Core.MapModels.SEO;
using HappyRE.Core.BLL.Repositories;

namespace HappyRE.Web.Controllers
{
    public class MarketPriceController : BaseController
    {
        public MarketPriceController(IUow uow) : base(uow)
        {
            ViewBag.Menu_ActiveCode = "menu-house-price";
        }

        // GET: MarketPrice
        public ActionResult Search()
        {
            return View();
        }
        public ActionResult MarketList()
        {
            List<DistrictHousePrice> data = _uow.MogiReport.GetHousePrice_District();
            MarketListViewModel model = new MarketListViewModel();
            model.Hanoi = data.Where(x => x.CityId == 24).ToList();
            model.HCM = data.Where(x => x.CityId == 30).ToList();

            //SEO
            ViewBag.Canonical = Core.Utils.Common.HomeUrl + Message.Routing_Market_Price;
            var seoObj = _uow.SEO_Page.SEO_ListingHousePrice();

            if (seoObj != null)
            {
                this.SetSEOTag(seoObj);
            }

            return View(model);
        }
        /// <summary>
        /// Lấy danh sách đường bởi district
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetByDistrict(int districtId)
        {
            var rp = new AjaxResponse();
            try
            {
                var obj = _uow.MogiReport.GetHousePrice_StreetBy_District(districtId);
                //foreach (var item in obj)
                //{
                //    item.BuilUrl();
                //}
                rp.Data = obj;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail_V2(HousingAvgPriceQuery query)
        {
            if (query != null && (query.StreetId == 0 || query.DistrictId == 0))
            {
                return RedirectToAction("MarketList");
            }

            List<HousePrice> data = new List<HousePrice>();
            List<HousePrice> items = new List<HousePrice>();
            string location = string.Empty;

            var streetName = "";
            var districtFullName = "";
            var objDistrict = _uow.City.Get(query.DistrictId);
            if (objDistrict == null)
            {
                objDistrict = new Core.Models.City();
            }

            var objStreet = _uow.Street.Get(query.StreetId);
            Core.Models.StreetName objStreetName = _uow.StreetName.GetByStreetId(query.StreetId);
            string codeUrl = "";
            if (objStreetName != null)
            {
                streetName = objStreetName.Name;
                districtFullName = _uow.City.GetFullName(query.DistrictId, ",");
                location = string.Format("{0}, {1}", streetName, districtFullName);
                codeUrl = _uow.Property.FriendlyUrl_Street(query.StreetId);
            }
            string streetCodeUrl = (objStreet == null ? "duong" : objStreet.CodeUrl);
            string canonical = string.Format(Message.Routing_MarketPrice_Detail, streetCodeUrl, query.DistrictId, query.StreetId);

            if (this.Request.Url.AbsolutePath.Contains(canonical) == false)
            {
                return Redirect(Core.Utils.Common.HomeUrl + canonical);
            }

            data = _uow.MogiReport.GetHousePrice_Detail(query.DistrictId,query.WardId, query.StreetId);
            if (data.Count > 0)
            {
                var objCity = _uow.City.Get(objDistrict.ParentId);
                items = data.GroupBy(c => c.PropertyStyle).Select(c => c.LastOrDefault()).ToList();

                string title = Resources.Message.MarketPrice_Detail_Title;
                foreach (var item in items)
                {
                    if (objStreetName != null)
                    {
                        title = string.Format(Resources.Message.MarketPrice_Detail_Title, item.PropertyStyle, objStreetName.Name);
                    }
                    var objPropStyle = _uow.PropertyType.Get(item.PropertyStyleId);
                    item.FrontEnd_SearchUrl = _uow.Property.FriendlyUrl(false, codeUrl, objCity, objDistrict, objPropStyle);
                    item.TitleDisplay = title;
                }
            }

            ViewBag.Months = this.GetMonth();
            ViewBag.Items = items;
            ViewBag.Location = location;
            ViewBag.CityId = objDistrict.ParentId;
            ViewBag.DistrictId = objDistrict.CityId;
            ViewBag.StreetId = query.StreetId;
            ViewBag.LocationUrl = Core.Utils.Common.ToFriendllyUrl(location);
            ViewBag.HistoryPriceTitle = string.Format(Resources.Message.MarketPrice_Property_Title, location);
            ViewBag.Canonical = Core.Utils.Common.HomeUrl + canonical;
            //SEO
            var seoObj = _uow.SEO_Page.SEO_DetailHousePrice(streetName, districtFullName);
            if (seoObj != null)
            {
                this.SetSEOTag(seoObj);
            }

            return View(data);
        }

        public ActionResult StreetList(HousingAvgPriceQuery query)
        {
            var districtObj = _uow.City.Get(query.DistrictId);
            if (districtObj == null)
            {
                return RedirectToAction("MarketList");
            }
            var cityObj = _uow.City.Get(districtObj.ParentId);

            ViewBag.CityId = districtObj.ParentId;
            ViewBag.DistrictId = query.DistrictId;
            ViewBag.Location = districtObj.FullName;
            ViewBag.DistrictName = districtObj.Name;
            #region Friendly Url
            var requestUrl = string.Format(Message.Routing_MarketPrice_District, districtObj.CodeUrl, districtObj.CityId);

            string urlToBuy = _uow.Property.FriendlyUrl(false, "", cityObj, districtObj);
            #endregion
            //SEO
            var seoObj = _uow.SEO_Page.SEO_LocationHousePrice(districtObj.FullName);
            if (seoObj != null)
            {
                this.SetSEOTag(seoObj);
            }
            ViewBag.Canonical = Core.Utils.Common.HomeUrl + requestUrl;
            ViewBag.UrlToBuy = urlToBuy;
            return View();
        }
        private List<string> GetMonth()
        {
            DateTime d = DateTime.Today;
            d = d.AddMonths(-7);
            List<string> s = new List<string>();
            for (int i = 1; i <= 6; i++)
            {
                var d1 = d.AddMonths(i);
                s.Add(d1.ToString("TM"));
                //s.Add(d1.ToString("MM/yyyy"));
            }
            return s;

        }
        public ActionResult Detail(HousingAvgPriceQuery query)
        {
            int total = 0;

            query.PageSize = int.MaxValue;
            query.PageIndex = 1;

            List<HousePrice> data = new List<HousePrice>();
            if (query.FromMonth == query.ToMonth)
            {
                data = _uow.MogiReport.GetByMonth(out total, query.PageIndex, query.PageSize, query.FromMonth, query.CityId,
                    query.DistrictId,query.WardId, query.StreetId, query.TypeId, query.StypeId, 1, 0, 0);
            }
            else
            {
                data = _uow.MogiReport.GetByRange(out total, query.PageIndex, query.PageSize, query.FromMonth, query.ToMonth, query.CityId,
                    query.DistrictId, query.WardId, query.StreetId, query.TypeId, query.StypeId, 1, 0, 0);
            }


            var objCity = _uow.City.Get(query.CityId);
            var objDistrict = _uow.City.Get(query.DistrictId);
            // Add URL to Listing Page
            foreach (var item in data)
            {
                var objPropStyle = _uow.PropertyType.Get(item.PropertyStyleId);
                string codeUrl = _uow.Property.FriendlyUrl_Street(item.StreetId);
                item.FrontEnd_SearchUrl = _uow.Property.FriendlyUrl(false, codeUrl, objCity, objDistrict, objPropStyle);
            }

            // Build Title Page
            var months = query.ToMonth - query.FromMonth + 1;
            if (data != null && data.Count > 0)
            {
                ViewBag.Title = string.Format(Message.MarKetPrice_TitlePage, months, objDistrict.FullName);
            }
            else
            {
                if (objDistrict != null)
                {
                    ViewBag.Title = string.Format(Message.MarKetPrice_TitlePageNoData01, objDistrict.FullName);

                }
                else
                {
                    ViewBag.Title = Message.MarKetPrice_TitlePageNoData;
                }
            }

            ViewBag.CityId = query.CityId;
            ViewBag.DistrictId = query.DistrictId;

            return View(data);
        }

        public JsonResult GetHousingAvgPrices(HousingAvgPriceQuery query)
        {
            var rp = new AjaxResponse();
            try
            {
                int total = 0;
                List<HousePrice> data = new List<HousePrice>();
                if (query.FromMonth == query.ToMonth)
                {
                    data = _uow.MogiReport.GetByMonth(out total, query.PageIndex, query.PageSize, query.FromMonth, query.CityId,
                        query.DistrictId,query.WardId, query.StreetId, query.TypeId, query.StypeId, 1, 0, 0);
                }
                else
                {
                    data = _uow.MogiReport.GetByRange(out total, query.PageIndex, query.PageSize, query.FromMonth, query.ToMonth, query.CityId,
                        query.DistrictId, query.WardId, query.StreetId, query.TypeId, query.StypeId, 1, 0, 0);

                }

                var model = new HousingAvgPriceViewModel
                {
                    Total = total,
                    Properties = data
                };

                rp.Data = model;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        private void SetSEOTag(SEO_MetaPage seo)
        {
            ViewBag.TitlePage = seo.TitlePage;
            ViewBag.Title = seo.Title;
            ViewBag.Description = seo.Description;
            ViewBag.PageDescription = seo.PageDescription;
            ViewBag.Keywords = seo.MetaKeywords;
            ViewBag.Footer = seo.Footer;
            //  ViewBag.Canonical = seo.Alternate;
        }

        #region Lich Su Gia

        public ActionResult DetailHistory(string id, int d = 0)
        {
            var objs = _uow.HousePrice.mspHousePrice_GetPropertyByUniqueAddress(id);
            ViewBag.Objs = objs;
            Core.MapModels.MogiHousePrice.PropertyHPDetailModel lstObj;
            if (d == 0) lstObj = objs.OrderByDescending(x => x.EndDate).FirstOrDefault();
            else lstObj = _uow.HousePrice.mspHousePrice_GetPropertyById(d);
            ViewBag.LastHistory = lstObj;
            var time = objs.GroupBy(t => t.EndDate.Year)
                           .Select(grp => grp.First())
                           .ToList().OrderByDescending(x => x.EndDate.Year).ToList();
            ViewBag.TimeLine = time;

            string location = string.Empty;
            Core.Models.StreetName street = _uow.StreetName.GetByStreetId(lstObj.StreetId);
            Core.MapModels.LinkItem node = new LinkItem();
            if (street != null)
            {
                string streetName = street.Name;
                string districtFullName = _uow.City.GetFullName(lstObj.DistrictId, ", ");
                location = string.Format("{0}, {1}", streetName, districtFullName);
                var objDistrict = _uow.City.Get(lstObj.DistrictId);
                var objStreet = _uow.Street.Get(lstObj.StreetId);
                ViewBag.NodeUrl = $"/gia-nha-dat-{objStreet.CodeUrl}-d{lstObj.DistrictId}-s{lstObj.StreetId}";
                ViewBag.NodeName = location;
                ViewBag.Node = node;
            }
            ViewBag.Location = string.Format("{0} {1} {2}", lstObj.PropertyTypeName.ToLower(), lstObj.Address, location);
            ViewBag.TitleProperty = string.Format("Lịch sử giá {0} {1} {2} {3}", lstObj.PropertyTypeName.ToLower(), lstObj.Address, location, lstObj.EndDate.ToString("dd/MM/yyyy"));
            ViewBag.TitlePage = string.Format("{0} {1} {2} {3} {4} {5}", Resources.Message.MarketPrice_History, lstObj.PropertyTypeName.ToLower(), lstObj.Address, location, Resources.Message.MarketPrice_History_Time, lstObj.EndDate.ToString("dd/MM/yyyy"));

            //SEO
            var seoObj = _uow.SEO_Page.SEO_HistoryHousePrice(lstObj.PropertyTypeName, location, lstObj.EndDate.ToString("dd/MM/yyyy"));
            if (seoObj != null)
            {
                this.SetSEOTag(seoObj);
            }

            return View();

        }
        /// <summary>
        /// Lấy danh sách bất động sản duy nhất theo đường
        /// </summary>
        /// <param name="streetId"></param>
        /// <returns></returns>
        public JsonResult GetPropertyByStreet(int streetId, int p)
        {
            var rp = new AjaxResponse();
            try
            {
                int total = 0;
                var objs = _uow.HousePrice.mspHousePrice_GetPropertyByStreet(out total, p, 10, streetId);
                rp.Data = new { Data = objs, Total = total };
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

		[OutputCache(Duration = CACHE_ONE_HOUR)]
		public JsonResult GetHistoryPropertyByStreet(int streetId, int fromArea, int toArea)
        {
            var rp = new AjaxResponse();
            try
            {
                var objs = _uow.HousePrice.mspHousePrice_GetPropertyByStreetAndArea(streetId, fromArea, toArea);
                rp.Data = objs;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Tham khao giam - new version
        public ActionResult Detail_V3(HousingAvgPriceQuery query)
        {
            if (query != null && (query.StreetId == 0 || query.DistrictId == 0))
            {
                return RedirectToAction("MarketList");
            }

            List<HousePrice> data = new List<HousePrice>();
            List<HousePrice> items = new List<HousePrice>();
            string location = string.Empty;

            var streetName = "";
            var districtFullName = "";

            var objDistrict = _uow.City.Get(query.DistrictId);
            if (objDistrict == null)
            {
                objDistrict = new Core.Models.City();
            }
            var objCity = _uow.City.Get(objDistrict.ParentId);

            var objStreet = _uow.Street.Get(query.StreetId);
            Core.Models.StreetName objStreetName = _uow.StreetName.GetByStreetId(query.StreetId);
            string codeUrl = "";
            if (objStreetName != null)
            {
                streetName = objStreetName.Name;
                districtFullName = _uow.City.GetFullName(query.DistrictId, ",");
                location = string.Format("{0}, {1}", streetName, districtFullName);
                codeUrl = _uow.Property.FriendlyUrl_Street(query.StreetId);
            }
            string streetCodeUrl = (objStreet == null ? "duong" : objStreet.CodeUrl);
            string canonical = string.Format(Message.Routing_MarketPrice_Detail, streetCodeUrl, query.DistrictId, query.StreetId);
            string urlToBuy = _uow.Property.FriendlyUrl(false, codeUrl, objCity, objDistrict, null);
            if (this.Request.Url.AbsolutePath.Contains(canonical) == false)
            {
                return Redirect(Core.Utils.Common.HomeUrl + canonical);
            }

            ViewBag.Months = this.GetMonth();
            ViewBag.Items = items;
            ViewBag.Location = location;
            ViewBag.CityId = objDistrict.ParentId;
            ViewBag.DistrictId = objDistrict.CityId;
            ViewBag.StreetId = query.StreetId;
            ViewBag.LocationUrl = Core.Utils.Common.ToFriendllyUrl(location);
            ViewBag.HistoryPriceTitle = string.Format(Resources.Message.MarketPrice_Property_Title, location);
            ViewBag.Canonical = Core.Utils.Common.HomeUrl + canonical;
            ViewBag.UrlToBuy = urlToBuy;
            //SEO
            var seoObj = _uow.SEO_Page.SEO_DetailHousePrice(streetName, districtFullName);
            if (seoObj != null)
            {
                this.SetSEOTag(seoObj);
            }

            var k = _uow.MogiReport.msp_Report_GetLatestAprovedTime();
            ViewBag.LatestApproved = DateTime.ParseExact(k.ToString(), "yyyyMM", null).ToString("yyyy-MM-dd");
            ViewBag.LatestApprovedDisPlay = DateTime.ParseExact(k.ToString(), "yyyyMM", null).ToString("MM/yyyy");
            return View();
        }
        public JsonResult GetHousePriceSummary_ByStreet(int districtId, int streetId, int? month = 1)
        {
            var rp = new AjaxResponse();
            try
            {
                var data = _uow.MogiReport.GetHousePrice_Street_Summary(districtId, streetId, month.Value);
                rp.Data = data;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHousePriceSummary_ByDistrict(int districtId, int? month = 3)
        {
            var rp = new AjaxResponse();
            try
            {
                var data = _uow.MogiReport.GetHousePrice_District_Summary(districtId, month.Value);
                rp.Data = data;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHousePriceMonthly_ByStreet(int districtId, int streetId, int? month = 24)
        {
            var rp = new AjaxResponse();
            try
            {
                var data = _uow.MogiReport.GetHousePrice_Street_Monthly(districtId, streetId, month.Value);
                rp.Data = data;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHousePrice_TopBy_AvgPrice(int districtId)
        {
            var rp = new AjaxResponse();
            try
            {
                var data = _uow.MogiReport.GetHousePrice_TopBy_AvgPrice(districtId);
                rp.Data = data;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHousePrice_TopBy_Total(int districtId)
        {
            var rp = new AjaxResponse();
            try
            {
                var data = _uow.MogiReport.GetHousePrice_TopBy_Total(districtId);
                rp.Data = data;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHousePrice_Street_vs_District(int districtId, int streetId)
        {
            var rp = new AjaxResponse();
            try
            {
                var data = _uow.MogiReport.GetHousePrice_Street_vs_District(districtId, streetId);
                rp.Data = data;
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Status = false;
                rp.Message = ex.Message;

            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PropertySummaryByPriceRange(ReportChartFilter data)
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                rp.Data = _uow.MogiReport.msp_Report_PropertySummaryByPriceRange(data);
                rp.Status = true;
            }
            catch (Core.BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception ex)
            {
                rp.Message = HappyRE.Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}