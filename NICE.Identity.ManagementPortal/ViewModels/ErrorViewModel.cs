namespace NICE.Identity.ManagementPortal.ViewModels
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