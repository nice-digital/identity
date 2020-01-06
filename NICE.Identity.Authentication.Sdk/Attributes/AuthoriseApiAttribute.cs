//#if NETFRAMEWORK
//using System;
//using System.Web.Http.Controllers;

//namespace NICE.Identity.Authentication.Sdk.Attributes
//{
//	/// <summary>
//	/// The AuthorizeAttribute in the System.Web.Http namespace is for use in ApiController's
//	/// 
//	/// </summary>
//	public class AuthoriseApiAttribute : System.Web.Http.AuthorizeAttribute
//	{
//		public AuthoriseApiAttribute()
//		{
//		}
//		public AuthoriseApiAttribute(string roles)
//		{
//			this.Roles = roles;
//		}

//		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
//		{
//			if (filterContext.HttpContext.User.Identity.IsAuthenticated)
//			{
//				filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
//			}
//			else
//			{
//				base.HandleUnauthorizedRequest(filterContext);
//			}
//		}
//	}
//}
//#endif