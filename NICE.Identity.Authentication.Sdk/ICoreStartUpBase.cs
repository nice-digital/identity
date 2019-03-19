using System;
using Microsoft.AspNetCore.Hosting;

namespace NICE.Identity.Authentication.Sdk
{
    public interface ICoreStartUpBase : IStartup
    {
        IServiceProvider ServiceProvider { get; }
    }
}