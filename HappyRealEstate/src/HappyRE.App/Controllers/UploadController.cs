using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HappyRE.App.Infrastructures;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using Kendo.Mvc.UI;

namespace HappyRE.App.Controllers
{
    [Authorize]
    public class UploadController : BaseController
    {
        public UploadController(IUow uow) : base(uow) { }
        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Avatar(IEnumerable<HttpPostedFileBase> files)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files.Take(3))
                {
                    if (file != null && file.ContentLength > 1000)
                    {
                        var res = await _uow.File.UploadImg(file.InputStream, file.FileName, (int)FileType.Avatar, DateTime.Today.ToString("MMYYYY"));
                        return Json(new { data = res.Thumb }, "text/plain"); 
                    };
                }
            }
            return Content("");
        }

        public async Task<ActionResult> Avatar1(IEnumerable<HttpPostedFileBase> files1)
        {
            // The Name of the Upload component is "files"
            if (files1 != null)
            {
                foreach (var file in files1.Take(3))
                {
                    if (file != null && file.ContentLength > 1000)
                    {
                        var res = await _uow.File.UploadImg(file.InputStream, file.FileName, (int)FileType.Avatar, DateTime.Today.ToString("MMYYYY"));
                        return Json(new { data = res.Thumb }, "text/plain");
                    };
                }
            }
            return Content("");
        }

        public async Task<ActionResult> Property(IEnumerable<HttpPostedFileBase> files,int? id)
        {
            if (files != null)
            {
                foreach (var file in files.Take(30))
                {
                    if (file != null && file.ContentLength > 1000)
                    {
                        var groupCode = DateTime.Now.GetHashCode().ToString("x");
                        var res = await _uow.File.UploadImg(file.InputStream, file.FileName, (int)FileType.Property, DateTime.Today.ToString("MMyyyy"),id, groupCode);
                        return Json(new { data = res.Thumb }, "text/plain");
                    };
                }
            }
            return Content("");
        }

        public async Task<ActionResult> Property1(IEnumerable<HttpPostedFileBase> files1, int? id)
        {
            if (files1 != null)
            {
                foreach (var file in files1.Take(30))
                {
                    if (file != null && file.ContentLength > 1000)
                    {
                        var groupCode = DateTime.Now.GetHashCode().ToString("x");
                        var res = await _uow.File.UploadImg(file.InputStream, file.FileName, (int)FileType.Property, DateTime.Today.ToString("MMyyyy"), id, groupCode);
                        return Json(new { data = res.Thumb }, "text/plain");
                    };
                }
            }
            return Content("");
        }

        public async Task<ActionResult> Customer(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                foreach (var file in files.Take(30))
                {
                    if (file != null && file.ContentLength > 1000)
                    {
                        var res = await _uow.File.UploadImg(file.InputStream, file.FileName, (int)FileType.Customer, DateTime.Today.ToString("MMyyyy"));
                        return Json(new { data = res.Thumb }, "text/plain");
                    };
                }
            }
            return Content("");
        }

        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    //if (postedFile != null && postedFile.ContentLength > 1000)
                    //{
                    //    res = _uow.File.UploadImg(postedFile.InputStream, postedFile.FileName, type, coid, CurrentUserId);
                    //    res.message = "Upload thành công";
                    //    res.code = 200;
                    //    return res;
                    //};ư

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }
    }
}