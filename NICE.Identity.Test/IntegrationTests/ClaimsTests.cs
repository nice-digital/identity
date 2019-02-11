using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.Models;
using NICE.Identity.Test.Infrastructure;
using Xunit;

namespace NICE.Identity.Test.IntegrationTests
{
	public class ClaimsTests : TestBase
	{
		[Fact]
		public async Task GetClaims()
		{
			//Arrange
			var context = GetContext();
			AddTestData(context);
			var client = GetClient(context);

			//Act
			var response = await client.GetAsync("api/claims/1");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			//Assert
			responseString.ShouldMatchApproved();
		}

		public void AddTestData(IdentityContext context)
		{
			context.Services.Add(new Services(1, "Comments"));
			context.Environments.Add(new Environments(1, "Test"));
			context.Websites.Add(new Websites(1, 1, 1, "test.com"));
			context.Roles.Add(new Roles(1, 1, "Admin"));
			context.UserRoles.Add(new UserRoles(1, 1, 1));
			context.Users.Add(new Users(1, null, null, null, "Joe", "Bloggs", true, true, null, null, true, null, null, null,
				true, true));
			context.SaveChanges();
		}
	}
}
