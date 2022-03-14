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
    public class NotificationController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger("NotificationController");
        public NotificationController(IUow uow) : base(uow) { }

        public ActionResult Index()
        {
            return View(new NotificationQuery());
        }

        [Authorize(Roles = "NOTIFICATION_CREATE,NOTIFICATION_DELETE")]
        public ActionResult Admin()
        {
            return View(new NotificationQuery());
        }

        [Authorize(Roles = Permission.NOTIFICATION_CREATE)]
        public ActionResult Create()
        {
            return View(new Core.Entities.Model.Notification());
        }
       
        public async Task<ActionResult> Detail(int id)
        {
            var res = await _uow.Notification.GetById(id);
            if (res != null)
            {
                await _uow.NotificationRead.Read(new NotificationRead()
                {
                    NotificationId = res.Id,
                    UserName = User.Identity.Name
                });
                this.Log("Notification", id, "Detail", null);
            }
            return View(res);
        }

        [Authorize(Roles = "NOTIFICATION_CREATE,NOTIFICATION_DELETE")]
        public async Task<ActionResult> DetailAdmin(int id)
        {
            var res = await _uow.Notification.GetById(id);
            if (res != null)
            {
                await _uow.NotificationRead.Read(new NotificationRead()
                {
                    NotificationId = res.Id,
                    UserName = User.Identity.Name
                });
            }
            return View(res);
        }

        [CompressFilter]
        public async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, NotificationQuery model)
        {
            try
            {
                model.Page = request.Page;
                model.Limit = request.PageSize;
                model.SentTo = User.Identity.Name;
                var res = await _uow.Notification.Search(model);

                return Json(new DataSourceResult()
                {
                    Data = res.Item1,
                    Total = res.Item2
                });
            }catch(Exception ex)
            {
                _log.Error(ex);
                return null;
            }
        }

        [CompressFilter]
        [Authorize(Roles = "NOTIFICATION_CREATE,NOTIFICATION_DELETE")]
        public async Task<ActionResult> SearchAdmin([DataSourceRequest] DataSourceRequest request, NotificationQuery model)
        {
            try
            {
                model.Page = request.Page;
                model.Limit = request.PageSize;
                var res = await _uow.Notification.SearchAdmin(model);

                return Json(new DataSourceResult()
                {
                    Data = res.Item1,
                    Total = res.Item2
                });
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return null;
            }
        }

        [Authorize(Roles = Permission.NOTIFICATION_DELETE)]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _uow.Notification.Delete(id);
            return View("Index");
        }

        #region Json
        [HttpGet]
        [CompressFilter]
        public async Task<JsonResult> _Search()
        {
            try
            {
                var res = await _uow.Notification.Search(new Core.Entities.NotificationQuery() { Page = 1, Limit = 15, SentTo=User.Identity.Name });
                return Json(new {data= res.Item1, total = res.Item2 }, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        [CompressFilter]
        public async Task<JsonResult> _UnReadCount()
        {
            try
            {
                var res = await _uow.Notification.UnReadCount(User.Identity.Name);
                return Json(res, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        [CompressFilter]
        public async Task<JsonResult> _Read(int id)
        {
            try
            {
                var res = await _uow.NotificationRead.Read(new Core.Entities.Model.NotificationRead()
                {
                    NotificationId=id,
                    UserName = User.Identity.Name
                });
                return Json(res, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        [Authorize(Roles = Permission.NOTIFICATION_CREATE)]
        public async Task<JsonResult> _Send(Core.Entities.Model.Notification data)
        {
            try {
                NotificationHub objNotifHub = new NotificationHub();
                bool isAll = false;
                if (data.SendTos==null || data.SendTos.Count == 0)
                {
                    isAll = true;
                    data.SentTo = "Tất cả";
                }
                else
                {
                    var users = await _uow.UserProfile.GetUserByDeparments(data.SendTos);
                    data.SendTos = users.ToList();
                    data.SentTo = String.Join(";", data.SendTos);                
                }
                
                var res = await _uow.Notification.IU(data);
                if (res.HasValue)
                {
                    data.Id = res.Value;
                    objNotifHub.SendNotificationToList(data, isAll);

                    this.Log("Notification", data.Id, "IU", null);
                }
                return Json(res, JsonRequestBehavior.AllowGet);
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

        [CompressFilter]
        [HttpPost]
        [Authorize(Roles = Permission.NOTIFICATION_CREATE)]
        public async Task<JsonResult> _IU(Core.Entities.Model.Notification data)
        {
            try
            {
                var res = await _uow.Notification.IU(data);
                return Json(res, JsonRequestBehavior.AllowGet);
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
        #endregion
    }
}