using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.Configuration;

namespace NICE.Identity.Authorisation.WebAPI.HealthChecks
{
	public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
	    public const string DefaultScheme = "APIKeyScheme";
	    public string Scheme => DefaultScheme;
	    public string AuthenticationType => DefaultScheme;

	    public const string APIKeyRole = "APIKeyRole";
    }

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
	    private const string ApiKeyHeaderName = "X-Api-Key";

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }

        
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.NoResult();
            }

            if (providedApiKey.Equals(AppSettings.EnvironmentConfig.HealthCheckPrivateAPIKey, StringComparison.OrdinalIgnoreCase))
            {
	            var claims = new List<Claim>
	            {
		            new Claim(ClaimTypes.NameIdentifier, "healthcheckuser"),

                    new Claim(ClaimTypes.Role, ApiKeyAuthenticationOptions.APIKeyRole)
	            };
	            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
	            var identities = new List<ClaimsIdentity> {identity};
	            var principal = new ClaimsPrincipal(identities);
	            var ticket = new AuthenticationTicket(principal, Options.Scheme);

	            return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Access denied.");
        }
    }
}
