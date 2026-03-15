using System;

namespace IRT.Modules.DataTransfer.Generic.Domain.Exceptions
{
    [Serializable]
    public class AttributeNotFoundException : Exception
    {
        public AttributeNotFoundException(Type attributeType, Type sourceType)
            : base(string.Format("'{0}' attribute not found on '{1}' provider.",
                attributeType.FullName,
                sourceType.FullName))
        {
        }
    }
}