﻿using System;
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

		public static void AddUser(ref IdentityContext context, int userId = 1, string firstName = "Steve", string lastName = "Zissou", string nameIdentifier = "some auth0 userid", string emailAddress = "steve@belafonte.com")
		{
			context.Users.Add(new User(userId, nameIdentifier, firstName, lastName, true, true, null, null, true, emailAddress, false, true, true, true));
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

        public static void AddJob(ref IdentityContext context, int jobId = 1, int userId = 1, int organisationId = 1, bool isLead = false)
        {
			context.Jobs.Add(new Job(jobId, userId, organisationId, isLead));
        }

		public static Organisation AddOrganisation(ref IdentityContext context, int organisationId = 1, string name = "NICE")
		{
			var organisation = new Organisation(organisationId, name);
			context.Organisations.Add(organisation);
			return organisation;
		}

		public static void AddOrganisationRole(ref IdentityContext context, int organisationRoleId = 1, int organisationId = 1, int roleId = 1)
        {
			context.OrganisationRoles.Add(new OrganisationRole(organisationRoleId, organisationId, roleId));
        }

        public static void AddAll(ref IdentityContext context)
		{
			AddService(ref context);
			AddEnvironment(ref context);
			AddWebsite(ref context);
			AddRole(ref context);
			AddUserRole(ref context);
            AddTermsVersion(ref context);
			AddUser(ref context);
            AddUserAcceptedTermsVersion(ref context);
			AddJob(ref context);
			AddOrganisation(ref context);
			AddOrganisationRole(ref context);
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

		public static void AddWithTwoRolesTwoOrganisations(ref IdentityContext context)
		{
			AddRole(ref context);
			AddRole(ref context, roleId: 2, websiteId: 1, name: "Moderator");
			AddOrganisation(ref context);
			AddOrganisation(ref context, 2, "NHS");
			context.SaveChanges();
		}

		public static void AddWithTwoJobsOneLead(ref IdentityContext context, string leadOrganisationName = "Belafonte")
		{
			AddAll(ref context);
			context.SaveChanges();

			var org2 = AddOrganisation(ref context, 2, leadOrganisationName);
			AddJob(ref context, 2, organisationId: 2, isLead: true);
			context.SaveChanges();
		}
	}
}
