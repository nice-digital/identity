namespace NICE.Identity.Authentication.Sdk.Domain
{
	public class Organisation
	{
		public Organisation(int organisationId, string organisationName, bool isLead)
		{
			OrganisationId = organisationId;
			OrganisationName = organisationName;
			IsLead = isLead;
		}

		public int OrganisationId { get; private set; }
		public string OrganisationName { get; private set; }
		public bool IsLead { get; private set; }
	}
}