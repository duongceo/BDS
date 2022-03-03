using HappyRE.App.Filters;
using HappyRE.Core.BLL.Repositories;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Web;
using HappyRE.App.Models;
using System;
using log4net;
using HappyRE.Core.Entities.ViewModel;

namespace HappyRE.App.Controllers
{
    [TCCorsPolicyProvider]
    public class BaseApiController : ApiController
    {
        internal IUow _uow;
        //private static readonly ILog _log = LogManager.GetLogger("BaseApiController");
        protected string _CurrentUserId = null;
        protected int? _Level=null;
        internal readonly string General_Err = Core.Entities.Resources.Messages.System_Err;
        internal readonly string CanNotDelete_Err = Core.Entities.Resources.Messages.CanNotDelete;
        internal readonly string RequiredUpgrade_MaxPage_Err = Core.Entities.Resources.Messages.RequiredUpgrade_MaxPage;
        public BaseApiController(IUow uow)
        {
            this._uow = uow;            
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

        //internal int Level
        //{
        //    get { if (_Level ==null) _Level = Helper.GetLevel(CurrentUserId); return _Level.Value; }
        //}

        internal bool isEditor()
        {
            if(User.IsInRole("editor") ==false && User.IsInRole("admin") == false){
                return false;
            }
            return true;
        }
        internal bool Can_UI(string userId)
        {
            if (User.IsInRole("editor") == true || User.IsInRole("admin") == true || CurrentUserId== userId)
            {
                return true;
            }
            return false;
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
    }
}
