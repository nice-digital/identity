using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
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
        void SendInActiveAccountRemovalNotifications(IList<string> toEmailAddresses);
}

	public class EmailService : IEmailService
	{
		private readonly string _notificationEmailHTMLPath;
		private readonly string _notificationEmailTextPath;
        private readonly string _inActiveNotificationEmailHTMLPath;
        private readonly string _inActiveNotificationEmailTextPath;

public EmailService(IWebHostEnvironment webHostEnvironment)
		{
			var pathToEmails = Path.Combine(webHostEnvironment.ContentRootPath, "Emails");
			_notificationEmailHTMLPath = Path.Combine(pathToEmails, "account_removal.html");
			_notificationEmailTextPath = Path.Combine(pathToEmails, "account_removal.txt");
            _inActiveNotificationEmailHTMLPath = Path.Combine(pathToEmails, "inactive_account_removal.html");
            _inActiveNotificationEmailTextPath = Path.Combine(pathToEmails, "inactive_account_removal.txt");
}

		public void SendPendingAccountRemovalNotifications(IList<string> toEmailAddresses)
		{
			if (toEmailAddresses == null || !toEmailAddresses.Any())
				return;

			toEmailAddresses = toEmailAddresses.Select(e => e.Trim()).Distinct().ToList();
			
			var useAllowList = (AppSettings.EmailConfig.Allowlist != null && AppSettings.EmailConfig.Allowlist.Any());
			if (useAllowList)
			{
				toEmailAddresses = toEmailAddresses.Where(emailAddress => AppSettings.EmailConfig.Allowlist.Contains(emailAddress, StringComparer.OrdinalIgnoreCase)).ToList();
			}

			if (!toEmailAddresses.Any())
			{
				return;
			}

			var bodyBuilder = new BodyBuilder();
			bodyBuilder.HtmlBody = File.ReadAllText(_notificationEmailHTMLPath);
			bodyBuilder.TextBody = File.ReadAllText(_notificationEmailTextPath);
			var messageBody = bodyBuilder.ToMessageBody();

			const string subject = "Unverified account removal";
			
			SendEmail(toEmailAddresses, subject, messageBody);
		}


        public void SendInActiveAccountRemovalNotifications(IList<string> toEmailAddresses)
        {
            if (toEmailAddresses == null || !toEmailAddresses.Any())
                return;

            toEmailAddresses = toEmailAddresses.Select(e => e.Trim()).Distinct().ToList();

            var useAllowList = (AppSettings.EmailConfig.Allowlist != null && AppSettings.EmailConfig.Allowlist.Any());
            if (useAllowList)
            {
                toEmailAddresses = toEmailAddresses.Where(emailAddress => AppSettings.EmailConfig.Allowlist.Contains(emailAddress, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            if (!toEmailAddresses.Any())
            {
                return;
            }

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = File.ReadAllText(_inActiveNotificationEmailHTMLPath);
            bodyBuilder.TextBody = File.ReadAllText(_inActiveNotificationEmailTextPath);
            var messageBody = bodyBuilder.ToMessageBody();

            const string subject = "Inactive account removal";

            SendEmail(toEmailAddresses, subject, messageBody);
        }
/// <summary>
        /// SendEmail using MailKit (because Microsoft's SMTPClient is deprecated and they recommend MailKit.
        /// </summary>
        /// <param name="toEmailAddresses">this is a list of single recipients, one email per address i.e.  it's NOT sending a single email to multiple recipients</param>
        /// <param name="subject"></param>
        /// <param name="body">in MimeEntity format - so it can include HTML and Text only versions - and text only is good for accessibility.</param>
        private static void SendEmail(IEnumerable<string> toEmailAddresses, string subject, MimeEntity body)
		{
			using (var client = new SmtpClient())
			{
				client.Connect(AppSettings.EmailConfig.Server, AppSettings.EmailConfig.Port, false);
				if (!string.IsNullOrEmpty(AppSettings.EmailConfig.Username) && !string.IsNullOrEmpty(AppSettings.EmailConfig.Password))
				{
					client.Authenticate(AppSettings.EmailConfig.Username, AppSettings.EmailConfig.Password);
				}

				var message = new MimeMessage();
				message.From.Add(MailboxAddress.Parse(AppSettings.EmailConfig.SenderAddress));
				message.Subject = subject;
				message.Body = body;

				foreach (var emailAddress in toEmailAddresses)
				{
					message.To.Clear();
					message.To.Add(MailboxAddress.Parse(emailAddress));

					client.Send(message);
				}

				client.Disconnect(true);
			}
		}

	}
}
