using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Test.Infrastructure
{
	public static class TestData
	{
		public static void AddService(ref IdentityContext context, int serviceId = 1, string serviceName = "Consultation comments")
		{
			context.Services.Add(new Service(serviceId, serviceName));
		}

        public static void AddEnvironment(ref IdentityContext context, int environemntId = 1, string environmentName = "beta")
		{
			context.Environments.Add(new Authorisation.WebAPI.DataModels.Environment(environemntId, environmentName));
		}

        public static void AddWebsite(ref IdentityContext context, int websiteId = 1, int serviceId = 1, int environmentId = 1, string host = "test.nice.org.uk")
		{
			context.Websites.Add(new Website(websiteId, serviceId, environmentId, host));
		}

        public static void AddRole(ref IdentityContext context, int roleId = 1, int websiteId = 1, string name = "Administrator", string description = "the administrator role is the super user")
		{
			context.Roles.Add(new Role(roleId, websiteId, name, description));
		}

        public static void AddUserRole(ref IdentityContext context, int userRoleId = 1, int roleId = 1, int userId = 1)
		{
			context.UserRoles.Add(new UserRole(userRoleId, roleId, userId));
		}

		public static void AddUser(ref IdentityContext context, int userId = 1, string firstName = "Steve", string lastName = "Zissou", string nameIdentifier = "some auth0 userid")
		{
			context.Users.Add(new User(userId, nameIdentifier, firstName, lastName, true, true, null, null, true, null, true, true, true, true));
		}

        private static void AddTermsVersion(ref IdentityContext context, int creatorId = 1, int version = 1, DateTime? versionDate = null)
        {
            var vdate = versionDate ?? new DateTime(2019, 1, 1);
            context.TermsVersions.Add(new TermsVersion(11, vdate, creatorId));
        }

        private static void AddUserAcceptedTermsVersion(ref IdentityContext context, int acceptedId = 1, int userId = 1, int versionId = 1, DateTime? acceptedDate = null)
        {
            var adate = acceptedDate ?? new DateTime(2019, 1, 1, 1, 1, 1);
            context.UserAcceptedTermsVersions.Add(new UserAcceptedTermsVersion(acceptedId, userId, versionId, adate));
        }

		public static void AddAll(ref IdentityContext context)
		{
			AddService(ref context);
			AddEnvironment(ref context);
			AddWebsite(ref context);
			AddRole(ref context);
			AddUserRole(ref context);
			AddUser(ref context);
            AddTermsVersion(ref context);
            AddUserAcceptedTermsVersion(ref context);
			context.SaveChanges();
		}

		public static void AddWithTwoRoles(ref IdentityContext context)
		{
			AddAll(ref context);
			AddRole(ref context, roleId: 2, websiteId: 1, name: "Moderator");
			AddUserRole(ref context, userRoleId: 2, roleId: 2);
			context.SaveChanges();
		}

		public static void AddUserNoRole(ref IdentityContext context)
		{
			AddUser(ref context);
			context.SaveChanges();
		}
	}
}
