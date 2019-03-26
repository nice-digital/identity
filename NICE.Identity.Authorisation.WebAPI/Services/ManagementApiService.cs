using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authorisation.WebAPI.Configuration;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IManagementApiService
	{
		Task<JwtToken> GetToken();
	}

	public class ManagementApiService : IManagementApiService
	{
		private static readonly HttpClient _client = new HttpClient();

		public async Task<JwtToken> GetToken()
		{
			_client.BaseAddress = new Uri("https://" + AppSettings.ManagementAPI.Domain);

			var request = new
			{
				grant_type = AppSettings.ManagementAPI.Grant_Type,
				client_id = AppSettings.ManagementAPI.ClientId,
				client_secret = AppSettings.ManagementAPI.ClientSecret,
				audience = AppSettings.ManagementAPI.ApiIdentifier
			};

			var httpResponse = await _client.PostAsync("oauth/token",
				new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

			if (httpResponse.StatusCode != HttpStatusCode.OK)
			{
				throw new HttpRequestException("An Error Occured");
			}

			var token = await httpResponse.Content.ReadAsAsync<JwtToken>();

			return token;
		}
	}
}