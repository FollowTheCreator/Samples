using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using IRT.Domain;
using Kernel.AspNetMvc.Security.Filters;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Filters
{
    public class ClientNetworkAuthorizationFilter : ClientWhitelistAuthorizationFilter
    {
        private readonly IOptions<IrtAppConfigManager> configManager;

        public ClientNetworkAuthorizationFilter(IOptions<IrtAppConfigManager> configManager)
        {
            this.configManager = configManager;
        }

        protected override IEnumerable<string> GetHostNameRegexWhitelist(HttpContext context) => configManager.Value.ApiWhitelist.SplitByComma();

        protected override IEnumerable<string> GetIpWhitelist(HttpContext context) => configManager.Value.ApiAllowListIp.SplitByComma();
    }
}
