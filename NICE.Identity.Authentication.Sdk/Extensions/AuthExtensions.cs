#if NET461 || NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NICE.Identity.Authentication.Sdk.Configuration;
#if NET461
using Owin;
#elif NETSTANDARD2_0
using Microsoft.Extensions.DependencyInjection;
#endif

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class AuthExtensions
	{

		public static void AddAuthentication(

			IServiceCollection services,

//#if NET461
//			IAppBuilder app, 
//#endif
			IAuthConfiguration authConfiguration, 
			HttpClient httpClient = null) //, RedisConfiguration redisConfiguration)
		{

			   


		}

	}
}
#endif