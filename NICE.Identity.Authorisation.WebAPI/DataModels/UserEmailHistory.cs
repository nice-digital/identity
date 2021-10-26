using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
	public class UserEmailHistory
	{
		public UserEmailHistory()
		{
		}

		public UserEmailHistory(int? userId, string emailAddress, int? archivedByUserId, DateTime? archivedDateUtc)
		{
			UserId = userId;
			EmailAddress = emailAddress;
			ArchivedByUserId = archivedByUserId;
			ArchivedDateUTC = archivedDateUtc;
		}

		public int UserEmailHistoryId { get; set; }
		public int? UserId { get; set; }
		public string EmailAddress { get; set; }
		public int? ArchivedByUserId { get; set; }
		public DateTime? ArchivedDateUTC { get; set; }

		[JsonIgnore]
		public User User { get; set; }
		[JsonIgnore]
		public User ArchivedByUser { get; set; }
	}
}
