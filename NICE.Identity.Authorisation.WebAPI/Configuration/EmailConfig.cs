using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Configuration
{
	public class EmailConfig
	{
		public string Server { get; set; }
		public int Port { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string[] Allowlist { get; set; }
		public string SenderAddress { get; set; }
	}
}
