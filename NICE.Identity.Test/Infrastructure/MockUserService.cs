﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.User;

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

		public List<User> GetUsers()
		{
			throw new NotImplementedException();
		}

		public List<UserDetails> FindUsers(IEnumerable<string> nameIdentifiers)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> nameIdentifiers, string host)
		{
			throw new NotImplementedException();
		}

		public User UpdateUser(int userId, User user)
		{
			throw new NotImplementedException();
		}

		public int DeleteUser(int userId)
		{
			throw new NotImplementedException();
		}

		public void ImportUsers(IList<ImportUser> usersToImport)
		{
			throw new NotImplementedException();
		}

		Task<User> IUsersService.UpdateUser(int userId, User user)
		{
			throw new NotImplementedException();
		}
	}
}