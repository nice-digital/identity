using Auth0.AuthenticationApi;
using Autofac;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using Microsoft.Owin.Hosting;

namespace NICE.Identity.TestClient.NETFramework461.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = Container();

            var url = "https://localhost:44052";

            // only way i can get it to call Owin startup
            // but for CrawlerScheduler we use TopShelf so how does that fit in?
            using (WebApp.Start(url))   
            {
                System.Console.WriteLine("Server running on {0}", url);


                System.Console.ReadLine();
            }
        }

        private static IContainer Container()
        {
            var builder = new ContainerBuilder();

            var authConfiguration = new AuthConfiguration(IdAMWebConfigSection.GetConfig());

            var config = IdAMWebConfigSection.GetConfig();
            System.Console.WriteLine("--- Idam settings ---");
            System.Console.WriteLine("  ApiIdentifier: " + config.ApiIdentifier);
            System.Console.WriteLine("  ClientId: " + config.ClientId);
            System.Console.WriteLine("  ClientSecret: " + config.ClientSecret);
            System.Console.WriteLine("  AuthorisationServiceUri: " + config.AuthorisationServiceUri);
            System.Console.WriteLine("  Domain: " + config.Domain);
            System.Console.WriteLine("  PostLogoutRedirectUri: " + config.PostLogoutRedirectUri);
            System.Console.WriteLine("  RedirectUri: " + config.RedirectUri);
            System.Console.WriteLine("  CallBackPath: " + config.CallBackPath);
            System.Console.WriteLine("  GoogleTrackingId: " + config.GoogleTrackingId);
            System.Console.WriteLine("  RedisEnabled: " + config.RedisEnabled);
            System.Console.WriteLine("  RedisConnectionString: " + config.RedisConnectionString);
            System.Console.WriteLine("---------------------");

            builder.RegisterInstance(authConfiguration)
                .As<IAuthConfiguration>()
                .SingleInstance();

            builder.RegisterType<HttpClientAuthenticationConnection>()
                .As<IAuthenticationConnection>()
                .SingleInstance();

            builder.RegisterType<ApiTokenClient>()
                .As<IApiTokenClient>()
                .SingleInstance();

            return builder.Build();
        }
    }
}
