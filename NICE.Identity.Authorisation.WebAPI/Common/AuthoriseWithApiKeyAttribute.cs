//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using NICE.Identity.Authorisation.WebAPI.Configuration;
//using System;
//using System.Linq;
//using System.Security;

//namespace NICE.Identity.Authorisation.WebAPI.Common
//{
//	/// <summary>
//	/// This is only here until M2M is working. once that's done, we can get rid of api key authorisation and use Auth0.
//	/// </summary>
//	public class AuthoriseWithApiKeyAttribute : Attribute, IAuthorizationFilter
//	{
//		public void OnAuthorization(AuthorizationFilterContext context)
//		{
//			if (!AppSettings.AuthorisationAPI.APIKey.HasValue)
//			{
//				throw new SecurityException("API Key route hit without an api key in configuration");
//			}

//			var apiKeyInHeader = context.HttpContext.Request.Headers["X-API-Key"];

//			if (apiKeyInHeader.Any() && Guid.TryParse(apiKeyInHeader.First(), out Guid parsedAPIKey))
//			{
//				if (!parsedAPIKey.Equals(AppSettings.AuthorisationAPI.APIKey.Value))
//				{
//					context.Result = new UnauthorizedResult();
//				}
//			}
//			else
//			{
//				context.Result = new UnauthorizedResult();
//			}
//		}
//	}
//}