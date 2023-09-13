using Microsoft.Extensions.Logging;
using Moq;
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

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Factories
{
    public class EmailGeneratorTests : TestBase
    {

        private readonly Mock<ILogger<UsersService>> _usersLogger;
        private readonly Mock<ILogger<RolesService>> _Roleslogger;
        private readonly Mock<ILogger<WebsitesService>> _websiteslogger;
        private readonly Mock<ILogger<ServicesService>> _serviceslogger;
        private readonly Mock<ILogger<EnvironmentsService>> _environmentslogger;
        private readonly Mock<IProviderManagementService> _providerManagementService;
        private readonly Mock<IEmailService> _emailService;

        readonly private string _htmlTemplate = "<html><head><Title><%%%TITLE%%%></Title></head><body><%%%BODY%%%></body></html>";
        readonly private string _textTemplate = "Title - <%%%TITLE%%%>; Body - <%%%BODY%%%>";
        readonly private string _fromAddress = "sender@example.com";
        private readonly string _tabs;
        private readonly string _tabsLevelTwo;

        public EmailGeneratorTests()
        {
            _tabs = "\t\t\t\t\t\t\t\t\t\t\t";
            _tabsLevelTwo = _tabs + "\t\t";

            _usersLogger = new Mock<ILogger<UsersService>>();
            _Roleslogger = new Mock<ILogger<RolesService>>();
            _websiteslogger = new Mock<ILogger<WebsitesService>>();
            _serviceslogger = new Mock<ILogger<ServicesService>>();
            _environmentslogger = new Mock<ILogger<EnvironmentsService>>();
            _providerManagementService = new Mock<IProviderManagementService>();
            _emailService = new Mock<IEmailService>();

            Identity.Authorisation.WebAPI.Configuration.AppSettings.EmailConfig.SenderAddress = _fromAddress;
        }

        [Fact]
        public void generate_lapsed_registration_account_removal_notification_email()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _usersLogger.Object, _providerManagementService.Object, _emailService.Object);

            var emailGenerationUser = "auth|emailGenerationUser_" + Guid.NewGuid();

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = emailGenerationUser,
                FirstName = "Email Generation",
                LastName = "User",
                EmailAddress = "EmailGenerationUser@example.com"
            });

            var emailGenerator = new DeletePendingRegistrationNotificationEmail(_htmlTemplate, _textTemplate);
            var user = context.FindUsers(emailGenerationUser).Single();

            //Act
            var email = emailGenerator.GenerateEmail(user);

            //Assert
            var expectedTitle = "Your unverified NICE Account has been deleted";
            var expectedSubject = "Unverified account removal";
            var expectedHtmlInnerBody = "\r\n" +
                                        $"{_tabs}Your account has been pending activation for 30 days. Unfortunately, the time allowed for activating your account has elapsed.\r\n" +
                                        $"{_tabs}<br /><br />\r\n" +
                                        $"{_tabs}We have deleted your details in compliance with GDPR. If you still want to register for a NICE account using this email address, you will have to re-submit your details on the <a href=\"https://www.nice.org.uk\">NICE website</a>.\r\n";
            var expectedTextInnerBody = "Your account has been pending activation for 30 days. Unfortunately, the time allowed for activating your account has elapsed.\r\n" +
                                        "\r\n" +
                                        "We have deleted your details in compliance with GDPR. If you still want to register for a NICE account using this email address, you will have to re-submit your details on the NICE website.\r\n";

            var expectedHTMLBody = _htmlTemplate.Replace("<%%%TITLE%%%>", expectedTitle).Replace("<%%%BODY%%%>", expectedHtmlInnerBody);
            var expectedTextBody = _textTemplate.Replace("<%%%TITLE%%%>", expectedTitle).Replace("<%%%BODY%%%>", expectedTextInnerBody);

            email.Subject.ShouldBe(expectedSubject);
            email.From.Mailboxes.First().Address.ShouldBe(_fromAddress);
            email.To.Mailboxes.First().Address.ShouldBe(user.EmailAddress);
            email.HtmlBody.ShouldBe(expectedHTMLBody);
            email.TextBody.ShouldBe(expectedTextBody);

        }

        [Fact]
        public void generate_dormant_account_removal_notification_email()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _usersLogger.Object, _providerManagementService.Object, _emailService.Object);

            var emailGenerationUser = "auth|emailGenerationUser_" + Guid.NewGuid();

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = emailGenerationUser,
                FirstName = "Email Generation",
                LastName = "User",
                EmailAddress = "EmailGenerationUser@example.com"
            });

            var emailGenerator = new DormantAccountRemovalNotificationEmailGenerator(_htmlTemplate, _textTemplate);
            var user = context.FindUsers(emailGenerationUser).Single();

            //Act
            var email = emailGenerator.GenerateEmail(user);

            //Assert
            var expectedTitle = "Your dormant NICE Account has been deleted";
            var expectedSubject = "Dormant account deletion";
            var expectedHtmlInnerBody = "\r\n" +
                                        $"{_tabs}Your NICE Account has not been used for 3 years. Unfortunately, we had to delete your details in compliance with GDPR.\r\n" +
                                        $"{_tabs}<br /><br />\r\n" +
                                        $"{_tabs}If you want to re-register for a NICE Account using this email address, you will have to re-submit your details on the <a href=\"https://www.nice.org.uk\">NICE website</a>.\r\n";
            var expectedTextInnerBody = "Your NICE Account has not been used for 3 years. Unfortunately, we had to delete your details in compliance with GDPR.\r\n" +
                                        "\r\n" +
                                        "If you want to re-register for a NICE Account using this email address, you will have to re-submit your details on the NICE website.\r\n";

            var expectedHTMLBody = _htmlTemplate.Replace("<%%%TITLE%%%>", expectedTitle).Replace("<%%%BODY%%%>", expectedHtmlInnerBody);
            var expectedTextBody = _textTemplate.Replace("<%%%TITLE%%%>", expectedTitle).Replace("<%%%BODY%%%>", expectedTextInnerBody);

            email.Subject.ShouldBe(expectedSubject);
            email.From.Mailboxes.First().Address.ShouldBe(_fromAddress);
            email.To.Mailboxes.First().Address.ShouldBe(user.EmailAddress);
            email.HtmlBody.ShouldBe(expectedHTMLBody);
            email.TextBody.ShouldBe(expectedTextBody);

        }

        [Fact]
        public void generate_pending_dormant_account_with_roles_removal_notification_email()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _usersLogger.Object, _providerManagementService.Object, _emailService.Object);
            var roleService = new RolesService(context, _Roleslogger.Object);
            var websiteService = new WebsitesService(context, _websiteslogger.Object);
            var serviceService = new ServicesService(context, _serviceslogger.Object);
            var environmentService = new EnvironmentsService(context, _environmentslogger.Object);

            var emailGenerationUser = "auth|emailGenerationUser_" + Guid.NewGuid();

            environmentService.CreateEnvironment(new ApiModels.Environment
            {
                Name = "Live",
                Order = 1,
            });
            var environment = context.Environments.Single();

            serviceService.CreateService(new ApiModels.Service
            {
                Name = "ServiceOne"
            });
            var serviceOne = context.Services.Where(s => s.Name == "ServiceOne").Single();

            serviceService.CreateService(new ApiModels.Service
            {
                Name = "ServiceTwo"
            });
            var serviceTwo = context.Services.Where(s => s.Name == "ServiceTwo").Single();

            websiteService.CreateWebsite(new ApiModels.Website
            {
                EnvironmentId = environment.EnvironmentId,
                Host = "www.WebsiteOne.org.uk",
                ServiceId = serviceOne.ServiceId
            });
            var websiteOne = context.Websites.Where(x => x.Host == "www.WebsiteOne.org.uk").Single();

            websiteService.CreateWebsite(new ApiModels.Website
            {
                EnvironmentId = environment.EnvironmentId,
                Host = "www.WebsiteTwo.org.uk",
                ServiceId = serviceTwo.ServiceId
            });
            var websiteTwo = context.Websites.Where(x => x.Host == "www.WebsiteTwo.org.uk").Single();

            roleService.CreateRole(new ApiModels.Role
            {
                Name = "ServiceOneRole",
                Description = "ServiceOne Role Description",
                WebsiteId = websiteOne.WebsiteId
            });
            var serviceOneRole = roleService.GetRoles().Where(x => x.Name == "ServiceOneRole").Single();

            roleService.CreateRole(new ApiModels.Role
            {
                Name = "ServicetTwoRole",
                Description = "ServiceTwo Role Description",
                WebsiteId = websiteTwo.WebsiteId
            });
            var serviceTwoRole = roleService.GetRoles().Where(x => x.Name == "ServicetTwoRole").Single();

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = emailGenerationUser,
                FirstName = "Email Generation",
                LastName = "User",
                EmailAddress = "EmailGenerationUser@example.com"
            });
            var arrangeUser = context.FindUsers(emailGenerationUser).Single();

            var roles = new List<Identity.Authorisation.WebAPI.ApiModels.UserRole> {
                new ApiModels.UserRole { UserId = arrangeUser.UserId, RoleId = serviceOneRole.RoleId },
                new ApiModels.UserRole { UserId = arrangeUser.UserId, RoleId = serviceTwoRole.RoleId }
            };

            userService.UpdateRolesForUser(arrangeUser.UserId, roles);

            var actUser = context.FindUsers(emailGenerationUser).Single();

            var emailGenerator = new PendingDormantAccountRemovalNotificationEmailGenerator(_htmlTemplate, _textTemplate);

            //Act
            var email = emailGenerator.GenerateEmail(actUser);

            //Assert
            var expectedTitle = "Your dormant NICE Account will be deleted soon";
            var expectedSubject = "Pending dormant account deletion";
            var expectedHtmlInnerBody = "\r\n" +
                                        $"{_tabs}Your NICE Account has not been used for 3 years so it is either dormant or no longer required.\r\n" +
                                        $"{_tabs}<br /><br />\r\n" +
                                        $"{_tabs}We must delete inactive accounts in line with our data retention policy and to comply with GDPR. We cannot retain your details if you have not used the account to interact with NICE for 3 years or longer.\r\n" +
                                        $"{_tabs}<br /><br />\r\n" +
                                        $"{_tabs}If you wish to retain your NICE account, you must sign-in within 30 calendar days from today on the <a href=\"https://www.nice.org.uk\">NICE website</a>. Otherwise, you may lose access to the following NICE systems:\r\n" +
                                        $"{_tabs}<br /><br />\r\n" +
                                        $"{_tabs}<ul>\r\n" +
                                        $"{_tabsLevelTwo}<li>ServiceOne</li>\r\n" +
                                        $"{_tabsLevelTwo}<li>ServiceTwo</li>\r\n" +
                                        $"{_tabs}</ul>\r\n";
            var expectedTextInnerBody = "Your NICE Account has not been used for 3 years so it is either dormant or no longer required.\r\n" +
                                        "\r\n" +
                                        "We must delete inactive accounts in line with our data retention policy and to comply with GDPR. We cannot retain your details if you have not used the account to interact with NICE for 3 years or longer.\r\n" +
                                        "\r\n" +
                                        "If you wish to retain your NICE account, you must sign-in within 30 calendar days from today on the NICE website. Otherwise, you may lose access to the following NICE systems:\r\n" +
                                        "\r\n" +
                                        "ServiceOne\r\n" +
                                        "ServiceTwo\r\n" + 
                                        "\r\n";

            var expectedHTMLBody = _htmlTemplate.Replace("<%%%TITLE%%%>", expectedTitle).Replace("<%%%BODY%%%>", expectedHtmlInnerBody);
            var expectedTextBody = _textTemplate.Replace("<%%%TITLE%%%>", expectedTitle).Replace("<%%%BODY%%%>", expectedTextInnerBody);

            email.Subject.ShouldBe(expectedSubject);
            email.From.Mailboxes.First().Address.ShouldBe(_fromAddress);
            email.To.Mailboxes.First().Address.ShouldBe(actUser.EmailAddress);
            email.HtmlBody.ShouldBe(expectedHTMLBody);
            email.TextBody.ShouldBe(expectedTextBody);

        }

        [Fact]
        public void generate_pending_dormant_account_without_roles_removal_notification_email()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _usersLogger.Object, _providerManagementService.Object, _emailService.Object);
            var roleService = new RolesService(context, _Roleslogger.Object);
            var websiteService = new WebsitesService(context, _websiteslogger.Object);
            var serviceService = new ServicesService(context, _serviceslogger.Object);
            var environmentService = new EnvironmentsService(context, _environmentslogger.Object);

            var emailGenerationUser = "auth|emailGenerationUser_" + Guid.NewGuid();

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = emailGenerationUser,
                FirstName = "Email Generation",
                LastName = "User",
                EmailAddress = "EmailGenerationUser@example.com"
            });

            var actUser = context.FindUsers(emailGenerationUser).Single();

            var emailGenerator = new PendingDormantAccountRemovalNotificationEmailGenerator(_htmlTemplate, _textTemplate);

            //Act
            var email = emailGenerator.GenerateEmail(actUser);

            //Assert
            var expectedTitle = "Your dormant NICE Account will be deleted soon";
            var expectedSubject = "Pending dormant account deletion";
            var expectedHtmlInnerBody = "\r\n" +
                                        $"{_tabs}Your NICE Account has not been used for 3 years so it is either dormant or no longer required.\r\n" +
                                        $"{_tabs}<br /><br />\r\n" +
                                        $"{_tabs}We must delete inactive accounts in line with our data retention policy and to comply with GDPR. We cannot retain your details if you have not used the account to interact with NICE for 3 years or longer.\r\n" +
                                        $"{_tabs}<br /><br />\r\n" +
                                        $"{_tabs}If you wish to retain your NICE account, you must sign-in within 30 calendar days from today on the <a href=\"https://www.nice.org.uk\">NICE website</a>.\r\n";
            var expectedTextInnerBody = "Your NICE Account has not been used for 3 years so it is either dormant or no longer required.\r\n" +
                                        "\r\n" +
                                        "We must delete inactive accounts in line with our data retention policy and to comply with GDPR. We cannot retain your details if you have not used the account to interact with NICE for 3 years or longer.\r\n" +
                                        "\r\n" +
                                        "If you wish to retain your NICE account, you must sign-in within 30 calendar days from today on the NICE website.\r\n";

            var expectedHTMLBody = _htmlTemplate.Replace("<%%%TITLE%%%>", expectedTitle).Replace("<%%%BODY%%%>", expectedHtmlInnerBody);
            var expectedTextBody = _textTemplate.Replace("<%%%TITLE%%%>", expectedTitle).Replace("<%%%BODY%%%>", expectedTextInnerBody);

            email.Subject.ShouldBe(expectedSubject);
            email.From.Mailboxes.First().Address.ShouldBe(_fromAddress);
            email.To.Mailboxes.First().Address.ShouldBe(actUser.EmailAddress);
            email.HtmlBody.ShouldBe(expectedHTMLBody);
            email.TextBody.ShouldBe(expectedTextBody);

        }
    }
}
