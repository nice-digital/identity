namespace z_deprecated_NICE.Identity.ViewModels
{
	public class ErrorViewModel : BaseViewModel
	{
		public ErrorViewModel(string htmlHeadTitle, string pageTitle, string requestId) : base(htmlHeadTitle, pageTitle)
		{
			RequestId = requestId;
		}

		public string RequestId { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
	}
}