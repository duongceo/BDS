using System.Collections.Generic;
using HappyRE.Core.MapModels.Report;

namespace HappyRE.Web.Models
{
	public class HomeTopProperty
	{
		public string ElementId { get; set; }
		public string Title { get; set; }
		public string Url { get; set; }
		public List<Core.MapModels.Search.PropertyItem> Properties { get; set; }
		public List<HousePriceTopPrice> Prices { get; set; }
	}
}