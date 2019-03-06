using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NICE.Identity.TestClient.M2MApp.Common;
using NICE.Identity.TestClient.M2MApp.Configuration;
using NICE.Identity.TestClient.M2MApp.Models;

namespace NICE.Identity.TestClient.M2MApp.Services
{
    public interface ITokenService
    {
        JwtToken GetToken();
    }

    public class Auth0TokenService : ITokenService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public Auth0TokenService(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public JwtToken GetToken()
        {
            var url = $"https://{AppSettings.Auth0Config.Domain}/oauth/token";

	        var request = new HttpRequestMessage(HttpMethod.Post, url)
	        {
		        Content = new StringContent("{\"grant_type\":\"client_credentials\"," +
		                                    "\"client_id\": \"" + AppSettings.Auth0Config.ClientId + "\"," +
		                                    "\"client_secret\": \"" + AppSettings.Auth0Config.ClientSecret + "\"," +
		                                    "\"audience\": \"" + AppSettings.Auth0Config.ApiIdentifier + "\"}",
			    Encoding.UTF8,
			    "application/json")
	        };

	        var response = HttpResponseHelpers.Send(_client, request);
			if (response.StatusCode != HttpStatusCode.OK)
			{
				throw new HttpRequestException("An Error Occured");
			}

			var content = HttpResponseHelpers.GetString(response);
            var token = JsonConvert.DeserializeObject<JwtToken>(content);
           
            return token;
        }
    }
}