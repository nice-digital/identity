using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.APIModels.Responses
{
	public class Status
	{
		public Status(bool isAuthenticated, string displayName, List<KeyValuePair<string, string>> links)
		{
			IsAuthenticated = isAuthenticated;
			DisplayName = displayName;
			Links = links;
		}

		public bool IsAuthenticated { get; }

		public string DisplayName { get; }

		public List<KeyValuePair<string, string>> Links { get; }
	}
}
