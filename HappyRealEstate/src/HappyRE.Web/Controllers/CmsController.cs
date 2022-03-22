

using HappyRE.Core.MapModels;
using HappyRE.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MBN.Utils;
using MBN.Utils.Extension;
using HappyRE.Core.MapModels.FrontEnd;

namespace HappyRE.Web.Controllers
{
    public class CmsController : BaseController
    {
        public CmsController(IUow uow) : base(uow)
        {
            ViewBag.Code = "";
        }

		[OutputCache(CacheProfile = "CMSListing")]
		public ActionResult List(string parentCodeUrl, string codeUrl)
        {
            ViewBag.CodeUrl = codeUrl;

            var objCat = _uow.CMSCategory.GetByCodeUrl(Core.Const.CMS_GROUP_GUIDE, codeUrl);
            if (objCat == null)
            {
                string fileLog = string.Format("cms_list_error_{0}.txt", DateTime.Today.yyyyMMdd());
                WebLog.Log.Data("NotFound: " + Request.Url.AbsoluteUri, true, fileLog);
                return View("Index", new List<CMS_News>());
            }

            //Lấy danh sách tin tức
            var obj = _uow.CMSNews.GetByCategoryId(objCat.CategoryId);
            if (obj.Count == 1)
            {
                var viewModel = CMSNewsViewModel.MapObject(obj.FirstOrDefault());

                /// Get SEO data
                if (viewModel.SEOId > 0)
                {
                    var seo = _uow.SEO.Get(viewModel.SEOId);
                    if (seo != null)
                    {
                        viewModel.SEOTitle = seo.Title;
                        viewModel.SEOKeyword = seo.Keyword;
                        viewModel.SEODescription = seo.Description;
                    }

                }
                this.SEO_CMSDetail(viewModel);
                return View("Detail", viewModel);
            }

            return View("List", obj);
        }

        private void SEO_CMSDetail(CMSNewsViewModel viewModel)
        {
            try
            {
                if (viewModel.SEOId == 0)
                {
                    ViewBag.TitlePage = viewModel.Subject;
                    ViewBag.Title = viewModel.Subject;
                    ViewBag.Description = viewModel.Summary;

                }
                else
                {
                    ViewBag.TitlePage = viewModel.SEOTitle;
                    ViewBag.Title = viewModel.SEOTitle;
                    ViewBag.Description = viewModel.SEODescription;
                    ViewBag.Keywords = viewModel.SEOKeyword;
                }

            }
            catch (Exception ex)
            {
                WebLog.Log.Error("PropertyController.SEO_Listing", ex);
                throw ex;
            }
        }

        // GET: Cms
        public ActionResult Index()
        {
            return View("List", new List<CMS_News>());
        }

		[OutputCache(CacheProfile = "CMSDetail")]
		public ActionResult Detail(int id)
        {
            //var obj = _uow.CMSNews.Get(id);
            var obj = _uow.CMSNews.FronEnd_Get(id);
            this.SEO_CMSDetail(obj);
            return View(obj);
        }

		//public JsonResult GetNews(int id=8)
		//{
		//    AjaxResponse rp = new AjaxResponse();
		//    try
		//    {
		//        CMS_News res = _uow.CMSNews.Get(id);
		//        if (res == null) res = new CMS_News();
		//        res.PublishDate = DateTime.Now;
		//        res.Created = DateTime.Now;
		//        res.Updated = DateTime.Now;
		//        rp.Status = true;
		//        SEO seo = new SEO();
		//        if (res.SEOId > 0)
		//        {
		//            seo = _uow.SEO.Get(res.SEOId);
		//        }
		//        rp.Data = new { News = res, Seo = seo };
		//    }
		//    catch (Exception ex)
		//    {
		//        rp.Message = ex.Message;
		//    }
		//    return Json(rp, JsonRequestBehavior.AllowGet);
		//}

		//public JsonResult GetListNews(CMSNewsQuery cmsNewsQuery)
		//{
		//    AjaxResponse rp = new AjaxResponse();
		//    try
		//    {
		//        int total = 0;
		//        var obj = _uow.CMSNews.GetCMSNewsByQuery(cmsNewsQuery, out total);
		//        rp.Data = new { Data = obj, Total = total };
		//        rp.Status = true;
		//    }
		//    catch (Exception ex)
		//    {
		//        rp.Message = ex.Message;
		//    }
		//    return Json(rp, JsonRequestBehavior.AllowGet);
		//}

