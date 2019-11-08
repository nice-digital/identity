using System.Net.Http;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Test.Infrastructure;
using Xunit;

namespace NICE.Identity.Test.IntegrationTests
{
    public class WebsiteTests : TestBase
    {
        [Fact]
        public async Task CreateWebsite()
        {
//            //Arrange
//            var context = GetContext();
//            TestData.AddAll(ref context);
//            var client = GetClient(context);
//
//            //Act
//            var response = await client.PostAsJsonAsync("api/websites", new Website()
//            {
//                EnvironmentId = 1,
//                ServiceId = 1,
//                Host = "test"
//            });
//            response.EnsureSuccessStatusCode();
//            var responseString = await response.Content.ReadAsStringAsync();
//
//            //Assert
//            responseString.ShouldMatchApproved();
        }
    }
}