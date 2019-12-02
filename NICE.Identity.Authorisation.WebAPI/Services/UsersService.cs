using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IUsersService
	{
		User CreateUser(User user);
		User GetUser(int userId);
		List<User> GetUsers();
		List<UserDetails> FindUsers(IEnumerable<string> auth0UserIds);
		Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> auth0UserIds, string host);
		User UpdateUser(int userId, User user);
		int DeleteUser(int userId);
		void ImportUsers(IList<ImportUser> usersToImport);
	}

	public class UsersService : IUsersService
	{
		private readonly IdentityContext _context;
		private readonly ILogger<UsersService> _logger;
		private readonly IProviderManagementService _providerManagementService;

		public UsersService(IdentityContext context, ILogger<UsersService> logger,
			IProviderManagementService providerManagementService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_providerManagementService = providerManagementService ??
			                             throw new ArgumentNullException(nameof(providerManagementService));
		}

		public User CreateUser(User user)
		{
			try
			{
				var userToCreate = new DataModels.User(user);
				if (user.AcceptedTerms == true)
				{
					var currentTerms = _context.GetLatestTermsVersion();
					if (currentTerms != null)
					{
						userToCreate.UserAcceptedTermsVersions = new List<DataModels.UserAcceptedTermsVersion>()
						{
							new DataModels.UserAcceptedTermsVersion(currentTerms)
						};
					}
				}

				return new User(_context.CreateUser(userToCreate));
			}
			catch (Exception e)
			{
				_logger.LogError($"Failed to create user {user.Auth0UserId} - exception: {e.Message}");
				throw new Exception($"Failed to create user {user.Auth0UserId} - exception: {e.Message}");
			}
		}

		public User GetUser(int userId)
		{
			var user = _context.Users.Where((u => u.UserId == userId)).FirstOrDefault();
			return user != null ? new User(user) : null;
		}

		public List<User> GetUsers()
		{
			return _context.Users.Select(user => new User(user)).ToList();
		}

		public List<UserDetails> FindUsers(IEnumerable<string> auth0UserIds)
		{
			return _context.Users.Where(user => auth0UserIds.Contains(user.Auth0UserId)).Select(user =>
				new UserDetails(user.Auth0UserId, user.DisplayName, user.EmailAddress)).ToList();
		}

		public Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> auth0UserIds, string host)
		{
			var users = _context.GetUsers(auth0UserIds);
			return users.ToDictionary(user => user.Auth0UserId,
				user => user.UserRoles
					.Where(userRole => userRole.Role.Website.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
					.Select(role => role.Role.Name));
		}

		// TODO: update user in identity provider if needed
		public User UpdateUser(int userId, User user)
		{
			try
			{
				var userToUpdate = _context.GetUser(userId);
				if (userToUpdate == null)
					throw new Exception($"User not found {userId.ToString()}");

				userToUpdate.UpdateFromApiModel(user);
				_providerManagementService.UpdateUser(userToUpdate.Auth0UserId, userToUpdate);

				_context.SaveChanges();
				return new User(userToUpdate);
			}
			catch (Exception e)
			{
				_logger.LogError($"Failed to update user {userId.ToString()} - exception: {e.InnerException.Message}");
				throw new Exception(
					$"Failed to update user {userId.ToString()} - exception: {e.InnerException.Message}");
			}
		}

		public int DeleteUser(int userId)
		{
			try
			{
				var userToDelete = _context.GetUser(userId);
				if (userToDelete == null)
					return 0;

				_context.Users.RemoveRange(userToDelete);
				_providerManagementService.DeleteUser(userToDelete.Auth0UserId);

				return _context.SaveChanges();
			}
			catch (Exception e)
			{
				_logger.LogError($"Failed to delete user {userId.ToString()} - exception: {e.Message}");
				throw new Exception($"Failed to delete user {userId.ToString()} - exception: {e.Message}");
			}
		}

		public void ImportUsers(IList<ImportUser> usersToImport)
		{
			if (!usersToImport.Any())
				return;
			
			foreach (var userToImport in usersToImport)
			{
				//first ignore any users with invalid (empty guid) ids, or without firstname or last names (these are all just test data). 
				if (userToImport.UserId.Equals(Guid.Empty) || string.IsNullOrEmpty(userToImport.FirstName) ||
				    string.IsNullOrEmpty(userToImport.LastName))
				{
					_logger.LogError($"Not importing user with details: user id:{userToImport.UserId} firstname: {userToImport.FirstName} lastname: {userToImport.LastName}");
					continue;
				}

				//create the user
				var insertedUser = _context.CreateUser(userToImport.AsUser);

				//now to insert the roles
				if (userToImport.Roles != null && userToImport.Roles.Any())
				{
					var lookedUpRoles = new List<Role>(); //this contains a list of lookedup roles, so the database doesn't get hit too often unnecessarily
					foreach (var importRole in userToImport.Roles)
					{
						if (!importRole.RoleId.HasValue)
						{
							//first try to find role in the lookup
							var foundRole = lookedUpRoles.FirstOrDefault(r =>
								r.Name.Equals(importRole.RoleName, StringComparison.OrdinalIgnoreCase) &&
								r.Website.Host.Equals(importRole.WebsiteHost, StringComparison.OrdinalIgnoreCase));

							if (foundRole == null)
							{
								//failing that, hit the database for the role.
								foundRole = _context.GetRole(importRole.WebsiteHost, importRole.RoleName);
								if (foundRole != null) 
									lookedUpRoles.Add(foundRole); //add to the lookup if found
							}

							if (foundRole != null)
							{
								importRole.RoleId = foundRole.RoleId;
							}
							else
							{
								_logger.LogWarning($"Could not find role: {importRole.RoleName} for website: {importRole.WebsiteHost}");
								continue;
							}
						}
						_context.AddUsersToRole(new List<DataModels.User> { insertedUser }, importRole.RoleId.Value);
					}
				}
			}
		}
	}
}