using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IRT.Modules.DataTransfer.Generic.Areas.ClientNotification.Models;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;

namespace IRT.Modules.DataTransfer.Generic.Areas.ClientNotification.Controllers
{
    [Route("api/DTGeneric/[controller]")]
    [AllowAnonymous]
    public class ReceiveClientNotificationWebApiController : BaseDTGenericWebApiController
    {
        [HttpPost]
        public IActionResult Post([FromBody]ReceiveClientNotificationViewModel clientNotificationViewModel)
        {
            var result = SendCommand(new ReceiveClientNotification
            {
                Title = clientNotificationViewModel.Title,
                Body = clientNotificationViewModel.Body,
                AdditionalInfo = clientNotificationViewModel.AdditionalInfo,
                SubjectId = clientNotificationViewModel.SubjectId,
                SubjectVisitId = clientNotificationViewModel.SubjectVisitId,
                SiteId = clientNotificationViewModel.SiteId,
                IsNotificationSent = true
            });

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
