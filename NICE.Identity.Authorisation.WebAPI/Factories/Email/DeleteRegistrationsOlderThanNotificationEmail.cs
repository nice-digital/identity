using MimeKit;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using System;

namespace NICE.Identity.Authorisation.WebAPI.Factories
{
    public class DeleteRegistrationsOlderThanNotificationEmail : EmailGenerator, IEmailGenerator
    {
        private readonly String _title;
        private readonly String _subject;
        private readonly String _htmlBody;
        private readonly String _textBody;

        public DeleteRegistrationsOlderThanNotificationEmail(string HTMLTemplate, string TextTemplate) : base(HTMLTemplate, TextTemplate)
        {
            _title = "Your unverified NICE Account has been deleted";
            _subject = "Unverified account removal";
            _htmlBody = "\r\n" +
                        "\t\t\t\t\t\t\t\t\t\t\tYour account has been pending activation for 30 days. Unfortunately, the time allowed for activating your account has elapsed.\r\n" +
                        "\t\t\t\t\t\t\t\t\t\t\t<br /><br />\r\n" +
                        "\t\t\t\t\t\t\t\t\t\t\tWe have deleted your details in compliance with GDPR. If you still want to register for a NICE account using this email address, you will have to re-submit your details on the <a href=\"https://www.nice.org.uk\">NICE website</a>.\r\n";
            _textBody = "Your account has been pending activation for 30 days. Unfortunately, the time allowed for activating your account has elapsed.\r\n" +
                        "\r\n" +
                        "We have deleted your details in compliance with GDPR. If you still want to register for a NICE account using this email address, you will have to re-submit your details on the NICE website.\r\n";
        }

        public MimeMessage GenerateEmail(User user)
        {
            return GetEmail(user, _title, _subject, _htmlBody, _textBody);
        }
    }
}
