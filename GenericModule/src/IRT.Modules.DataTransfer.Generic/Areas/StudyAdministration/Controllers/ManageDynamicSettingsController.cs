using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Configuration;
using Frameworks.ExtendedProperties.Metadata;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.ExtendedProperties.Registries;
using IRT.Domain;
using IRT.Domain.Resources;
using IRT.Domain.Services.Interfaces;
using IRT.Domain.ViewsSql.Depot;
using IRT.Domain.ViewsSql.Study;
using IRT.Domain.ViewsSql.User;
using IRT.Domain.ViewsSql.Visit;
using IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DtExtendedProperty;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Commands;
using IRT.Modules.DataTransfer.Generic.Domain.Database;
using IRT.Shared.Controllers;
using IRT.Shared.Models;
using Kernel.AspNetMvc.Extensions;
using Kernel.AspNetMvc.Security.Attributes;
using Kernel.AspNetMvc.Security.Services;
using Kernel.DDD.Domain.Commands;
using Kernel.Kendo.Extensions;
using Kernel.Utilities;
using Kernel.Utilities.Extensions;
using Unity;
using Operations = IRT.Modules.DataTransfer.Generic.Domain.Operations;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Controllers
{
    [AllowAuthorizedFor(Operations.GenericDataTransfer.ManageDynamicSettings)]
    [Area(Shared.Helpers.Constants.AreaNames.StudyAdministration)]
    public class ManageDynamicSettingsController : ExtendedPropertiesCrudController
    {
        private Regex clearSpaceTabNewlineRegex = new Regex(@"^(\\t|\\n|\s)+|(\\t|\\n|\s)+$");

        public ManageDynamicSettingsController(
            IExtendedPropertiesFunctionConfigurator functionConfigurator,
            IExtendedPropertiesRegistry extendedPropertiesRegistry,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IFunctionNamingService functionNamingService,
            IUnityContainer unityContainer,
            ModuleDbContext dbContext,
            IRemoteCacheService remoteCacheService,
            IOptions<IrtAppConfigManager> appSettings,
            IContextService<StudySqlView, UserSqlView> contextService)
             : base(
                  functionConfigurator,
                  extendedPropertiesRegistry,
                  extendedPropertiesValueProvider,
                  functionNamingService,
                  unityContainer,
                  dbContext,
                  remoteCacheService,
                  appSettings,
                  contextService)
        {
        }

        [HttpGet]
        public virtual ActionResult Create(Guid id)
        {
            ViewBag.EntityType = GetEntityTypes();
            ViewBag.OperationId = id;

            return View();
        }

        [HttpPost]
        public virtual ActionResult Create(Guid id, DynamicSettingCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dynamicSettingId = SequenticalGuid.NewGuid();
            var defaultValue = ClearSpaceTabNewline(viewModel.DefaultValue, string.Empty);

            var command = new CreateDynamicSetting
            {
                DynamicSettingId = dynamicSettingId,
                DefaultValue = defaultValue,
                Name = viewModel.Name,
                Description = viewModel.Description,
                EntityTypeName = viewModel.EntityTypeName,
                ExtendedPropertyName = viewModel.ExtendedPropertyName
            };

            var commandResult = SendCommand(command);

            if (!commandResult)
            {
                this.FlashError(commandResult);

                ViewBag.EntityType = GetEntityTypes();
                ViewBag.OperationId = id;
                return View(viewModel);
            }

            this.FlashSuccess(IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Views.ManageDynamicSettings.Resources.Create.DynamicSettingWasSuccessfullyCreated);
            return RedirectToAction("Index", new { id = id });
        }

        [HttpPost]
        public override ActionResult Update(
            [DataSourceRequest] DataSourceRequest request,
            Guid id,
            string entityTypeName,
            string entityId,
            ExtendedPropertyViewModel viewModel)
        {
            var function = functionConfigurator.GetCrudFunction<ExtendedPropertiesCrudFunction>(id);
            var entityBehavior = functionConfigurator.GetEntityBehavior(entityTypeName);
            var value = ClearSpaceTabNewline(viewModel.Value, string.Empty);

            var dtExtendedPropertyDto = new DtExtendedPropertyDto
            {
                EntityId = entityId,
                EntityTypeName = entityTypeName,
                ExtendedPropertiesTypeName = viewModel.ExtendedPropertiesTypeName,
                ExtendedPropertyName = viewModel.ExtendedPropertyName,
                OperationId = id,
                Value = value
            };

            var command = new UpdateCommand<DtExtendedPropertyDto> { Item = dtExtendedPropertyDto };

            var commandResult = SendCommand(command);

            var message = (function.UpdateMessage != null ? function.UpdateMessage.GetString() : ExtendedPropertiesResources.UpdateExtendedPropertyMessage).F(
                GetEntityDisplayName(entityBehavior.EntityQueryBuilder(dbContext, viewModel.ExtendedPropertiesTypeName).FirstOrDefault(x => x.EntityId == entityId),
                    entityBehavior.EntityDisplayNameDefaultResourceType),
                GetEntityTypeDisplayName(entityBehavior),
                extendedPropertiesRegistry.GetExtendedProperty(entityTypeName, viewModel.ExtendedPropertiesTypeName, viewModel.ExtendedPropertyName).GetDisplayName(unityContainer));

            return this.KendoGrid(viewModel, request, message);
        }

        private List<SelectListItem> GetEntityTypes()
        {
            return new List<SelectListItem>()
            {
                new ()
                {
                    Text = Resources.ManageDynamicSettingsController.Study,
                    Value = typeof(StudySqlView).FullName
                },
                new ()
                {
                    Text = Resources.ManageDynamicSettingsController.Depot,
                    Value = typeof(DepotSqlView).FullName
                },
                new ()
                {
                    Text = Resources.ManageDynamicSettingsController.Visit,
                    Value = typeof(VisitSqlView).FullName
                }
            };
        }

        private string ClearSpaceTabNewline(string input, string replacement)
        {
            return clearSpaceTabNewlineRegex.Replace(input, replacement);
        }
    }
}
