using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using DataModels = NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class EmailServiceTests : TestBase
    {

        private readonly Mock<ILogger<EmailService>> _emailServiceLogger;
        private readonly MockWebHostEnvironment _webHostEnvironment;
        private readonly SmtpClient _smtpClient;
        private readonly int _localSmtpPort;

        public EmailServiceTests()
        {
            _emailServiceLogger = new Mock<ILogger<EmailService>>();
            _webHostEnvironment = new MockWebHostEnvironment();
            _smtpClient = new SmtpClient();

            _localSmtpPort = 3333;

            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Server = "localhost";
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Port = _localSmtpPort;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.SenderAddress = "sender@example.com";

        }

        [Fact]
        public void test_single_email()
        {
            //Arrange
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, _smtpClient);
            var user = new User() { EmailAddress = "address@example.com" };

            using var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort);

            //Act
            emailService.SendEmail<TestEmailGenerator>(user);

            //Assert
            var email = emailServer.ReceivedEmail.Single();

            email.FromAddress.ToString().ShouldBe("sender@example.com");
            email.ToAddresses.Single().ToString().ShouldBe("address@example.com");

            email.MessageParts
                .Single(x => x.HeaderData.Contains("text/plain"))
                .BodyData
                .ShouldBe("Title - EmailTitle; Body - TextEmailBody");

            email.MessageParts
                .Single(x => x.HeaderData.Contains("text/html"))
                .BodyData
                .ShouldBe("<html>\r\n<head>\r\n    <Title>EmailTitle</Title>\r\n</head>\r\n<body>\r\n    HtmlEmailBody\r\n    <a href=\"mailto:contactus@example.com?subject=Test%20contact%20us%20subject\">Contact Us</a>\r\n</body>\r\n</html>");
        }

        [Fact]
        public void send_pending_registration_deleted_email()
        {
            //Arrange
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, _smtpClient);
            
            var users = new List<User>() {
                new User {
                    NameIdentifier = "auth|userOne",
                    EmailAddress = "UserOne@example.com" },
                new User {
                    NameIdentifier = "auth|userTwo",
                    EmailAddress = "UserTwo@example.com" },
            };

            using var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort);
            //Act
            emailService.SendPendingRegistrationDeletedEmail(users);

            //Assert
            emailServer.ReceivedEmail
                .Count(x => x.ToAddresses.Count(y => y.ToString() == "UserOne@example.com") == 1)
                .ShouldBe(1);
                
            emailServer.ReceivedEmail
                .Count(x => x.ToAddresses.Count(y => y.ToString() == "UserTwo@example.com") == 1)
                .ShouldBe(1);
        }
        
        [Fact]
        public void send_pending_registration_deleted_email_failure()
        {
            //Arrange
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Server = null;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Port = 0;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.SenderAddress = "invalidemailaddress";

            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, _smtpClient);
            
            var users = new List<User>() {
                new User {
                    NameIdentifier = "auth|userOne",
                    EmailAddress = "UserOne@example.com" }
            };

            using var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort);

            //Act
            emailService.SendPendingRegistrationDeletedEmail(users);

            //Assert
            emailServer.ReceivedEmail
                .Count(x => x.ToAddresses.Count(y => y.ToString() == "UserOne@example.com") == 1)
                .ShouldBe(0);
        }

        [Fact]
        public void send_marked_for_deletion_email()
        {

            //Arrange
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, new SmtpClient());
            
            var users = new List<User>() {
                new User {
                    NameIdentifier = "auth|Migrated",
                    EmailAddress = "Migrated@example.com",
                    IsMigrated = true },
                new User {
                    NameIdentifier = "auth|NotMigrated",
                    EmailAddress = "NotMigrated@example.com",
                    IsMigrated = false }
            };

            using var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort);
            //Act
            emailService.SendMarkedForDeletionEmail(users);

            //Assert Emails
            emailServer.ReceivedEmail
                .Count(x => x.ToAddresses.Count(y => y.ToString() == "Migrated@example.com") == 1)
                .ShouldBe(0);

            emailServer.ReceivedEmail
                .Count(x => x.ToAddresses.Count(y => y.ToString() == "NotMigrated@example.com") == 1)
                .ShouldBe(1);
        }

        [Fact]
        public void send_marked_for_deletion_email_failure()
        {

            //Arrange
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Server = null;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Port = 0;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.SenderAddress = "invalidemailaddress";
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, new SmtpClient());
            
            var users = new List<User>() {
                new User {
                    NameIdentifier = "auth|userOne",
                    EmailAddress = "UserOne@example.com",
                    IsMarkedForDeletion = false }
            };

            using var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort);
            //Act
            emailService.SendMarkedForDeletionEmail(users);

            //Assert Emails
            emailServer.ReceivedEmail
                .Count(x => x.ToAddresses.Count(y => y.ToString() == "UserOne@example.com") == 1)
                .ShouldBe(0);

            users[0].IsMarkedForDeletion.ShouldBe(false);
        }

        [Fact]
        public void send_dormant_account_deleted_email()
        {

            //Arrange
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, new SmtpClient());

            var users = new List<User>() {
                new User {
                    NameIdentifier = "auth|Migrated",
                    EmailAddress = "Migrated@example.com",
                    LastLoggedInDate = DateTime.Now,
                    IsMigrated = true },
                new User {
                    NameIdentifier = "auth|NotMigrated",
                    EmailAddress = "NotMigrated@example.com",
                    LastLoggedInDate = DateTime.Now,
                    IsMigrated = false },
            };

            using var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort);
            //Act
            emailService.SendDormantAccountDeletedEmail(users);

            //Assert Emails
            emailServer.ReceivedEmail
                .Count(x => x.ToAddresses.Count(y => y.ToString() == "Migrated@example.com") == 1)
                .ShouldBe(0);

            emailServer.ReceivedEmail
                .Count(x => x.ToAddresses.Count(y => y.ToString() == "NotMigrated@example.com") == 1)
                .ShouldBe(1);
        }
    }
}
