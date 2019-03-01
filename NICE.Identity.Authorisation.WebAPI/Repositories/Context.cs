using System.Linq;
using Microsoft.EntityFrameworkCore;
using NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Authorisation.WebAPI.Repositories
{
    public partial class IdentityContext : DbContext
    {
        #region User Methods

        public User GetUser(string authenticationProviderUserId)
        {
            var result = Users.Where(users => users.Auth0UserId.Equals(authenticationProviderUserId))
                .Include(users => users.UserRoles)
                .ThenInclude(userRoles => userRoles.Role).ToList();

            return !result.Any() ? null : result.Single();
        }

        public void AddUser(User user)
        {
            Users.Add(user);
            SaveChanges();
        }

        #endregion

        #region TermsVersion Methods

        /// <summary>
        /// Get the TermsVersion with the highest Id: this should be the page version from Orchard
        /// </summary>
        /// <returns>
        /// a TermVersio or NULL if there are none
        /// </returns>
        public TermsVersion GetLatestTermsVersion()
        {
            return TermsVersions.OrderByDescending(x => x.TermsVersionId).FirstOrDefault();
        }

        #endregion
    }
}