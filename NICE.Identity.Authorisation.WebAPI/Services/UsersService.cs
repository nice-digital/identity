using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using Role = NICE.Identity.Authorisation.WebAPI.DataModels.Role;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IUsersService
	{
		User CreateUser(User user);
		User GetUser(int userId);
		List<User> GetUsers();
		List<UserDetails> FindUsers(IEnumerable<string> nameIdentifiers);
		Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> nameIdentifiers, string host);
		Task<User> UpdateUser(int userId, User user);
		int DeleteUser(int userId);
		void ImportUsers(IList<ImportUser> usersToImport);
        UserRolesByWebsite GetUserRolesByWebsite(int userId, int websiteId);
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
				_logger.LogError($"Failed to create user {user.NameIdentifier} - exception: {e.Message}");
				throw new Exception($"Failed to create user {user.NameIdentifier} - exception: {e.Message}");
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

		public List<UserDetails> FindUsers(IEnumerable<string> nameIdentifiers)
		{
			return _context.Users.Where(user => nameIdentifiers.Contains(user.NameIdentifier)).Select(user =>
				new UserDetails(user.NameIdentifier, user.DisplayName, user.EmailAddress)).ToList();
		}

		public Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> nameIdentifiers, string host)
		{
			var users = _context.GetUsers(nameIdentifiers);
			return users.ToDictionary(user => user.NameIdentifier,
				user => user.UserRoles
					.Where(userRole => userRole.Role.Website.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
					.Select(role => role.Role.Name));
		}

		public async Task<User> UpdateUser(int userId, User user)
		{
			try
			{
				var userToUpdate = _context.GetUser(userId);
				if (userToUpdate == null)
					throw new Exception($"User not found {userId.ToString()}");

				userToUpdate.UpdateFromApiModel(user);
				await _providerManagementService.UpdateUser(userToUpdate.NameIdentifier, userToUpdate);

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
				_providerManagementService.DeleteUser(userToDelete.NameIdentifier);

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

        public UserRolesByWebsite GetUserRolesByWebsite(int userId, int websiteId)
        {
            var user = _context.Users.Where((u => u.UserId == userId)).FirstOrDefault();
            var website = _context.Websites
                .Include(w => w.Service)
                .Include(w => w.Environment)
                .Where((w => w.WebsiteId == websiteId))
                .FirstOrDefault();
            var userRoles = _context.UserRoles.Where((ur => ur.UserId == userId)).ToList();

            if (user == null || website == null)
                return null;

            var rolesByWebsite = _context.Roles.Where(r => r.WebsiteId == websiteId)
                .Select(role => new WebsiteRole()
                {
                    Id = role.RoleId,
                    Name = role.Name,
                    Description = role.Description,
                    HasRole = userRoles.Exists(userRole => userRole.RoleId == role.RoleId)
                }).ToList();

            var userRolesByWebsite = new UserRolesByWebsite()
            {
                UserId = user.UserId,
                WebsiteId = website.WebsiteId,
                ServiceId = website.ServiceId,
                Website = new ApiModels.Website(website),
                Service = new ApiModels.Service()
                {
                    ServiceId = website.Service.ServiceId,
                    Name = website.Service.Name
                },
                Roles = rolesByWebsite
            };
            
            return userRolesByWebsite;
        }
    }
}