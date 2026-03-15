using Kernel.AspNetMvc.Configuration;

namespace IRT.Modules.DataTransfer.Generic.Domain.Configuration
{
    public class DTGenericConfigManager : KernelAppConfigManager
    {
        public string ApacheHopUsername { get; set; }

        public string ApacheHopPassword { get; set; }

        public string ApacheHopBaseUrl { get; set; }
    }
}
