using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace NICE.Identity.Authentication.Sdk
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationSdk(this IServiceCollection services)
        {
            return services;
        }
    }
}