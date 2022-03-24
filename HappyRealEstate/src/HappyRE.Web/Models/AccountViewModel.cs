using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using MBN.Utils.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
	public class ProfileLoginInfo
	{
		public ProfileLoginInfo()
		{
			//FirstName = LastName = Member = Token = "";
		}

		public Guid? ClientId { get; set; }
		public int ProfileId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Avatar { get; set; }
		public bool IsAuth { get; set; }
		public string Member { get; set; }
		public string GAUserId { get; set; }
		public string Token { get; set; }
		public int TotalFavorite { get; set; }
		public bool Thirdparty { get; set; }
		//public Models.AlertSmall Alert { get; set; }
	}

	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class ExternalLoginListViewModel
	{
		public string ReturnUrl { get; set; }
		public Dictionary<string, string> OpenId { get; set; }
		public bool SignUp = false;

		public string GetJson()
		{
			string appId = "", version = "", state = "", redirectUrl = "";
			if (OpenId != null)
			{
				OpenId.TryGetValue("ZALO_APP_ID", out appId);
				OpenId.TryGetValue("ZALO_APP_VERSION", out version);
				OpenId.TryGetValue("ZALO_APP_CALLBACK", out redirectUrl);
				OpenId.TryGetValue("STATE", out state);
				redirectUrl += "?cmd=" + (this.SignUp ? "signup" : "");
			}

			return (new { version, appId, redirectUrl, state }).ToJson();
		}
	}

	public class SendCodeViewModel
	{
		public string SelectedProvider { get; set; }
		public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
		public string ReturnUrl { get; set; }
		public bool RememberMe { get; set; }
	}

	public class VerifyCodeViewModel
	{
		[Required]
		public string Provider { get; set; }

		[Display(Name = "ConfirmMobile_VerifiedCode", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		public string Code { get; set; }
		public string ReturnUrl { get; set; }

		[Display(Name = "Remember this browser?")]
		public bool RememberBrowser { get; set; }

		public bool RememberMe { get; set; }
	}

	public class ForgotViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class LoginViewModel
	{
		[Display(Name = "UserProfile_Mobile", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		[RegularExpression(Conts.REGEX_PATTERN_MOBILE, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string Mobile { get; set; }

		[Display(Name = "UserProfile_Password", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }

		public string GoogleCaptchaResponse { get; set; }
		public string ReturnUrl { get; set; }
		public Dictionary<string, string> OpenId { get; set; }

	}

	public class ACKResponseModel
	{
		[Display(Name = "UserProfile_FirstName", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string FirstName { get; set; }

		[Display(Name = "UserProfile_LastName", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string LastName { get; set; }

		[Display(Name = "UserProfile_Mobile", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		[RegularExpression(Conts.REGEX_PATTERN_MOBILE, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string Mobile { get; set; }

		[Display(Name = "UserProfile_Password", ResourceType = typeof(Core.Entities.Resources.Model))]
		[RegularExpression(Conts.VALIDATION_PASSWORD, ErrorMessageResourceName = "PaswordInvalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public int ZoneId { get; set; }
		public int LevelId { get; set; }
		public int UserTypeId { get; set; }
		public int? ReferalCode { get; set; }

		public string Code { get; set; }
		public string Csrf_Nonce { get; set; }
	}

	public class RegisterModel
	{
		public RegisterModel()
		{
		}

		public RegisterModel(UserProfile user)
		{
			FirstName = user.FullName;
			LastName = user.FullName;
			Email = user.Email;
			Mobile = user.Mobile;
			CityId = 1;// user.CityId;
			LevelId = user.LevelId??0;
			IsVerifiedMobile = user.IsVerifyMobile;
			IsVerifiedEmail = user.IsVerifyEmail;
		}

		public RegisterModel(ACKResponseModel model)
		{
			this.Mobile = model.Mobile;
			this.FirstName = model.FirstName;
			this.LastName = model.LastName;
			this.Password = model.Password;
			this.ZoneId = model.ZoneId;
			this.LevelId = model.LevelId;
			this.UserTypeId = model.UserTypeId;
		}

		public RegisterModel(OTPConfirmViewModel model)
		{
			this.Mobile = model.Mobile;
			this.FirstName = model.FirstName;
			this.LastName = model.LastName;
			this.Password = model.Password;
			this.ZoneId = model.ZoneId;
			this.LevelId = model.LevelId;
			this.UserTypeId = model.UserTypeId;
		}

		[Display(Name = "UserProfile_FirstName", ResourceType = typeof(HappyRE.Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string FirstName { get; set; }

		[Display(Name = "UserProfile_LastName", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string LastName { get; set; }

		[Display(Name = "UserProfile_Mobile", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		//[Range(0, Int64.MaxValue, ErrorMessageResourceName = "FieldMustBeNumeric", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		[RegularExpression(Conts.REGEX_PATTERN_MOBILE, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string Mobile { get; set; }

		[Display(Name = "UserProfile_Password", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[RegularExpression(Conts.VALIDATION_PASSWORD, ErrorMessageResourceName = "PaswordInvalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		//[DataType(DataType.Password)]
		//[Display(Name = "Register_ConfirmPassword", ResourceType = typeof(Core.Entities.Resources.Model))]
		//[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		//[Compare("Password", ErrorMessageResourceName = "PaswordConfirmInvalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		//public string ConfirmPassword { get; set; }

		[Display(Name = "UserProfile_Email", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		[RegularExpression(Conts.REGEX_PATTERN_EMAIL, ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		public string Email { get; set; }

		[Display(Name = "UserProfile_CityId", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		public int CityId { get; set; }

		[Display(Name = "UserProfile_ZoneId", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		public int ZoneId { get; set; }

		[Display(Name = "ConfirmMobile_Code", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[RegularExpression(Conts.VALIDATION_OTP, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		public string Code { get; set; }

		public DateTime? Birthday { get; set; }
		public string Avatar { get; set; }
		public string IDCard { get; set; }
		public string IDCard1 { get; set; }
		public string IDCard2 { get; set; }
		public int UserTypeId { get; set; }

		public bool IsVerifiedMobile { get; set; }
		public bool IsVerifiedEmail { get; set; }
		public bool IsMogiPro { get; set; }
		public int LevelId { get; set; }
		public bool IsAnonymous()
		{
			return (LevelId == 0);
		}

		public string Agreement { get; set; }
		//public AccountKitAuthModel ACK { get; set; }
		public AccountReferalModel Referal { get; set; }
		public Dictionary<string, string> OpenId { get; set; }
		public string Step { get; set; }
		public string ReturnUrl { get; set; }
	}

	public class ForgotPasswordViewModel
	{
		[Display(Name = "UserProfile_Mobile", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		[RegularExpression(Conts.REGEX_PATTERN_MOBILE, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string Mobile { get; set; }

		[Display(Name = "UserProfile_NewPassword", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[RegularExpression(Conts.VALIDATION_PASSWORD, ErrorMessageResourceName = "PaswordInvalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "ConfirmMobile_Code", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[RegularExpression(Conts.VALIDATION_OTP, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		public string Code { get; set; }
		public string GoogleCaptchaResponse { get; set; }
		//public AccountKitAuthModel ACK { get; set; }
		public Dictionary<string, string> OpenId { get; set; }
		public bool Facebook { get; set; }
		public string FacebookCode { get; set; }
		public string Csrf_Nonce { get; set; }

	}

	public class OTPConfirmViewModel
	{
		[Display(Name = "UserProfile_FirstName", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string FirstName { get; set; }

		[Display(Name = "UserProfile_LastName", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string LastName { get; set; }

		[Display(Name = "UserProfile_Mobile", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		[RegularExpression(Conts.REGEX_PATTERN_MOBILE, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string Mobile { get; set; }

		[Display(Name = "UserProfile_Password", ResourceType = typeof(Core.Entities.Resources.Model))]
		[RegularExpression(Conts.VALIDATION_PASSWORD, ErrorMessageResourceName = "PaswordInvalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public int ZoneId { get; set; }
		public int LevelId { get; set; }
		public int UserTypeId { get; set; }

		[Display(Name = "ConfirmMobile_Code", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[RegularExpression(Conts.VALIDATION_OTP, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		public string Code { get; set; }
		public string GoogleCaptchaResponse { get; set; }

		//public AccountKitAuthModel ACK { get; set; }
		public bool Facebook { get; set; }
		public string ReturnUrl { get; set; }
		public bool NeedUpdate()
		{
			return string.IsNullOrEmpty(this.Mobile) || string.IsNullOrEmpty(this.FirstName) || string.IsNullOrEmpty(this.LastName);
		}
	}

	public class OTPSendModel
	{
		[Display(Name = "UserProfile_Mobile", ResourceType = typeof(Core.Entities.Resources.Model))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		[MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation))]
		[RegularExpression(Conts.REGEX_PATTERN_MOBILE, ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string Mobile { get; set; }

		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Core.Entities.Resources.Validation), ErrorMessage = null)]
		public string Command { get; set; }

		public string GoogleCaptchaResponse { get; set; }
	}

	[Serializable]
	public class OTPSession
	{
		public int ProfileId { get; set; }
		public string Mobile { get; set; }
		public long Tick { get; set; }
		public string Code { get; set; }
	}

	public class OpenIDUserModel
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string Provider { get; set; }

		public bool IsGoogle()
		{
			return this.Provider == Core.Entities.Conts.PROFILE_PROVIDER_GOOGLE;
		}
		public bool IsFacebook()
		{
			return this.Provider == Core.Entities.Conts.PROFILE_PROVIDER_FACEBOOK;
		}

		public bool IsZalo()
		{
			return this.Provider == Core.Entities.Conts.PROFILE_PROVIDER_ZALO;
		}
	}

	public class Notification
	{
		public string Title { get; set; }
		public string Description { get; set; }
	}

	public class AccountReferalModel
	{
		public int LeadId { get; set; }
		public Guid? LeadKey { get; set; }
	}
}