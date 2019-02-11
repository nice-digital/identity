using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Linq;
using NICE.Identity.Authorisation.WebAPI.Models.Responses;
using Xunit;

namespace NICE.Identity.Test.UnitTests
{
	public class ClaimsServiceTests : TestBase
	{
		[Fact]
		public void UserIsReturnedInClaims()
		{
			//Arrange
			var context = GetContext();
			TestData.AddWithTwoRoles(ref context);
			var claimsService = new ClaimsService(context);

			//Act
			var claims = claimsService.GetClaims(1);

			//Assert
			claims.Single(claim => claim.Type.Equals(ClaimType.FirstName)).Value.ShouldBe("Steve");
		}


	}
}
