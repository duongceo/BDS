using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


using MBN.Utils;
using MBN.Utils.Extension;
using System.Web.Http.Results;
using System.Net.Http.Headers;
using System.Text;
using HappyRE.Core.MapModels.FrontEnd;
using Microsoft.AspNet.Identity;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using System.Web;

namespace HappyRE.Web.Controllers.API
{
	//[EnableCors("*","*","*", PreflightMaxAge = 2592000, SupportsCredentials = true)]
	[AllowAnonymous]
	[RoutePrefix("api/common")]
    public class CommonController : BaseAPIController
    {
        public CommonController(IUow uow) : base(uow) {}

		[HttpOptions]
		public HttpResponseMessage Options()
		{
			return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
		}

		#region Favorite
		[HttpPost]
        [Route("favorite-add")]
        public bool Favorite_Add(int propertyId)
        {
            try
            {
                if (propertyId <= 0) return false;
                if (this.IsAuthenticated())
                {
                    var profileId = this.UserData.ProfileId;
                    _uow.Favorite.Add(propertyId, profileId);
                }
                else
                {
                    _uow.Favorite.Add(propertyId, this.ClientId());
                }

                return true;
            }
            catch (BusinessException ex)
            {
                throw Error(HttpStatusCode.BadRequest, "BusinessInvalid", ex.Message);
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                this.WriteError("CommonController.Favorite_Add", ex);
            }
            throw Error(HttpStatusCode.InternalServerError, "InternalServerError", Core.Resources.Message.GeneralError);
        }

        [HttpPost]
        [Route("favorite-remove")]
        public bool Favorite_Remove(int propertyId)
        {
            try
            {
                if (propertyId <= 0) return false;
                if (this.IsAuthenticated())
                {
                    var profileId = this.UserData.ProfileId;
                    _uow.Favorite.Remove(propertyId, profileId);
                }
                else
                {
                    _uow.Favorite.Remove(propertyId, this.ClientId());
                }

                return true;
            }
            catch (BusinessException ex)
            {
                throw Error(HttpStatusCode.BadRequest, "BusinessInvalid", ex.Message);
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                this.WriteError("CommonController.Favorite_Remove", ex);
            }
            throw Error(HttpStatusCode.InternalServerError, "InternalServerError", Core.Resources.Message.GeneralError);
        }

		[HttpPost,HttpGet,HttpOptions]
        [Route("favorite-get-list")]
        public string[] Favorite_GetList(string[] ids)
        {
            try
            {
				if (ids == null || ids.Length == 0 || ids.Length > 30) return null;

                if (this.IsAuthenticated())
                {
                    var profileId = this.UserData.ProfileId;
                    return _uow.Favorite.GetByProperties(ids, profileId);
                }
                else
                {
                    return _uow.Favorite.GetByProperties(ids, this.ClientId());
                }
            }
            catch (BusinessException ex)
            {
                throw Error(HttpStatusCode.BadRequest, "BusinessInvalid", ex.Message);
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                this.WriteError("CommonController.Favorite_GetList", ex);
            }
            throw Error(HttpStatusCode.InternalServerError, "InternalServerError", Core.Resources.Message.GeneralError);
        }

		#endregion

		#region Suggest
		[WebAPIOutputCache(CacheProfile = "Suggest", SharedMaxAge = Const.CACHE_CLIENT_ONEDAY)]
		[HttpPost,HttpGet]
        [Route("suggest-map")]
        public async Task<HttpResponseMessage> Suggest_MapAsync(string q)
        {
			return await Task.Run(() =>
			{
				var data = _uow.Map.Suggest(q);
				return Request.CreateResponse(HttpStatusCode.OK, data, "application/json");
			});
        }

		[WebAPIOutputCache(CacheProfile = "Suggest", SharedMaxAge = Const.CACHE_CLIENT_ONEDAY)]
		[HttpPost, HttpGet]
        [Route("suggest-agent")]
        public async Task<HttpResponseMessage> Suggest_AgentAsync(string q)
        {
            //return _uow.UserProfile.FrontEnd_Suggest(q);
			return await Task.Run(() =>
			{
				var data = _uow.UserProfile.FrontEnd_Suggest(q);
				return Request.CreateResponse(HttpStatusCode.OK, data, "application/json");
			});
		}

