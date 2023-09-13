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

            using (var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort))
            {
                //Act
                emailService.SendEmail<TestEmailGenerator>(user);

                //Assert
                var email = emailServer.ReceivedEmail.Single();

                email.FromAddress.ToString().ShouldBe("sender@example.com");
                email.ToAddresses.Single().ToString().ShouldBe("address@example.com");

                email.MessageParts
                    .Where(x => x.HeaderData.Contains("text/plain"))
                    .Single()
                    .BodyData
                    .ShouldBe("Title - EmailTitle; Body - TextEmailBody");

                email.MessageParts
                    .Where(x => x.HeaderData.Contains("text/html"))
                    .Single()
                    .BodyData
                    .ShouldBe("<html>\r\n<head>\r\n    <Title>EmailTitle</Title>\r\n</head>\r\n<body>\r\n    HtmlEmailBody\r\n    <a href=\"mailto:contactus@example.com?subject=Test%20contact%20us%20subject\">Contact Us</a>\r\n</body>\r\n</html>");
            }
        }

        [Fact]
        public void send_pending_registration_deleted_email()
        {
            //Arrange
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, _smtpClient);

            const string userOneIdentifier = "auth|userOne";
            const string userTwoIdentifier = "auth|userTwo";

            var users = new List<User>() {
                new User {
                    NameIdentifier = userOneIdentifier,
                    EmailAddress = "UserOne@example.com" },
                new User {
                    NameIdentifier = userTwoIdentifier,
                    EmailAddress = "UserTwo@example.com" },
            };

            using (var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort))
            {
                //Act
                emailService.SendPendingRegistrationDeletedEmail(users);

                //Assert
                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "UserOne@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);
                
                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "UserTwo@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);
            }

        }
        
        [Fact]
        public void send_pending_registration_deleted_email_failure()
        {
            //Arrange
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Server = null;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Port = 0;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.SenderAddress = "invalidemailaddress";

            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, _smtpClient);

            const string userOneIdentifier = "auth|userOne";

            var users = new List<User>() {
                new User {
                    NameIdentifier = userOneIdentifier,
                    EmailAddress = "UserOne@example.com" }
            };

            using (var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort))
            {
                //Act
                emailService.SendPendingRegistrationDeletedEmail(users);

                //Assert
                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "UserOne@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);
            }

        }

        [Fact]
        public void send_marked_for_deletion_email()
        {

            //Arrange
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, new SmtpClient());

            const string userOneIdentifier = "auth|userOne";
            const string userTwoIdentifier = "auth|userTwo";

            var users = new List<User>() {
                new User {
                    NameIdentifier = userOneIdentifier,
                    EmailAddress = "UserOne@example.com",
                    IsMigrated = false },
                new User {
                    NameIdentifier = userTwoIdentifier,
                    EmailAddress = "UserTwo@example.com",
                    IsMigrated = false }
            };

            using (var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort))
            {
                //Act
                emailService.SendMarkedForDeletionEmail(users);

                //Assert Emails
                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "UserOne@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "UserTwo@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);
            }
        }

        [Fact]
        public void send_marked_for_deletion_email_failure()
        {

            //Arrange
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Server = null;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.Port = 0;
            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.SenderAddress = "invalidemailaddress";
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, new SmtpClient());

            const string userOneIdentifier = "auth|userOne";

            var users = new List<User>() {
                new User {
                    NameIdentifier = userOneIdentifier,
                    EmailAddress = "UserOne@example.com",
                    IsMarkedForDeletion = true }
            };

            using (var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort))
            {
                //Act
                emailService.SendMarkedForDeletionEmail(users);

                //Assert Emails
                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "UserOne@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                users[0].IsMarkedForDeletion.ShouldBe(false);

            }
        }

        [Fact]
        public void send_dormant_account_deleted_email()
        {

            //Arrange
            var emailService = new EmailService(_webHostEnvironment, _emailServiceLogger.Object, new SmtpClient());

            var users = new List<User>() {
                new User {
                    NameIdentifier = "auth|userOne",
                    EmailAddress = "UserOne@example.com",
                    LastLoggedInDate = DateTime.Now,
                    IsMigrated = false },
                new User {
                    NameIdentifier = "auth|userTwo",
                    EmailAddress = "UserTwo@example.com",
                    LastLoggedInDate = DateTime.Now,
                    IsMigrated = false },
                new User {
                    NameIdentifier = "auth|lastLoggedInDateNullMigrated",
                    EmailAddress = "lastLoggedInDateNullMigrated@example.com",
                    LastLoggedInDate = null,
                    IsMigrated = true },
                new User {
                    NameIdentifier = "auth|lastLoggedInDateNotNullMigrated",
                    EmailAddress = "lastLoggedInDateNotNullMigrated@example.com",
                    LastLoggedInDate = DateTime.Now,
                    IsMigrated = true },
                new User {
                    NameIdentifier = "auth|lastLoggedInDateNullNotMigrated",
                    EmailAddress = "lastLoggedInDateNullNotMigrated@example.com",
                    IsMigrated = false },
                new User {
                    NameIdentifier = "auth|lastLoggedInDateNotNullNotMigrated",
                    EmailAddress = "lastLoggedInDateNotNullNotMigrated@example.com",
                    LastLoggedInDate = DateTime.Now,
                    IsMigrated = false }
            };

            using (var emailServer = netDumbster.smtp.SimpleSmtpServer.Start(_localSmtpPort))
            {
                //Act
                emailService.SendDormantAccountDeletedEmail(users);

                //Assert Emails
                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "UserOne@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "UserTwo@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "lastLoggedInDateNullMigrated@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(0);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "lastLoggedInDateNotNullMigrated@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "lastLoggedInDateNullNotMigrated@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);

                emailServer.ReceivedEmail
                    .Where(x => x.ToAddresses.Where(x => x.ToString() == "lastLoggedInDateNotNullNotMigrated@example.com").Count() == 1)
                    .Count()
                    .ShouldBe(1);
            }
        }
    }
}
