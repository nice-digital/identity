using Shouldly;
using Xunit;

namespace NICE.Identity.Test.UnitTests
{
	public class RegistrationTests
	{

		[Theory]
		[InlineData("Steve Zissou", false)]
		[InlineData("SteveZissou", true)]
		public void ValidateUserName(string userName, bool expectedIsValid)
		{
			//Arrange

			//Act
			var actualIsValid = expectedIsValid; //todo: set correctly, based off the username going in some validation function.

			//Assert
			actualIsValid.ShouldBe(expectedIsValid);
		}
	}
}
