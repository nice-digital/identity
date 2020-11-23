using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
	/// <summary>
	/// This class is used by HealthChecks.UserSync and is used to display discrepencies between the local database and the remote (auth provider) db.
	/// </summary>
	public class UserSync
	{
		public UserSync(int userCountLocalDB, int userCountLocalDbWhereMarkedInRemote, int userCountRemoteDB, IEnumerable<BasicUserInfo> usersNotInLocalDbOfTheTenMostRecent)
		{
			UserCountLocalDB = userCountLocalDB;
			UserCountLocalDBWhereMarkedInRemote = userCountLocalDbWhereMarkedInRemote;
			UserCountRemoteDB = userCountRemoteDB;
			UsersNotInLocalDBOfTheTenMostRecent = usersNotInLocalDbOfTheTenMostRecent;
		}

		public int UserCountLocalDB { get; private set; }
		public int UserCountLocalDBWhereMarkedInRemote { get; private set; }
		public int UserCountRemoteDB { get; private set; }

		/// <summary>
		/// This does not include all the users. only the 10 most recent.
		/// </summary>
		public IEnumerable<BasicUserInfo> UsersNotInLocalDBOfTheTenMostRecent { get; private set; }
	}
}