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
            var result = Users.Where(user => EF.Functions.Like(user.NameIdentifier, authenticationProviderUserId))
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
                .ThenInclude(userRoles => userRoles.Role)
                .ThenInclude(website => website.Website)
                .ToList();

            return !result.Any() ? null : result.Single();
        }

        public List<User> GetUsers(IEnumerable<string> authenticationProviderUserIds)
        {
            var lowercasedAuthenticationProviderUserIds = authenticationProviderUserIds.Select(userId => userId.ToLower());
	        return Users.Where(users => lowercasedAuthenticationProviderUserIds.Contains(users.NameIdentifier.ToLower()))
		        .Include(users => users.UserRoles)
		        .ThenInclude(userRoles => userRoles.Role)
		        .ThenInclude(website => website.Website)
		        .ToList();
        }

		public User CreateUser(User user)
        {
			//there might be multiple users with the same email address, e.g. if someone logs in with AD using the same email address as has been imported from nice accounts 
			//so lookup with name identifier, which is unique
            var foundUser = Users.FirstOrDefault(u => EF.Functions.Like(u.NameIdentifier, user.NameIdentifier));
            if (foundUser != null)
            {
	            return foundUser;
            }

			user.InitialRegistrationDate = DateTime.UtcNow;
            Users.Add(user);
            SaveChanges();
            return user;
        }

		public void UpdateUserLastLoggedInDate(User user)
		{
			user.LastLoggedInDate = DateTime.UtcNow;
			Users.Update(user);
			SaveChanges();
		}

        #endregion

        #region Websites Methods
        
        public List<Website> GetWebsites()
        {
            return Websites.Include(w => w.Environment).ToList();
        }

        public Website GetWebsite(int websiteId)
        {
            return Websites.Include(w => w.Environment)
                .Where((w => w.WebsiteId == websiteId))
                .FirstOrDefault();
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
                EF.Functions.Like(r.Website.Host, websiteHost) &&
                EF.Functions.Like(r.Name, roleName));
		}


		public int AddUsersToRole(IEnumerable<User> users, int roleId)
		{
			var userRolesAdded = 0;
			foreach (var user in users)
			{
				if (!user.UserRoles.Any(r => r.RoleId == roleId))
				{
					UserRoles.Add(new UserRole {RoleId = roleId, UserId = user.UserId});
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