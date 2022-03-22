using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MBN.Utils;
using MBN.Utils.Extension;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.Entities;
using MBN.Utils.Caching;
using HappyRE.Core.MapModels.Report;
using HappyRE.Core.MapModels.FrontEnd;
using HappyRE.Core.MapModels;
using HappyRE.Web.Models;

namespace HappyRE.Web.Controllers
{
    public class CachePartialController : BaseController
    {
        public CachePartialController(IUow uow) : base(uow) { }

        #region TopMenu
        [ChildActionOnly]
        //[OutputCache(CacheProfile = "Cache1Day")]
        public ActionResult TopMenu(string code, bool IsAuthenticated = false)
        {
            ViewBag.Code = (code ?? string.Empty);

            // Language
            Language lang = this.GetLanguage(Core.Utils.Common.IsUS() ? Core.Const.LANG_VN_ID : Core.Const.LANG_US_ID);
            ViewBag.LangUrl = (lang == null ? "/" : lang.HomeUrl);

            var model = _uow.CMSCategory.GetByGroupId(Core.Const.CMS_GROUP_MENU);

            return PartialView("_TopMenu", model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = CACHE_ONE_DAY)]
        public ActionResult SlideMenu(bool isMember = false, string code = "")
        {
            var model = _uow.CMSCategory.GetByGroupId(Core.Const.CMS_GROUP_MENU);
            return PartialView("_SlideMenu", model);
        }
        #endregion

