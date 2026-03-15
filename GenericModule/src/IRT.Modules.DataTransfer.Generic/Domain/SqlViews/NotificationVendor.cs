using System.ComponentModel.DataAnnotations;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews
{
    public enum NotificationVendor
    {
        [Display(Name = Resources.GenericNotificationDefinition.ResourceNames.Rave, ResourceType = typeof(Resources.GenericNotificationDefinition))]
        Rave,
        [Display(Name = Resources.GenericNotificationDefinition.ResourceNames.Veeva, ResourceType = typeof(Resources.GenericNotificationDefinition))]
        Veeva
    }
}
