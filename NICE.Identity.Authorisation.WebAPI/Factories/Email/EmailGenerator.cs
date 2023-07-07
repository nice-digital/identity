using MimeKit;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using System;

namespace NICE.Identity.Authorisation.WebAPI.Factories
{
    public interface IEmailGenerator
    {
        public MimeMessage GenerateEmail(DataModels.User user);
    }

    public abstract class EmailGenerator
    {
        private readonly string _emailHTMLTemplate;
        private readonly string _emailTextTemplate;

        public EmailGenerator(string emailHTMLTemplate, string emailTextTemplate)
        {

            _emailHTMLTemplate = emailHTMLTemplate;
            _emailTextTemplate = emailTextTemplate;

        }

        protected MimeMessage GetEmail(string title, string subject, string htmlBody, string textBody)
        {
            return GetEmail(null, title, subject, htmlBody, textBody);
        }

        protected MimeMessage GetEmail(User user, string title, string subject, string htmlBody, string textBody)
        {
            var bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = substitutePlaceholders(_emailHTMLTemplate, title, htmlBody);
            bodyBuilder.TextBody = substitutePlaceholders(_emailTextTemplate, title, textBody);

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(AppSettings.EmailConfig.SenderAddress));
            message.Subject = subject;
            message.Body = bodyBuilder.ToMessageBody();

            if (user != null)
            {
                message.To.Add(MailboxAddress.Parse(user.EmailAddress));
            }

            return message;

        }

        private string substitutePlaceholders(string template, string title, string body)
        {
            return template.Replace("<%%%TITLE%%%>", title)
                           .Replace("<%%%BODY%%%>", body);
        }

    }
}
