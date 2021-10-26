using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
	public class UserEmailHistory
	{

		public int UserEmailHistoryId { get; set; }
		public int? UserId { get; set; }
		public string EmailAddress { get; set; }
		public int? ArchivedByUserId { get; set; }
		public DateTime? ArchivedDateUTC { get; set; }

		public User User { get; set; }
		public User ArchivedByUser { get; set; }
	}
}
