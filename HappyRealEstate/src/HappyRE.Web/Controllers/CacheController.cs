using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.MapModels.MogiPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MBN.Utils.Caching;
using MBN.Utils.Extension;

namespace HappyRE.Web.Controllers
{
    public class CacheController : BaseController
    {
        public CacheController(IUow uow) : base(uow) { }

        public dynamic Flush(string id)
        {
            return BLL.Queues.CacheVersionTask.Flush(_uow, id);
        }

		public dynamic Remove(string id)
		{
			var res = _uow.CacheVersion.RemoveCache(id);
			return DateTime.Now.yyyyMMddHHmmss() + " - Remove CacheId: " + id;
		}

		public dynamic RemoveMemoryCache(string cacheId)
		{
			if (string.IsNullOrEmpty(cacheId) == true) {
				return DateTime.Now.yyyyMMddHHmmss() + " FAILD - Remove Memory CacheId: " + cacheId;
			}

			var v = CacheManager.CacheClient.GetValue(cacheId);

			CacheManager.RemoveCache(cacheId);

			return DateTime.Now.yyyyMMddHHmmss() + $" DONE - Remove Memory CacheId: {cacheId}, Data: {v.ToJson()}";
		}
	}
}