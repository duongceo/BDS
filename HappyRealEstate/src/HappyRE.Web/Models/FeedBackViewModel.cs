using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class FeedBackViewModel
    {
        [Display(Name = "FeedBack_Email", ResourceType = typeof(HappyRE.Core.Resources.Model))]
        [RegularExpression(Core.Const.REGEX_PATTERN_EMAIL, ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(HappyRE.Core.Resources.Validation))]
        public string Email { get; set; }

        [Display(Name = "FeedBack_Mobile", ResourceType = typeof(HappyRE.Core.Resources.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(HappyRE.Core.Resources.Validation))]
        [MaxLength(11, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(HappyRE.Core.Resources.Validation))]
        [RegularExpression(Core.Const.REGEX_PATTERN_MOBILE, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(HappyRE.Core.Resources.Validation), ErrorMessage = null)]
        public string Mobile { get; set; }
        [Display(Name = "FeedBack_Content", ResourceType = typeof(HappyRE.Core.Resources.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(HappyRE.Core.Resources.Validation))]
        public string Content { get; set; }

        public string Captcha { get; set; }
        public string Guid { get; set; }
    }
}