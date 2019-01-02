using NICE.Identity.Models;
using NICE.Identity.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace NICE.Identity.Test.IntegrationTests
{
	public class DatabaseTests : TestBase
	{
		[Fact]
		public void WriteEntry()
		{
			//Arrange
			var context = GetContext();
			var auditService = new AuditService(context);
			var now = DateTime.Now;
			var userId = Guid.NewGuid();
			const string tableName = "User";
			const string fieldName = "FirstName";
			const string oldValue = "Peter";
			const string newValue = "Phil";

			//Act
			var rowsUpdated = auditService.WriteAuditEntry(new Audit(tableName, fieldName, oldValue, newValue, now, userId));
			var allAuditLogs = context.Audit.ToList();

			//Assert
			rowsUpdated.ShouldBe(1);
			var auditRecord = allAuditLogs.Single();
			auditRecord.UserId.ShouldBe(userId);
			auditRecord.Date.ShouldBe(now);
			auditRecord.FieldName.ShouldBe(fieldName);
			auditRecord.TableName.ShouldBe(tableName);
			auditRecord.OldValue.ShouldBe(oldValue);
			auditRecord.NewValue.ShouldBe(newValue);
		}
	}
}
