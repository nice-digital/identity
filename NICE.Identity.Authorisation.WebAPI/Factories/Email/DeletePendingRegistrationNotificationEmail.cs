using MimeKit;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using System;

namespace NICE.Identity.Authorisation.WebAPI.Factories
{
    public class DeletePendingRegistrationNotificationEmail : EmailGenerator, IEmailGenerator
    {
        private readonly String _title;
        private readonly String _subject;
        private readonly String _htmlBody;
        private readonly String _textBody;
        private readonly string _tabs;
        private readonly string _contactUsSubject;

        public DeletePendingRegistrationNotificationEmail(string HTMLTemplate, string TextTemplate) : base(HTMLTemplate, TextTemplate)
        {
            _tabs = "\t\t\t\t\t\t\t\t\t\t\t";

            _title = "Your unverified NICE Account has been deleted";
            _contactUsSubject = "NICE accounts activation help";
            _subject = "Unverified account removal";
            _htmlBody = "\r\n" +
                        $"{_tabs}Your account has been pending activation for 30 days. Unfortunately, the time allowed for activating your account has elapsed.\r\n" +
                        $"{_tabs}<br /><br />\r\n" +
                        $"{_tabs}We have deleted your details in compliance with GDPR. If you still want to register for a NICE account using this email address, you will have to re-submit your details on the <a href=\"https://www.nice.org.uk\">NICE website</a>.\r\n" +
                        $"{_tabs}<br /><br />\r\n"; 
            _textBody = "Your account has been pending activation for 30 days. Unfortunately, the time allowed for activating your account has elapsed.\r\n" +
                        "\r\n" +
                        "We have deleted your details in compliance with GDPR. If you still want to register for a NICE account using this email address, you will have to re-submit your details on the NICE website.\r\n";
        }

        public MimeMessage GenerateEmail(User user)
        {
            return GetEmail(user, _title, _subject, _htmlBody, _textBody, _contactUsSubject);
        }
    }
}
