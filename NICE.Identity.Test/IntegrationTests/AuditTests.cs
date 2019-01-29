using NICE.Identity.Models;
using NICE.Identity.Test.Infrastructure;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace NICE.Identity.Test.IntegrationTests
{
	public class AuditTests : TestBase
	{
		[Fact]
		public async Task GetAudit()
		{
			//Arrange
			var context = GetContext();
			context.Audit.Add(new Audit("User", "FirstName", "Peter", "Phil", DateTime.ParseExact("03-Jan-2019 10:25", "dd-MMM-yyyy hh:mm", DateTimeFormatInfo.InvariantInfo), Guid.Empty));
			context.SaveChanges();
			var client = GetClient(context);

			//Act
			var response = await client.GetAsync("/admin/api/Audit");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			//Assert
			responseString.ShouldMatchApproved(new Func<string, string>[]{ Scrubbers.ScrubAuditId });
		}
	}
}
