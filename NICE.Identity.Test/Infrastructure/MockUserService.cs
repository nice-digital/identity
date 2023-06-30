using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.User;
using UserRole = NICE.Identity.Authorisation.WebAPI.ApiModels.UserRole;

namespace NICE.Identity.Test.Infrastructure
{
	public class MockUserService : IUsersService
	{
		public User CreateUser(User user)
		{
			throw new NotImplementedException();
		}

		public User GetUser(int userId)
		{
			throw new NotImplementedException();
		}

		public IList<User> GetUsers(string filter)
		{
			throw new NotImplementedException();
		}

		public IList<UserDetails> FindUsers(IEnumerable<string> nameIdentifiers)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> nameIdentifiers, string host)
		{
			throw new NotImplementedException();
		}

		public Task<User> UpdateUser(int userId, User user, string nameIdentifierOfUserUpdatingRecord)
		{
			throw new NotImplementedException();
		}

		public Task<int> DeleteUser(int userId)
		{
			throw new NotImplementedException();
		}

		public void ImportUsers(IList<ImportUser> usersToImport)
		{
			throw new NotImplementedException();
		}

		public UserRolesByWebsite GetRolesForUserByWebsite(int userId, int websiteId)
		{
			throw new NotImplementedException();
		}

		public async Task<UserRolesByWebsite> UpdateRolesForUserByWebsite(int userId, int websiteId, UserRolesByWebsite userRolesByWebsite)
		{
			throw new NotImplementedException();
		}

		public IList<UserRole> GetRolesForUser(int userId)
		{
			throw new NotImplementedException();
		}

		public IList<UserRole> UpdateRolesForUser(int userId, List<UserRole> userRolesToUpdate)
		{
			throw new NotImplementedException();
		}

		public Task<int> DeleteAllUsers()
		{
			throw new NotImplementedException();
		}

		public Task DeleteRegistrationsOlderThan(bool notify, int daysToKeepPendingRegistration)
		{
			throw new NotImplementedException();
		}

		Task<User> UpdateUser(int userId, User user)
		{
			throw new NotImplementedException();
		}

		public IList<User> GetUsersByOrganisationId(int organisationId)
		{
			throw new NotImplementedException();
		}

        public UsersAndJobIdsForOrganisation GetUsersAndJobIdsByOrganisationId(int organisationId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateFieldsDueToLogin(string userToUpdateIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDormantAccounts(DateTime BaseDate)
        {
            throw new NotImplementedException();
        }

        public Task SendPendingDeletionEmails(DateTime BaseDate)
        {
            throw new NotImplementedException();
        }
    }
}
