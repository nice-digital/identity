using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

		public APIService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_authorisationServiceUri = configuration.GetSection("WebAppConfiguration").GetSection("AuthorisationServiceUri").Value; //TODO: revisit this.
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

			var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
			if (string.IsNullOrEmpty(accessToken))
				throw new Exception("Access token not found");

			var client = httpClient ?? new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			var uri = new Uri($"{_authorisationServiceUri}{Constants.AuthorisationURLs.FindUsers}");

			var nameIdentifiersInJSON = JsonConvert.SerializeObject(nameIdentifiers);
			var content = new StringContent(nameIdentifiersInJSON, Encoding.UTF8, "application/json");

			var responseMessage = await client.PostAsync(uri, content); //call the api to get all the claims for the current user
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

			var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
			if (string.IsNullOrEmpty(accessToken))
				throw new Exception("Access token not found");

			var client = httpClient ?? new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			var uri = new Uri($"{_authorisationServiceUri}{Constants.AuthorisationURLs.FindRoles}{host}");

			var nameIdentifiersInJSON = JsonConvert.SerializeObject(nameIdentifiers);
			var content = new StringContent(nameIdentifiersInJSON, Encoding.UTF8, "application/json");

			var responseMessage = await client.PostAsync(uri, content); //call the api to get all the claims for the current user
			if (responseMessage.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<string>>>(await responseMessage.Content.ReadAsStringAsync());
			}
			throw new Exception($"Error calling the API. status code: {(int)responseMessage.StatusCode}");
		}
	}
}
