using System.ComponentModel.DataAnnotations;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews
{
    public enum NotificationType
    {
        [Display(Name = Resources.GenericNotificationDefinition.ResourceNames.Generic, ResourceType = typeof(Resources.GenericNotificationDefinition))]
        Generic,
        [Display(Name = Resources.GenericNotificationDefinition.ResourceNames.Client, ResourceType = typeof(Resources.GenericNotificationDefinition))]
        Client,
        [Display(Name = Resources.GenericNotificationDefinition.ResourceNames.SelfTransform, ResourceType = typeof(Resources.GenericNotificationDefinition))]
        SelfTransform,
    }
}
