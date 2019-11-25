using System;
using Microsoft.Owin.Security.Provider;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
	public class ImportUser
	{
		public Guid UserId { get; set; }

		//weirdness here it support comments' output only including displayname instead of a firstname and lastname.
		private string _firstName;
		public string FirstName
		{
			get
			{
				if (!string.IsNullOrEmpty(_firstName) || !string.IsNullOrEmpty(_lastName))
				{
					return _firstName;
				}

				if (!string.IsNullOrEmpty(DisplayName))
				{
					var names = DisplayName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
					if (names.Length == 2)
					{
						return names[0];
					}
				}
				return null;
			}
			set => _firstName = value;
		}

		private string _lastName;
		public string LastName
		{
			get
			{
				if (!string.IsNullOrEmpty(_firstName) || !string.IsNullOrEmpty(_lastName))
				{
					return _lastName;
				}

				if (!string.IsNullOrEmpty(DisplayName))
				{
					var names = DisplayName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
					if (names.Length == 2)
					{
						return names[1];
					}
				}
				return null;
			}
			set => _lastName = value;
		}

		public string DisplayName { get; set; }
		public string EmailAddress { get; set; }

		public string NameIdentifier =>
			$"{AuthenticationConstants.NameIdentifierDefaultPrefix}{UserId.ToString().ToLower()}";
	}
}