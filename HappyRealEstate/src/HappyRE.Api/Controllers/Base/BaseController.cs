using HappyRE.Core.BLL.Repositories;
using System.Web.Mvc;

namespace HappyRE.Api.Controllers
{
    public class BaseController : Controller
    {
        internal IUow _uow;
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
    }
}
