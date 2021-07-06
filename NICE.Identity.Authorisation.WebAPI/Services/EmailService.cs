using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IEmailService
	{
		void SendPendingAccountRemovalNotifications(IList<string> toEmailAddresses);
	}

	public class EmailService : IEmailService
	{
		private readonly ILogger<EmailService> _logger;
		private readonly IWebHostEnvironment _webHostEnvironment;

		private readonly string _notificationEmailHTMLPath;
		private readonly string _notificationEmailTextPath;

		public EmailService(ILogger<EmailService> logger, IWebHostEnvironment webHostEnvironment)
		{
			_logger = logger;
			_webHostEnvironment = webHostEnvironment;
			
			var pathToEmails = Path.Combine(_webHostEnvironment.ContentRootPath, "Emails");
			_notificationEmailHTMLPath = Path.Combine(pathToEmails, "account_removal.html");
			_notificationEmailTextPath = Path.Combine(pathToEmails, "account_removal.txt");
		}

		public void SendPendingAccountRemovalNotifications(IList<string> toEmailAddresses)
		{
			if (toEmailAddresses == null || !toEmailAddresses.Any())
				return;

			toEmailAddresses = toEmailAddresses.Select(e => e.Trim()).ToList();
			
			var useAllowList = AppSettings.EmailConfig.Allowlist.Any();

			var bodyBuilder = new BodyBuilder();
			bodyBuilder.HtmlBody = File.ReadAllText(_notificationEmailHTMLPath);
			bodyBuilder.TextBody = File.ReadAllText(_notificationEmailTextPath);
			var messageBody = bodyBuilder.ToMessageBody();

			const string subject = "Notification of account removal";

			foreach (var toEmailAddress in toEmailAddresses)
			{
				if (useAllowList && !AppSettings.EmailConfig.Allowlist.Contains(toEmailAddress, StringComparer.OrdinalIgnoreCase))
				{
					break;
				}
				
				SendEmail(toEmailAddress, subject, messageBody);
			}
		}

		private static void SendEmail(string toEmailAddress, string subject, MimeEntity body)
		{
			var message = new MimeMessage();
			message.From.Add(MailboxAddress.Parse(AppSettings.EmailConfig.SenderAddress));
			message.To.Add(MailboxAddress.Parse(toEmailAddress));
			message.Subject = subject;
			message.Body = body;

			using (var client = new SmtpClient())
			{
				client.Connect(AppSettings.EmailConfig.Server, AppSettings.EmailConfig.Port, false);

				if (!string.IsNullOrEmpty(AppSettings.EmailConfig.Username) && !string.IsNullOrEmpty(AppSettings.EmailConfig.Password))
				{
					client.Authenticate(AppSettings.EmailConfig.Username, AppSettings.EmailConfig.Password);
				}

				client.Send(message);
				client.Disconnect(true);
			}
		}

	}
}
