using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Kernel.AspNetMvc;
using Kernel.AspNetMvc.Configuration;
using Kernel.AspNetMvc.Extensions;
using Kernel.DDD.Dispatching;
using Kernel.DDD.Domain.Commands;
using Unity;
using Unity.Resolution;

namespace IRT.Modules.DataTransfer.Generic.Areas.ClientNotification.Controllers
{
    public class BaseDTGenericWebApiController : Controller
    {
        [Dependency]
        public ICommandBus CommandBus { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        public KernelAppConfigManager ConfigManager => UnityContainer.Resolve<IOptions<KernelAppConfigManager>>(Array.Empty<ResolverOverride>()).Value;

        protected CommandResult SendCommand(ICommand command, bool addErrorsToModelState = true)
        {
            CommandResult commandResult = CommandResult.CreateFromResult(CommandBus.SendCommand(command), ConfigManager.ShowDetailedErrors);
            if (commandResult.IsFailure && addErrorsToModelState)
            {
                base.ModelState.AddGeneralModelError(commandResult);
            }

            return commandResult;
        }
    }
}
