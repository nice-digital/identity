namespace NICE.Identity.Authorisation.WebAPI.Configuration
{
    public class AccountDeletionConfig
    {
        public int DaysToKeepPendingRegistrations { get; set; } = 30;
        public int MonthsToKeepDormantAccounts { get; set; } = 36;
    }
}