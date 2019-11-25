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
            var result = Users.Where(users => users.Auth0UserId.Equals(authenticationProviderUserId, StringComparison.OrdinalIgnoreCase))
                .Include(users => users.UserRoles)
                .ThenInclude(userRoles => userRoles.Role)
                .ThenInclude(website => website.Website)
                .ToList();

            return !result.Any() ? null : result.Single();
        }

        public User GetUser(int userId)
        {
            var result = Users.Where(users => users.UserId.Equals(userId))
                .Include(users => users.UserRoles)
                .ThenInclude(userRoles => userRoles.Role).ToList();

            return !result.Any() ? null : result.Single();
        }

        public List<User> GetUsers(IEnumerable<string> authenticationProviderUserIds)
        {
	        return Users.Where(users => authenticationProviderUserIds.Contains(users.Auth0UserId, StringComparer.OrdinalIgnoreCase))
		        .Include(users => users.UserRoles)
		        .ThenInclude(userRoles => userRoles.Role)
		        .ThenInclude(website => website.Website)
		        .ToList();
        }

		public User CreateUser(User user)
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
	            SaveChanges();
            }
            return foundUser;
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

		#region Roles 

		public Role GetRole(string websiteHost, string roleName)
		{
			return Roles.FirstOrDefault(r =>
				r.Website.Host.Equals(websiteHost, StringComparison.OrdinalIgnoreCase) &&
				r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
		}


		public int AddUsersToRole(IEnumerable<User> users, Role roleToAdd)
		{
			var userRolesAdded = 0;
			foreach (var user in users)
			{
				if (!user.UserRoles.Any(r => r.RoleId == roleToAdd.RoleId))
				{
					UserRoles.Add(new UserRole {RoleId = roleToAdd.RoleId, UserId = user.UserId});
					userRolesAdded++;
				}
			}

			if (userRolesAdded > 0)
			{
				SaveChanges();
			}

			return userRolesAdded;
		}

		#endregion

	}
}