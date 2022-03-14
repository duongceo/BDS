using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HappyRE.App.Infrastructures;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using Kendo.Mvc.UI;
using log4net;

namespace HappyRE.App.Controllers
{
    [Authorize]
    public class ImageFileController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("ImageFileController");
        public ImageFileController(IUow uow) : base(uow) { }

        public ActionResult Index()
        {
            return View(new ImageFileQuery());
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, ImageFileQuery model)
        {
            try
            {
                model.Page = request.Page;
                model.Limit = 20;//request.PageSize;
                var res = await _uow.ImageFile.Search(model);

                return Json(new DataSourceResult()
                {
                    Data = res.Item1,
                    Total = res.Item2
                });
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Warn(ex);
                Response.StatusCode = 400;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                Response.StatusCode = 400;
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> Delete(int id)
        {
            try { 
                await _uow.ImageFile.Delete(new ImageFile() {Id=id },true);
                return Json(new DataSourceResult());
            }
            catch (HappyRE.Core.BLL.BusinessException ex)
            {
                _log.Warn(ex);
                var r = new DataSourceResult()
                {
                    Errors = ex.Message
                };
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                var r = new DataSourceResult()
                {
                    Errors = "Không thể xóa thông tin này vì đang sử dụng!"
                };
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        #region Json
        [HttpPost]
        [Authorize(Roles = Permission.COMMENT_DELETE)]
        public async Task<JsonResult> _Delete(int id)
        {
            var res=await _uow.ImageFile.Delete(new ImageFile() { Id = id }, true);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion

        

    }
}