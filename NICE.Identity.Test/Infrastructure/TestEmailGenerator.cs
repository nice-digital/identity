using MimeKit;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Factories;
using System;

namespace NICE.Identity.Test.Infrastructure
{
    public class TestEmailGenerator : EmailGenerator, IEmailGenerator
    {
        private readonly String _title;
        private readonly String _subject;
        private readonly String _htmlBody;
        private readonly String _textBody;
        private readonly String _contactUsSubject;

        public TestEmailGenerator(string HTMLTemplate, string TextTemplate) : base(HTMLTemplate, TextTemplate)
        {
            _title = "EmailTitle";
            _subject = "EmailSubject";
            _htmlBody = "HtmlEmailBody";
            _textBody = "TextEmailBody";
            _contactUsSubject = "Test contact us subject";
        }

        public MimeMessage GenerateEmail(User user)
        {
            return GetEmail(user, _title, _subject, _htmlBody, _textBody, _contactUsSubject);
        }
    }
}
