using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NICE.Identity.TestClient.M2MApp.Common;
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
            var domain = _config["Auth0:Domain"];

            var url = $"https://{domain}/oauth/token";

	        var request = new HttpRequestMessage(HttpMethod.Post, url)
	        {
		        Content = new StringContent("{\"grant_type\":\"client_credentials\"," +
		                                    "\"client_id\": \"" + _config["Auth0:ClientId"] + "\"," +
		                                    "\"client_secret\": \"" + _config["Auth0:ClientSecret"] + "\"," +
		                                    "\"audience\": \"" + _config["Auth0:ApiIdentifier"] + "\"}",
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