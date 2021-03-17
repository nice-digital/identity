using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NICE.Identity.Test.Infrastructure
{
	public class FakeAuthenticationConnection : IAuthenticationConnection
	{
		readonly object _accessTokenResponse; //object so it's castable to T

		public int HitCount = 0;

		public FakeAuthenticationConnection(AccessTokenResponse accessTokenResponse = null)
		{
			_accessTokenResponse = accessTokenResponse ?? new AccessTokenResponse { AccessToken = "token", ExpiresIn = 123, TokenType = "type" };
		}

		public Task<T> GetAsync<T>(Uri uri, IDictionary<string, string> headers = null) 
		{
			throw new NotImplementedException();
		}

		public Task<T> SendAsync<T>(HttpMethod method, Uri uri, object body, IDictionary<string, string> headers = null)
		{
			HitCount++;
			return Task.FromResult((T)_accessTokenResponse);
		}

		public static FakeAuthenticationConnection Get(AccessTokenResponse accessTokenResponse = null)
		{
			return new FakeAuthenticationConnection(accessTokenResponse);
		}
	}
}
