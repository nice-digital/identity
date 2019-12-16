using System;
using System.Collections.Generic;
using System.Text;

namespace NICE.Identity.Authentication.Sdk
{
	public static class Constants
	{
		public static class AuthorisationURLs
		{
			public const string GetClaims = "/api/claims/";

			private const string ApiUsersPath = "/api/users/";
			public const string FindUsersRoute = "findusers";
			public const string FindUsersFullPath = ApiUsersPath + FindUsersRoute;

			public const string FindRolesRoute = "findroles/";
			public const string FindRolesFullPath = ApiUsersPath + FindRolesRoute;
			
			public const string GetRolesByWebsiteRoute = "getrolesbywebsite";
			public const string GetRolesByWebsiteFullPath = "/api/roles/" + GetRolesByWebsiteRoute + "/";

		}
	}
}
