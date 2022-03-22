using HappyRE.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using MBN.Utils;

using HappyRE.Core.MapModels;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.MapModels.SEO;
using MBN.Utils.Extension;
using HappyRE.Core.Resources;

namespace HappyRE.Web.Controllers
{
    public class ProjectController : BaseController
    {
        public ProjectController(IUow uow) : base(uow)
        {
            ViewBag.Menu_ActiveCode = "menu-project";
        }

        // GET: Project
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// cuong.phung
        /// review: quy.vu - 11:00 2018-06-20
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[OutputCache(CacheProfile = "ProjectDetail")]
        public ActionResult Detail(int id = 0)
        {
            if (id <= 0)
            {
                return RedirectToAction("List");
            }

            var objDetail = _uow.Project.GetProjectDetail(id);
            if (objDetail == null)
            {
                WebLog.Log.Data(DateTime.Now.yyyyMMddHHmmss() + "\tProjectId: " + id, true, "Project_NotFound_" + DateTime.Today.ToString("yyyyMM") + ".txt");
                return RedirectToAction("List");
            }

            ProjectDetailModel model = new ProjectDetailModel();
            model.ProjectDetail = objDetail;
            var objProject = objDetail.Project;
            var objCity = _uow.City.Get(objProject.CityId);
            var objDistrict = _uow.City.Get(objProject.DistrictId);

            // Url
            string codeUrl = objProject.SEO_Url;
            if (string.IsNullOrEmpty(codeUrl) == true)
            {
                codeUrl = _uow.Property.FriendlyUrl_Project(objProject.ProjectId, objProject.CodeUrl);
            }
            ViewBag.SaleUrl = _uow.Property.FriendlyUrl(false, codeUrl, objCity, objDistrict);


            // SEO
            ViewBag.Title = objProject.SEO_Title;
            ViewBag.Description = objProject.SEO_Description;
            ViewBag.Keywords = objProject.SEO_Keyword;
            ViewBag.Canonical = Core.Utils.Common.HomeUrl + _uow.Project.FriendlyUrl_Detail(objDistrict.CodeUrl, codeUrl);

            var seoObj = _uow.SEO_Page.SEO_ProjectDetail(objDetail);
            if (seoObj != null)
            {
                this.SetSEOTag(seoObj);
            }

            // Listing by Org
            ViewBag.OrgListing = string.Empty;
            if (model.ProjectDetail.Investor != null)
            {
                var org = model.ProjectDetail.Investor;
                ViewBag.OrgListing = string.Format(Message.Routing_Project_Org, org.CodeUrl, org.OrgId);
            }

            return View(model);
        }

        /// <summary>
        /// cuong.phung
        /// review: quy.vu - 16:19 2018-06-20
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [OutputCache(CacheProfile = "ProjectListing")]
        public ActionResult List(FrontEndProjectQuery query)
        {
            int cityId = query.CityId;
            int distId = 0;

            bool has_filter = query.HasFilter();

            var results = _uow.Project.FrontEnd_Search(query, 0);
            var model = new ProjectsViewModel();
            var requestUrl = _uow.Project.FriendlyUrl_Listing(query);

            model.Query = query;
            model.Query.Keyword = GetKeyWord(query);
            model.Query.Url = (this.Request.Url.Segments.Count() == 3) ? this.Request.Url.Segments.Last() : "";
            model.Projects = results.Response.Data;

            model.Paging = this.GetPaging(requestUrl, query.cp, results.Response.Total, query.PageSize);

			#region Total - disabled by quy.vu - 11:39 2019-04-11
			/*
			var stats = _uow.Property.FrontEnd_GetTotalByProject(model.Projects.Select(c => c.ProjectId).ToList());
            foreach (var item in model.Projects)
            {
                item.TotalSale = item.TotalRent = 0;
                if (stats.ContainsKey(item.ProjectId) == false) continue;
                var obj = (Core.MapModels.Search.ProjectStatistic)stats[item.ProjectId];
                item.TotalSale = obj.TotalSale;
                item.TotalRent = obj.TotalRent;

                if (cityId == 0 && has_filter)
                {
                    cityId = item.CityId;
                    distId = item.DistrictId;
                }
            }
			*/
			#endregion

			ViewBag.CityId = distId == 0 ? cityId : distId;
            ViewBag.DistrictId = distId;
            ViewBag.UserData = this.UserData;
            ViewBag.Canonical = Core.Utils.Common.HomeUrl + requestUrl;

            //SEO
            var seo = _uow.SEO_Page.SEO_ProjectListing(query);
            if (seo != null)
            {
                this.SetSEOTag(seo);
            }

            return View(model);
        }

        #region Private
        /// <summary>
        /// Get Paging for List Object
        /// </summary>
        /// <param name="model"></param>
        private Paging GetPaging(string pageUrl, int pageIndex, int totalPage, int pageSize = 10)
        {
            Paging paging = new Paging();
            paging.Total = totalPage;
            paging.CurrentPage = pageIndex;
            paging.PageSize = pageSize;
            paging.Url = pageUrl;
            return paging;
        }

        private string GetKeyWord(FrontEndProjectQuery filter)
        {

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                return filter.Keyword;
            }

            if (filter.OrgId > 0)
            {
                var objOrg = _uow.Org.Get(filter.OrgId);
                if (objOrg != null)
                {
                    return objOrg.Name;
                }
                return string.Empty;
            }

            var ReferId = 0;
            var ReferTypeId = 0;
            if (filter.ProjectId > 0)
            {
                ReferId = filter.ProjectId;
                ReferTypeId = Const.MAP_REFERTYPE_PROJECT;
            }
            else if (filter.StreetId > 0)
            {
                ReferId = filter.StreetId;
                ReferTypeId = Const.MAP_REFERTYPE_STREET;
            }
            else if (filter.WardId > 0)
            {
                ReferId = filter.WardId;
                ReferTypeId = Const.MAP_REFERTYPE_WARD;
            }
            else if (filter.CityId > 0)
            {
                var cityObj = _uow.City.Get(filter.CityId);
                if (cityObj != null)
                {
                    ReferTypeId = (cityObj.ParentId == 0) ? Const.MAP_REFERTYPE_CITY : Const.MAP_REFERTYPE_DISTRICT;
                }
                ReferId = filter.CityId;

            }
            var objMap = _uow.Map.GetBy(ReferId, ReferTypeId);
            if (objMap != null)
            {
                return objMap.Name;
            }
            return string.Empty;
        }


        private void SetSEOTag(SEO_MetaPage seo)
        {
            ViewBag.TitlePage = seo.TitlePage;
            ViewBag.Title = seo.Title;
            ViewBag.Description = seo.Description;
            ViewBag.PageDescription = seo.PageDescription;
            ViewBag.Keywords = seo.MetaKeywords;
            ViewBag.Footer = seo.Footer;
            //ViewBag.Canonical = seo.Alternate;
        }
        #endregion
    }


}