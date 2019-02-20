using System;
using System.Collections.Generic;
using System.Text;

namespace NICE.Identity.Authentication.Sdk
{
	public class AuthConfiguration
	{
		public string Domain { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string RedirectUri { get; set; }
		public string PostLogoutRedirectUri { get; set; }
	}
}