		[WebAPIOutputCache(CacheProfile = "Suggest", SharedMaxAge = Const.CACHE_CLIENT_ONEDAY)]
		[HttpPost, HttpGet]
        [Route("suggest-project")]
        public async Task<HttpResponseMessage> Suggest_ProjectAsync(string q)
        {
            //return _uow.Project.FrontEnd_Suggest(q);
			return await Task.Run(() =>
			{
				var data = _uow.Project.FrontEnd_Suggest(q);
				return Request.CreateResponse(HttpStatusCode.OK, data, "application/json");
			});
		}

		[WebAPIOutputCache(CacheProfile = "Suggest", SharedMaxAge = Const.CACHE_CLIENT_ONEDAY)]
		[HttpPost, HttpGet]
        [Route("suggest-street")]
        public async Task<HttpResponseMessage> Suggest_StreetAsync(string q)
        {
			return await Task.Run(() =>
			{
				var data = _uow.Map.Suggest_Street(q);
				return Request.CreateResponse(HttpStatusCode.OK, data, "application/json");
			});
		}
        #endregion

        #region Service
        [HttpPost, HttpGet]
        [Route("service-top")]
        public dynamic Service_Top(int id, bool rent = false, int pageIndex = 1)
        {
            return _uow.Property.FrontEnd_ServiceTop(id, rent, pageIndex);
        }
		#endregion

		#region Profile
		[AllowAnonymous]
		[Route("profile-get-info")]
		public async Task<HttpResponseMessage> GetInfoAsync()
		{
			var model = new ProfileLoginInfo()
			{
				ProfileId = 0,
				FirstName = "",
				LastName = "",
				IsAuth = User.Identity.IsAuthenticated,
				Member = "member",
				GAUserId = "",
				Token = "",
				TotalFavorite = 0,
				Thirdparty = Core.Utils.Common.JavaScriptThirdparty

			};
			if (model.IsAuth == true)
			{
				var claim = this.UserData;
				if (claim != null)
				{
					model.FirstName = (claim.FirstName ?? "");
					model.LastName = (claim.LastName ?? "");
					model.Email = (claim.Email ?? "");
					model.Avatar = (string.IsNullOrEmpty(claim.Avatar) == false ? claim.Avatar : Core.Utils.Common.AvatarLoginUrl);
					model.ProfileId = claim.ProfileId;

					if (claim.IsMogiPro == true)
					{
						model.Member = "mogipro";
					}
					model.GAUserId = $"agent.{claim.ProfileId}" + (string.IsNullOrEmpty(claim.Mobile) ? "" : $".m{claim.Mobile}");
					model.Token = Helpers.LoginHelper.GetToken(claim); // Token
				}
				else
				{
					try
					{
						HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
					}
					catch { }
				}
			}

			if (model.ProfileId == 0)
			{
				model.ClientId = this.ClientId();
			}

			string searchKey = WebUtils.GetQuery("searchkey", "");

			return await Task.Run(() =>
			{				
				if (model.IsAuth == true)
				{
					// AlertSearch
					if (string.IsNullOrEmpty(searchKey) == false)
					{
						bool has_alert = _uow.AlertSearch.HasAlertSearch(model.ProfileId);
						if (has_alert == true)
						{
							model.Alert = _uow.AlertSearch.Get(model.ProfileId, searchKey);
						}
					}

					// Favorite
					try { model.TotalFavorite = _uow.Favorite.GetTotal(model.ProfileId, model.ClientId); } catch { }
				}

				string body = $"window.MOGI = {model.ToJson()};";
				if (string.IsNullOrEmpty(model.GAUserId) == false)
				{
					body += "window.dataLayer=window.dataLayer || [];window.dataLayer.push({\"userID\":\"" + model.GAUserId + "\"});";
				}
				var resp = this.Request.CreateResponse(HttpStatusCode.OK);
				resp.Content = new StringContent(body, Encoding.UTF8, "application/json");
				return resp;
			});
		}

		[AllowAnonymous]
		[Route("profile-get-inbox")]
		public dynamic GetInbox()
		{
			if (User.Identity.IsAuthenticated == false)
			{
				return new List<MessageItem>();
			}
			var res = _uow.UserInbox.GetLastestMessages(this.UserData.ProfileId, 3);

			return res;
		}
		#endregion

		#region Tracking
		/// <summary>
		/// Tracking xem chi tiết tin
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpGet]
		[Route("tracking-property")]
		public HttpResponseMessage TrackingProperty(int id)
		{
			BLL.Queues.PropertyTrackingQueue.EnqueueView(id, this.ClientIP());

			return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
		}
		#endregion
	}
}
