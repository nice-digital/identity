namespace NICE.Identity.Authentication.Sdk
{
	internal interface IAuth0HttpClient
	{
		Publication GetPublication(string url, JwtToken token);
	}
}