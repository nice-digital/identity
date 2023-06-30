using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IEmailService
	{
        void SendEmail<T>(User user) where T : IEmailGenerator;
    }

    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISmtpClient _smtpClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IWebHostEnvironment webHostEnvironment, ILogger<EmailService> logger, ISmtpClient smtpClient)
        {
            _webHostEnvironment = webHostEnvironment;
            _smtpClient = smtpClient;
        }

        /// <summary>
        /// SendEmail using MailKit (because Microsoft's SMTPClient is deprecated and they recommend MailKit.
        /// </summary>
        /// <param name="user">a user to send the email to</param>
        public void SendEmail<T>(User user) where T : IEmailGenerator
        {
            if (user == null) return;

            //Get generator based on template
            var pathToEmailTemplates = Path.Combine(_webHostEnvironment.ContentRootPath, "Emails");

            var htmlTemplate = File.ReadAllText(Path.Combine(pathToEmailTemplates, "BasicTemplate.html"));
            var textTemplate = File.ReadAllText(Path.Combine(pathToEmailTemplates, "BasicTemplate.txt"));

            var emailGenerator = (T) Activator.CreateInstance(typeof(T), args: new[] { htmlTemplate, textTemplate });

            //Create connection to SMTP Server
            _smtpClient.Connect(AppSettings.EmailConfig.Server, AppSettings.EmailConfig.Port, false);
				
            if (!string.IsNullOrEmpty(AppSettings.EmailConfig.Username) && !string.IsNullOrEmpty(AppSettings.EmailConfig.Password))
			{
                _smtpClient.Authenticate(AppSettings.EmailConfig.Username, AppSettings.EmailConfig.Password);
			}

            //Check allow list
            var useAllowList = (AppSettings.EmailConfig.Allowlist != null && AppSettings.EmailConfig.Allowlist.Any());
            if (useAllowList)
            {
                if (!AppSettings.EmailConfig.Allowlist.Contains(user.EmailAddress, StringComparer.OrdinalIgnoreCase))
                {
                    return;
                }

            }

            //Send
            _smtpClient.Send(emailGenerator.GenerateEmail(user));

            _smtpClient.Disconnect(true);
		}

	}
}
