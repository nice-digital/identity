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

        public DormantAccountRemovalNotificationEmailGenerator(string HTMLTemplate, string TextTemplate) : base(HTMLTemplate, TextTemplate)
        {
            _title = "Your dormant NICE Account has been deleted";
            _subject = "Dormant account deletion";
            _htmlBody = "\r\n" +
                        "\t\t\t\t\t\t\t\t\t\t\tYour NICE Account has not been used for 3 years. Unfortunately, we had to delete your details in compliance with GDPR.\r\n" +
                        "\t\t\t\t\t\t\t\t\t\t\t<br /><br />\r\n" +
                        "\t\t\t\t\t\t\t\t\t\t\tIf you want to re-register for a NICE Account using this email address, you will have to re-submit your details on the <a href=\"https://www.nice.org.uk\">NICE website</a>.If you need help you can contact usThis email has been generated automatically. Please do not reply.\r\n";
            _textBody = "Your NICE Account has not been used for 3 years. Unfortunately, we had to delete your details in compliance with GDPR.\r\n" +
                        "\r\n" +
                        "If you want to re-register for a NICE Account using this email address, you will have to re-submit your details on the NICE website.If you need help you can contact usThis email has been generated automatically. Please do not reply.\r\n";
        }

        public MimeMessage GenerateEmail(User user)
        {
            return GetEmail(user, _title, _subject, _htmlBody, _textBody);
        }
    }
}
