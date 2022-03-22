using HappyRE.Core.MapModels.FrontEnd;
using HappyRE.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using HappyRE.Core.MapModels.Report;
using System.ComponentModel.DataAnnotations;

namespace HappyRE.Web.Models
{
    public class PropertyDetailViewModel
    {
        public PropertyDetailViewModel()
        {
        }
        public PropertyDisplay Property { get; set; }
        public UserInboxViewModel UserInbox { get; set; }
		public string SimilarUrl { get; set; }
		public string SearchUrl { get; set; }
	}


	public class PropertyMarketPriceModel
	{
		public int StreetId { get; set; }
		public int SubPropertyTypeId { get; set; }
		public int PropertyTypeId { get; set; }
		public int DistrictId { get; set; }
		public int CityId { get; set; }
		
	}

	public class PropertySendMessage
	{
		[Display(Name = "UserInbox_Sender", ResourceType = typeof(Core.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Resources.Validation), ErrorMessage = null)]
		[MaxLength(150, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Resources.Validation), ErrorMessage = null)]
		public string SenderName { get; set; }

		[Display(Name = "UserInbox_Mobile", ResourceType = typeof(Core.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Resources.Validation), ErrorMessage = null)]
		[MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Resources.Validation))]
		[RegularExpression(Const.REGEX_PATTERN_MOBILE, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Resources.Validation), ErrorMessage = null)]
		public string SenderMobile { get; set; }

		[Display(Name = "UserInbox_Email", ResourceType = typeof(Core.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Resources.Validation))]
		[RegularExpression(Const.REGEX_PATTERN_EMAIL, ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(Core.Resources.Validation))]
		public string SenderEmail { get; set; }

		[Display(Name = "UserInbox_Body", ResourceType = typeof(Core.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Resources.Validation), ErrorMessage = null)]
		public string SenderMessage { get; set; }
	}
}