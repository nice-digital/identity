using System;
using NICE.Identity.Test.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace NICE.Identity.Test.IntegrationTests
{
	public class HomeTests : TestBase
	{
		[Fact]
		public async Task GetHomepage()
		{
			//Arrange
			var client = GetClient();

			//Act
			var response = await client.GetAsync("/");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			//Assert
			responseString.ShouldMatchApproved(new Func<string, string>[] { Scrubbers.ScrubCookieString });
		}

	}
}
