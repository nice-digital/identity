using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using DataModels = NICE.Identity.Authorisation.WebAPI.DataModels;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;

namespace NICE.Identity.Test.IntegrationTests.Authorisation.WebAPI
{
    public class UserTests : TestBase
    {
        private readonly Mock<ILogger<EmailService>> _emailServiceLogger;
        private readonly Mock<ILogger<UsersService>> _userServiceLogger;
        private readonly MockWebHostEnvironment _webHostEnvironment;
        private readonly Mock<IProviderManagementService> _providerManagementService;
        private readonly int _localSmtpPort;

        public UserTests()
        {
            _providerManagementService = new Mock<IProviderManagementService>();
            _emailServiceLogger = new Mock<ILogger<EmailService>>();
            _userServiceLogger = new Mock<ILogger<UsersService>>();
            _webHostEnvironment = new MockWebHostEnvironment();

            _localSmtpPort = 61272;

            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Server = "localhost";
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Port = _localSmtpPort;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.SenderAddress = "sender@example.com";

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task delete_registrations_older_than(bool notify)
        {
            //Arrange
            var context = GetContext();
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, new SmtpClient());
            var userService = new UsersService(context, _userServiceLogger.Object, _providerManagementService.Object, emailService);

            const string user1NameIdentifier = "auth|user1";
            const string user2NameIdentifier = "auth|user2";

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = user1NameIdentifier,
                FirstName = "FirstName1",
                LastName = "LastName1",
                EmailAddress = "user1@example.com",
                HasVerifiedEmailAddress = false
            });
            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = user2NameIdentifier,
                FirstName = "User to be deleted",
                LastName = "",
                EmailAddress = "user2@example.com",
                HasVerifiedEmailAddress = false
            });

            context.Users.Single(u => u.NameIdentifier == user1NameIdentifier).InitialRegistrationDate = DateTime.Now.AddDays(-30);
            context.Users.Single(u => u.NameIdentifier == user2NameIdentifier).InitialRegistrationDate = DateTime.Now.AddDays(-31);

            context.SaveChanges();

            using (var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort))
            {
                //Act
                await userService.DeleteRegistrationsOlderThan(notify, daysToKeepPendingRegistration: 30);

                //Assert
                if (notify)
                {
                    emailServer.ReceivedEmailCount.ShouldBe(1);
                }
                else
                {
                    emailServer.ReceivedEmailCount.ShouldBe(0);
                }

                context.Users
                    .Where(x => x.NameIdentifier == user1NameIdentifier)
                    .SingleOrDefault()
                    .ShouldNotBeNull();

                context.Users
                    .Where(x => x.NameIdentifier == user2NameIdentifier)
                    .SingleOrDefault()
                    .ShouldBeNull();
            }

        }

        [Fact]
        public async Task send_pending_dormant_account_notification_emails()
        {

            //Arrange
            var context = GetContext();
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, new SmtpClient());
            var userService = new UsersService(context, _userServiceLogger.Object, _providerManagementService.Object, emailService);

            var baseDate = new DateTime(2020, 6, 1); //Arbitrary Base Date
            var monthsTillDormant = Identity.Authorisation.WebAPI.Configuration.AppSettings.EnvironmentConfig.MonthsUntilDormantAccountsDeleted;
            
            var beforePendingDeletionWindowDate = baseDate.AddMonths(-monthsTillDormant).AddDays(31);
            var insidePendingDeletionWindowDate = baseDate.AddMonths(-monthsTillDormant).AddDays(30);
            var beyondPendingDeletionWindowDate = baseDate.AddMonths(-monthsTillDormant);

            var niceEmployeeIdentifier = "auth|niceEmployee_" + Guid.NewGuid();
            var alreadyPendingIdentifier = "auth|AlreadyPendingDeletion_" + Guid.NewGuid();
            var pendingDeletionIdentifier = "auth|PendingDeletion_" + Guid.NewGuid();
            var notPendingDeletion = "auth|NotPendingDeletion_" + Guid.NewGuid();
            var beyondPendingDeletionWindowIdentifier = "auth|BeyondPendingDeletionWindow_" + Guid.NewGuid();
            var migratedUserIdentifier = "auth|MigratedUser_" + Guid.NewGuid();

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = niceEmployeeIdentifier,
                FirstName = "Nice",
                LastName = "Employee",
                EmailAddress = "NiceEmployee@nice.org.uk",
                IsMarkedForDeletion = false,
                LastLoggedInDate = insidePendingDeletionWindowDate
            });
            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = alreadyPendingIdentifier,
                FirstName = "AlreadyPending",
                LastName = "Deletion",
                EmailAddress = "AlreadyPendingDeletion@example.com",
                IsMarkedForDeletion = true,
                LastLoggedInDate = insidePendingDeletionWindowDate
            });
            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = pendingDeletionIdentifier,
                FirstName = "Pending",
                LastName = "Deletion",
                EmailAddress = "PendingDeletion@example.com",
                IsMarkedForDeletion = false,
                LastLoggedInDate = insidePendingDeletionWindowDate
            });
            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = notPendingDeletion,
                FirstName = "NotPending",
                LastName = "Deletion",
                EmailAddress = "NotPendingDeletion@example.com",
                IsMarkedForDeletion = false,
                LastLoggedInDate = beforePendingDeletionWindowDate
            });
            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = beyondPendingDeletionWindowIdentifier,
                FirstName = "BeyondPending",
                LastName = "DeletionWindow",
                EmailAddress = "BeyondPendingDeletionWindow@example.com",
                IsMarkedForDeletion = false,
                LastLoggedInDate = beyondPendingDeletionWindowDate
            });
            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = migratedUserIdentifier,
                FirstName = "MigratedUser",
                LastName = "InsideWindow",
                EmailAddress = "MigratedUserInsideWindow@example.com",
                IsMarkedForDeletion = false,
                IsMigrated = true,
                LastLoggedInDate = insidePendingDeletionWindowDate
            });

            context.SaveChanges();

            using (var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort))
            {

                //Act
                await userService.MarkAccountsForDeletion(baseDate);

                //Assert Emails
                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "NiceEmployee@nice.org.uk").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "AlreadyPendingDeletion@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "PendingDeletion@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "NotPendingDeletion@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "BeyondPendingDeletionWindow@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "MigratedUserInsideWindow@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                //Assert Flags
                context.FindUsers(niceEmployeeIdentifier)
                    .Single()
                    .IsMarkedForDeletion
                    .ShouldBe(false);

                context.FindUsers(alreadyPendingIdentifier)
                    .Single()
                    .IsMarkedForDeletion
                    .ShouldBe(true);

                context.FindUsers(pendingDeletionIdentifier)
                    .Single()
                    .IsMarkedForDeletion
                    .ShouldBe(true);

                context.FindUsers(notPendingDeletion)
                    .Single()
                    .IsMarkedForDeletion
                    .ShouldBe(false);

                context.FindUsers(beyondPendingDeletionWindowIdentifier)
                    .Single()
                    .IsMarkedForDeletion
                    .ShouldBe(false);

                context.FindUsers(migratedUserIdentifier)
                    .Single()
                    .IsMarkedForDeletion
                    .ShouldBe(true);
            }
        }

        [Fact]
        public async Task delete_dormant_accounts()
        {
            //Arrange
            var context = GetContext();
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, new SmtpClient());
            var userService = new UsersService(context, _userServiceLogger.Object, _providerManagementService.Object, emailService);

            var baseDate = new DateTime(2020, 6, 1); //Arbitrary Base Date
            var monthsTillDormant = Identity.Authorisation.WebAPI.Configuration.AppSettings.EnvironmentConfig.MonthsUntilDormantAccountsDeleted;
            
            var thisDateWillTriggerDeletion = baseDate.AddMonths(-monthsTillDormant).AddDays(-1);
            var thisDateWillTriggerDeletionExact = baseDate.AddMonths(-monthsTillDormant);
            var thisDateWillNotTriggerDeletion = baseDate.AddMonths(-monthsTillDormant).AddDays(1);

            var niceEmployeeDontDeleteIdentifier = "auth|EmployeeDontDeleteIdentifier_" + Guid.NewGuid();
            var triggerDeletionIdentifier = "auth|TriggerDeletion_" + Guid.NewGuid();
            var triggerDeletionExactIdentifier = "auth|TriggerDeletionExact_" + Guid.NewGuid();
            var notTriggerDeletionIdentifier = "auth|NotTriggerDeletion_" + Guid.NewGuid();
            var triggerDeletionLastLoginDateNullMigratedIdentifier = "auth|TriggerDeletionLastLoginDateNullMigrated_" + Guid.NewGuid();
            var triggerDeletionLastLoginDateNullNotMigratedIdentifier = "auth|TriggerDeletionLastLoginDateNullNotMigrated_" + Guid.NewGuid();
            var notTriggerDeletionLastLoginDateNullMigratedIdentifier = "auth|NotTriggerDeletionLastLoginDateNullMigrated_" + Guid.NewGuid();
            var notTriggerDeletionLastLoginDateNullNotMigratedIdentifier = "auth|NotTriggerDeletionLastLoginDateNullNotMigrated_" + Guid.NewGuid();

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = niceEmployeeDontDeleteIdentifier,
                FirstName = "Nice Employee",
                LastName = "Don't Delete",
                EmailAddress = "NiceEmployeeDontDelete@nice.org.uk",
                LastLoggedInDate = thisDateWillTriggerDeletion
            });

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = triggerDeletionIdentifier,
                FirstName = "Trigger",
                LastName = "Deletion",
                EmailAddress = "TriggerDeletion@example.com",
                LastLoggedInDate = thisDateWillTriggerDeletion
            });

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = triggerDeletionExactIdentifier,
                FirstName = "Trigger",
                LastName = "Deletion Exact",
                EmailAddress = "TriggerDeletionExact@example.com",
                LastLoggedInDate = thisDateWillTriggerDeletionExact
            });

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = notTriggerDeletionIdentifier,
                FirstName = "Not Trigger",
                LastName = "Deletion",
                EmailAddress = "NotTriggerDeletion@example.com",
                LastLoggedInDate = thisDateWillNotTriggerDeletion
            });

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = triggerDeletionLastLoginDateNullMigratedIdentifier,
                FirstName = "Trigger Deletion Last Login Date",
                LastName = "Null Migrated",
                IsMigrated = true,
                EmailAddress = "TriggerDeletionLastLoginDateNullMigrated@example.com"
            });

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = triggerDeletionLastLoginDateNullNotMigratedIdentifier,
                FirstName = "Trigger Deletion Last Login Date",
                LastName = "Null Not Migrated",
                IsMigrated = false,
                EmailAddress = "TriggerDeletionLastLoginDateNullNotMigrated@example.com"
            });

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = notTriggerDeletionLastLoginDateNullMigratedIdentifier,
                FirstName = "Not Trigger Deletion Last Login Date",
                LastName = "Null Migrated",
                IsMigrated = true,
                EmailAddress = "NotTriggerDeletionLastLoginDateNullMigrated@example.com"
            });

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = notTriggerDeletionLastLoginDateNullNotMigratedIdentifier,
                FirstName = "Not Trigger Deletion Last Login Date",
                LastName = "Null Not Migrated",
                IsMigrated = false,
                EmailAddress = "NotTriggerDeletionLastLoginDateNullNotMigrated@example.com"
            });

            context.Users.Single(u => u.NameIdentifier == triggerDeletionLastLoginDateNullMigratedIdentifier).InitialRegistrationDate = thisDateWillTriggerDeletion;
            context.Users.Single(u => u.NameIdentifier == triggerDeletionLastLoginDateNullNotMigratedIdentifier).InitialRegistrationDate = thisDateWillTriggerDeletion;
            context.Users.Single(u => u.NameIdentifier == notTriggerDeletionLastLoginDateNullMigratedIdentifier).InitialRegistrationDate = thisDateWillNotTriggerDeletion;
            context.Users.Single(u => u.NameIdentifier == notTriggerDeletionLastLoginDateNullNotMigratedIdentifier).InitialRegistrationDate = thisDateWillNotTriggerDeletion;

            context.SaveChanges();
            context.SaveChanges();

            using (var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort))
            {
                //Act
                await userService.DeleteDormantAccounts(baseDate);

                //Assert Emails
                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "NiceEmployee@nice.org.uk").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "TriggerDeletion@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "TriggerDeletionExact@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "NotTriggerDeletion@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "TriggerDeletionLastLoginDateNullMigrated@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "TriggerDeletionLastLoginDateNullNotMigrated@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "NotTriggerDeletionLastLoginDateNullMigrated@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "NotTriggerDeletionLastLoginDateNullNotMigrated@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                //Assert Context Users
                context.FindUsers(niceEmployeeDontDeleteIdentifier)
                    .SingleOrDefault()
                    .ShouldNotBeNull();

                context.FindUsers(triggerDeletionIdentifier)
                    .SingleOrDefault()
                    .ShouldBeNull();

                context.FindUsers(triggerDeletionExactIdentifier)
                    .SingleOrDefault()
                    .ShouldBeNull();

                context.FindUsers(notTriggerDeletionIdentifier)
                    .SingleOrDefault()
                    .ShouldNotBeNull();

                context.FindUsers(triggerDeletionLastLoginDateNullMigratedIdentifier)
                    .SingleOrDefault()
                    .ShouldBeNull();

                context.FindUsers(triggerDeletionLastLoginDateNullNotMigratedIdentifier)
                    .SingleOrDefault()
                    .ShouldBeNull();

                context.FindUsers(notTriggerDeletionLastLoginDateNullMigratedIdentifier)
                    .SingleOrDefault()
                    .ShouldNotBeNull();

                context.FindUsers(notTriggerDeletionLastLoginDateNullNotMigratedIdentifier)
                    .SingleOrDefault()
                    .ShouldNotBeNull();
            }
        }
    }
}
