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

        public void AddOrUpdateUser(User newUserOrUserToUpdate)
        {
	        var foundUserWithMatchingEmail = Users.FirstOrDefault(u => u.EmailAddress.Equals(newUserOrUserToUpdate.EmailAddress, StringComparison.OrdinalIgnoreCase));
	        if (foundUserWithMatchingEmail  == null) //new user
	        {
		        Users.Add(newUserOrUserToUpdate);
	        }
	        else if (string.IsNullOrEmpty(foundUserWithMatchingEmail.Auth0UserId)) //existing user, just update the auth0 user id. this happens when the user logs in for the first time with AD, if that user has been imported.
			{
		        foundUserWithMatchingEmail.Auth0UserId = newUserOrUserToUpdate.Auth0UserId;
	        }
	        else
	        {
		        throw new Exception("Attempt to create or update an existing user");
	        }
	        SaveChanges();
        }

		/// <summary>
		/// deletes a user without querying the database for it first unnecessarily.
		/// </summary>
		/// <param name="userId"></param>
	    public void DeleteUser(int userId)
	    {
			Users.RemoveRange(new List<User> { new User { UserId = userId } });
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