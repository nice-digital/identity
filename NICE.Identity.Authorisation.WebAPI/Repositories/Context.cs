using System;
using System.Collections.Generic;
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

        public User AddUser(User user)
        {
            // find by email address as this should be unique
            var foundUser = Users.FirstOrDefault(
                u => u.EmailAddress.Equals(user.EmailAddress, StringComparison.OrdinalIgnoreCase));

            // existing user, just update the auth0 user id.
            // this happens when the user logs in for the first time with AD,
            // if that user has been imported.
            if (foundUser == null)
            {
                //new user
                Users.Add(user);
                SaveChanges();
                return user;
            }
            if (string.IsNullOrEmpty(foundUser.Auth0UserId))
            {
                foundUser.Auth0UserId = user.Auth0UserId;
                UpdateUser(foundUser);
                SaveChanges();
                return foundUser;
            }
            return null;
        }

        public User UpdateUser(User user)
        {
            Users.Update(user);
            SaveChanges();
            return user;
        }

        /// <summary>
        /// deletes a user without querying the database for it first unnecessarily.
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteUser(int userId)
        {
            var user = Users.Find(userId);
            Users.RemoveRange(user);
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