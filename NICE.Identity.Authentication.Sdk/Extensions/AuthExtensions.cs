#if NET461 || NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Owin;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class AuthExtensions
	{

		public static void AddAuthentication(IServiceCollection services, IAppBuilder app, IAuthConfiguration authConfiguration, HttpClient httpClient = null) //, RedisConfiguration redisConfiguration)
		{

			   


		}

	}
}
#endif