namespace NICE.Identity.Authorisation.WebAPI.Configuration
{
    public class GeneralConfig
    {
        public int DaysToKeepPendingRegistrations { get; set; } = 30;
        public int MonthsToKeepDormantAccounts { get; set; } = 36;
    }
}
