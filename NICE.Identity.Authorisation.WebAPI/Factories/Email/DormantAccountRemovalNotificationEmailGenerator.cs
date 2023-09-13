using MimeKit;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using System;

namespace NICE.Identity.Authorisation.WebAPI.Factories
{
    public class DormantAccountRemovalNotificationEmailGenerator : EmailGenerator, IEmailGenerator
    {
        private readonly string _title;
        private readonly string _subject;
        private readonly string _htmlBody;
        private readonly string _textBody;
        private readonly string _tabs;
        private readonly string _contactUsSubject;

        public DormantAccountRemovalNotificationEmailGenerator(string HTMLTemplate, string TextTemplate) : base(HTMLTemplate, TextTemplate)
        {
            _tabs = "\t\t\t\t\t\t\t\t\t\t\t";

            _title = "Your dormant NICE Account has been deleted";
            _contactUsSubject = "Help with dormant NICE account";
            _subject = "Dormant account deletion";
            _htmlBody = "\r\n" +
                        $"{_tabs}Your NICE Account has not been used for 3 years. Unfortunately, we had to delete your details in compliance with GDPR.\r\n" +
                        $"{_tabs}<br /><br />\r\n" +
                        $"{_tabs}If you want to re-register for a NICE Account using this email address, you will have to re-submit your details on the <a href=\"https://www.nice.org.uk\">NICE website</a>.\r\n";
            _textBody = "Your NICE Account has not been used for 3 years. Unfortunately, we had to delete your details in compliance with GDPR.\r\n" +
                        "\r\n" +
                        "If you want to re-register for a NICE Account using this email address, you will have to re-submit your details on the NICE website.\r\n";
        }

        public MimeMessage GenerateEmail(User user)
        {
            return GetEmail(user, _title, _subject, _htmlBody, _textBody, _contactUsSubject);
        }
    }
}
