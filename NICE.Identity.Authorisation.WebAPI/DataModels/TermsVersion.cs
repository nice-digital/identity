using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public partial class TermsVersion
    {
        public int TermsVersionId { get; set; }
        public DateTime VersionDate { get; set; }
        public int? CreatedByUserId { get; set; }

        public User CreatedByUser { get; set; }
        public ICollection<UserAcceptedTermsVersion> UserAcceptedTermsVersions { get; set; }

        public TermsVersion() { }

        public TermsVersion(int version, DateTime versionDate, int creatorId)
        {
            TermsVersionId = version;
            VersionDate = versionDate;
            CreatedByUserId = creatorId;
        }

    }
}
