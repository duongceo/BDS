using Microsoft.AspNet.Identity;
using HappyRE.Core.BLL.Repositories;
using System.Web.Mvc;
using System.Threading.Tasks;
using HappyRE.App.Infrastructures;
using System.Collections.Generic;
using System;

namespace HappyRE.App.Controllers
{
    public class BaseController : Controller
    {
        internal IUow _uow;
        protected string _CurrentUserId = null;
        public BaseController(IUow uow)
        {
            this._uow = uow;
        }

        private string _ClientIP = string.Empty;
        internal string ClientIP
        {
            get
            {
                if (string.IsNullOrEmpty(_ClientIP))
                {
                    _ClientIP = HappyRE.Core.Utils.StringUtils.GetRequestIP();
                }

                return _ClientIP;
            }
        }

        internal string UserName
        {
            get
            {
                var identity = System.Web.HttpContext.Current.User.Identity;
                if (identity.IsAuthenticated == true)
                {
                    return identity.Name;
                }
                return null;
            }
        }
        internal string GetUserId()
        {
            var identity = System.Web.HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated == true)
            {
                return identity.GetUserId();
            }
            return null;
        }
        internal string CurrentUserId
        {
            get { if (string.IsNullOrEmpty(_CurrentUserId)) _CurrentUserId = GetUserId(); return _CurrentUserId; }
        }

        internal bool IsAdmin
        {
            get { return User.IsInRole("ADMIN") || User.IsInRole("SYS_ADMIN"); }
        }

        internal bool IsSysAdmin
        {
            get { return User.IsInRole("SYS_ADMIN"); }
        }

        internal void Log(string tableName,int? tableKeyId, string action, string contents)
        {
            try
            {
                Hangfire.BackgroundJob.Enqueue<IHistoryLogRepository>(x => x.AddTrackingLog(new Core.Entities.Model.HistoryLog()
                {
                    TableKeyId = tableKeyId,
                    TableName = tableName,
                    Action = action,
                    Contents = contents,
                    CreatedBy = this.UserName
                }));
            }catch(Exception ex)
            {

            }
        }
    }
}
