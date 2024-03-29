﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using Organisation = NICE.Identity.Authorisation.WebAPI.DataModels.Organisation;
using Role = NICE.Identity.Authorisation.WebAPI.DataModels.Role;
using User = NICE.Identity.Authorisation.WebAPI.DataModels.User;
using UserRole = NICE.Identity.Authorisation.WebAPI.DataModels.UserRole;
using Website = NICE.Identity.Authorisation.WebAPI.DataModels.Website;

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
                .Include(users => users.Jobs)
					.ThenInclude(jobs => jobs.Organisation)
                .ToList();

            return !result.Any() ? null : result.Single();
        }

        public User GetUser(int userId)
        {
            var result = Users.Where(users => users.UserId.Equals(userId))
                .Include(users => users.UserRoles)
                    .ThenInclude(userRoles => userRoles.Role)
                    .ThenInclude(website => website.Website)
                .Include(email => email.UserEmailHistory)
                    .ThenInclude(archived => archived.ArchivedByUser)
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
        internal IEnumerable<User> GetUsersByOrganisationId(int organisationId)
        {
            return Jobs.Where(jobs => jobs.OrganisationId.Equals(organisationId))
                           .Include(users => users)
                            .Select(x => x.User)
                           .ToList();
        }

        internal UsersAndJobIdsForOrganisation GetUsersAndJobIdsByOrganisationId(int organisationId)
        {
            var organisation = Organisations.Where(org => org.OrganisationId.Equals(organisationId))
                .Include(j => j.Jobs)
                .ThenInclude(u => u.User)
                .ToList();

            var usersAndJobs = new List<UserAndJobId>();
            foreach (var job in organisation.FirstOrDefault().Jobs)
            {
                var userAndJobId = new UserAndJobId();
                userAndJobId.UserId = job.UserId;
                userAndJobId.User = new ApiModels.User(job.User);
                userAndJobId.JobId = job.JobId;

                usersAndJobs.Add(userAndJobId);
            }

            var usersForOrganisation = new UsersAndJobIdsForOrganisation();
            usersForOrganisation.OrganisationId = organisationId;
            usersForOrganisation.Organisation = new ApiModels.Organisation(organisation.FirstOrDefault());
            usersForOrganisation.UsersAndJobIds = usersAndJobs.OrderBy(u => u.User.FirstName).ThenBy(u => u.User.LastName).ToList();

            return usersForOrganisation;
        }


        public List<User> FindUsers(string filter)
        {
	        filter ??= "";
            
	        var userIdsWithMatchingEmailHistory = 
		        UserEmailHistory
			        .Where(ueh => EF.Functions.Like(ueh.EmailAddress, $"%{filter}%"))
			        .Select(ueh => ueh.UserId)
			        .ToList();

	        return Users.Where(u => (u.FirstName != null && EF.Functions.Like(u.FirstName, $"%{filter}%"))
	                                || (u.LastName != null && EF.Functions.Like(u.LastName, $"%{filter}%"))
	                                || (u.FirstName != null && u.LastName != null && EF.Functions.Like(u.FirstName + " " + u.LastName, $"%{filter}%"))
	                                || (u.NameIdentifier != null && EF.Functions.Like(u.NameIdentifier, $"%{filter}%"))
	                                || (u.EmailAddress != null && EF.Functions.Like(u.EmailAddress, $"%{filter}%"))
	                                || (userIdsWithMatchingEmailHistory.Contains(u.UserId)))

		        .Include(users => users.UserRoles)
					.ThenInclude(userRoles => userRoles.Role)
						.ThenInclude(website => website.Website)

					.OrderByDescending(user => user.UserId)
		        .ToList();
        }

        internal IEnumerable<Organisation> FindOrganisations(string filter)
        {
            filter ??= "";

            return Organisations.Where(w => (w.Name != null && EF.Functions.Like(w.Name, $"%{filter}%")))
                .Select(x => new Organisation()
                {
                    OrganisationId = x.OrganisationId,
                    Name = x.Name,
                    DateAdded = x.DateAdded != null ? x.DateAdded : new DateTime(2021, 12, 1)
                })
                .OrderBy(w => w.Name)
                .ToList();
        }

        public User CreateUser(User user, bool importing = false)
        {
			var foundUser = Users.FirstOrDefault(u => EF.Functions.Like(u.NameIdentifier, user.NameIdentifier));
            if (foundUser != null)
            {
	            return foundUser;
            }

            var foundUserByEmail = Users.FirstOrDefault(u => EF.Functions.Like(u.EmailAddress, user.EmailAddress));

            ////if users are imported, where that user has already signed in, this will evaluate to truee and a new user shouldn't be created, the found user should be returned.
            if (foundUserByEmail != null && importing)
            {
                return foundUserByEmail;
            }

            //if someone logs in with AD using the same email address as has been imported from nice accounts there will already be an existing record in the database with that email, but the nameidentifier will be wrong.
            //in which case, update the nameidentifier this one time, set the last logged in date and update the database.
            if (foundUserByEmail != null && foundUserByEmail.IsMigrated && !foundUserByEmail.IsInAuthenticationProvider)
            {
	            foundUserByEmail.NameIdentifier = user.NameIdentifier;
	            if (!importing) //when importing don't set the isinauthentication provider flag explicitly. this is important so that when imports are run multiple times, the isinauthenticationprovider flag isn't mistakenly set to true.
	            {
		            foundUserByEmail.IsInAuthenticationProvider = true;
	            }
	            foundUserByEmail.LastLoggedInDate = DateTime.UtcNow;
                Users.Update(foundUserByEmail);
                SaveChanges();
	            return foundUserByEmail;
            }

            if (foundUserByEmail != null)
            {
	            throw new DuplicateEmailException($"Duplicate email address supplied: {foundUserByEmail.EmailAddress}");
            }

            user.InitialRegistrationDate = DateTime.UtcNow;
            Users.Add(user);
            SaveChanges();
            return user;
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

        public async Task<int> DeleteUsers(IList<User> users)
        {
            //before removing a user, we also need to remove the UserAcceptedTermsVersion, Job and UserRole for the user, if they exist.
            if (!users.Any())
            {
	            return 0;
            }

            var userIds = users.Select(user => user.UserId).ToList();

            var userRolesForUsers = UserRoles.Where(ur => userIds.Contains(ur.UserId));
            if (userRolesForUsers.Any())
            {
	            UserRoles.RemoveRange(userRolesForUsers);
            }

            var jobsForUsers = Jobs.Where(job => userIds.Contains(job.UserId));
            if (jobsForUsers.Any())
            {
	            Jobs.RemoveRange(jobsForUsers);
            }

            var acceptedTermsForUsers = UserAcceptedTermsVersions.Where(uatv => userIds.Contains(uatv.UserId));
            if (acceptedTermsForUsers.Any())
            {
	            UserAcceptedTermsVersions.RemoveRange(acceptedTermsForUsers);
            }

            var userEmailHistories = UserEmailHistory.Where(ueh => userIds.Contains(ueh.UserId));
            if (userEmailHistories.Any()) {
                UserEmailHistory.RemoveRange(userEmailHistories);
            }

            Users.RemoveRange(users);
            
            return await SaveChangesAsync();
        }

        #endregion Users

        #region Websites Methods

        public List<Website> FindWebsites(string filter)
        {
            filter ??= "";

            return Websites.Where(w => (w.Host != null && EF.Functions.Like(w.Host, $"%{filter}%"))
                                       || (w.Service.Name != null && EF.Functions.Like(w.Service.Name, $"%{filter}%")))
                .Include(w => w.Environment)
                .Include(w => w.Service)
                .OrderBy(w => w.Environment.Order)
                .ThenBy(w => w.Host)
                .ToList();
        }

        public Website GetWebsite(int websiteId)
        {
            return Websites.Include(w => w.Environment)
                .Include(w => w.Service)
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