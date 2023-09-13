using MimeKit;
using System.Text;
using System;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using System.Linq;

namespace NICE.Identity.Authorisation.WebAPI.Factories
{
    public class PendingDormantAccountRemovalNotificationEmailGenerator : EmailGenerator, IEmailGenerator
    {
        private readonly string _title;
        private readonly string _subject;
        private readonly string _htmlBody;
        private readonly string _textBody;
        private readonly string _tabs;
        private readonly string _tabsLevelTwo;
        private readonly string _contactUsSubject;

        public PendingDormantAccountRemovalNotificationEmailGenerator(string HTMLTemplate, string TextTemplate) : base(HTMLTemplate, TextTemplate)
        {
            _tabs = "\t\t\t\t\t\t\t\t\t\t\t";
            _tabsLevelTwo = _tabs + "\t\t";

            _title = "Your dormant NICE Account will be deleted soon";
            _subject = "Pending dormant account deletion";
            _contactUsSubject = "Help with dormant NICE account";
            _htmlBody = "\r\n" +
                        $"{_tabs}Your NICE Account has not been used for 3 years so it is either dormant or no longer required.\r\n" +
                        $"{_tabs}<br /><br />\r\n" +
                        $"{_tabs}We must delete inactive accounts in line with our data retention policy and to comply with GDPR. We cannot retain your details if you have not used the account to interact with NICE for 3 years or longer.\r\n" +
                        $"{_tabs}<br /><br />\r\n" +
                        $"{_tabs}If you wish to retain your NICE account, you must sign-in within 30 calendar days from today on the <a href=\"https://www.nice.org.uk\">NICE website</a>." +
                        "<%%%SERVICELIST%%%>";
            _textBody = "Your NICE Account has not been used for 3 years so it is either dormant or no longer required.\r\n" +
                        "\r\n" +
                        "We must delete inactive accounts in line with our data retention policy and to comply with GDPR. We cannot retain your details if you have not used the account to interact with NICE for 3 years or longer.\r\n" +
                        "\r\n" +
                        "If you wish to retain your NICE account, you must sign-in within 30 calendar days from today on the NICE website." +
                        "<%%%SERVICELIST%%%>";
        }

        public MimeMessage GenerateEmail(User user)
        {
            var dynamicHtmlBody = _htmlBody;
            var dynamicTextBody = _textBody;

            if (user.UserRoles != null && user.UserRoles.Count() > 0)
            {
                var serviceNames = user.UserRoles
                    .Select(x => x.Role)
                    .Select(x => x.Website.Service.Name)
                    .Distinct()
                    .ToList();

                if (serviceNames != null && serviceNames.Count > 0)
                {
                    var htmlStringBuilder = new StringBuilder();
                    htmlStringBuilder.Append(" Otherwise, you may lose access to the following NICE systems:\r\n");
                    htmlStringBuilder.Append($"{_tabs}<br /><br />\r\n");
                    htmlStringBuilder.Append(_tabs + "<ul>\r\n");
                    serviceNames.ForEach(x => htmlStringBuilder.Append($"{_tabsLevelTwo}<li>{x}</li>\r\n"));
                    htmlStringBuilder.Append($"{_tabs}</ul>\r\n");

                    var textStringBuilder = new StringBuilder();
                    textStringBuilder.Append(" Otherwise, you may lose access to the following NICE systems:\r\n");
                    textStringBuilder.Append("\r\n");
                    serviceNames.ForEach(x => textStringBuilder.Append($"{x}\r\n"));
                    textStringBuilder.Append("\r\n");

                    dynamicHtmlBody = _htmlBody.Replace("<%%%SERVICELIST%%%>", htmlStringBuilder.ToString());
                    dynamicTextBody = _textBody.Replace("<%%%SERVICELIST%%%>", textStringBuilder.ToString());
                }
            }

            //if the service list placeholders still exist by this point, then no services have been found, replace them.
            dynamicHtmlBody = dynamicHtmlBody.Replace("<%%%SERVICELIST%%%>", "\r\n");
            dynamicTextBody = dynamicTextBody.Replace("<%%%SERVICELIST%%%>", "\r\n");

            return GetEmail(user, _title, _subject, dynamicHtmlBody, dynamicTextBody, _contactUsSubject);

        }
    }
}