		[OutputCache(CacheProfile = "Cache1Hour")]
		public JsonResult GetCategoriesByGroup(int id)
        {
            AjaxResponse rp = new AjaxResponse();
            try
            {
                var data = _uow.CMSCategory.GetByGroupId(id);
                rp.Data = data;
                rp.Status = true;
            }
            catch (BusinessException ex)
            {
                rp.Message = ex.Message;
            }
            catch (Exception)
            {
                rp.Message = Core.Resources.Message.GeneralError;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public PartialViewResult CategoriesByGroup(int groupId = 0, int categoryId = 0, string codeUrl = "")
        {
            if (groupId == 0) groupId = HappyRE.Core.Const.CMS_GROUP_GUIDE;

            List<CMS_Category> data = _uow.CMSCategory.GetByGroupId(groupId);
            CMS_Category obj = null;
            if (categoryId > 0)
            {
                obj = data.FirstOrDefault(c => c.CategoryId == categoryId);
            }
            else if (string.IsNullOrEmpty(codeUrl) == false)
            {
                codeUrl = codeUrl.ToLower();
                obj = data.FirstOrDefault(c => c.CodeUrl == codeUrl);
            }
            if (obj != null)
            {
                obj.Selected = true;
                if (obj.ParentId > 0)
                {
                    var objParent = data.FirstOrDefault(c => c.CategoryId == obj.ParentId);
                    if (objParent != null) objParent.Selected = true;
                }
            }
            return PartialView("_Categories", data);
        }

        private string GetCodeUrl()
        {
            string[] segments = Request.Url.Segments;
            if (segments == null || segments.Length == 0) return string.Empty;

            return segments[segments.Length - 1];
        }

        /// <summary>
        /// Liên hệ
        /// </summary>
        /// <param name="codeUrl"></param>
        /// <returns></returns>
        public ActionResult Contact(string codeUrl = "")
        {
            ViewBag.CodeUrl = codeUrl.ToLower();
            return View();
        }

		#region Văn phòng giao dịch

		/// <summary>
		/// Văn phòng giao dịch
		/// </summary>
		/// <param name="codeUrl"></param>
		/// <returns></returns>
		[OutputCache(CacheProfile = "Cache1Day")]
		public ActionResult BranchOffice(string codeUrl = "")
        {
            if (string.IsNullOrEmpty(codeUrl))
            {
                codeUrl = this.GetCodeUrl();
            }
            ViewBag.CodeUrl = codeUrl.ToLower();
            return View();
        }


		/// <summary>
		/// Lấy danh sách khu vực có VPGD
		/// </summary>
		/// <param name="cityId"></param>
		/// <returns></returns>
		[OutputCache(CacheProfile = "Cache1Day")]
		public JsonResult GetLocationAgent(int cityId = 0)
        {
            ResponseAjax rp = new ResponseAjax();
            try
            {
                rp.Data = BLL.Utils.MBNAPIUtils.GetLocationAgent(cityId);
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Message = ex.Message;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

		/// <summary>
		/// Lấy danh sách VPGD theo khu vực
		/// </summary>
		/// <param name="cityId"></param>
		/// <param name="districtId"></param>
		/// <returns></returns>
		[OutputCache(CacheProfile = "Cache1Day")]
		public JsonResult GetMBNAgents(int cityId = 0, int districtId = 0)
        {
            ResponseAjax rp = new ResponseAjax();
            try
            {
                rp.Data = BLL.Utils.MBNAPIUtils.GetMBNAgents(cityId, districtId);
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Message = ex.Message;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Register-mogipro-agreement
        public JsonResult GetByUrlCode(int groupId, string urlCode)
        {
            ResponseAjax rp = new ResponseAjax();
            try
            {
                rp.Data = _uow.CMSCategory.GetByCodeUrl(groupId, urlCode);
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Message = ex.Message;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
		#endregion

		#region Notarize Office
		/// <summary>
		/// Văn phòng công chứng
		/// </summary>
		/// <param name="codeUrl"></param>
		/// <returns></returns>
		[OutputCache(CacheProfile = "Cache1Day")]
		public ActionResult NotarizeOffice(string codeUrl = "")
        {
            if (string.IsNullOrEmpty(codeUrl))
            {
                codeUrl = this.GetCodeUrl();
            }
            ViewBag.CodeUrl = codeUrl.ToLower();
            return View();
        }

		[OutputCache(CacheProfile = "Cache1Day")]
		public JsonResult GetNotarizeOffices()
        {
            ResponseAjax rp = new ResponseAjax();
            try
            {
                rp.Data = _uow.Org.GetNotarizeOffice();
                rp.Status = true;
            }
            catch (Exception ex)
            {
                rp.Message = ex.Message;
            }
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}