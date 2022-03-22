using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Security;

namespace HappyRE.Web
{
	public class MogiAuthorizationServerProvider : OAuthAuthorizationServerProvider
	{
		public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			await Task.Run(() =>
			{
				context.Validated();
			});
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			await Task.Run(() =>
			{
				Core.Models.UserProfile profile = null;
				try
				{
					if (string.IsNullOrEmpty(context.UserName) == false && string.IsNullOrEmpty(context.Password) == false)
					{
						var uow = Core.ObjectFactory.GetInstance<Core.Interfaces.IUow>();
						profile = uow.UserProfile.ValidUser(context.UserName, context.Password);
					}
				}
				catch (Core.BusinessException ex)
				{
					context.SetError("LoginFailed", ex.Message);
					return;
				}
				catch (Exception)
				{
					context.SetError("LoginFailed", Core.Resources.Message.GeneralError);
					return;
				}

				if (profile == null)
				{
					context.SetError("LoginFailed", Core.Resources.Message.User_InvalidLogin);
					return;
				}

				var ticket = Helpers.LoginHelper.GetTicket(profile);
				context.Validated(ticket);
			});
		}

		public override Task TokenEndpoint(OAuthTokenEndpointContext context)
		{
			foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
			{
				context.AdditionalResponseParameters.Add(property.Key, property.Value);
			}

			return Task.FromResult<object>(null);
		}
		public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
		{
			return base.GrantRefreshToken(context);
		}
	}
}


