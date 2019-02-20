﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Authorisation.WebAPI.Repositories
{
    public partial class IdentityContext : DbContext
    {
        //public List<UserRoles> GetClaims(int userId)
        //{
        //    return EntityFrameworkQueryableExtensions.Include<UserRoles, Roles>(
        //            UserRoles.Where(userRole => userRole.UserId.Equals(userId)), userRoles => userRoles.Role)
        //        .Include(userRoles => userRoles.User)
        //        .ToList();
        //}

        public Users GetUser(string authenticationProviderUserId)
        {
            return Users.Where(users => users.Auth0UserId.Equals(authenticationProviderUserId))
                .Include(users => users.UserRoles)
					.ThenInclude(userRoles => userRoles.Role)
                .Single();
        }

    }
}