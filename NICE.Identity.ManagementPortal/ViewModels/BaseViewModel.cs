namespace z_deprecated_NICE.Identity.ViewModels
{
	public class BaseViewModel
    {
	    public BaseViewModel(string htmlHeadTitle, string pageTitle)
	    {
		    HtmlHeadTitle = htmlHeadTitle;
		    PageTitle = pageTitle;
	    }

	    /// <summary>
		/// The title that goes in: &lt;html&gt;&lt;head&gt;&lt;title&gt;&lt;/title&gt;&lt;/head&gt;&lt;/html&gt;
		/// </summary>
		public string HtmlHeadTitle { get; }

		/// <summary>
		/// The title that goes an h1 on the page
		/// </summary>
	    public string PageTitle { get; }
    }
}
