namespace NICE.Identity.TestClient.M2MApp.Models
{
    public class JwtToken
    {
	    public string AccessToken { get; set; }

	    public string TokenType { get; set; }

	    public int ExpiresIn { get; set; }
	}
}
