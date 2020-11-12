using System;

namespace NICE.Identity.Authorisation.WebAPI
{
	public class DuplicateEmailException : ArgumentException
	{
		public DuplicateEmailException(string? message) : base(message) {}
	}
}
