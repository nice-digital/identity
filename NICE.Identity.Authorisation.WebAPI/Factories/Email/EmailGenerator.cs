using MimeKit;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using System;

namespace NICE.Identity.Authorisation.WebAPI.Factories
{
    public interface IEmailGenerator
    {
        public MimeMessage GenerateEmail(User user);
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
        
        protected MimeMessage GetEmail(User user, string title, string subject, string htmlBody, string textBody, string contactUsSubject)
        {
            var bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = substitutePlaceholders(_emailHTMLTemplate, title, htmlBody, contactUsSubject);
            bodyBuilder.TextBody = substitutePlaceholders(_emailTextTemplate, title, textBody, contactUsSubject);

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

        private string substitutePlaceholders(string template, string title, string body, string contactUsSubject)
        {
            return template
                .Replace("<%%%TITLE%%%>", title)
                .Replace("<%%%BODY%%%>", body)
                .Replace("<%%%CONTACT_US_SUBJECT%%%>", Uri.EscapeDataString(contactUsSubject));
        }
    }
}
