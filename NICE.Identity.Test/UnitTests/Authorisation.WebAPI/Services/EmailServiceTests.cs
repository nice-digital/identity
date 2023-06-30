using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Environments;
using NICE.Identity.Authorisation.WebAPI.Factories;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;
using DataModels = NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class EmailServiceTests : TestBase
    {

        private readonly Mock<ILogger<EmailService>> _logger;
        private readonly MockWebHostEnvironment _webHostEnvironment;
        private readonly SmtpClient _smtpClient;
        private readonly int _localSmtpPort;

        public EmailServiceTests()
        {
            _logger = new Mock<ILogger<EmailService>>();
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
            var emailService = new EmailService(_webHostEnvironment, _logger.Object, _smtpClient);
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
                    .ShouldBe("<html>\r\n<head>\r\n    <Title>EmailTitle</Title>\r\n</head>\r\n<body>\r\n    HtmlEmailBody\r\n</body>\r\n</html>");
            }
        }

    }
}
