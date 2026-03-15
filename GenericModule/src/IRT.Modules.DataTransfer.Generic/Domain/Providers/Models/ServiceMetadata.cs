using System;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Domain.Providers.Models
{
    public class ServiceMetadata
    {
        public ServiceMetadata(string serviceId, string displayName)
        {
            if (serviceId.IsNullOrWhiteSpace())
            {
                throw new ArgumentException($"'{nameof(serviceId)}' cannot be null or whitespace.", nameof(serviceId));
            }

            if (displayName.IsNullOrWhiteSpace())
            {
                throw new ArgumentException($"'{nameof(displayName)}' cannot be null or whitespace.", nameof(serviceId));
            }

            ServiceId = serviceId;
            DisplayName = displayName;
        }

        public string ServiceId { get; }

        public string DisplayName { get; }

        public bool IsAbstract => string.IsNullOrWhiteSpace(DisplayName);
    }
}