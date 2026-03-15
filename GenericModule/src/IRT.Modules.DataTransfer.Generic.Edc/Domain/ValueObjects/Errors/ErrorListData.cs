using System;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.Errors
{
    public class ErrorListData
    {
        public string Title { get; set; }

        public Guid NotificationId { get; set; }

        public string ErrorMessage { get; set; }

        public ErrorListData(string title, Guid notificationId, string errorMessage)
        {
            Title = title;
            NotificationId = notificationId;
            ErrorMessage = errorMessage;
        }
    }
}
