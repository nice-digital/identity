using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.API
{
	public interface IAPIService
	{
		Task<IEnumerable<UserDetails>> FindUsers(IEnumerable<string> nameIdentifiers, HttpClient httpClient = null);
		Task<Dictionary<string, IEnumerable<string>>> FindRoles(IEnumerable<string> nameIdentifiers, string host, HttpClient httpClient = null);
	}

	public class APIService : IAPIService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly string _authorisationServiceUri;

		public APIService(IHttpContextAccessor httpContextAccessor, IAuthConfiguration authConfiguration)
		{
			_httpContextAccessor = httpContextAccessor;
			_authorisationServiceUri = authConfiguration.WebSettings.AuthorisationServiceUri;
		}

		/// <summary>
		/// Find users
		/// </summary>
		/// <param name="nameIdentifiers"></param>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		public async Task<IEnumerable<UserDetails>> FindUsers(IEnumerable<string> nameIdentifiers, HttpClient httpClient = null)
		{
			if (!nameIdentifiers.Any())
				return new List<UserDetails>();

			var httpContext = _httpContextAccessor.HttpContext;
			var accessToken = await httpContext.GetTokenAsync("access_token");
			if (string.IsNullOrEmpty(accessToken))
				throw new Exception("Access token not found");

			var client = httpClient ?? new HttpClient();
			var uri = new Uri($"{_authorisationServiceUri}{Constants.AuthorisationURLs.FindUsers}");

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
			{
				Content = new StringContent(JsonConvert.SerializeObject(nameIdentifiers), Encoding.UTF8, "application/json"),
				Headers = { 
					Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
				}
			};
			httpRequestMessage.Headers.Add(AuthenticationConstants.HeaderForAddingAllRolesForWebsite, httpContext.Request.Host.Host);

			var responseMessage = await client.SendAsync(httpRequestMessage); //call the api to get all the claims for the current user
			if (responseMessage.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<UserDetails[]>(await responseMessage.Content.ReadAsStringAsync());
			}
			throw new Exception($"Error calling the API. status code: {(int)responseMessage.StatusCode}");
		}


		/// <summary>
		/// Find Roles
		/// </summary>
		/// <param name="nameIdentifiers"></param>
		/// <param name="host"></param>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		public async Task<Dictionary<string, IEnumerable<string>>> FindRoles(IEnumerable<string> nameIdentifiers, string host, HttpClient httpClient = null)
		{
			if (!nameIdentifiers.Any())
				return new Dictionary<string, IEnumerable<string>>();

			var httpContext = _httpContextAccessor.HttpContext;
			var accessToken = await httpContext.GetTokenAsync("access_token");
			if (string.IsNullOrEmpty(accessToken))
				throw new Exception("Access token not found");

			var client = httpClient ?? new HttpClient();
			var uri = new Uri($"{_authorisationServiceUri}{Constants.AuthorisationURLs.FindRoles}{host}");

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
			{
				Content = new StringContent(JsonConvert.SerializeObject(nameIdentifiers), Encoding.UTF8, "application/json"),
				Headers = {
					Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
				}
			};
			httpRequestMessage.Headers.Add(AuthenticationConstants.HeaderForAddingAllRolesForWebsite, httpContext.Request.Host.Host);

			var responseMessage = await client.SendAsync(httpRequestMessage); //call the api to get all the claims for the current user
			if (responseMessage.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<string>>>(await responseMessage.Content.ReadAsStringAsync());
			}
			throw new Exception($"Error calling the API. status code: {(int)responseMessage.StatusCode}");
		}
	}
}
