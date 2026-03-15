using System;

namespace IRT.Modules.DataTransfer.Generic.Domain.Exceptions
{
    [Serializable]
    public class ProviderNotFoundException : Exception
    {
        public ProviderNotFoundException(string providerTypeName)
            : base(string.Format("'{0}' provider not found.", providerTypeName))
        {
        }
    }
}