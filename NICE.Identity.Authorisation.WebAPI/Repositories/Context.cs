using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			var foundUser = Users.FirstOrDefault(u => EF.Functions.Like(u.NameIdentifier, user.NameIdentifier));
            if (foundUser != null)
            {
	            return foundUser;
            }

            //if someone logs in with AD using the same email address as has been imported from nice accounts there will already be an existing record in the database with that email, but the nameidentifier will be wrong.
            //in which case, update the nameidentifier this one time, set the last logged in date and update the database.
            var foundUserByEmail = Users.FirstOrDefault(u => EF.Functions.Like(u.EmailAddress, user.EmailAddress));
            if (foundUserByEmail != null && foundUserByEmail.IsMigrated && !foundUserByEmail.IsInAuthenticationProvider)
            {
	            foundUserByEmail.NameIdentifier = user.NameIdentifier;
	            foundUserByEmail.IsInAuthenticationProvider = true;
                foundUserByEmail.LastLoggedInDate = DateTime.UtcNow;
                SaveChanges();
	            return foundUserByEmail;
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

        /// <summary>
        /// This delete all users method is temporary. it also can only be called on non-production environments, by an admin.
        /// </summary>
        /// <returns></returns>
		public async Task<int> DeleteAllUsers()
		{
			return await Database.ExecuteSqlRawAsync(@"
				DELETE FROM UserAcceptedTermsVersion
				DELETE FROM UserRoles
				DELETE FROM Users
			");
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