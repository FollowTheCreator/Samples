using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Frameworks.Notifications;
using Frameworks.Notifications.Entities;
using Frameworks.Notifications.Resources;
using IRT.Domain;
using IRT.Domain.Notifications;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.Templating;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Services.Implementations;
using Kernel.EntityFramework.Extensions;
using Kernel.EntityFramework.Interfaces;
using Kernel.Utilities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Database
{
    public class EdcDbSeedingService : IDbSeedingService<EdcDbContext>
    {
        private IGenericNotificationDataServiceRepository notificationDataProviderRepository;
        private StringResourceDescriptor dummyNotificationDefinitionResource = new StringResourceDescriptor(typeof(DynamicNotificationDefinition), DynamicNotificationDefinition.ResourceNames.DummyResourceName);

        public EdcDbSeedingService(IGenericNotificationDataServiceRepository notificationDataProviderRepository)
        {
            this.notificationDataProviderRepository = notificationDataProviderRepository;
        }

        public void Seed(EdcDbContext context)
        {
            SeedRaveSelfTransformNotificationDefinitions(context);
            SeedVeevaSelfTransformNotificationDefinitions(context);

            RegisterRuntimeNotificationDefinitions(context);
        }

        private void SeedRaveSelfTransformNotificationDefinitions(IRTDbContext context)
        {
            Guid raveSTSubjectNotificationTypeId = new Guid("297F8097-A435-4198-8AC2-EAE8A93F25DF");
            Guid raveSTScreenFailedNotificationTypeId = new Guid("297F8097-A435-4198-8AC2-6310E5148D63");
            Guid raveSTDemographicsNotificationTypeId = new Guid("297F8097-A435-4198-8AC2-34C9A3318861");
            Guid raveSTRandomizationNotificationTypeId = new Guid("297F8097-A435-4198-8AC2-53A26D71F1C5");
            Guid raveSTDrugDispensationNotificationTypeId = new Guid("297F8097-A435-4198-8AC2-8C9234DD0418");

            var notificationDefinitionSqlViews = new List<NotificationDefinitionSqlView>();
            var notificationDefinitionTemplateSqlViews = new List<NotificationDefinitionTemplateSqlView>();

            // Subject form.
            if (!context.NotificationDefinitions.Any(n => n.NotificationTypeId == raveSTSubjectNotificationTypeId))
            {
                notificationDefinitionSqlViews.Add(new NotificationDefinitionSqlView
                {
                    ResourceManagerName = dummyNotificationDefinitionResource.ResourceType.FullName,
                    ResourceKey = dummyNotificationDefinitionResource.ResourceKey,
                    NotificationTypeId = raveSTSubjectNotificationTypeId,
                    IsSiteSpecific = false,
                    IsDepotSpecific = false,
                    IsUnblinded = true,
                    OperationsGroup = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.DataTransferOperationGroup.ToString(),
                    AllowedOperationIds = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.ViewClientNotifications,
                    PendingNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    RetransmitNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    DataServiceType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.RaveSubjectNotificationDataService",
                    InvariantName = "Rave ST Subject",
                    IsDynamicallyCreated = true,
                    RendererType = typeof(IRT.Modules.DataTransfer.Generic.Domain.Notifications.Templating.GenericNotificationDefinitionRenderer).FullName,
                    ViewModelType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models.RaveScreeningViewModel",
                    DynamicDataJson = "{\"ApacheHopUrl\":\"IRT_Rave_SendClientNotification\"}",

                });

                notificationDefinitionTemplateSqlViews.Add(new NotificationDefinitionTemplateSqlView
                {
                    Id = Guid.NewGuid(),
                    NotificationDefinitionId = raveSTSubjectNotificationTypeId,
                    Title = "Subject Form - @(string.IsNullOrEmpty(this.Model.ModelData.Subject.SubjectNumber) ? this.Model.ModelData.Subject.ScreeningNumber + \"\" : this.Model.ModelData.Subject.SubjectNumber)",
                    Body = "@using Microsoft.AspNetCore.Html  \r\n" +
                        "<ODM CreationDateTime=\"@(this.Model.ModelData.CreationDateTime + \"\")\" FileOID=\"@(new HtmlString(Guid.NewGuid() + \"\"))\" FileType=\"Transactional\" ODMVersion=\"1.3\" testMode=\"true\" xmlns=\"http://www.cdisc.org/ns/odm/v1.3\" xmlns:mdsol=\"http://www.mdsol.com/ns/odm/metadata\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">\r\n" +
                        "  <ClinicalData MetaDataVersionOID=\"1\" StudyOID=\"@(this.Model.ModelData.Study.StudyName + \"\")\">\r\n" +
                        "    <SubjectData SubjectKey=\"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\">\r\n" +
                        "      <SiteRef LocationOID=\"@(this.Model.ModelData.Study.StudyName + \"_\" + this.Model.ModelData.Subject.SiteId)\"/>\r\n" +
                        "      <StudyEventData StudyEventOID=\"SUBJECT\">\r\n" +
                        "        <FormData FormOID=\"SI\" TransactionType=\"Update\">\r\n" +
                        "          <ItemGroupData ItemGroupOID=\"SI\">\r\n" +
                        "            <ItemData ItemOID=\"SITENUM\" Value=\"@(this.Model.ModelData.Subject.SiteId + \"\")\"/>\r\n" +
                        "            <ItemData ItemOID=\"SUBJID\" Value=\"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\"/>\r\n" +
                        "          </ItemGroupData>\r\n" +
                        "        </FormData>\r\n" +
                        "      </StudyEventData>\r\n" +
                        "    </SubjectData>\r\n" +
                        "  </ClinicalData>\r\n" +
                        "</ODM>"
                });
            }

            // Screen Failed Form
            if (!context.NotificationDefinitions.Any(n => n.NotificationTypeId == raveSTScreenFailedNotificationTypeId))
            {
                notificationDefinitionSqlViews.Add(new NotificationDefinitionSqlView
                {
                    ResourceManagerName = dummyNotificationDefinitionResource.ResourceType.FullName,
                    ResourceKey = dummyNotificationDefinitionResource.ResourceKey,
                    NotificationTypeId = raveSTScreenFailedNotificationTypeId,
                    IsSiteSpecific = false,
                    IsDepotSpecific = false,
                    IsUnblinded = true,
                    OperationsGroup = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.DataTransferOperationGroup.ToString(),
                    AllowedOperationIds = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.ViewClientNotifications,
                    PendingNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    RetransmitNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    DataServiceType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.RaveScreeningNotificationDataService",
                    InvariantName = "Rave ST Screen Failed",
                    IsDynamicallyCreated = true,
                    RendererType = typeof(GenericNotificationDefinitionRenderer).FullName,
                    ViewModelType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models.RaveScreeningViewModel",
                    DynamicDataJson = "{\"ApacheHopUrl\":\"IRT_Rave_SendClientNotification\"}"
                });

                notificationDefinitionTemplateSqlViews.Add(new NotificationDefinitionTemplateSqlView
                {
                    Id = Guid.NewGuid(),
                    NotificationDefinitionId = raveSTScreenFailedNotificationTypeId,
                    Title = "Screen Failed Form - @(string.IsNullOrEmpty(this.Model.ModelData.Subject.SubjectNumber) ? this.Model.ModelData.Subject.ScreeningNumber + \"\" : this.Model.ModelData.Subject.SubjectNumber)",
                    Body = "@using Microsoft.AspNetCore.Html " +
                            "<ODM CreationDateTime=\"@(this.Model.ModelData.CreationDateTime + \"\")\" FileOID=\"@(new HtmlString(Guid.NewGuid() + \"\"))\" FileType=\"Transactional\" ODMVersion=\"1.3\" testMode=\"true\" " +
                            "xmlns=\"http://www.cdisc.org/ns/odm/v1.3\" xmlns:mdsol=\"http://www.mdsol.com/ns/odm/metadata\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">  \r\n " +
                            "\t<ClinicalData MetaDataVersionOID=\"1\" StudyOID=\"@(this.Model.ModelData.Study.StudyName + \"\")\"> \r\n " +
                            "\t\t<SubjectData SubjectKey=\"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\"> \r\n " +
                            "\t\t\t<SiteRef LocationOID=\"@(this.Model.ModelData.Study.StudyName + \"_\" + this.Model.ModelData.Subject.SiteId)\"/> \r\n " +
                            "\t\t\t<StudyEventData StudyEventOID=\"SCR\"> \r\n " +
                            "\t\t\t\t<FormData FormOID=\"SCR\" TransactionType=\"Update\"> \r\n " +
                            "\t\t\t\t\t<ItemGroupData ItemGroupOID=\"SCR\"> \r\n " +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"SCRNFYN\" Value=\"@((this.Model.ModelData.IsBackout ? \"\" : (this.Model.ModelData.Notification.VisitId == \"RAND\" ? \"N\" : (this.Model.ModelData.Subject.ScreenFailDate != null ? \"Y\" : \"\"))))\"/> \r\n " +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"SCRNFDAT\" Value=\"@((this.Model.ModelData.IsBackout ? \"\" : (this.Model.ModelData.Subject.ScreenFailDate?.ToString(\"yyyyMMMdd\") ?? \"\")))\"/> \r\n " +
                            "\t\t\t\t\t</ItemGroupData> \r\n " +
                            "\t\t\t\t</FormData> \r\n " +
                            "\t\t\t</StudyEventData> \r\n " +
                            "\t\t</SubjectData> \r\n " +
                            "\t</ClinicalData> \r\n " +
                            "</ODM>"
                });
            }

            // Demographics Form
            if (!context.NotificationDefinitions.Any(n => n.NotificationTypeId == raveSTDemographicsNotificationTypeId))
            {
                notificationDefinitionSqlViews.Add(new NotificationDefinitionSqlView
                {
                    ResourceManagerName = dummyNotificationDefinitionResource.ResourceType.FullName,
                    ResourceKey = dummyNotificationDefinitionResource.ResourceKey,
                    NotificationTypeId = raveSTDemographicsNotificationTypeId,
                    IsSiteSpecific = false,
                    IsDepotSpecific = false,
                    IsUnblinded = true,
                    OperationsGroup = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.DataTransferOperationGroup.ToString(),
                    AllowedOperationIds = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.ViewClientNotifications,
                    PendingNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    RetransmitNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    DataServiceType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.RaveDemographicsNotificationDataService",
                    InvariantName = "Rave ST Demographics",
                    IsDynamicallyCreated = true,
                    RendererType = typeof(GenericNotificationDefinitionRenderer).FullName,
                    ViewModelType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models.RaveDemographicsViewModel",
                    DynamicDataJson = "{\"ApacheHopUrl\":\"IRT_Rave_SendClientNotification\"}"
                });

                notificationDefinitionTemplateSqlViews.Add(new NotificationDefinitionTemplateSqlView
                {
                    Id = Guid.NewGuid(),
                    NotificationDefinitionId = raveSTDemographicsNotificationTypeId,
                    Title = "Demographics Form - @(string.IsNullOrEmpty(this.Model.ModelData.Subject.SubjectNumber) ? this.Model.ModelData.Subject.ScreeningNumber + \"\" : this.Model.ModelData.Subject.SubjectNumber)",
                    Body = "@using Microsoft.AspNetCore.Html " +
                            "<ODM CreationDateTime=\"@(this.Model.ModelData.CreationDateTime + \"\")\" FileOID=\"@(new HtmlString(Guid.NewGuid() + \"\"))\" FileType=\"Transactional\" ODMVersion=\"1.3\" testMode=\"true\" " +
                            "xmlns=\"http://www.cdisc.org/ns/odm/v1.3\" xmlns:mdsol=\"http://www.mdsol.com/ns/odm/metadata\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">  \r\n " +
                            "\t<ClinicalData MetaDataVersionOID=\"1\" StudyOID=\"@(this.Model.ModelData.Study.StudyName + \"\")\"> \r\n " +
                            "\t\t<SubjectData SubjectKey=\"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\"> \r\n " +
                            "\t\t\t<SiteRef LocationOID=\"@(this.Model.ModelData.Study.StudyName + \"_\" + this.Model.ModelData.Subject.SiteId)\"/> \r\n " +
                            "\t\t\t<StudyEventData StudyEventOID=\"SCRN\"> \r\n " +
                            "\t\t\t\t<FormData FormOID=\"DM\" TransactionType=\"Update\"> \r\n " +
                            "\t\t\t\t\t<ItemGroupData ItemGroupOID=\"DM\"> \r\n " +
                            "\t\t\t\t\t@if (!this.Model.ModelData.Subject.IsRescreen)" + "\r\n " +
                            "\t\t\t\t\t{" + "\r\n " +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"SEX\" Value=\"@((this.Model.ModelData.Subject.Gender + \"\") == \"Male\" ? \"M\" : \"F\")\"/> \r\n " +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"BRTHDAT\" Value=\"@(this.Model.ModelData.Subject.DateOfBirth?.ToString(\"yyyy\") ?? \"\")\"/> \r\n " +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"AGE\" Value=\"@(this.Model.ModelData.Subject.AgeAtScreening + \"\")\"/> \r\n " +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"NS_IRT_SSTATUS\" Value=\"1\"/> \r\n " +
                            "\t\t\t\t\t}" + "\r\n " +
                            "\t\t\t\t\telse" + "\r\n " +
                            "\t\t\t\t\t{" + "\r\n " +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"NS_IRT_SSTATUS\" Value=\"@((this.Model.ModelData.IsBackout && this.Model.ModelData.Subject.LastVisitId == \"__RESCR\") ? \"1\" : \"2\")\"/> \r\n " +
                            "\t\t\t\t\t}" + "\r\n " +
                            "\t\t\t\t\t</ItemGroupData> \r\n " +
                            "\t\t\t\t</FormData> \r\n " +
                            "\t\t\t</StudyEventData> \r\n " +
                            "\t\t</SubjectData> \r\n " +
                            "\t</ClinicalData> \r\n " +
                            "</ODM>"
                });
            }

            // Randomization Form
            if (!context.NotificationDefinitions.Any(n => n.NotificationTypeId == raveSTRandomizationNotificationTypeId))
            {
                notificationDefinitionSqlViews.Add(new NotificationDefinitionSqlView
                {
                    ResourceManagerName = dummyNotificationDefinitionResource.ResourceType.FullName,
                    ResourceKey = dummyNotificationDefinitionResource.ResourceKey,
                    NotificationTypeId = raveSTRandomizationNotificationTypeId,
                    IsSiteSpecific = false,
                    IsDepotSpecific = false,
                    IsUnblinded = true,
                    OperationsGroup = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.DataTransferOperationGroup.ToString(),
                    AllowedOperationIds = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.ViewClientNotifications,
                    PendingNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    RetransmitNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    DataServiceType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.RaveRandomizationNotificationDataService",
                    InvariantName = "Rave ST Randomization",
                    IsDynamicallyCreated = true,
                    RendererType = typeof(IRT.Modules.DataTransfer.Generic.Domain.Notifications.Templating.GenericNotificationDefinitionRenderer).FullName,
                    ViewModelType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models.RaveRandomizationViewModel",
                    DynamicDataJson = "{\"ApacheHopUrl\":\"IRT_Rave_SendClientNotification\"}",
                });

                notificationDefinitionTemplateSqlViews.Add(new NotificationDefinitionTemplateSqlView
                {
                    Id = Guid.NewGuid(),
                    NotificationDefinitionId = raveSTRandomizationNotificationTypeId,
                    Title = "Randomization Form - @(string.IsNullOrEmpty(this.Model.ModelData.Subject.SubjectNumber) ? this.Model.ModelData.Subject.ScreeningNumber + \"\" : this.Model.ModelData.Subject.SubjectNumber)",
                    Body = "@using Microsoft.AspNetCore.Html  \r\n" +
                            "<ODM CreationDateTime=\"@(this.Model.ModelData.CreationDateTime + \"\")\" FileOID=\"@(new HtmlString(Guid.NewGuid() + \"\"))\" FileType=\"Transactional\" ODMVersion=\"1.3\" testMode=\"true\" " +
                            "xmlns=\"http://www.cdisc.org/ns/odm/v1.3\" xmlns:mdsol=\"http://www.mdsol.com/ns/odm/metadata\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">  \r\n " +
                            "\t<ClinicalData MetaDataVersionOID=\"1\" StudyOID=\"@(this.Model.ModelData.Study.StudyName + \"\")\">\r\n" +
                            "\t\t<SubjectData SubjectKey=\"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\">\r\n" +
                            "\t\t\t<SiteRef LocationOID=\"@(this.Model.ModelData.Study.StudyName + \"_\" + this.Model.ModelData.Subject.SiteId)\"/>\r\n" +
                            "\t\t\t<StudyEventData StudyEventOID=\"W0\">\r\n" +
                            "\t\t\t\t<FormData FormOID=\"RAND\" TransactionType=\"Update\">\r\n" +
                            "\t\t\t\t\t<ItemGroupData ItemGroupOID=\"RAND\">\r\n" +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"RAND_ID\" Value=\"@(this.Model.ModelData.RandomizationNumberId + \"\")\"/>\r\n" +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"RANDOMIZED_AT\" Value=\"@(this.Model.ModelData.RandomizationDate + \"\")\"/>\r\n" +
                            "\t\t\t\t\t</ItemGroupData>\r\n" +
                            "\t\t\t\t</FormData>\r\n" +
                            "\t\t\t</StudyEventData>\r\n" +
                            "\t\t</SubjectData>\r\n" +
                            "\t</ClinicalData>\r\n" +
                            "</ODM>"
                });
            }

            // Drug Dispensation Form
            if (!context.NotificationDefinitions.Any(n => n.NotificationTypeId == raveSTDrugDispensationNotificationTypeId))
            {
                notificationDefinitionSqlViews.Add(new NotificationDefinitionSqlView
                {
                    ResourceManagerName = dummyNotificationDefinitionResource.ResourceType.FullName,
                    ResourceKey = dummyNotificationDefinitionResource.ResourceKey,
                    NotificationTypeId = raveSTDrugDispensationNotificationTypeId,
                    IsSiteSpecific = false,
                    IsDepotSpecific = false,
                    IsUnblinded = true,
                    OperationsGroup = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.DataTransferOperationGroup.ToString(),
                    AllowedOperationIds = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.ViewClientNotifications,
                    PendingNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    RetransmitNotificationSenderTags = typeof(RaveWebApiNotificationSenderService).FullName,
                    DataServiceType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.RaveDrugDispensationNotificationDataService",
                    InvariantName = "Rave ST Drug Dispensation",
                    IsDynamicallyCreated = true,
                    RendererType = typeof(GenericNotificationDefinitionRenderer).FullName,
                    ViewModelType = "IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models.RaveDrugDispensationViewModel",
                    DynamicDataJson = "{\"ApacheHopUrl\":\"IRT_Rave_SendClientNotification\"}"
                });

                notificationDefinitionTemplateSqlViews.Add(new NotificationDefinitionTemplateSqlView
                {
                    Id = Guid.NewGuid(),
                    NotificationDefinitionId = raveSTDrugDispensationNotificationTypeId,
                    Title = "Drug Dispensation Form - @(string.IsNullOrEmpty(this.Model.ModelData.Subject.SubjectNumber) ? this.Model.ModelData.Subject.ScreeningNumber + \"\" : this.Model.ModelData.Subject.SubjectNumber)",
                    Body = "@using Microsoft.AspNetCore.Html " +
                            "<ODM CreationDateTime=\"@(this.Model.ModelData.CreationDateTime + \"\")\" FileOID=\"@(new HtmlString(Guid.NewGuid() + \"\"))\" FileType=\"Transactional\" ODMVersion=\"1.3\" testMode=\"true\" " +
                            "xmlns=\"http://www.cdisc.org/ns/odm/v1.3\" xmlns:mdsol=\"http://www.mdsol.com/ns/odm/metadata\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">  \r\n " +
                            "\t<ClinicalData MetaDataVersionOID=\"1\" StudyOID=\"@(this.Model.ModelData.Study.StudyName + \"\")\"> \r\n " +
                            "\t\t<SubjectData SubjectKey=\"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\"> \r\n " +
                            "\t\t\t<SiteRef LocationOID=\"@(this.Model.ModelData.Study.StudyName + \"_\" + this.Model.ModelData.Subject.SiteId)\"/> \r\n " +
                            "\t\t\t<StudyEventData StudyEventOID=\"LOG\"> \r\n " +
                            "\t\t\t\t<FormData FormOID=\"DISPENSE\"> \r\n " +
                            "\t\t\t\t\t@if (this.Model.ModelData.SerializableDrugUnitList != null)" + "\r\n " +
                            "\t\t\t\t\t{" + "\r\n " +
                            "\t\t\t\t\t@foreach (var drugItem in this.Model.ModelData.SerializableDrugUnitList)" + "\r\n " +
                            "\t\t\t\t\t{" + "\r\n " +
                            "\t\t\t\t\t<ItemGroupData ItemGroupOID=\"DISPENSE\" ItemGroupRepeatKey=\"@(drugItem.AssignedItemGroupRepeatKey != null ? drugItem.AssignedItemGroupRepeatKey.RepeatKey ?? \"\" : \"\")\" TransactionType= \"@(drugItem.AssignedItemGroupRepeatKey != null ? drugItem.AssignedItemGroupRepeatKey.TransactionType ?? \"\" : \"\")\"> \r\n " +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"VISITID\" Value=\"@(this.Model.ModelData.VisitEdcMapping ?? \"\")\"/> \r\n " +
                            "\t\t\t\t\t\t<ItemData ItemOID=\"DISREFID\" Value=\"@((this.Model.ModelData.IsBackout) ? \"\" : (drugItem.SubjectVisitLabeledDrug != null ? drugItem.SubjectVisitLabeledDrug.AssignedDrugUnitId ?? \"\" : \"\"))\"/> \r\n " +
                            "\t\t\t\t\t</ItemGroupData> \r\n " +
                            "\t\t\t\t\t}" + "\r\n " +
                            "\t\t\t\t\t}" + "\r\n " +
                            "\t\t\t\t</FormData> \r\n " +
                            "\t\t\t</StudyEventData> \r\n " +
                            "\t\t</SubjectData> \r\n " +
                            "\t</ClinicalData> \r\n " +
                            "</ODM>"
                });
            }

            if (notificationDefinitionSqlViews.Any())
            {
                notificationDefinitionSqlViews.BulkInsert(context);
            }

            if (notificationDefinitionTemplateSqlViews.Any())
            {
                notificationDefinitionTemplateSqlViews.BulkInsert(context);
            }
        }

        private void SeedVeevaSelfTransformNotificationDefinitions(IRTDbContext context)
        {
            var veevaCreateCasebookNotificationTypeId = new Guid("297F8097-A435-4198-8AC2-35AEA4EFD0A1");
            var veevaSetEventDateForScreeningNotificationTypeId = new Guid("297F8097-A435-4198-8AC2-846C707F44D2");
            var veevaDemographicsNotificationTypeId = new Guid("297F8097-A435-4198-8AC2-9163AB3AB3A9");
            var veevaSetSubjectStatusForScreeningNotificationTypeId = new Guid("297F8097-A435-4198-8AC2-F134C5E08D88");

            var notificationDefinitionSqlViews = new List<NotificationDefinitionSqlView>();
            var notificationDefinitionTemplateSqlViews = new List<NotificationDefinitionTemplateSqlView>();

            // Veeva Create Casebook
            if (!context.NotificationDefinitions.Any(n => n.NotificationTypeId == veevaCreateCasebookNotificationTypeId))
            {
                notificationDefinitionSqlViews.Add(
                    new NotificationDefinitionSqlView
                    {
                        ResourceManagerName = dummyNotificationDefinitionResource.ResourceType.FullName,
                        ResourceKey = dummyNotificationDefinitionResource.ResourceKey,
                        NotificationTypeId = veevaCreateCasebookNotificationTypeId,
                        IsSiteSpecific = false,
                        IsDepotSpecific = false,
                        IsUnblinded = true,
                        OperationsGroup = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.DataTransferOperationGroup.ToString(),
                        AllowedOperationIds = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.ViewClientNotifications,
                        PendingNotificationSenderTags = typeof(VeevaWebApiNotificationSenderService).FullName,
                        RetransmitNotificationSenderTags = typeof(VeevaWebApiNotificationSenderService).FullName,
                        DataServiceType = "IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.SelfTransformSubjectNotificationDataService",
                        InvariantName = "Veeva ST Create Casebook",
                        IsDynamicallyCreated = true,
                        RendererType = typeof(GenericNotificationDefinitionRenderer).FullName,
                        ViewModelType = "IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.Models.SelfTransformSubjectNotificationViewModel",
                        DynamicDataJson = "{\"ApacheHopUrl\":\"IRT_Veeva_SendClientNotificationCreateCasebook\"}"
                    });

                notificationDefinitionTemplateSqlViews.Add(
                    new NotificationDefinitionTemplateSqlView
                    {
                        Id = Guid.NewGuid(),
                        NotificationDefinitionId = veevaCreateCasebookNotificationTypeId,
                        Title = "Veeva Create casebook - @(string.IsNullOrEmpty(this.Model.ModelData.Subject.SubjectNumber) ? this.Model.ModelData.Subject.ScreeningNumber + \"\" : this.Model.ModelData.Subject.SubjectNumber)",
                        Body = "{ \r\n " +
                        "   \"study_name\": \"@(this.Model.ModelData.Study.StudyName + \"\")\", \r\n " +
                        "   \"subjects\": [ \r\n " +
                        "         { \r\n " +
                        "            \"site\": \"@(this.Model.ModelData.Site.Name + \"\")\", \r\n " +
                        "            \"study_country\": \"@(this.Model.ModelData.Site.CountryName + \"\")\", \r\n " +
                        "            \"subject\": \"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\" \r\n " +
                        "         } \r\n " +
                        "      ] \r\n " +
                        "}",
                    });
            }

            // Veeva Set Event Date for Screening
            if (!context.NotificationDefinitions.Any(n => n.NotificationTypeId == veevaSetEventDateForScreeningNotificationTypeId))
            {
                notificationDefinitionSqlViews.Add(
                    new NotificationDefinitionSqlView
                    {
                        ResourceManagerName = dummyNotificationDefinitionResource.ResourceType.FullName,
                        ResourceKey = dummyNotificationDefinitionResource.ResourceKey,
                        NotificationTypeId = veevaSetEventDateForScreeningNotificationTypeId,
                        IsSiteSpecific = false,
                        IsDepotSpecific = false,
                        IsUnblinded = true,
                        OperationsGroup = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.DataTransferOperationGroup.ToString(),
                        AllowedOperationIds = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.ViewClientNotifications,
                        PendingNotificationSenderTags = typeof(VeevaWebApiNotificationSenderService).FullName,
                        RetransmitNotificationSenderTags = typeof(VeevaWebApiNotificationSenderService).FullName,
                        DataServiceType = "IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.SelfTransformSubjectNotificationDataService",
                        InvariantName = "Veeva Set Event Date for Screening",
                        IsDynamicallyCreated = true,
                        RendererType = typeof(GenericNotificationDefinitionRenderer).FullName,
                        ViewModelType = "IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.Models.SelfTransformSubjectNotificationViewModel",
                        DynamicDataJson = "{\"ApacheHopUrl\":\"IRT_Veeva_SendClientNotificationSetEventDate\"}"
                    });

                notificationDefinitionTemplateSqlViews.Add(
                    new NotificationDefinitionTemplateSqlView
                    {
                        Id = Guid.NewGuid(),
                        NotificationDefinitionId = veevaSetEventDateForScreeningNotificationTypeId,
                        Title = "Veeva Set Event Date for Screening - @(string.IsNullOrEmpty(this.Model.ModelData.Subject.SubjectNumber) ? this.Model.ModelData.Subject.ScreeningNumber + \"\" : this.Model.ModelData.Subject.SubjectNumber)",
                        Body =
                        "{ \r\n " +
                        "   \"study_name\": \"@(this.Model.ModelData.Study.StudyName + \"\")\", \r\n " +
                        "   \"events\": [ \r\n " +
                        "       { \r\n " +
                        "           \"site\": \"@(this.Model.ModelData.Site.SiteId)\", \r\n " +
                        "           \"study_country\": \"@(this.Model.ModelData.Site.CountryName + \"\")\", \r\n " +
                        "           \"subject\": \"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\", \r\n " +
                        "           \"eventgroup_name\": \"SCR\", \r\n " +
                        "           \"event_name\": \"SCR\", \r\n " +
                        "           \"date\": \"@(this.Model.ModelData.Subject.ScreeningDate + \"\")\", \r\n " +
                        "           \"allow_planneddate_override\": \"true\", \r\n " +
                        "           \"change_reason\": \"Integrated by IRT\" \r\n " +
                        "       } \r\n " +
                        "   ] \r\n " +
                        "}",
                    });
            }

            // Veeva Set Form Data for Screening
            if (!context.NotificationDefinitions.Any(n => n.NotificationTypeId == veevaDemographicsNotificationTypeId))
            {
                notificationDefinitionSqlViews.Add(new NotificationDefinitionSqlView
                {
                    ResourceManagerName = dummyNotificationDefinitionResource.ResourceType.FullName,
                    ResourceKey = dummyNotificationDefinitionResource.ResourceKey,
                    NotificationTypeId = veevaDemographicsNotificationTypeId,
                    IsSiteSpecific = false,
                    IsDepotSpecific = false,
                    IsUnblinded = true,
                    OperationsGroup = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.DataTransferOperationGroup.ToString(),
                    AllowedOperationIds = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.ViewClientNotifications,
                    PendingNotificationSenderTags = typeof(VeevaWebApiNotificationSenderService).FullName,
                    RetransmitNotificationSenderTags = typeof(VeevaWebApiNotificationSenderService).FullName,
                    DataServiceType = "IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.SelfTransformSubjectNotificationDataService",
                    InvariantName = "Veeva Set Form Data for Screening",
                    IsDynamicallyCreated = true,
                    RendererType = typeof(GenericNotificationDefinitionRenderer).FullName,
                    ViewModelType = "IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.Models.SelfTransformSubjectNotificationViewModel",
                    DynamicDataJson = "{\"ApacheHopUrl\":\"IRT_Veeva_SendClientNotificationCombinationFormDataUpdate\"}"
                });

                notificationDefinitionTemplateSqlViews.Add(new NotificationDefinitionTemplateSqlView
                {
                    Id = Guid.NewGuid(),
                    NotificationDefinitionId = veevaDemographicsNotificationTypeId,
                    Title = "Veeva Set Set Form Data for Screening - @(string.IsNullOrEmpty(this.Model.ModelData.Subject.SubjectNumber) ? this.Model.ModelData.Subject.ScreeningNumber + \"\" : this.Model.ModelData.Subject.SubjectNumber)",
                    Body = "{ \r\n " +
                    "  \"study_name\": \"@(this.Model.ModelData.Study.StudyName + \"\")\", \r\n " +
                    "  \"reopen\": true, \r\n " +
                    "  \"submit\": true, \r\n " +
                    "  \"change_reason\": \"Integrated by IRT\", \r\n " +
                    "  \"externally_owned\": true, \r\n " +
                    "  \"form\": { \r\n " +
                    "      \"study_country\": \"@(this.Model.ModelData.Site.CountryName + \"\")\", \r\n " +
                    "      \"site\": \"@(this.Model.ModelData.Site.Name + \"\")\", \r\n " +
                    "      \"subject\": \"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\", \r\n " +
                    "      \"eventgroup_name\": \"SCR\", \r\n " +
                    "      \"event_name\": \"SCR\", \r\n " +
                    "      \"form_name\": \"DM\", \r\n " +
                    "      \"itemgroups\": [ \r\n " +
                    "          { \r\n " +
                    "              \"itemgroup_name\": \"DM\", \r\n " +
                    "              \"items\": [ \r\n " +
                    "                  { \r\n " +
                    "                    \"item_name\": \"BRTHYY\", \r\n " +
                    "                    \"value\": \"@(this.Model.ModelData.Subject.DateOfBirth?.Year.ToString() ?? \"\")\", \r\n " +
                    "                    \"unit_value\": null \r\n " +
                    "                  }, \r\n " +
                    "                  { \r\n " +
                    "                    \"item_name\": \"LBSEX\", \r\n " +
                    "                    \"value\": \"@(this.Model.ModelData.Subject.Gender?.Value == 1 ? \"Male\" : " +
                    "this.Model.ModelData.Subject.Gender?.Value == 2 ? \"Female\" : " +
                    "this.Model.ModelData.Subject.Gender?.Value == 3 ? \"Intersex\" : " +
                    "this.Model.ModelData.Subject.Gender?.Value == 4 ? \"Undisclosed\" : \"\")\", \r\n " +
                    "                    \"unit_value\": null \r\n " +
                    "                  } \r\n " +
                    "              ] \r\n " +
                    "          } \r\n " +
                    "       ] \r\n " +
                    "    } \r\n " +
                    " }"
                });
            }

            // Veeva Set Subject Status for Screening
            if (!context.NotificationDefinitions.Any(n => n.NotificationTypeId == veevaSetSubjectStatusForScreeningNotificationTypeId))
            {
                notificationDefinitionSqlViews.Add(
                    new NotificationDefinitionSqlView
                    {
                        ResourceManagerName = dummyNotificationDefinitionResource.ResourceType.FullName,
                        ResourceKey = dummyNotificationDefinitionResource.ResourceKey,
                        NotificationTypeId = veevaSetSubjectStatusForScreeningNotificationTypeId,
                        IsSiteSpecific = false,
                        IsDepotSpecific = false,
                        IsUnblinded = true,
                        OperationsGroup = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.DataTransferOperationGroup.ToString(),
                        AllowedOperationIds = IRT.Modules.DataTransfer.Generic.Domain.Operations.GenericDataTransfer.ViewClientNotifications,
                        PendingNotificationSenderTags = typeof(VeevaWebApiNotificationSenderService).FullName,
                        RetransmitNotificationSenderTags = typeof(VeevaWebApiNotificationSenderService).FullName,
                        DataServiceType = "IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.SelfTransformSubjectNotificationDataService",
                        InvariantName = "Veeva Set Subject Status for Screening",
                        IsDynamicallyCreated = true,
                        RendererType = typeof(GenericNotificationDefinitionRenderer).FullName,
                        ViewModelType = "IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.Models.SelfTransformSubjectNotificationViewModel",
                        DynamicDataJson = "{\"ApacheHopUrl\":\"IRT_Veeva_SendClientNotificationSetSubjectStatus\"}"
                    });
                notificationDefinitionTemplateSqlViews.Add(
                    new NotificationDefinitionTemplateSqlView
                    {
                        Id = Guid.NewGuid(),
                        NotificationDefinitionId = veevaSetSubjectStatusForScreeningNotificationTypeId,
                        Title = "Veeva Set Subject Status - @(string.IsNullOrEmpty(this.Model.ModelData.Subject.SubjectNumber) ? this.Model.ModelData.Subject.ScreeningNumber + \"\" : this.Model.ModelData.Subject.SubjectNumber)",
                        Body =
                        "{ \r\n " +
                        "   \"study_name\": \"@(this.Model.ModelData.Study.StudyName + \"\")\", \r\n " +
                        "   \"subjects\": [ \r\n " +
                        "       { \r\n " +
                        "           \"subject_status\": \"@(this.Model.ModelData.Subject.Status + \"\")\", \r\n " +
                        "           \"date\": \"@(this.Model.ModelData.Subject.ScreeningDate + \"\")\", \r\n " +
                        "           \"site\": \"@(this.Model.ModelData.Site.SiteId)\", \r\n " +
                        "           \"study_country\": \"@(this.Model.ModelData.Site.CountryName + \"\")\", \r\n " +
                        "           \"subject\": \"@(this.Model.ModelData.Subject.ScreeningNumber + \"\")\" \r\n " +
                        "       } \r\n " +
                        "   ] \r\n " +
                        "}",
                    });
                }

            if (notificationDefinitionSqlViews.Any())
            {
                notificationDefinitionSqlViews.BulkInsert(context);
            }

            if (notificationDefinitionTemplateSqlViews.Any())
            {
                notificationDefinitionTemplateSqlViews.BulkInsert(context);
            }
        }

        private void RegisterRuntimeNotificationDefinitions(IRTDbContext context)
        {
            var notificationDefinitions = context.NotificationDefinitions
                .Include(x => x.NotificationTemplate)
                .Where(x => x.IsDynamicallyCreated)
                .ToList();

            foreach (var notificationDefinition in notificationDefinitions)
            {
                if (!NotificationDefinitionRegistry.TryGetDefinition(notificationDefinition.NotificationTypeId, out NotificationDefinition outNotificationDefinition))
                {
                    NotificationDefinitionRegistry.Register(new NotificationDefinition
                    {
                        Id = notificationDefinition.NotificationTypeId,
                        InvariantName = notificationDefinition.InvariantName,
                        ViewModelType = FindTypeByName(notificationDefinition.ViewModelType),
                        DataServiceType = FindTypeByName(notificationDefinition.DataServiceType),
                        RendererType = FindTypeByName(notificationDefinition.RendererType),
                        IsUnblinded = notificationDefinition.IsUnblinded,
                        IsAlert = notificationDefinition.IsAlert,
                        OperationsGroup = notificationDefinition.OperationsGroup,
                        AllowedOperations = notificationDefinition.AllowedOperationIds.Split(','),
                        PendingNotificationSenderTypes = [.. notificationDefinition.PendingNotificationSenderTags.Split(',').Select(FindTypeByName)],
                        RetransmitNotificationSenderTypes = [.. notificationDefinition.RendererType.Split(',').Select(FindTypeByName)],
                        TemplateId = notificationDefinition.NotificationTemplate.NotificationDefinitionId,
                        IsSiteSpecific = notificationDefinition.IsSiteSpecific,
                        IsDepotSpecific = notificationDefinition.IsDepotSpecific,
                        NameResourceDescriptor = dummyNotificationDefinitionResource
                    },
                    []);
                }
            }
        }

        private Type? FindTypeByName(string typeName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == typeName);
        }
    }
}