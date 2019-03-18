using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public class UserAcceptedTermsVersion
    {
        public int UserAcceptedTermsVersionId { get; set; }
        public int UserId { get; set; }
        public int TermsVersionId { get; set; }
        public DateTime UserAcceptedDate { get; set; }

        public TermsVersion TermsVersion { get; set; }
        public User User { get; set; }

        public UserAcceptedTermsVersion() { 
        }

        public UserAcceptedTermsVersion(int id, int userId, int versionId, DateTime acceptedDate)
        {
            UserAcceptedTermsVersionId = id;
            UserId = userId;
            TermsVersionId = versionId;
            UserAcceptedDate = acceptedDate;
        }

        public UserAcceptedTermsVersion(TermsVersion tv) : this ()
        {
            UserAcceptedDate = DateTime.Now;
            TermsVersion = tv;
        }
    }
}
