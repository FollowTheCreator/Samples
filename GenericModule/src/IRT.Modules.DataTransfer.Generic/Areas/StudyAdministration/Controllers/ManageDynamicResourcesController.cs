using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Configuration;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.ExtendedProperties.Registries;
using Frameworks.ExtendedProperties.Services.Interfaces;
using IRT.Domain;
using IRT.Domain.Infrastructure.ExtendedProperties.Interfaces;
using IRT.Domain.Services.Interfaces;
using IRT.Domain.ViewsSql.Study;
using IRT.Domain.ViewsSql.User;
using IRT.Shared.Controllers;
using Kernel.AspNetMvc.Security.Attributes;
using Kernel.AspNetMvc.Security.Services;
using Kernel.Globalization.Providers;
using Unity;
using Operations = IRT.Modules.DataTransfer.Generic.Domain.Operations;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Controllers
{
    [AllowAuthorizedFor(Operations.GenericDataTransfer.ManageDynamicResources)]
    [Area(Shared.Helpers.Constants.AreaNames.StudyAdministration)]
    public class ManageDynamicResourcesController : EntityCrudController
    {
        public ManageDynamicResourcesController(
            IExtendedPropertiesFunctionConfigurator functionConfigurator,
            IEntityDataService entityDataService,
            IEntityCrudFunctionDataService entityCrudFunctionDataService,
            IExtendedPropertiesRegistry extendedPropertiesRegistry,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IFunctionNamingService functionNamingService,
            IUnityContainer unityContainer,
            IRTDbContext dbContext,
            IOptions<IrtAppConfigManager> appSettings,
            IContextService<StudySqlView, UserSqlView> contextService,
            IDateTimeFormatProviderService dateTimeFormatProviderService)
            : base(functionConfigurator: functionConfigurator,
                   entityDataService: entityDataService,
                   entityCrudFunctionDataService: entityCrudFunctionDataService,
                   extendedPropertiesRegistry: extendedPropertiesRegistry,
                   extendedPropertiesValueProvider: extendedPropertiesValueProvider,
                   functionNamingService: functionNamingService,
                   unityContainer: unityContainer,
                   dbContext: dbContext,
                   appSettings: appSettings,
                   contextService: contextService,
                   dateTimeFormatProviderService: dateTimeFormatProviderService)
        {
        }

        public ActionResult View(Guid id)
        {
            var viewModel = GetEntityCrudViewModel(id);

            return View("Index", viewModel);
        }
    }
}