using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	public interface IApiToken
    {
	    Task<JwtToken> GetAccessToken(string domain, string clientId, string clientSecret, string audience);
		Task<JwtToken> GetAccessToken(IAuthConfiguration authConfiguration);
    }

    public class ApiToken : IApiToken
    {
	    public async Task<JwtToken> GetAccessToken(string domain, string clientId, string clientSecret, string audience)
	    {
		    using (var client = new HttpClient() { BaseAddress = new Uri($"https://{domain}") })
		    {
			    var tokenRequestFormContent = new FormUrlEncodedContent(new Dictionary<string, string>
			    {
				    {"client_id", clientId },
				    {"client_secret", clientSecret },
				    {"audience", audience },
				    {"grant_type", $"client_credentials" }
			    });
			    tokenRequestFormContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
			    var response = await client.PostAsync("/oauth/token", tokenRequestFormContent);
			    if (response.IsSuccessStatusCode)
			    {
				    var serialisedJwtToken = await response.Content.ReadAsStringAsync();
				    return JsonConvert.DeserializeObject<JwtToken>(serialisedJwtToken);
			    }
			    return null;
		    }
        }

        public async Task<JwtToken> GetAccessToken(IAuthConfiguration authConfiguration)
        {
	        return await GetAccessToken(authConfiguration.TenantDomain,
		        authConfiguration.WebSettings.ClientId,
		        authConfiguration.WebSettings.ClientSecret,
		        audience: authConfiguration.MachineToMachineSettings.ApiIdentifier);
        }
    }
}