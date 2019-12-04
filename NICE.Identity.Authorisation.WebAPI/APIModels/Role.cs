namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Role
    {
        public Role()
        {
        }

        public Role(int roleId, int websiteId, string name, string description)
        {
            RoleId = roleId;
            WebsiteId = websiteId;
            Name = name;
            Description = description;
        }

        public Role(DataModels.Role role)
        {
            RoleId = role.RoleId;
            WebsiteId = role.WebsiteId;
            Name = role.Name;
            Description = role.Description;
        }

        public int? RoleId { get; set; }
        public int? WebsiteId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}