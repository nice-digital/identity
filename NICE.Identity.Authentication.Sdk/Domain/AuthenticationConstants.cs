namespace NICE.Identity.Authentication.Sdk.Domain
{
	public static class AuthenticationConstants
	{
		public const string AuthenticationScheme = "Auth0";

		/// <summary>
		/// This issuer string is used for claims like the user's name, which is owned by IdAMIssuer.
		/// Other issuers are typically going to be the website host for roles.
		/// </summary>
		public const string IdAMIssuer = "IdAMIssuer";

		public const string CookieName = "IdAM";


		public const string NameIdentifierDefaultPrefix = "auth0|";

		public const string HeaderForAddingAllRolesForWebsite = "X-HOST";

		public const string JWTAuthenticationScheme = "Bearer";

		public const string ClientCredentials = "client-credentials";

		public static class Tokens
		{
			public const string IdToken = "id_token";
			public const string AccessToken = "access_token";
			public const string AccessTokenExpires = "expires_at";
			public const string TokenType = "token_type";
			public const string RefreshToken = "refresh_token";

			public const string ExpiresIn = "expires_in"; //this token is used in the refresh token response
		}

	}
}