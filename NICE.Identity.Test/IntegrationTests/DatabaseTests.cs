using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace NICE.Identity.Test.IntegrationTests
{
	public class DatabaseTests : TestBase
	{
		//[Fact]
		//public void WriteEntry()
		//{
		//	//Arrange
		//	var context = GetContext();
		//	var auditService = new AuditService(context);
		//	var auditEntry = new Audit("User", "FirstName", "Peter", "Phil", DateTime.Now, Guid.NewGuid());

		//	//Act
		//	var rowsUpdated = auditService.WriteAuditEntry(auditEntry);
		//	var allAuditLogs = context.Audit.ToList();

		//	//Assert
		//	rowsUpdated.ShouldBe(1);
		//	var auditRecord = allAuditLogs.Single();
		//	auditRecord.UserId.ShouldBe(auditEntry.UserId);
		//	auditRecord.Date.ShouldBe(auditEntry.Date);
		//	auditRecord.FieldName.ShouldBe(auditEntry.FieldName);
		//	auditRecord.TableName.ShouldBe(auditEntry.TableName);
		//	auditRecord.OldValue.ShouldBe(auditEntry.OldValue);
		//	auditRecord.NewValue.ShouldBe(auditEntry.NewValue);
		//}
	}
}