        [ChildActionOnly]
        public ActionResult AccountInfo()
        {
            //ViewBag.Code = (code ?? string.Empty);
            //var lst = _uow.CMSCategory.GetByGroupId(Core.Const.CMS_GROUP_MENU);
            return PartialView("_AccountInfo", null);
        }

        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_DAY)]
        public ActionResult BreadCrumb1(string pageName, int cityId, int categoryId, long minPrice, long maxPrice, string tagName)
        {
            //ViewBag.IsDetail = (pageName == "detail");

            //var lst = BLL.Utils.Common.GetBreadCrumb(cityId, categoryId, minPrice, maxPrice, tagName);
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", null);
        }

        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult Footer()
        {
            Models.FooterViewModel model = new Models.FooterViewModel();
            model.Footer = _uow.CMSCategory.GetByGroupId(Core.Const.CMS_GROUP_FOOTER, true);
            model.Popular = _uow.CMSCategory.GetByGroupId(Core.Const.CMS_GROUP_POPULAR, true);

            return PartialView("_Footer", model);
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb(Core.MapModels.SearchFilter filter)
        {
            Models.Breadcrumb obj = Models.Breadcrumb.MapObject(filter);

            obj.ShowAlertSearch = true;

            this.GetBreadcrumb(obj);

            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb_Detail(Core.MapModels.FrontEnd.PropertyDisplay objProp)
        {
            Models.Breadcrumb obj = Models.Breadcrumb.MapObject(objProp);
            this.GetBreadcrumb(obj);
            obj.Items.Add(new LinkItem() { Id = 0, Name = objProp.Title });
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb_AgentList(string keyWord, int cityId = 0)
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetAgentBreadcrumb(keyWord, cityId); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb_AgentDetail(string name)
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetAgentBreadcrumb(name, 0); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }


        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult SearchSimilar(bool rent, int cityId = 0, int districtId = 0, int propertyTypeId = 0, int propertyStyleId = 0, string gtmCat = "")
        {
            Core.MapModels.Search.SearchSimilarResult res = _uow.Property.FrontEnd_SearchSimilar(rent, cityId, districtId,0, propertyTypeId, propertyStyleId);
            res.GtmCat = gtmCat;
            return PartialView("~/Views/Property/SearchSimilar.cshtml", res);
        }

        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult SearchRefined(bool rent, int cityId = 0, int districtId = 0, int propertyTypeId = 0, int propertyStyleId = 0, string gtmCat = "")
        {
            Core.MapModels.Search.SearchSimilarResult res = _uow.Property.FrontEnd_SearchSimilar(rent, cityId, districtId,0, propertyTypeId, propertyStyleId);
            res.GtmCat = gtmCat;
            return PartialView("~/Views/Property/SearchRefined.cshtml", res);
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb_ProjectList(string keyWord, int cityId = 0, int wardId = 0, int streetId = 0, int orgId = 0, int projectId = 0)
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetProjectBreadcrumb(keyWord, cityId, wardId, streetId, orgId, projectId);
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb_Project(string keyWord, int cityId, int projectId)
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetProjectBreadcrumb(keyWord, cityId, 0, 0, 0, projectId);
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb_BuyingGuide()
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();
            obj.Items = this.GetBuyingGuideBreadcrumb();

            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb_MarketPrice()
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetMarketPriceBreadcrumb(); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }
        [ChildActionOnly]
        public ActionResult Breadcrumb_MarketPriceDetail(int districtId, string location)
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetMarketPriceDetailBreadcrumb(districtId, location); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb_MarketPriceStreet(string location)
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetMarketPriceDetailBreadcrumb(0, location); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }
        [ChildActionOnly]
        public ActionResult Breadcrumb_MarketPriceDetailHistory(string location, string url, string name)
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetMarketPriceDetailBreadcrumbHistory(location, url, name); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        public ActionResult Breadcrumb_StudentRentHouse()
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetStudentRentHouseBreadcrumb(); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }
        public ActionResult Breadcrumb_IndustrialParkRentHouse()
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetIndustrialParkRentHouseBreadcrumb(); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        public ActionResult Breadcrumb_WardReviews()
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetWardReviewsBreadcrumb(); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }

        public ActionResult Breadcrumb_WardReviews_District(int cid = 0)
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetWardReviewsBreadcrumbDistrict(cid);
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }
        public ActionResult Breadcrumb_WardReviews_Wards(int did = 0)
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetWardReviewsBreadcrumbWards(did);
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }
        public ActionResult Breadcrumb_WardReview(int did = 0,string ward = "")
        {
            Models.Breadcrumb obj = new Models.Breadcrumb();

            obj.Items = this.GetWardReviewBreadcrumbs(did, ward); ;
            return PartialView("~/Views/Shared/Partials/_TopBreadcrumb.cshtml", obj);
        }
        #region Breadcrumb

        #region GetBreadcrumb_BAK_20180822
        private List<LinkItem> GetBreadcrumb_BAK_20180822(Models.Breadcrumb data)
        {
            City objCity = _uow.City.Get(data.CityId);
            City objDistrict = _uow.City.Get(data.DistrictId);
            PropertyType objPropType = _uow.PropertyType.Get(data.PropertyTypeId);
            List<PropertyType> lstPropStyle = new List<PropertyType>();
            if (data.PropertyStyles != null && data.PropertyStyles.Count != 0)
            {
                for (int i = 0; i < data.PropertyStyles.Count && i < 20; i++)
                {
                    var objStyle = _uow.PropertyType.Get(data.PropertyStyles[i]);
                    if (objStyle != null) lstPropStyle.Add(objStyle);
                }
            }
            Ward objWard = null;
            Street objStreet = null;
            Project objProject = null;
            if (data.WardId > 0) objWard = _uow.Ward.Get(data.WardId);
            if (data.StreetId > 0) objStreet = _uow.Street.Get(data.StreetId);
            if (data.ProjectId > 0) objProject = _uow.Project.Get(data.ProjectId);

            string description = string.Empty;
            string url = "", codeUrl = "", q = "", propStyles = "";
            try
            {
                data.Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });

                // TransType
                if (data.Rent == true)
                {
                    url = "/" + Core.Resources.Message.Routing_Rent_CodeUrl + "-" + Core.Resources.Message.Routing_PropertyType_All;
                    data.Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_003, Url = url });
                }
                else
                {
                    url = "/" + Core.Resources.Message.Routing_Buy_CodeUrl + "-" + Core.Resources.Message.Routing_PropertyType_All;
                    data.Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_002, Url = url });
                }

                // City
                if (objCity != null)
                {
                    url = _uow.Property.FriendlyUrl(data.Rent, string.Empty, objCity, null, null);
                    data.Items.Add(new LinkItem() { Id = objCity.CityId, Name = objCity.Name, Url = url });
                }

                // District
                if (objDistrict != null)
                {
                    url = _uow.Property.FriendlyUrl(data.Rent, string.Empty, objCity, objDistrict, null);
                    data.Items.Add(new LinkItem() { Id = objDistrict.CityId, Name = objDistrict.Name, Url = url });
                }

                // Project
                if (objProject != null)
                {
                    codeUrl = _uow.Property.FriendlyUrl_Project(objProject.ProjectId);
                    url = _uow.Property.FriendlyUrl(data.Rent, codeUrl, objCity, objDistrict, null);
                    data.Items.Add(new LinkItem() { Id = objProject.ProjectId, Name = objProject.Name, Url = url });
                }
                else if (objStreet != null) // Street
                {
                    var objStreetName = _uow.StreetName.Get(objStreet.StreetNameId);
                    if (objStreetName != null)
                    {
                        string streetName = (objStreetName == null ? "" : objStreetName.Name);
                        codeUrl = _uow.Property.FriendlyUrl_Street(objStreet.StreetId);
                        url = _uow.Property.FriendlyUrl(data.Rent, codeUrl, objCity, objDistrict, null);
                        data.Items.Add(new LinkItem() { Id = objStreet.StreetId, Name = streetName, Url = url });
                    }
                }
                else if (objWard != null) // Ward
                {
                    codeUrl = _uow.Property.FriendlyUrl_Ward(objWard.WardId);
                    url = _uow.Property.FriendlyUrl(data.Rent, codeUrl, objCity, objDistrict, null);
                    data.Items.Add(new LinkItem() { Id = objWard.WardId, Name = objWard.Name, Url = url });
                }

                // PropertyType
                if (lstPropStyle != null && lstPropStyle.Count > 0)
                {
                    if (lstPropStyle.Count == 1)
                    {
                        objPropType = lstPropStyle[0];
                    }
                    else
                    {
                        foreach (var item in lstPropStyle)
                        {
                            q += (q == "" ? "" : "&") + "psid=" + item.PropertyTypeId;
                            propStyles += (propStyles == "" ? "" : "/") + item.Name;
                        }
                    }
                }

                if (objPropType != null)
                {
                    url = _uow.Property.FriendlyUrl(data.Rent, codeUrl, objCity, objDistrict, objPropType);
                    url += (q == "" ? "" : "?") + q;
                    data.Items.Add(new LinkItem() { Id = objPropType.PropertyTypeId, Name = objPropType.FullName + (propStyles == "" ? "" : " (" + propStyles + ")"), Url = url });
                }

                // Price
                if (data.HasPrice() == true)
                {
                    description += (description == "" ? "" : ", ") + data.GetPriceText();
                }

                // Area
                if (data.HasArea() == true)
                {
                    description += (description == "" ? "" : ", ") + data.GetAreaText();
                }

                // bed
                if (data.HasBedRoom() == true)
                {
                    description += (description == "" ? "" : ", ") + data.GetBedRoomText();
                }

                // Direction
                if (data.DirectionId > 0)
                {
                    var objDirection = _uow.RE_Direction.Get(data.DirectionId);
                    if (objDirection != null)
                    {
                        description += (description == "" ? "" : ", ") + Core.Resources.Model.Property_DirectionId + " " + objDirection.Name;
                        //description += (description == "" ? "" : ", ") + objDirection.Name;
                    }
                }

                // Legal
                if (data.LegalId > 0)
                {
                    var objLegal = _uow.Legal.Get(data.LegalId);
                    if (objLegal != null)
                    {
                        //description += (description == "" ? "" : ", ") + Core.Resources.Model.Property_LegalId + ": " + objLegal.Name;
                        description += (description == "" ? "" : ", ") + objLegal.Name;
                    }
                }

                // Time
                if (data.Days > 0)
                {
                    description += (description == "" ? "" : ", ") + string.Format(Core.Resources.Message.Breadcrumb_Days, data.Days);
                }

                if (string.IsNullOrEmpty(description) == false)
                {
                    data.Items.Add(new LinkItem() { Name = description });
                }
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetBreadcrumb: " + data.ToJson(), ex);
                throw ex;
            }

            return data.Items;
        }
        #endregion

        private List<LinkItem> GetBreadcrumb(Models.Breadcrumb data)
        {
            City objCity = _uow.City.Get(data.CityId);
            City objDistrict = _uow.City.Get(data.DistrictId);
            PropertyType objPropType = _uow.PropertyType.Get(data.PropertyTypeId);
            if (data.PropertyStyles != null && data.PropertyStyles.Count != 0)
            {
                var objStyle = _uow.PropertyType.Get(data.PropertyStyles[0]);
                if (objStyle != null)
                {
                    objPropType = objStyle;
                }
            }
            Ward objWard = null;
            Street objStreet = null;
            Project objProject = null;
            Place objPlace = null;
            if (data.WardId > 0) objWard = _uow.Ward.Get(data.WardId);
            if (data.StreetId > 0) objStreet = _uow.Street.Get(data.StreetId);
            if (data.ProjectId > 0) objProject = _uow.Project.Get(data.ProjectId);
            if (data.PlaceId > 0) objPlace = _uow.Place.Get(data.PlaceId);

            string description = string.Empty;
            string url = "", codeUrl = "", propTypeName = "", propTypeAll = "", propStyle = "";
            try
            {
                data.Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });

                // TransType
                if (data.Rent == true)
                {
                    propTypeAll = Core.Resources.Message.Breadcrumb_PropType_Rent_All;
                    propStyle = Core.Resources.Message.Breadcrumb_PropStyle_Rent;
                    url = "/" + Core.Resources.Message.Routing_Rent_CodeUrl + "-" + Core.Resources.Message.Routing_PropertyType_All;
                    data.Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_003, Url = url });
                }
                else
                {
                    propTypeAll = Core.Resources.Message.Breadcrumb_PropType_Sale_All;
                    propStyle = Core.Resources.Message.Breadcrumb_PropStyle_Sale;
                    url = "/" + Core.Resources.Message.Routing_Buy_CodeUrl + "-" + Core.Resources.Message.Routing_PropertyType_All;
                    data.Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_002, Url = url });
                }
                propTypeName = (objPropType == null ? propTypeAll : string.Format(propStyle, objPropType.FullName));

                bool add_district = true, add_city = true;

                // Project
                if (objProject != null)
                {
                    codeUrl = _uow.Property.FriendlyUrl_Project(objProject.ProjectId);
                    url = _uow.Property.FriendlyUrl(data.Rent, codeUrl, objCity, objDistrict, objPropType);
                    data.Items.Add(new LinkItem() { Id = objProject.ProjectId, Name = propTypeName + " " + objProject.Name, Url = url });
                }
                else if (objPlace != null) // Place
                {
                    codeUrl = _uow.Property.FriendlyUrl_Place(objPlace.PlaceId);
                    url = _uow.Property.FriendlyUrl(data.Rent, codeUrl, objCity, objDistrict, objPropType);
                    data.Items.Add(new LinkItem() { Id = objPlace.PlaceId, Name = propTypeName + " " + objPlace.Name, Url = url });
                }
                else if (objStreet != null) // Street
                {
                    var objStreetName = _uow.StreetName.Get(objStreet.StreetNameId);
                    if (objStreetName != null)
                    {
                        string streetName = (objStreetName == null ? "" : objStreetName.Name);
                        codeUrl = _uow.Property.FriendlyUrl_Street(objStreet.StreetId);
                        url = _uow.Property.FriendlyUrl(data.Rent, codeUrl, objCity, objDistrict, objPropType);
                        data.Items.Add(new LinkItem() { Id = objStreet.StreetId, Name = propTypeName + " " + streetName, Url = url });
                    }
                }
                else if (objWard != null) // Ward
                {
                    codeUrl = _uow.Property.FriendlyUrl_Ward(objWard.WardId);
                    url = _uow.Property.FriendlyUrl(data.Rent, codeUrl, objCity, objDistrict, objPropType);
                    data.Items.Add(new LinkItem() { Id = objWard.WardId, Name = propTypeName + " " + objWard.Name, Url = url });
                }
                else if (objDistrict != null) // District
                {
                    url = _uow.Property.FriendlyUrl(data.Rent, string.Empty, objCity, objDistrict, objPropType);
                    data.Items.Add(new LinkItem() { Id = objDistrict.CityId, Name = propTypeName + " " + objDistrict.Name, Url = url });
                    add_district = false;
                }
                else if (objCity != null) // City
                {
                    url = _uow.Property.FriendlyUrl(data.Rent, string.Empty, objCity, null, objPropType);
                    data.Items.Add(new LinkItem() { Id = objCity.CityId, Name = propTypeName + " " + objCity.Name, Url = url });
                    add_city = false;
                }
                else if (objPropType != null) // Toàn quốc và có Loại BĐS hoặc Kiểu BĐS
                {
                    url = _uow.Property.FriendlyUrl(data.Rent, string.Empty, null, null, objPropType);
                    data.Items.Add(new LinkItem() { Id = 0, Name = propTypeName, Url = url });
                }

                // City
                if (add_city == true && objCity != null)
                {
                    url = _uow.Property.FriendlyUrl(data.Rent, string.Empty, objCity, null, objPropType);
                    data.Items.Insert(data.Items.Count - 1, new LinkItem() { Id = objCity.CityId, Name = objCity.Name, Url = url });
                }

                // District
                if (add_district == true && objDistrict != null)
                {
                    url = _uow.Property.FriendlyUrl(data.Rent, string.Empty, objCity, objDistrict, objPropType);
                    data.Items.Insert(data.Items.Count - 1, new LinkItem() { Id = objDistrict.CityId, Name = objDistrict.Name, Url = url });
                }

                // Price
                if (data.HasPrice() == true)
                {
                    description += (description == "" ? "" : ", ") + data.GetPriceText();
                }

                // Area
                if (data.HasArea() == true)
                {
                    description += (description == "" ? "" : ", ") + data.GetAreaText();
                }

                // bed
                if (data.HasBedRoom() == true)
                {
                    description += (description == "" ? "" : ", ") + data.GetBedRoomText();
                }

                // Direction
                if (data.DirectionId > 0)
                {
                    var objDirection = _uow.RE_Direction.Get(data.DirectionId);
                    if (objDirection != null)
                    {
                        description += (description == "" ? "" : ", ") + Core.Resources.Model.Property_DirectionId + " " + objDirection.Name;
                        //description += (description == "" ? "" : ", ") + objDirection.Name;
                    }
                }

                // Legal
                if (data.LegalId > 0)
                {
                    var objLegal = _uow.Legal.Get(data.LegalId);
                    if (objLegal != null)
                    {
                        //description += (description == "" ? "" : ", ") + Core.Resources.Model.Property_LegalId + ": " + objLegal.Name;
                        description += (description == "" ? "" : ", ") + objLegal.Name;
                    }
                }

                // Time
                if (data.Days > 0)
                {
                    description += (description == "" ? "" : ", ") + string.Format(Core.Resources.Message.Breadcrumb_Days, data.Days);
                }

                if (string.IsNullOrEmpty(description) == false)
                {
                    data.Items.Add(new LinkItem() { Name = description });
                }
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetBreadcrumb: " + data.ToJson(), ex);
                throw ex;
            }

            return data.Items;
        }

        private List<LinkItem> GetAgentBreadcrumb(string keyword, int cityId = 0)
        {

            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_004, Url = Core.Resources.Message.Routing_Finding_Agent });
                if (cityId != 0)
                {
                    var cityObj = _uow.City.Get(cityId);
                    if (cityObj != null)
                    {
                        string url = string.Format(Core.Resources.Message.Routing_Finding_Agent1, cityObj.CodeUrl, cityId);
                        string name = string.Format(Core.Resources.Message.Breadcrumb_Agent_001, cityObj.Name);
                        Items.Add(new LinkItem() { Id = 0, Name = name, Url = url });
                    }
                }
                else if (string.IsNullOrEmpty(keyword) == false)
                {
                    Items.Add(new LinkItem() { Id = 0, Name = keyword, Url = "/" });
                }
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetAgentBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }

        private List<LinkItem> GetProjectBreadcrumb(string keyword, int cityId, int wardId = 0, int streetId = 0, int orgId = 0, int projectId = 0)
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_006, Url = Core.Resources.Message.Routing_Finding_Project });

                string url, name;
                bool add_city = true, add_district = true;
                City objCity = null, objDistrict = null;
                Ward objWard = null;
                Street objStreet = null;
                Org objOrg = null;
                Map objMap = null;
                if (wardId > 0)
                {
                    objWard = _uow.Ward.Get(wardId);
                    if (objWard != null) cityId = objWard.DistrictId;
                }
                if (streetId > 0)
                {
                    objStreet = _uow.Street.Get(streetId);
                    if (objStreet != null) cityId = objStreet.DistrictId;
                }
                if (orgId > 0)
                {
                    objOrg = _uow.Org.Get(orgId);
                    if (objOrg != null) cityId = objOrg.DistrictId;
                }
                if (projectId > 0)
                {
                    objMap = _uow.Map.GetBy(projectId, Core.Const.MAP_REFERTYPE_PROJECT);
                    if (objMap != null)
                    {
                        cityId = objMap.DistrictId;
                    }
                }
                if (cityId > 0)
                {
                    objCity = objDistrict = _uow.City.Get(cityId);
                    if (objDistrict != null)
                    {
                        objCity = _uow.City.Get(objDistrict.ParentId);
                    }
                }

                if (objStreet != null)
                {
                    var objStreetName = _uow.StreetName.GetByStreetId(streetId);
                    url = string.Format(Core.Resources.Message.Routing_Finding_Project_Street, objStreet.CodeUrl, objStreet.StreetId);
                    name = string.Format(Core.Resources.Message.Breadcrumb_Project_001, objStreetName.Name + ", " + objDistrict.Name);
                    Items.Add(new LinkItem() { Id = 0, Name = name, Url = url });
                }
                else if (objWard != null)
                {
                    url = string.Format(Core.Resources.Message.Routing_Finding_Project_Ward, objWard.CodeUrl, objWard.WardId);
                    name = string.Format(Core.Resources.Message.Breadcrumb_Project_001, objWard.Name + ", " + objDistrict.Name);
                    Items.Add(new LinkItem() { Id = 0, Name = name, Url = url });
                }
                else if (objOrg != null)
                {
                    url = string.Format(Core.Resources.Message.Routing_Finding_Project_Org, objOrg.CodeUrl, objOrg.OrgId);
                    name = string.Format(Core.Resources.Message.Breadcrumb_Project_001, objOrg.Name);
                    Items.Add(new LinkItem() { Id = 0, Name = name, Url = url });
                }
                else if (projectId > 0 && string.IsNullOrEmpty(keyword) == false)
                {
                    name = string.Format(Core.Resources.Message.Breadcrumb_Project_001, keyword);
                    Items.Add(new LinkItem() { Id = 0, Name = name, Url = "/" });
                }
                else if (objDistrict != null)
                {
                    url = string.Format(Core.Resources.Message.Routing_Finding_Project_City, objDistrict.CodeUrl, objDistrict.CityId);
                    name = string.Format(Core.Resources.Message.Breadcrumb_Project_001, objDistrict.Name);
                    Items.Add(new LinkItem() { Id = 0, Name = name, Url = url });

                    add_district = false;
                }
                else if (objCity != null)
                {
                    url = string.Format(Core.Resources.Message.Routing_Finding_Project_City, objCity.CodeUrl, objCity.CityId);
                    name = string.Format(Core.Resources.Message.Breadcrumb_Project_001, objCity.Name);
                    Items.Add(new LinkItem() { Id = 0, Name = name, Url = url });

                    add_district = false;
                    add_city = false;
                }

                if (add_city == true && objCity != null)
                {
                    url = string.Format(Core.Resources.Message.Routing_Finding_Project_City, objCity.CodeUrl, objCity.CityId);
                    Items.Insert(Items.Count - 1, new LinkItem() { Id = 0, Name = objCity.Name, Url = url });
                }

                if (add_district == true && objDistrict != null)
                {
                    url = string.Format(Core.Resources.Message.Routing_Finding_Project_City, objDistrict.CodeUrl, objDistrict.CityId);
                    Items.Insert(Items.Count - 1, new LinkItem() { Id = 0, Name = objDistrict.Name, Url = url });
                }


            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetProjectBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }

        private List<LinkItem> GetBuyingGuideBreadcrumb()
        {

            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_008, Url = "" });
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }

        private List<LinkItem> GetMarketPriceBreadcrumb()
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_009, Url = "" });
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }
        private List<LinkItem> GetMarketPriceDetailBreadcrumb(int districtId, string location)
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_009, Url = Core.Resources.Message.Routing_Market_Price });

                if (districtId > 0)
                {
                    var cityObj = _uow.City.Get(districtId);
                    Items.Add(new LinkItem() { Id = 0, Name = cityObj.Name, Url = string.Format(Core.Resources.Message.Routing_MarketPrice_District, cityObj.CodeUrl, cityObj.CityId) });
                }
                Items.Add(new LinkItem() { Id = 0, Name = location, Url = "" });
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }

        private List<LinkItem> GetMarketPriceDetailBreadcrumbHistory(string location, string lsturl, string lstname)
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_009, Url = Core.Resources.Message.Routing_Market_Price });
                Items.Add(new LinkItem() { Id = 0, Name = lstname, Url = lsturl });
                Items.Add(new LinkItem() { Id = 0, Name = location, Url = "" });
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }
        private List<LinkItem> GetStudentRentHouseBreadcrumb()
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_010, Url = Core.Resources.Message.Routing_StudentRentHouse });

            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetStudentRentHouseBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }

        private List<LinkItem> GetIndustrialParkRentHouseBreadcrumb()
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_012, Url = Core.Resources.Message.Routing_IndustrialParkRentHouse });

            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetIndustrialParkRentHouseBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }

        private List<LinkItem> GetWardReviewsBreadcrumb()
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_013, Url = Core.Resources.Message.Routing_WardReview });

            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetIndustrialParkRentHouseBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }
        private List<LinkItem> GetWardReviewsBreadcrumbDistrict(int cid = 0)
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_013, Url = Core.Resources.Message.Routing_WardReview });

                var c = _uow.City.Get(cid);
                if(c != null)
                {
                    string url = string.Format(Core.Resources.Message.Routing_WardReview_DistrictListing, c.CodeUrl, c.CityId);
                    Items.Add(new LinkItem() { Id = 0, Name = c.FullName, Url = url });
                }

            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetIndustrialParkRentHouseBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }
        private List<LinkItem> GetWardReviewsBreadcrumbWards(int did = 0)
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_013, Url = Core.Resources.Message.Routing_WardReview });

                var d = _uow.City.Get(did);
                if (d != null)
                {
                    var c = _uow.City.Get(d.ParentId);
                    if(c!= null)
                    {
                        string curl = string.Format(Core.Resources.Message.Routing_WardReview_DistrictListing, c.CodeUrl, c.CityId);
                        Items.Add(new LinkItem() { Id = 0, Name = c.FullName, Url = curl });
                    }

                    string url = string.Format(Core.Resources.Message.Routing_WardReview_WardListing, d.CodeUrl, d.CityId);
                    Items.Add(new LinkItem() { Id = 0, Name = d.Name, Url = url });
                }

            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetIndustrialParkRentHouseBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }
        private List<LinkItem> GetWardReviewBreadcrumbs(int did = 0,string ward = "")
        {
            List<LinkItem> Items = new List<LinkItem>();
            try
            {
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_001, Url = "/" });
                Items.Add(new LinkItem() { Id = 0, Name = Core.Resources.Message.Breadcrumb_013, Url = Core.Resources.Message.Routing_WardReview });

                var d = _uow.City.Get(did);
                if (d != null)
                {
                    var c = _uow.City.Get(d.ParentId);
                    if (c != null)
                    {
                        string curl = string.Format(Core.Resources.Message.Routing_WardReview_DistrictListing, c.CodeUrl, c.CityId);
                        Items.Add(new LinkItem() { Id = 0, Name = c.FullName, Url = curl });
                    }

                    string url = string.Format(Core.Resources.Message.Routing_WardReview_WardListing, d.CodeUrl, d.CityId);
                    Items.Add(new LinkItem() { Id = 0, Name = d.Name, Url = url });
                }

                Items.Add(new LinkItem() { Id = 0, Name = ward, Url = "" });
            }
            catch (Exception ex)
            {
                WebLog.Log.Error("CachePartialController.GetIndustrialParkRentHouseBreadcrumb: " + Items.ToJson(), ex);
                throw ex;
            }

            return Items;
        }
        #endregion

        [ChildActionOnly]
        public ActionResult SearchProjectSimilar(int cityId)
        {
            Core.MapModels.Search.ProjectSimilarResult res = _uow.Project.FrontEnd_SearchSimilar(cityId);
            return PartialView("~/Views/Project/Partials/_SimilarProject.cshtml", res);
        }

        #region News
        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult Blog()
        {
            List<CMS_News> res = null;
            var objCat = _uow.CMSCategory.Get(Core.Const.CMS_CATEGORY_HOME_FOCUS);
            if (objCat != null)
            {
                res = _uow.CMSNews.GetByCategoryId(objCat.CategoryId, 1, 6);
                ViewBag.LinkMore = objCat.TargetUrl;
            }
            if (res == null) res = new List<CMS_News>();
            return PartialView("_Blog", res);
        }

        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult Blogv2()
        {
            List<CMS_News> res = null;
            var objCat = _uow.CMSCategory.Get(Core.Const.CMS_CATEGORY_HOME_FOCUS);
            if (objCat != null)
            {
                res = _uow.CMSNews.GetByCategoryId(objCat.CategoryId, 1, 6);
                ViewBag.LinkMore = objCat.TargetUrl;
            }
            if (res == null) res = new List<CMS_News>();
            return PartialView("_Blogv2", res);
        }


        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult HomeNews()
        {
            List<CMS_News> res = null;
            var objCat = _uow.CMSCategory.Get(Core.Const.CMS_CATEGORY_HOME_FOCUS);
            if (objCat != null)
            {
                res = _uow.CMSNews.GetByCategoryId(objCat.CategoryId, 1, 6);
                ViewBag.LinkMore = objCat.TargetUrl;
            }
            if (res == null) res = new List<CMS_News>();
            return PartialView("_HomeNews", res);
        }
        #endregion

        #region GoogeMap
        [OutputCache(Duration = CACHE_ONE_DAY)]
        public ActionResult GoogleStaticMap(int cityId, int districtId)
        {
            int serverId = 6;
            var objServer = _uow.MediaServer.Get(serverId);
            string imageUrl = "";
            if (districtId == 0)
            {
                imageUrl = string.Format("{0}/0/city-{1}.png", cityId, cityId);
            }
            else
            {
                imageUrl = string.Format("{0}/{1}/district-{1}.png", cityId, districtId);
            }
            var objCity = _uow.City.Get((districtId == 0 ? cityId : districtId));
            string applicationPath = "";
            if (objServer != null) applicationPath = objServer.ApplicationPath;
            ViewBag.ImageUrl = applicationPath + imageUrl;
            ViewBag.MapTitle = objCity.FullName;

            return PartialView("_GoogleStaticMap");
        }
        #endregion

        #region MarketPrice
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sid">Street Id</param>
        /// <param name="psid">Property Sub-style Id</param>
        /// <param name="cityid"></param>
        /// <param name="did"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        //[ChildActionOnly]
        //[OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult MarketPrice(int sid, int psid, int cityid, int did, string text = "")
        {
            MarketPrices model = null;
            try
            {
                ViewBag.LocationName = string.Empty;
                ViewBag.MarketLink = Core.Resources.Message.Routing_Market_Price;
                //k = "6082_16";
                var k = sid.ToString() + "_" + psid.ToString();
                model = Core.Const.REDIS_PROPERTY_HOUSEPRICE.HashGet<MarketPrices>(k);
                if (model != null)
                {
                    var objType = _uow.PropertyType.Get(psid);
                    var objDistrict = _uow.City.Get(did);

                    // Search link for market history
                    var objStreet = _uow.Street.Get(sid);
                    if (objStreet.CodeUrl != null)
                    {
                        ViewBag.MarketLink = string.Format(Core.Resources.Message.Routing_MarketPrice_Detail, objStreet.CodeUrl, did, sid);
                    }
                    ViewBag.LocationName = $"{objType.FullName}, {objStreet.Name}, {objDistrict.FullName}";

                    return PartialView("~/Views/Property/Partials/_MarketPrice.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                var p = new { sid, psid, cityid, did, text };
                WebLog.Log.Error("MarketPrice: " + p.ToJson(), ex);
                WebLog.Log.Error("MarketPrice: " + model.ToJson(), ex);
                //throw ex;
            }
            return this.Content("");
        }

        /// <summary>
        /// Lấy tham khảo giá trong khu vực (quận, đường, loại bđs)
        /// </summary>
        /// <param name="did"></param>
        /// <param name="sid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult MarketPriceArea(int did, int? sid = 0, int? pid = 1)
        {
            var res = _uow.MogiReport.mspFrontEnd_MarketPrice(did, sid.Value, pid.Value);
            string title = "";
            string url = "";
            if (res.Count > 0)
            {
                var p = res.FirstOrDefault();
                var d = _uow.City.Get(did);
                if (d != null)
                {
                    title = string.Format(HappyRE.Core.Resources.Message.MarketPriceArea_Label, p.PropertyTypeDisplay.ToLower(), d.Name);
                    url = string.Format(Core.Resources.Message.Routing_MarketPrice_District, d.CodeUrl, d.CityId);
                }
                if (sid > 0)
                {
                    var s = _uow.Street.Get(sid.Value);
                    var sn = _uow.StreetName.GetByStreetId(sid.Value);
                    if (s != null && sn != null)
                    {
                        title = string.Format(HappyRE.Core.Resources.Message.MarketPriceOnStreet_Label, p.PropertyTypeDisplay.ToLower(), sn.Name);
                        url = string.Format(HappyRE.Core.Resources.Message.Routing_MarketPrice_Detail, s.CodeUrl, d.CodeUrl, d.CityId, s.StreetId);
                    }
                }
                ViewBag.Title = title;
                ViewBag.MoreUrl = url;
                return PartialView("~/Views/Property/Partials/_MarketPriceArea.cshtml", p);
            }
            return this.Content("");
        }

        #endregion

        #region Inbox Message
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sid">Street Id</param>
        /// <param name="psid">Property Sub-style Id</param>
        /// <returns></returns>
        [ChildActionOnly]
        // [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult Message()
        {
            ViewBag.IsAuthenticated = "false";
            int profileId = 0;
            Guid? clientId = null;
            List<MessageItem> userMessage = null;
            List<MessageItem> userInbox = null;

            if (this.IsAuthenticated() == true)
            {
                var claim = this.UserData;
                profileId = claim.ProfileId;
                ViewBag.IsAuthenticated = "true";
                if (claim.IsMogiPro)
                {
                    userMessage = _uow.InboxMessage.GetLatestForMogiPro();

                }
                else
                {
                    userMessage = _uow.InboxMessage.GetLatestForMember();
                }
                userInbox = _uow.UserInbox.GetLastestMessages(profileId, 5);

            }
            else
            {
                clientId = this.ClientId();
                userMessage = _uow.InboxMessage.GetLatestForAnonymous();
            }

            if (userMessage != null || userInbox != null)
            {
                UserMessages messages = new UserMessages
                {
                    UserInbox = userInbox,
                    UserMessage = userMessage
                };
                return PartialView("~/Views/Profile/Partials/Message.cshtml", messages);
            }
            return new EmptyResult();
        }


        #endregion

        #region Agent
        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult AgentSimilar(int cid)
        {
            Core.MapModels.Search.AgentSimilarResult res = _uow.UserProfile.FrontEnd_SearchSimilar(cid);
            //res.GtmCat = gtmCat;
            return PartialView("~/Views/Agent/AgentSimilar.cshtml", res);
        }
        #endregion

        #region Home
        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult TopHilightProjects(int size = 3)
        {
            var projectFilter = new FrontEndProjectQuery
            {
                PageSize = size,
                cp = 1,

            };
            var projects = _uow.Project.FrontEnd_Search(projectFilter, 0);
            return PartialView("_Top3HilightProjects", projects.Response.Data);
        }

        [ChildActionOnly]
        [OutputCache(Duration = CACHE_30_MINUTES)]
        public ActionResult TopPropertiesForSale(int size = 3, int cid = 0, bool rent = false, string title = "", string url = "", string elementId = "")
        {
            var model = new Models.HomeTopProperty();

            SearchFilter propFilter = new SearchFilter()
            {
                Rent = rent,
                PageIndex = 1,
                PageSize = size,
                CityId = cid
            };

            var cityObj = _uow.City.Get(cid);
            model.Url = url;
            model.ElementId = elementId;
            model.Title = string.Empty;
            model.Url = (rent) ? Core.Resources.Message.Routing_Rent : Core.Resources.Message.Routing_Buy;
            if (cityObj != null)
            {
                model.Title = string.Format(title, cityObj.Name);
                model.Url = _uow.Property.FriendlyUrl(rent, "", cityObj);
            }

            var properties = _uow.Property.FrontEnd_Search(propFilter);

            model.Properties = properties.Response.Data;
            model.Prices = _uow.MogiReport.GetHousePrice_TopStreetInCity_ByAvgPrice(cid);

            return PartialView("_TopPropertiesForSale", model);
        }

        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult TopStreetByAvgPrice(int cid = 0)
        {
            if (cid == 0)
            {
                return Content("");
            }
            var streetPrices = _uow.MogiReport.GetHousePrice_TopStreetInCity_ByAvgPrice(cid);

            return PartialView("_TopStreetByAvgPrice", streetPrices);
        }

        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult HomeFeature()
        {
            var model = _uow.CMSCategory.GetByGroupId(Core.Const.CMS_GROUP_HOME_FEATURE, false);
            if (model == null || model.Count == 0)
            {
                return Content("");
            }
            return PartialView("_HomeFeature", model);
        }


        [ChildActionOnly]
        [OutputCache(Duration = CACHE_ONE_HOUR)]
        public ActionResult HomeWardReview()
        {
            var model = _uow.WardReview.GetCitySummarize();
            if (model == null || model.Count == 0)
            {
                return Content("");
            }
            return PartialView("_HomeWardReview", model);
        }

        #endregion

        #region WardReview
        public ActionResult SearchPropertyInWard(int cid, int did, int wid, string wardName = "")
        {
            var model = new WardReviewPropertyViewModel();

            SearchFilter filter = new SearchFilter()
            {
                CityId = cid,
                DistrictId = did,
                WardId = wid,
                PageIndex = 1,
                PageSize = 8,
                Rent = true
            };
            var rent = _uow.Property.FrontEnd_Search(filter);
            model.Rent = rent.Response.Data;

            filter.Rent = false;
            var sale = _uow.Property.FrontEnd_Search(filter); ;
            model.Sale = sale.Response.Data;

            model.WardName = wardName;

            return PartialView("~/Views/WardReview/Partials/_WardProperty.cshtml", model);
        }

        public ActionResult GetWardAgents(int wid = 0)
        {
            var model = _uow.WardReview.GetWardAgents(wid);
            if (model == null || model.Count == 0)
                return new EmptyResult();
            return PartialView("~/Views/WardReview/Partials/_Agents.cshtml", model);
        }

        public ActionResult WardPropertyByType(int cid = 0,int did = 0, int wid = 0, string wardName = "", string saleUrl = "", string rentUrl = "")
        {
            var model = new WardReviewPropertyTypeViewModel();

            Core.MapModels.Search.SearchSimilarResult sale = _uow.Property.FrontEnd_SearchSimilar(rent : false,cityId: cid,districtId: did, wardId : wid);
            Core.MapModels.Search.SearchSimilarResult rent = _uow.Property.FrontEnd_SearchSimilar(rent : true, cityId: cid, districtId: did, wardId : wid);

            if (sale.HasPropType == false && rent.HasPropType == false)
            {
                return new EmptyResult();
            }

            model.Sale = sale.PropType;
            model.Rent = rent.PropType;
            model.WardName = wardName;
            model.SaleUrl = saleUrl;
            model.RentUrl = rentUrl;

            return PartialView("~/Views/WardReview/Partials/_PropertyByType.cshtml", model);
        }
        public ActionResult WardNeighbor(int wid = 0, int did = 0)
        {
            var model = new WardRelationViewModel();
            model.Wards = _uow.WardReview.GetWardRelation(wid);

            if (model.Wards == null || model.Wards.Count == 0)
                return new EmptyResult();

            City d = _uow.City.Get(did);
            if(d !=null)
            {
                model.DistrictName = d.Name;
                model.WardReviewUrl = string.Format(HappyRE.Core.Resources.Message.Routing_WardReview_WardListing, d.CodeUrl, d.CityId);
            }


            return PartialView("~/Views/WardReview/Partials/_Neighbor.cshtml", model);
        }


		[OutputCache(Duration = CACHE_ONE_HOUR)]
		public ActionResult GetWardReview(int wardId = 0, bool forRent = false)
        {
            ViewBag.ForRent = forRent;
            var model = _uow.WardReview.GetWardReviewByWard(wardId);
            if (model == null)
            {
                return Content("");
            }
            return PartialView("~/Views/WardReview/Partials/_WardReviewItem.cshtml", model);
        }


		[OutputCache(Duration = CACHE_ONE_HOUR)]
		public ActionResult GetWardReviewByDistrict(int districtId = 0, bool forRent = false)
        {
            ViewBag.ForRent = forRent;
            var model = _uow.WardReview.GetWardReviewByDistrict(districtId);
            if (model == null || model.Count == 0)
            {
                return Content("");
            }
            return PartialView("~/Views/WardReview/Partials/_WardReviewItems.cshtml", model);
        }
        #endregion
    }
}