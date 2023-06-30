using MimeKit;

namespace NICE.Identity.Authorisation.WebAPI.Factories
{

    public interface IEmailGenerator
    {
        public MimeMessage GenerateEmail(DataModels.User user);
    }
}
