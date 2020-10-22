using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
	/// <summary>
	/// This class is used by HealthChecks.UserSync and is used to display discrepencies between the local database and the remote (auth provider) db.
	/// </summary>
	public class UserSync
	{
		public UserSync(int totalUsersInLocalDatabase, int totalUsersInRemoteDatabase, IEnumerable<BasicUserInfo> usersNotInLocalDbOfTheTenMostRecent)
		{
			TotalUsersInLocalDatabase = totalUsersInLocalDatabase;
			TotalUsersInRemoteDatabase = totalUsersInRemoteDatabase;
			UsersNotInLocalDBOfTheTenMostRecent = usersNotInLocalDbOfTheTenMostRecent;
		}

		public int TotalUsersInLocalDatabase { get; private set; }
		public int TotalUsersInRemoteDatabase { get; private set; }

		/// <summary>
		/// This does not include all the users. only the 10 most recent.
		/// </summary>
		public IEnumerable<BasicUserInfo> UsersNotInLocalDBOfTheTenMostRecent { get; private set; }
	}
}