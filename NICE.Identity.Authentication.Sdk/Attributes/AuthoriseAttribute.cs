//using System;
//using System.Web.Mvc;

//namespace NICE.Identity.Authentication.Sdk.Attributes
//{
//	public class AuthoriseAttribute : System.Web.Mvc.AuthorizeAttribute
//	{
//		public AuthoriseAttribute() { }
//		public AuthoriseAttribute(string roles)
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
//    }
//}
