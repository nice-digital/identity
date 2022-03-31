using System;
using System.Collections.Generic;
using System.Linq;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{

    public class UsersAndJobIdsForOrganisation
    {
        public int OrganisationId { get; set; }
        public Organisation Organisation { get; set; }
        public List<UserAndJobId> Users { get; set; }
    }

    public class UserAndJobId
    {
        public UserAndJobId()
        {
        }
        
        public int UserId { get; set; }
        public User User { get; set; }
        public int JobId { get; set; }
    }
}