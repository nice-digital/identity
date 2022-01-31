using System;

namespace NICE.Identity.Authorisation.WebAPI
{
	public class DuplicateEmailException : ArgumentException
	{
		public DuplicateEmailException(string? message) : base(message) {}
	}

	/// <summary>
	/// This type of error is handled differently by the UI. - Instead of a big error screen, it'll show validation exceptions.
	/// </summary>
	public class ValidationException : ApplicationException
	{
		public ValidationException(string? message) : base(message) {}
	}
}
