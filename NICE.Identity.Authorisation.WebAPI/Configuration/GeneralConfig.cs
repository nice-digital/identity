namespace NICE.Identity.Authorisation.WebAPI.Configuration
{
    public class GeneralConfig
    {
        public int MonthsUntilDormantAccountsDeleted { get; set; } = 36;
        public int DaysToKeepPendingRegistrations { get; set; } = 30;
    }
}
