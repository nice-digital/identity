using System.Net.Http;

namespace NICE.Identity.TestClient.M2MApp.Common
{
    public static class HttpResponseHelpers
    {
	    public static HttpResponseMessage Send(HttpClient client, HttpRequestMessage request)
	    {
		    return client.SendAsync(request).Result;
	    }

	    public static string GetString(HttpResponseMessage response)
	    {
		    var content = response.Content;
		    var readTask = content.ReadAsStringAsync();
		    return readTask.Result;
	    }
	}
}
