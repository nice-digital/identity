using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using System;
using System.Linq;
using System.Security;

namespace NICE.Identity.Authorisation.WebAPI.Common
{
	public class AuthoriseWithApiKeyAttribute : Attribute, IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			if (!AppSettings.AuthorisationAPI.APIKey.HasValue)
			{
				throw new SecurityException("API Key route hit without an api key in configuration");
			}

			var apiKeyInHeader = context.HttpContext.Request.Headers["X-API-Key"];

			if (apiKeyInHeader.Any() && Guid.TryParse(apiKeyInHeader.First(), out Guid parsedAPIKey))
			{
				if (!parsedAPIKey.Equals(AppSettings.AuthorisationAPI.APIKey.Value))
				{
					context.Result = new NotFoundResult();
				}
			}
			else
			{
				context.Result = new NotFoundResult();
			}
		}
	}
}