using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.Models;

namespace NICE.Identity.Test.Infrastructure
{
	public static class TestData
	{
		private static void AddService(ref IdentityContext context, int serviceId = 1, string serviceName = "Consultation comments")
		{
			context.Services.Add(new Services(serviceId, serviceName));
		}

		private static void AddEnvironment(ref IdentityContext context, int environemntId = 1, string environmentName = "beta")
		{
			context.Environments.Add(new Environments(environemntId, environmentName));
		}

		private static void AddWebsite(ref IdentityContext context, int websiteId = 1, int serviceId = 1, int environmentId = 1, string host = "test.nice.org.uk")
		{
			context.Websites.Add(new Websites(websiteId, serviceId, environmentId, host));
		}

		private static void AddRole(ref IdentityContext context, int roleId = 1, int websiteId = 1, string name = "Administrator")
		{
			context.Roles.Add(new Roles(roleId, websiteId, name));
		}

		private static void AddUserRole(ref IdentityContext context, int userRoleId = 1, int roleId = 1, int userId = 1)
		{
			context.UserRoles.Add(new UserRoles(userRoleId, roleId, userId));
		}

		private static void AddUser(ref IdentityContext context, int userId = 1, string title = "Mr", string firstName = "Steve", string lastName = "Zissou")
		{
			context.Users.Add(new Users(userId, null, null, title, firstName, lastName, true, true, null, null, true, null, null, null, true, true));
		}

		public static void AddAll(ref IdentityContext context)
		{
			AddService(ref context);
			AddEnvironment(ref context);
			AddWebsite(ref context);
			AddRole(ref context);
			AddUserRole(ref context);
			AddUser(ref context);
			context.SaveChanges();
		}

		public static void AddWithTwoRoles(ref IdentityContext context)
		{
			AddAll(ref context);
			AddRole(ref context, roleId: 2, websiteId: 1, name: "Moderator");
			AddUserRole(ref context, userRoleId: 2, roleId: 2);
			context.SaveChanges();
		}
	}
}
