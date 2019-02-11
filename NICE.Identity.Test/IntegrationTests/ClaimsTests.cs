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
			TestData.AddAll(ref context);
			var client = GetClient(context);

			//Act
			var response = await client.GetAsync("api/claims/1");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			//Assert
			responseString.ShouldMatchApproved();
		}

		
	}
}
