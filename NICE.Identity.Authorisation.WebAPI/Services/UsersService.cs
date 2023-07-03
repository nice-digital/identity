﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Factories;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using Role = NICE.Identity.Authorisation.WebAPI.DataModels.Role;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.User;
using UserRole = NICE.Identity.Authorisation.WebAPI.ApiModels.UserRole;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IUsersService
	{
		User CreateUser(User user);
		User GetUser(int userId);
		IList<User> GetUsers(string filter);
		IList<UserDetails> FindUsers(IEnumerable<string> nameIdentifiers);
		Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> nameIdentifiers, string host);
		Task<User> UpdateUser(int userId, User user, string nameIdentifierOfUserUpdatingRecord);
        Task<int> DeleteUser(int userId);
		void ImportUsers(IList<ImportUser> usersToImport);
        UserRolesByWebsite GetRolesForUserByWebsite(int userId, int websiteId);
        Task<UserRolesByWebsite> UpdateRolesForUserByWebsite(int userId, int websiteId, UserRolesByWebsite userRolesByWebsite);
        IList<UserRole> GetRolesForUser(int userId);
        IList<UserRole> UpdateRolesForUser(int userId, List<UserRole> userRolesToUpdate);
		Task<int> DeleteAllUsers();
		Task DeleteRegistrationsOlderThan(bool notify, int daysToKeepPendingRegistration);
		IList<User> GetUsersByOrganisationId(int organisationId);
        UsersAndJobIdsForOrganisation GetUsersAndJobIdsByOrganisationId(int organisationId);
        Task UpdateFieldsDueToLogin(string userToUpdateIdentifier);
        Task DeleteDormantAccounts(DateTime BaseDate);
        Task MarkAccountsForDeletion(DateTime BaseDate);
    }

	public class UsersService : IUsersService
	{
		private readonly IdentityContext _context;
		private readonly ILogger<UsersService> _logger;
		private readonly IProviderManagementService _providerManagementService;
		private readonly IEmailService _emailService;

		public UsersService(IdentityContext context, ILogger<UsersService> logger, IProviderManagementService providerManagementService, IEmailService emailService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_providerManagementService = providerManagementService ??
			                             throw new ArgumentNullException(nameof(providerManagementService));
			_emailService = emailService;
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
			catch (DuplicateEmailException e)
			{
				_logger.LogError($"CreateUsers duplicate email address: {user.EmailAddress}, name identifier: ${user.NameIdentifier} - exception: {e.Message}");
				throw;
			}
			catch (Exception e)
			{
				_logger.LogError($"Failed to create user {user.NameIdentifier} - exception: {e.Message}");
				throw new Exception($"Failed to create user {user.NameIdentifier} - exception: {e.Message}");
			}
		}

		public User GetUser(int userId)
		{
			var user = _context.Users
				.Include(u => u.UserEmailHistory)
					.ThenInclude(u => u.ArchivedByUser)
				.Where(u => u.UserId.Equals(userId))
				.FirstOrDefault();
			
			return user != null ? new User(user) : null;
		}

		public IList<User> GetUsers(string filter = null)
		{
			return _context.FindUsers(filter)
							.Select(user => new User(user))
							.ToList();
		}
		public IList<User> GetUsersByOrganisationId(int organisationId)
		{
			return _context.GetUsersByOrganisationId(organisationId)
								.Select(user => new User(user))
                                .OrderBy(u => u.FirstName)
                                .ThenBy(u => u.LastName)
								.ToList();
		}

        public UsersAndJobIdsForOrganisation GetUsersAndJobIdsByOrganisationId(int organisationId)
        {
            return _context.GetUsersAndJobIdsByOrganisationId(organisationId);
        }

		public IList<UserDetails> FindUsers(IEnumerable<string> nameIdentifiers)
		{
			return _context.Users.Where(user => nameIdentifiers.Contains(user.NameIdentifier)).Select(user =>
				new UserDetails(user.NameIdentifier, user.DisplayName, user.EmailAddress)).ToList();
		}

		public Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> nameIdentifiers, string host)
		{
			var users = _context.GetUsers(nameIdentifiers);
			return users.ToDictionary(user => user.NameIdentifier,
				user => user.UserRoles
					.Where(userRole => EF.Functions.Like(userRole.Role.Website.Host, host))
					.Select(role => role.Role.Name));
		}

		/// <summary>
		/// UpdateUser
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="user"></param>
		/// <param name="nameIdentifierOfUserUpdatingRecord">this could be null if the user is updated when calling the api via postman + client credentials grant. it will be not null when using the identity management site.</param>
		/// <returns></returns>
		public async Task<User> UpdateUser(int userId, User user, string nameIdentifierOfUserUpdatingRecord)
		{
			try
			{
                var userToUpdate = _context.GetUser(userId);

				if (userToUpdate == null)
					throw new Exception($"User not found {userId.ToString()}");

				var emailAddressUpdated = (user.EmailAddress != null) && !userToUpdate.EmailAddress.Equals(user.EmailAddress, StringComparison.OrdinalIgnoreCase);
				if  (emailAddressUpdated)
				{
					//todo: verify email address isn't in use
					var usersWithMatchingEmailAddress = _context.Users.Where(u => EF.Functions.Like(u.EmailAddress, user.EmailAddress)).ToList();
					if (usersWithMatchingEmailAddress.Any())
					{
						if (usersWithMatchingEmailAddress.Count > 1)
						{
							throw new ValidationException("Multiple users found with same email address."); //shouldn't be possible
						}

						var userWithMatchingEmailAddress = usersWithMatchingEmailAddress.Single();
						if (!userWithMatchingEmailAddress.NameIdentifier.Equals(userToUpdate.NameIdentifier, StringComparison.OrdinalIgnoreCase))
						{
							throw new ValidationException("Email address is already in use");
						}
						//currently we're allowing you to set your email address to a previous one in your history only. if that decision changes, this would be the place to implement it.
					}

					if (userToUpdate.EmailAddress.EndsWith(Constants.Email.StaffEmailAddressEndsWith, StringComparison.OrdinalIgnoreCase) &&
					    !user.EmailAddress.EndsWith(Constants.Email.StaffEmailAddressEndsWith, StringComparison.OrdinalIgnoreCase))
					{
						throw new ValidationException($"A staff email address ending with '{Constants.Email.StaffEmailAddressEndsWith}' cannot be changed to a non-staff email address.");
					}

					int? userIdOfUserUpdatingRecord = null;
					if (!string.IsNullOrEmpty(nameIdentifierOfUserUpdatingRecord))
					{
						userIdOfUserUpdatingRecord = _context.Users.FirstOrDefault(u => EF.Functions.Like(u.NameIdentifier, nameIdentifierOfUserUpdatingRecord))?.UserId;
					}

					var emailArchiveRecord = new UserEmailHistory(userId, userToUpdate.EmailAddress, userIdOfUserUpdatingRecord, DateTime.UtcNow);

					_context.UserEmailHistory.Add(emailArchiveRecord);
				}

				userToUpdate.UpdateFromApiModel(user);
				if (emailAddressUpdated)
				{
					userToUpdate.HasVerifiedEmailAddress = false;
				}

				if (userToUpdate.IsInAuthenticationProvider)
                {
                    await _providerManagementService.UpdateUser(userToUpdate.NameIdentifier, userToUpdate);

                    if (emailAddressUpdated)
                    {
	                    await _providerManagementService.VerificationEmail(userToUpdate.NameIdentifier);
                    }
                }

				_context.SaveChanges();
				return new User(userToUpdate);
			}
			catch (Exception e)
			{
				_logger.LogError($"Failed to update user {userId.ToString()} - exception: {e} - {e.ToString()}");
				throw;
			}
		}

		public async Task<int> DeleteUser(int userId)
		{
			try
			{
				var userToDelete = _context.Users.Find(userId);
				if (userToDelete == null)
					return 0;
                var userRolesToDelete = _context.UserRoles.Where(u => u.UserId == userId);
                var userAcceptedTermsVersionToDelete = _context.UserAcceptedTermsVersions.Where(u => u.UserId == userId);
				var userEmailHistoryToDelete = _context.UserEmailHistory.Where(ueh => ueh.UserId.HasValue && ueh.UserId.Value.Equals(userId));

                _context.UserRoles.RemoveRange(userRolesToDelete);
                _context.UserAcceptedTermsVersions.RemoveRange(userAcceptedTermsVersionToDelete);
				_context.UserEmailHistory.RemoveRange(userEmailHistoryToDelete);
                _context.Users.RemoveRange(userToDelete);

                if (userToDelete.IsInAuthenticationProvider)
                {
                    await _providerManagementService.DeleteUser(userToDelete.NameIdentifier);
                }

				return _context.SaveChanges();
			}
			catch (Exception e)
			{
				_logger.LogError($"Failed to delete user {userId.ToString()} - exception: {e} - {e.InnerException}");
				throw new Exception($"Failed to delete user {userId.ToString()} - exception: {e} - {e.InnerException}");
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
				var insertedUser = _context.CreateUser(userToImport.AsUser, importing: true);
				var userWithRoles = _context.GetUser(insertedUser.UserId);

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

						if (!userWithRoles.UserRoles.Any(userRole => userRole.RoleId.Equals(importRole.RoleId.Value)))
						{
							_context.AddUsersToRole(new List<DataModels.User> { insertedUser }, importRole.RoleId.Value);
                        }
						else
						{
							_logger.LogWarning($"User: { insertedUser.UserId} already has role: {importRole.RoleName} for website: {importRole.WebsiteHost}");
                        }
					}
				}
			}
		}

        public UserRolesByWebsite GetRolesForUserByWebsite(int userId, int websiteId)
        {
            var user = _context.Users.Where((u => u.UserId == userId)).FirstOrDefault();
            var website = _context.Websites
                .Include(w => w.Service)
                .Include(w => w.Environment)
                .Where((w => w.WebsiteId == websiteId))
                .FirstOrDefault();

            if (user == null || website == null)
                return null;

            var userRoles = _context.UserRoles.Where((ur => ur.UserId == userId)).ToList();
            var rolesByWebsite = _context.Roles.Where(r => r.WebsiteId == websiteId).ToList()
                .Select(role => new UserRoleDetailed()
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

        public async Task<UserRolesByWebsite> UpdateRolesForUserByWebsite(int userId, int websiteId, UserRolesByWebsite userRolesByWebsite)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                _logger.LogError($"Failed to update user {userId.ToString()} role for " +
                                 $"website {websiteId.ToString()} - user not found");
                throw new Exception($"Failed to update user {userId.ToString()} role for " +
                                    $"website {websiteId.ToString()} - user not found");
            }

            var website = _context.Websites
                .Include(ws => ws.Service)
                .Include(ws => ws.Environment)
                .FirstOrDefault(ws => ws.WebsiteId == websiteId);
            if (website == null)
            {
                _logger.LogError($"Failed to update user {userId.ToString()} role for " +
                                 $"website {websiteId.ToString()} - website not found");
                throw new Exception($"Failed to update user {userId.ToString()} role for " +
                                    $"website {websiteId.ToString()} - website not found");
            }

            if (userRolesByWebsite.Roles == null)
            {
                _logger.LogError($"Failed to update user {userId.ToString()} role for " +
                                 $"website {websiteId.ToString()} - no user roles to update");
                throw new Exception($"Failed to update user {userId.ToString()} role for " +
                                    $"website {websiteId.ToString()} - no user roles to update");
            }

            var rolesForWebsite = _context.Roles.Where(r => r.WebsiteId == websiteId).Select(r=>r.RoleId).ToList();
            userRolesByWebsite.Roles.ForEach(r =>
            {
                if (rolesForWebsite.Contains(r.Id)) return;
                _logger.LogError($"Failed to update user {userId.ToString()} role for " +
                                 $"website {websiteId.ToString()} - role {r.Id.ToString()} is not related to " +
                                 $"website {websiteId.ToString()}");
                throw new Exception($"Failed to update user {userId.ToString()} role for " +
                                    $"website {websiteId.ToString()} - role {r.Id.ToString()} is not related to " +
                                    $"website {websiteId.ToString()}");
            });

            try
            {
                userRolesByWebsite.Roles.ForEach(updatedRole =>
                {
                    var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == updatedRole.Id);
                    // if the user has the role and HasRole is false remove the role
                    // if the user does not have the role and and HasRole is true add the role
                    if (userRole != null && updatedRole.HasRole == false)
                    {
                        _context.UserRoles.Remove(userRole);
                    }else if(userRole == null && updatedRole.HasRole)
                    {
                        var userRoleToCreate = new DataModels.UserRole
                        {
                            UserId = userId, 
                            RoleId = updatedRole.Id
                        };
                        _context.UserRoles.Add(userRoleToCreate);
                    }
                });

				await _providerManagementService.RevokeRefreshTokensForUser(user.NameIdentifier);

				_context.SaveChanges();

                return GetRolesForUserByWebsite(userId,websiteId);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update user role {userId.ToString()} for " +
                                 $"website {websiteId.ToString()} - exception: {e.Message}");
                throw new Exception(
                    $"Failed to update user role {userId.ToString()} for " +
                    $"website {websiteId.ToString()}- exception: {e.Message}");
            }
        }

        public IList<UserRole> GetRolesForUser(int userId)
        {
            var userRoles = _context.UserRoles
                .Where((ur => ur.UserId == userId))
                .Select(ur => new UserRole(ur))
                .ToList();
            return userRoles;
        }

        public IList<UserRole> UpdateRolesForUser(int userId, List<UserRole> userRolesToUpdate)
        {
            try
            {
                var userRoles = _context.UserRoles.Where(ur => ur.UserId == userId).ToList();
                userRolesToUpdate.ForEach(updatedRole =>
                {
                    // if the user role exists update it. if not add it.
                    var userRole = userRoles.Find(ur => ur.UserRoleId == updatedRole.UserRoleId);
                    if (userRole != null)
                    {
                        userRole.UpdateFromApiModel(updatedRole);
                    }
                    else
                    {
                        var userRoleToCreate = new DataModels.UserRole();
                        userRoleToCreate.UpdateFromApiModel(updatedRole);
                        _context.UserRoles.Add(userRoleToCreate);
                    }
                });
                _context.SaveChanges();
                return _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => new UserRole(ur)).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update user role {userId.ToString()} - exception: {e.ToString()}");
                throw new Exception($"Failed to update user role {userId.ToString()} - exception: {e.ToString()}", e);
            }
        }

        public async Task<int> DeleteAllUsers()
        {
			return await _context.DeleteAllUsers();
        }

        public async Task DeleteRegistrationsOlderThan(bool notify, int daysToKeepPendingRegistration)
        {
            _logger.LogWarning($"DeleteRegistrationsOlderThan - Deleting Registrations Older Than {daysToKeepPendingRegistration} days. Notify via email: {notify}"); //extra logging here in order to verify that the scheduled task is running via kibana.

	        var allUsersWithPendingRegistrationsOverAge = _context.GetPendingUsersOverAge(daysToKeepPendingRegistration).ToList();

	        if (!allUsersWithPendingRegistrationsOverAge.Any())
	        {
		        _logger.LogWarning("DeleteRegistrationsOlderThan - No registrations found to delete. exiting");
		        return;
	        }

	        var uniqueEmailAddresses = allUsersWithPendingRegistrationsOverAge.Select(u => u.EmailAddress).Distinct().ToList();
	        if (uniqueEmailAddresses.Count()  != allUsersWithPendingRegistrationsOverAge.Count())
	        {
                _logger.LogWarning("Pending registrations exist for the same email address.");
	        }

	        //1. delete user accounts: allUsersWithPendingRegistrationsOverAge
	        var recordsDeleted = await _context.DeleteUsers(allUsersWithPendingRegistrationsOverAge);

	        //2. delete user account in auth0
            foreach (var user in allUsersWithPendingRegistrationsOverAge)
            {
	            await _providerManagementService.DeleteUser(user.NameIdentifier);
            }

            //3. send notification to the email addresses, one email per email address.
            if (notify)
            {
				allUsersWithPendingRegistrationsOverAge.ForEach(x =>
				{
                    try
                    {
                        _emailService.SendEmail<DeleteRegistrationsOlderThanNotificationEmail>(x);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Failed to send delete registrations older than notification email - exception: {e}");
                    }
                    

                });

            }

            _logger.LogWarning($"DeleteRegistrationsOlderThan - Total registrations deleted : {recordsDeleted}");
        }

        public async Task MarkAccountsForDeletion(DateTime BaseDate)
        {
            try {
				
                _logger.LogWarning($"MarkAccountsForDeletion - Marking accounts for deletion");

                var cutoffDate = BaseDate.AddMonths(-AppSettings.EnvironmentConfig.MonthsUntilDormantAccountsDeleted);
			    var pendingCutOffDate = cutoffDate.AddMonths(1); //Accounts in their last month before deletion

				var users = await _context.Users
										  .Where(x => !x.IsMarkedForDeletion
												   && !x.EmailAddress.EndsWith("@nice.org.uk")
												   && x.LastLoggedInDate <= pendingCutOffDate
                                                   && x.LastLoggedInDate > cutoffDate
                                                )
										  .ToListAsync();

                if (!users.Any())
                {
                    _logger.LogWarning("MarkAccountsForDeletion - No accounts found to mark for deletion. exiting");
                    return;
                }

                users.ForEach(x =>
				{
                    x.IsMarkedForDeletion = true;

                    try
                    {
                        if (!x.IsMigrated)
                            _emailService.SendEmail<PendingDormantAccountRemovalNotificationEmailGenerator>(x);
                    }
                    catch (Exception e)
                    {
                        x.IsMarkedForDeletion = false;
                        _logger.LogError($"Failed to send pending deletion email - exception: {e}");
                    }

                });

                _context.SaveChanges();

                var totalMarkedForDeletion = 0;

                if (users != null)
                    totalMarkedForDeletion = users.Where(x => x.IsMarkedForDeletion).Count();

                _logger.LogWarning($"MarkAccountsPendingDeletion - Total accounts marked for deletion : {totalMarkedForDeletion}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to mark accounts for deletion - exception: {e}");
                throw new Exception($"Failed to mark accounts for deletion - exception: {e}", e);
			}
		}

        public async Task DeleteDormantAccounts(DateTime BaseDate)
        {
            try
            {

                _logger.LogWarning($"DeleteDormantAccounts - Deleting dormant accounts");

                var cutoffDate = BaseDate.AddMonths(-AppSettings.EnvironmentConfig.MonthsUntilDormantAccountsDeleted);

                var users = await _context.Users
                                          .Where(x => (x.LastLoggedInDate <= cutoffDate || (x.LastLoggedInDate == null && x.InitialRegistrationDate <= cutoffDate)) 
                                                      && !x.EmailAddress.EndsWith("@nice.org.uk"))
                                          .ToListAsync();

                if (!users.Any())
                {
                    _logger.LogWarning("DeleteDormantAccounts - No accounts found for deletion. exiting");
                    return;
                }

                var emailUserList = new List<DataModels.User>();

                users.ForEach(x =>
                {
                    if (x.LastLoggedInDate != null || (x.LastLoggedInDate == null && !x.IsMigrated))
                    {
                        try
                        {
                            _emailService.SendEmail<DormantAccountRemovalNotificationEmailGenerator>(x);

                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Failed to send dormant account removal notification email - exception: {e}");
                        }
                    }

                });

                var recordsDeleted = await _context.DeleteUsers(users);

                users.ForEach (x =>
                {
                    try
                    {
                        if (x.IsInAuthenticationProvider)
                            _providerManagementService.DeleteUser(x.NameIdentifier);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Failed to remove deleted user from Auth0 tenant. Email:{x.EmailAddress} Name:{x.NameIdentifier} - exception: {e}");
                    }
                });

                _logger.LogWarning($"DeleteDormantAccounts - Total accounts deleted : {recordsDeleted}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete dormant accounts - exception: {e}");
                throw new Exception($"Failed to delete dormant accounts - exception: {e}", e);
            }
        }

        /// <summary>
        /// The fields updated on login are the LastLoggedInDate, IsLockedOut, IsInAuthenticationProvider and HasVerifiedEmail address
        ///
        /// IsLockedOut and IsInAuthentication provider are set to false and true respectively, as the user is logging in from auth0, so that must be the case.
        ///
        /// likewise HasVerifiedEmail is set here as again, they'd be unable to login without verifying.
        /// Also, currently when the user clicks on the activate link in the email, it updates the auth0 db, but doesn't update our database - hence our db is potentially out of sync on this property
        /// until the user logs in, and we can't make it sync with our db currentl without exposing our api directly to the user, which we don't want to do.
        /// todo: when the profile site is up, handle the activate link in there, then redirect to confirmation page on s3. a page on the profile site can hit the api server side.
        /// </summary>
        /// <param name="userToUpdateIdentifier"></param>
        public async Task UpdateFieldsDueToLogin(string userToUpdateIdentifier)
        {
            var user = _context.Users.SingleOrDefault(x => x.NameIdentifier == userToUpdateIdentifier);

            user.LastLoggedInDate = DateTime.UtcNow;
            user.IsLockedOut = false;
            user.IsInAuthenticationProvider = true;
            user.HasVerifiedEmailAddress = true;
            user.IsMarkedForDeletion = false;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
            
        }
    }
}