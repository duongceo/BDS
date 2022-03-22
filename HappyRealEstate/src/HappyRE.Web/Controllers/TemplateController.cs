using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.BLL.Repositories;


namespace HappyRE.Web.Controllers
{
    public class TemplateController : Controller
    {
		internal IUow _uow;
		public TemplateController(IUow uow)
		{
			_uow = uow;
		}

		// GET: Template
		public ActionResult Index()
        {
            return View();
        }

		[OutputCache(CacheProfile = "Template")]
		public async Task<ActionResult> Layout_TopMenu(string id = "", string v = "")
		{
			return await Task.Run(() => {
				ViewBag.Menu_ActiveCode = id;
				return View("Layout_TopMenu");
			});
		}

		[OutputCache(CacheProfile = "Template")]
		public async Task<ActionResult> Layout_Footer()
		{
			return await Task.Run(() => {
				return View("Layout_Footer");
			});
		}

		[OutputCache(CacheProfile = "Template")]
		public async Task<ActionResult> Home_SearchBar()
		{
			return await Task.Run(() => { return View("Home_SearchBar"); });
		}

		[OutputCache(CacheProfile = "Cache10Minutes")]
		public async Task<ActionResult> Home_TopProperties(string id = "")
		{
			return await Task.Run(() =>
			{
				ViewBag.IsMobile = (id == "mobile");
				return View("Home_TopProperties");
			});
		}

		[OutputCache(CacheProfile = "Template")]
		public async Task<ActionResult> Property_SearchBar()
		{
			return await Task.Run(() => { return View("Property_SearchBar"); });
		}

		[OutputCache(CacheProfile = "Cache1Hour")]
		public async Task<ActionResult> Property_SideBar(bool rent, int cid = 0, int did = 0, int pid = 0, int psid = 0, int sid = 0)
		{
			Core.MapModels.SearchFilter filter = new Core.MapModels.SearchFilter()
			{
				Rent = rent,
				CityId = cid,
				DistrictId = did,
				PropertyTypeId = pid,
				PropertyStyles = new List<int>() { psid },
				StreetId = sid
			};
			var model = new HappyRE.Web.Models.ListViewModel() { Filter = filter };
			return await Task.Run(() => { return View("Property_SideBar", model); });
		}

		[OutputCache(CacheProfile = "Cache1Hour")]
		public async Task<ActionResult> Property_MarketPrice(int sid, int psid, int did, int cid)
		{
			var model = new Models.PropertyMarketPriceModel()
			{
				StreetId = sid,
				SubPropertyTypeId = psid,
				DistrictId = did,
				CityId = cid			};
			return await Task.Run(() => { return View("Property_MarketPrice", model); });
		}

		[OutputCache(CacheProfile = "Cache1Hour")]
		public async Task<ActionResult> Property_Similar(bool rent, int psid, int did, long price)
		{
			var filer = new Core.MapModels.SearchSimilarProperty()
			{
				IsRent = rent,
				PropertyStyleId = psid,
				DistrictId = did,
				Price = price
			};
			
			return await Task.Run(() =>
			{
				var resp = _uow.Property.FrontEnd_Similar(filer);
				return View("Property_Similar", resp.Response.Data);
			});
		}

		[OutputCache(CacheProfile = "Template")]
		public async Task<ActionResult> Property_SendMessage()
		{
			return await Task.Run(() => { return View("Property_SendMessage"); });
		}
	}
}