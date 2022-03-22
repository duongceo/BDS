using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HappyRE.Core.Entities;
using HappyRE.Core.MapModels;
using HappyRE.Web.Models;

using HappyRE.Core.MapModels.Search;
using HappyRE.Core.Resources;
using System.ComponentModel;
using MBN.Utils;
using HappyRE.Web.Helpers;
using HappyRE.Core.MapModels.FrontEnd;
using System.Net;
using HappyRE.Core.BLL.Repositories;

namespace HappyRE.Web.Controllers
{
    public class SitemapController : BaseController
    {
        private static readonly string CachePath = WebUtils.AppSettings("Sitemap_CachePath", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache\\sitemaps"));

        public SitemapController(IUow uow) : base(uow)
        {
            
        }

        public void Index1(int id)
        {
            //string fileName = Path.GetFileName(Request.FilePath);
            //fileName = Path.Combine(CachePath, fileName) + ".zip";
            //byte[] data = System.IO.File.ReadAllBytes(fileName);
            byte[] data = null;// this.RenderCache();

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Headers["Content-Encoding"] = "gzip";
            Response.Headers["Content-Length"] = data.Length.ToString();
            Response.ContentType = "text/xml; charset=utf-8";
            Response.OutputStream.Write(data, 0, data.Length);
            Response.End();
        }

        // Sitemap Root
        public ActionResult Index(int id)
        {
            if (this.RenderCache() == true)
            {
                return null;
            }

            string xml = this.GetCacheFile();
            if (string.IsNullOrEmpty(xml) == true)
            {
                List<SitemapItem> items = _uow.Sitemap.Google_GetByRefer(id);
                xml = Helpers.Utils.RenderViewToString(this.ControllerContext, "_index", items);
                this.SaveCacheFile(xml);
            }

            if (string.IsNullOrEmpty(xml) == true)
            {
                return null;
            }

            return Content(xml, "text/xml", System.Text.Encoding.UTF8);
        }

        public ActionResult SitemapCategory(int id, int sitemapType = 0)
        {
            if (this.RenderCache() == true)
            {
                return null;
            }

            string xml = this.GetCacheFile();
            if (string.IsNullOrEmpty(xml) == true)
            {
                List<SitemapItem> items = _uow.Sitemap.Google_GetByRefer(id);
                xml = Helpers.Utils.RenderViewToString(this.ControllerContext, "_listing", items);
                this.SaveCacheFile(xml);
            }

            if (string.IsNullOrEmpty(xml) == true)
            {
                return null;
            }

            return Content(xml, "text/xml", System.Text.Encoding.UTF8);
        }

        public ActionResult SitemapDetailXml()
        {
            if (this.RenderCache() == true)
            {
                return null;
            }

            string xml = this.GetCacheFile();
            if (string.IsNullOrEmpty(xml) == true)
            {
                List<SitemapItem> items = _uow.SitemapDetail.Google_GetRootXml();
                xml = Utils.RenderViewToString(this.ControllerContext, "_index", items);
                this.SaveCacheFile(xml);
            }

            if (string.IsNullOrEmpty(xml) == true)
            {
                return null;
            }

            return Content(xml, "text/xml", System.Text.Encoding.UTF8);
        }

        public ActionResult SitemapDetailPage(int id)
        {
            if (this.RenderCache() == true)
            {
                return null;
            }

            string xml = this.GetCacheFile();
            if (string.IsNullOrEmpty(xml) == true)
            {

                SitemapDetail item = _uow.SitemapDetail.Get(id);
                if (item == null || item.PropertyId != 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                List<SitemapItem> items = _uow.SitemapDetail.Google_GetByParent(id);
                xml = Utils.RenderViewToString(this.ControllerContext, "_listing", items);

                this.SaveCacheFile(xml);
            }

            if (string.IsNullOrEmpty(xml) == true)
            {
                return null;
            }

            return Content(xml, "text/xml", System.Text.Encoding.UTF8);
        }

        private string GetCacheFile()
        {
            string res = string.Empty;
            if (WebUtils.GetQuery("cache", false) == true) return res;

            
            string fileName = Path.GetFileName(Request.FilePath);
            fileName = Path.Combine(CachePath, fileName);
            if (System.IO.File.Exists(fileName) == true)
            {
                res = System.IO.File.ReadAllText(fileName);
            }
            return res;
        }

        private void SaveCacheFile(string contents)
        {
            if (WebUtils.GetQuery("cache", false) == false) return;
            if (Directory.Exists(CachePath) == false) Directory.CreateDirectory(CachePath);

            string fileName = Path.GetFileName(Request.FilePath);
            fileName = Path.Combine(CachePath, fileName);
            System.IO.File.WriteAllText(fileName, contents);

            this.CompressGZip(fileName);
        }

        private bool RenderCache()
        {
            if (WebUtils.GetQuery("cache", false) == true)
            {
                return false;
            }

            string encoding = this.GetCompressType();
            if (string.IsNullOrEmpty(encoding) == true)
            {
                return false;
            }

            string fileName = Path.GetFileName(Request.FilePath);
            fileName = Path.Combine(CachePath, fileName) + ".zip";
            if (System.IO.File.Exists(fileName) == false)
            {
                //string f1 = Path.GetFileName(Request.FilePath);
                //f1 = Path.Combine(CachePath, f1);
                //this.CompressGZip(f1);

                return false;
            }

            byte[] buff = System.IO.File.ReadAllBytes(fileName);
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Headers["Content-Encoding"] = "gzip";
            Response.Headers["Content-Length"] = buff.Length.ToString();
            Response.ContentType = "text/xml; charset=utf-8";
            Response.OutputStream.Write(buff, 0, buff.Length);
            Response.End();

            return true;
        }

        private void CompressGZip(string fileName)
        {
            string fileGZip = fileName + ".zip";
            byte[] data = System.IO.File.ReadAllBytes(fileName);
            using (FileStream fs = new FileStream(fileGZip, FileMode.Create))
            {
                using (GZipStream gz = new GZipStream(fs, CompressionMode.Compress))
                {
                    gz.Write(data, 0, data.Length);
                    gz.Close();
                }
                fs.Close();
            }
        }


        /// <summary>
        /// Kiểm tra Browser chấp nhận Encoding hay không
        /// </summary>
        /// <returns>defalte,gzip</returns>
        private string GetCompressType()
        {
            string res = "";
            string acceptEncoding = Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncoding)==false)
            {
                acceptEncoding = acceptEncoding.ToLower();
                if (acceptEncoding.Contains("gzip"))
                {
                    res = "gzip";
                }
                else if (acceptEncoding.Contains("deflate") || acceptEncoding == "*")
                {
                    res = "defalte";
                }
            }
            return res;
        }
    }
}