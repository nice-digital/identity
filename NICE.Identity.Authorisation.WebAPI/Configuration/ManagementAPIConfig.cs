using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Configuration
{
    public class ManagementAPIConfig
    {
        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string ApiIdentifier { get; set; }
        public string Grant_Type { get; set; }
        public string DurationOfBreakInMinutes { get; set; }
        public string HandledEventsAllowedBeforeBreaking { get; set; }
    }
}
