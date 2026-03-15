using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using Kendo.Mvc.UI;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Frameworks.Notifications;
using Frameworks.Notifications.Resources;
using IRT.Domain;
using IRT.Domain.Notifications;
using IRT.Domain.Services.Interfaces;
using IRT.Domain.ViewsSql.Study;
using IRT.Domain.ViewsSql.Subject;
using IRT.Domain.ViewsSql.User;
using IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces.Clients;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency;
using IRT.Modules.DataTransfer.Generic.Helpers.Extensions;
using Kernel.AspNetMvc;
using Kernel.AspNetMvc.Extensions;
using Kernel.AspNetMvc.Security.Attributes;
using Kernel.AspNetMvc.Security.Services;
using Kernel.AspNetMvc.Utilities;
using Kernel.EntityFramework.Entities.TempTables;
using Kernel.EntityFramework.Extensions;
using Kernel.Globalization.Constants;
using Kernel.Globalization.Providers;
using Kernel.Kendo.Extensions;
using Kernel.SharedDomain.Enums;
using Kernel.Utilities;
using Kernel.Utilities.Extensions;
using Unity;
using Notification = IRT.Domain.ValueObjects.Notifications.Notification;
using Operations = IRT.Modules.DataTransfer.Generic.Domain.Operations;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Controllers
{
    [AllowAuthorizedFor(Operations.GenericDataTransfer.ViewGenericNotifications)]
    [Area(Shared.Helpers.Constants.AreaNames.StudyAdministration)]
    public class ClientNotificationsController : BaseUserMvcController
    {
        private readonly IRTDbContext dbContext;
        private readonly INotificationService notificationService;
        private readonly IVisitNameLocalizationHelper visitNameLocalizationHelper;
        private readonly IContextService<StudySqlView, UserSqlView> contextService;
        private readonly IAuthenticationService authenticationService;
        private readonly IUnityContainer container;

        public ClientNotificationsController(
            IRTDbContext dbContext,
            IVisitNameLocalizationHelper visitNameLocalizationHelper,
            INotificationService notificationService,
            IContextService<StudySqlView, UserSqlView> contextService,
            IAuthenticationService authenticationService,
            IUnityContainer container)
        {
            this.notificationService = notificationService;
            this.dbContext = dbContext;
            this.visitNameLocalizationHelper = visitNameLocalizationHelper;
            this.contextService = contextService;
            this.authenticationService = authenticationService;
            this.container = container;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = GetNotifications(true)
                .Select(x => new ClientNotificationViewModel
                {
                    Id = x.Id,
                    IsNotificationSent = x.IsNotificationSent,
                    Body = x.Body,
                    Title = x.Title,
                    AdditionalInfo = x.AdditionalInfo,
                    SentUtcDateTime = x.SentUtcDateTime,
                    FailedSendAttempts = x.FailedSendAttempts,
                    GeneratedUtcDateTime = x.GeneratedUtcDateTime,
                    NotificationDefinitionName = x.NotificationDefinitionName,
                    Status = x.IsNotificationSent ?
                        Resources.GenericNotificationsController.NotificationStatusSent :
                        Resources.GenericNotificationsController.NotificationStatusPending
                });

            request.WithDefaultOrder<ClientNotificationViewModel>
                (x => x.GeneratedUtcDateTime, Kendo.Mvc.ListSortDirection.Descending);

            return this.KendoGrid(data, request);
        }

        public ActionResult Details(Guid id)
        {
            var notification = GetNotification(id);
            if (notification == null)
            {
                return RedirectToAction("Index");
            }

            var notificationDefinition = NotificationDefinitionRegistry.GetDefinition(notification.NotificationDefinitionId);
            var notificationContent = GetNotificationDetails(id);

            if (notificationContent == null || (notificationDefinition.IsUnblinded && !IsCurrentUserUnblinded()))
            {
                return RedirectToAction("Index");
            }

            var clientNotificationDetails = new ClientNotificationDetailsViewModel
            {
                Notification = notification,
                NotificationDefinition = notificationDefinition,
                NotificationContent = notificationContent
            };

            if (IsVeevaClient(notificationDefinition))
            {
                ViewBag.IsVeevaNotification = true;
                var genericNotificationDependency = dbContext.Set<NotificationDependencySqlView>().FirstOrDefault(g => g.NotificationId == notification.Id);
                if (genericNotificationDependency != null && genericNotificationDependency.DependsOnId != null)
                {
                    var notificationLocalizedContent = dbContext.NotificationLocalizedContent
                        .AsNoTracking()
                        .First(x => x.NotificationId == (Guid)genericNotificationDependency.DependsOnId &&
                                    x.LanguageId == CultureInfo.CurrentUICulture.Name);

                    var dependentNotificationText = "{0} {1}".F(
                                genericNotificationDependency.DependsOnId,
                                notificationLocalizedContent.Title);

                    clientNotificationDetails.DependentNotification = dependentNotificationText;
                }
            }

            return View(clientNotificationDetails);
        }

        public ActionResult Update(Guid id)
        {
            var notification = GetNotifications(true)
                .SingleOrDefault(x => x.Id == id);

            if (notification == null)
            {
                this.FlashError(Resources.ClientNotificationController.NotificationNotFoundOrLackOfPermissions);
                return RedirectToAction("Index");
            }

            var notificationViewModel = new ClientNotificationUpdateViewModel
            {
                NotificationId = notification.Id,
                NotificationDefinitionName = notification.NotificationDefinitionName,
                Title = notification.Title,
                Body = notification.Body,
                IsNotificationSent = notification.IsNotificationSent,
                FailedSubmitAttempts = notification.FailedSendAttempts
            };

            var notificationDefinition = NotificationDefinitionRegistry.GetNotificationDefinitions(null)
                .FirstOrDefault(n => n.Id == notification.NotificationDefinitionId);

            if (IsVeevaClient(notificationDefinition))
            {
                var genericNotificationDependency = dbContext.Set<NotificationDependencySqlView>().FirstOrDefault(g => g.NotificationId == notification.Id);
                if (genericNotificationDependency != null && genericNotificationDependency.DependsOnId != null)
                {
                    var notificationLocalizedContent = dbContext.NotificationLocalizedContent
                        .AsNoTracking()
                        .First(x => x.NotificationId == (Guid)genericNotificationDependency.DependsOnId &&
                                    x.LanguageId == CultureInfo.CurrentUICulture.Name);

                    var dependentNotificationText = "{0} {1}".F(
                                genericNotificationDependency.DependsOnId,
                                notificationLocalizedContent.Title);

                    notificationViewModel.DependentNotification = dependentNotificationText;
                }
            }

            return View(notificationViewModel);
        }

        [HttpPost]
        public ActionResult Update(ClientNotificationUpdateViewModel updateViewModel)
        {
            ValidateCredentialsAndAddErrorIfInvalid(updateViewModel);
            ValidateBodyXmlAndAddErrorIfInvalid(updateViewModel.Body);

            if (!ModelState.IsValid)
            {
                return View(updateViewModel);
            }

            var notificationViewModel = GetNotifications(true).Select(
                        x => new ClientNotificationUpdateViewModel
                        {
                            NotificationId = x.Id,
                            NotificationDefinitionName = x.NotificationDefinitionName,
                            Title = x.Title,
                            Body = x.Body,
                            IsNotificationSent = x.IsNotificationSent,
                            FailedSubmitAttempts = x.FailedSendAttempts
                        }).SingleOrDefault(x => x.NotificationId == updateViewModel.NotificationId);

            if (notificationViewModel == null)
            {
                this.FlashError(Resources.ClientNotificationController.NotificationNotFoundOrLackOfPermissions);
                return RedirectToAction("Index");
            }

            var commandResult = SendCommand(
                new UpdateClientNotification
                {
                    NotificationId = updateViewModel.NotificationId,
                    IsNotificationSent = updateViewModel.IsNotificationSent,
                    FailedSendAttempts = updateViewModel.FailedSubmitAttempts,
                    Title = updateViewModel.Title,
                    Body = updateViewModel.Body
                });

            if (!commandResult)
            {
                this.FlashError(commandResult);
                return View(updateViewModel);
            }

            this.FlashSuccess(Resources.ClientNotificationController.NotificationWasSuccessfullyUpdated);
            return RedirectToAction("Details", new { id = updateViewModel.NotificationId });
        }

        public ActionResult Create(Guid? id)
        {
            var notificationViewModel = GetNotifications(true)
                .Where(x => x.Id == id)
                .Select(
                        x => new ClientNotificationCreateViewModel
                        {
                            Title = x.Title,
                            Body = x.Body,
                            NotificationDefinitionId = x.NotificationDefinitionId,
                            SubjectId = x.SubjectId,
                            SubjectVisitId = x.SubjectVisitId
                        }).SingleOrDefault();

            if (id.HasValue && notificationViewModel == null)
            {
                this.FlashError(Resources.ClientNotificationController.NotificationNotFoundOrLackOfPermissions);
                return RedirectToAction("Index");
            }

            if (!id.HasValue)
            {
                notificationViewModel = new ClientNotificationCreateViewModel();
            }

            return View(notificationViewModel);
        }

        [HttpPost]
        public ActionResult Create(ClientNotificationCreateViewModel viewModel)
        {
            ValidateCredentialsAndAddErrorIfInvalid(viewModel);
            ValidateBodyXmlAndAddErrorIfInvalid(viewModel.Body);

            if (!ModelState.IsValid || !ValidateSubject(viewModel.SubjectId))
            {
                return View(viewModel);
            }

            var notificationDefinition = NotificationDefinitionRegistry.GetNotificationDefinitions(null)
               .FirstOrDefault(n => n.Id == viewModel.NotificationDefinitionId.Value);

            if (viewModel.DependentNotificationId.HasValue &&
                viewModel.NotificationDefinitionId.HasValue &&
                IsVeevaClient(notificationDefinition))
            {
                var dependentNotification = notificationService.GetNotification(viewModel.DependentNotificationId.Value);
                var genericNotificationDependency = dbContext.Set<NotificationDependencySqlView>()
                    .Include(g => g.Notification)
                    .FirstOrDefault(g => g.NotificationId == dependentNotification.Id);

                if (genericNotificationDependency == null)
                {
                    ModelState.AddModelError(Constants.ErrorMessageKeys.DependentNotificationError, Resources.ClientNotificationController.InvalidNotificationDependency);
                    return View(viewModel);
                }

                if (genericNotificationDependency.Notification.SubjectId != viewModel.SubjectId)
                {
                    ModelState.AddModelError(Constants.ErrorMessageKeys.DependentNotificationError, Resources.ClientNotificationController.InvalidMatchingSubjectId);
                    return View(viewModel);
                }
            }

            var notificationId = SequenticalGuid.NewGuid();
            var subjectVisit = dbContext.SubjectVisits.Find(viewModel.SubjectVisitId);

            var command = new CreateClientNotification
            {
                NotificationId = notificationId,
                NotificationDefinitionId = viewModel.NotificationDefinitionId ?? default,
                SubjectId = viewModel.SubjectId ?? default,
                SubjectVisitId = viewModel.SubjectVisitId,
                VisitId = subjectVisit?.VisitId,
                Title = viewModel.Title,
                Body = viewModel.Body,
                DependentNotificationId = viewModel.DependentNotificationId
            };

            var commandResult = SendCommand(command);

            if (!commandResult)
            {
                this.FlashError(commandResult);
                return View(viewModel);
            }

            this.FlashSuccess(Resources.ClientNotificationController.NotificationWasSuccessfullyCreated);
            return RedirectToAction("Details", new { id = notificationId });
        }

        [HttpPost]
        public ActionResult NotificationDefinitions_Read(string text)
        {
            var notificationDefinitions = GetNotificationDefinitionsDropDownItems();

            if (!text.IsNullOrWhiteSpace())
            {
                notificationDefinitions = notificationDefinitions.Where(x => x.Text.Contains(text));
            }

            return Json(notificationDefinitions);
        }

        [HttpPost]
        public ActionResult Subjects_Read(string text)
        {
            var subjectsQuery = GetSubjectsDropdownQuery();

            if (!text.IsNullOrWhiteSpace())
            {
                subjectsQuery = subjectsQuery.Where(x => x.Text.Contains(text));
            }

            return Json(subjectsQuery);
        }

        [HttpPost]
        public ActionResult SubjectVisits_Read(Guid? subjectId)
        {
            var dropDownItemViewModels = subjectId.HasValue
                                             ? GetSubjectVisitsDropdown(subjectId.Value)
                                             : Enumerable.Empty<DropDownItemViewModel<Guid>>();

            return Json(dropDownItemViewModels);
        }

        [HttpPost]
        public ActionResult DependentNotificationIds_Read(Guid? notificationDefinitionId)
        {
            var dropDownItemViewModels = GetDependentNotificationIdsDropdown(notificationDefinitionId);
            return Json(dropDownItemViewModels);
        }

        [HttpGet]
        public ActionResult MarkNotifications()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MarkNotificationsPost()
        {
            var notificationIds = GetNotifications(false)
                .Where(x => !x.IsNotificationSent)
                .Select(x => x.Id)
                .ToArray();

            var commandResult = SendCommand(new MarkNotificationsAsSent() { NotificationIds = notificationIds });

            if (!commandResult)
            {
                this.FlashError(commandResult);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        private IQueryable<DropDownItemViewModel<Guid>> GetSubjectsDropdownQuery()
        {
            var siteFilter = GetSiteFilter();
            IQueryable<SubjectSqlView> subjectsDropDown = dbContext.Subjects.AsNoTracking();
            if (siteFilter.Any())
            {
                subjectsDropDown = subjectsDropDown.Where(x => siteFilter.Contains(x.SiteId));
            }

            return subjectsDropDown.Select(
                x => new DropDownItemViewModel<Guid>
                {
                    Text = x.SubjectNumber ?? x.ScreeningNumber,
                    Value = x.SubjectId
                });
        }

        private IEnumerable<DropDownItemViewModel<Guid>> GetSubjectVisitsDropdown(Guid subjectId)
        {
            var dateDisplayFormat = DateTimeFormatProvider.DateDisplayFormatString;
            return dbContext.SubjectVisits.AsNoTracking().Where(x => x.SubjectId == subjectId)
                .Join(
                    visitNameLocalizationHelper.GetVisitsWithLocalizedNames(),
                    x => x.VisitId,
                    x => x.VisitId,
                    (x, y) => new { SubjectVisitId = x.Id, x.VisitDate, y.VisitName, x.VisitIndex })
                .OrderBy(x => x.VisitIndex)
                .ToArray()
                .Select(
                    x => new DropDownItemViewModel<Guid>
                    {
                        Text = !x.VisitDate.HasValue
                                        ? x.VisitName
                                        : "{0} ({1})".F(x.VisitName, x.VisitDate.Value.ToString(dateDisplayFormat)),
                        Value = x.SubjectVisitId
                    }).ToArray();
        }

        private IEnumerable<DropDownItemViewModel<Guid>> GetDependentNotificationIdsDropdown(Guid? notificationDefinitionId)
        {
            if (!notificationDefinitionId.HasValue)
            {
                return Enumerable.Empty<DropDownItemViewModel<Guid>>();
            }

            var notificationDefinitions = NotificationDefinitionRegistry.GetNotificationDefinitions(null);
            var notificationDefinition = notificationDefinitions.FirstOrDefault(n => n.Id == notificationDefinitionId.Value);

            var veevaNotificationDefinitionIds = notificationDefinitions
                .Where(x => x.PendingNotificationSenderTypes
                    .All(x => typeof(IVeevaWebApiNotificationSenderService).IsAssignableFrom(x)))
                .Select(x => x.Id)
                .ToList();

            if (!IsVeevaClient(notificationDefinition))
            {
                return Enumerable.Empty<DropDownItemViewModel<Guid>>();
            }

            try
            {
                var notificationDependencies = dbContext.Set<NotificationDependencySqlView>()
                  .Where(g => veevaNotificationDefinitionIds.Contains(g.Notification.NotificationDefinitionId))
                  .Include(g => g.Notification)
                  .ThenInclude(g => g.NotificationLocalizedContentEntries)
                  .GroupBy(l => l.GroupKey)
                  .Select(g => g.OrderByDescending(c => c.CreatedAt).FirstOrDefault())
                  .ToList();

                return notificationDependencies.Select(
                        x => new DropDownItemViewModel<Guid>
                        {
                            Text = "{0} {1}".F(
                                    x.NotificationId,
                                    x.Notification.NotificationLocalizedContentEntries
                                        .Where(z => z.LanguageId == CultureInfo.CurrentUICulture.Name)
                                        .OrderByDescending(z => z.GeneratedUtcDateTime)
                                        .Select(z => z.Title)
                                        .FirstOrDefault()),
                            Value = x.NotificationId
                        }).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
        }

        private string[] GetSiteFilter()
        {
            var siteSpecificUser = dbContext.Users
                .Where(u => u.Id == UserId)
                .Select(x => x.Role.IsSiteSpecific)
                .SingleOrDefault();

            return siteSpecificUser
                ? dbContext.UserToSites.Where(u => u.UserId == UserId && u.Site.IsActive).Select(s => s.SiteId).ToArray()
                : dbContext.UserToSites.Where(u => u.UserId == UserId).Select(s => s.SiteId).ToArray();
        }

        protected virtual Notification GetNotification(Guid notificationId)
        {
            var notification = notificationService.GetNotification(notificationId);

            if (notification == null)
            {
                return null;
            }

            return new Notification
            {
                Id = notification.Id,
                NotificationDefinitionId = notification.NotificationDefinitionId,
                AdditionalInfo = notification.AdditionalInfo,
                AttachmentMediaType = notification.AttachmentMediaType
            };
        }

        private bool IsCurrentUserUnblinded()
        {
            return dbContext.Users
                .Where(x => x.Id == UserId)
                .Select(x => x.Role.IsUnblinded)
                .SingleOrDefault();
        }

        private NotificationLocalizedContent GetNotificationDetails(Guid notificationId)
        {
            var notificationContent = notificationService.GetNotificationContent(notificationId);

            if (notificationContent == null)
            {
                return null;
            }

            return new NotificationLocalizedContent
            {
                NotificationId = notificationContent.NotificationId,
                Title = notificationContent.Title,
                Body = notificationContent.Body,
                AttachmentName = notificationContent.AttachmentName,
                Attachment = notificationContent.Attachment,
                GeneratedUtcDateTime = notificationContent.GeneratedUtcDateTime
            };
        }

        private void ValidateCredentialsAndAddErrorIfInvalid(ElectronicSignatureViewModel signature)
        {
            var currentUsername = contextService.UserProfile.Username;
            if (signature is null || currentUsername != signature.SignatureUsername
                                  || signature.SignaturePassword.IsNullOrEmpty()
                                  || !authenticationService.VerifyCredentials(
                                      signature.SignatureUsername,
                                      signature.SignaturePassword,
                                      IrtPlatform.IWR))
            {
                ModelState.AddModelError(
                    Constants.ErrorMessageKeys.SignatureServerError,
                    Resources.ClientNotificationController.SignatureInvalidCredentials);
            }
        }

        private void ValidateBodyXmlAndAddErrorIfInvalid(string body)
        {
            if (!body.CanFormatToXml() && !body.CanFormatToJson())
            {
                ModelState.AddModelError(Constants.ErrorMessageKeys.BodyServerError, Resources.ClientNotificationController.InvalidBodyFormat);
            }
        }

        private IQueryable<NotificationTempDto> GetNotifications(bool isApplySiteFilter)
        {
            NotificationDefinition[] allNotificationDefinitions = NotificationDefinitionRegistry.GetNotificationDefinitions(null);
            var clientNotificationDefinitions = new List<NotificationDefinition>();

            foreach (var senderService in container.ResolveAll<INotificationSenderService>())
            {
                clientNotificationDefinitions.AddRange(allNotificationDefinitions.Where(n => n.PendingNotificationSenderTypes.Contains(senderService.GetType())));
            }

            var localizedNotificationDefinitionTypes = clientNotificationDefinitions
                .Where(n => n.NameResourceDescriptor.ResourceType != typeof(DynamicNotificationDefinition))
                .Select(x => new
                {
                    Id = x.NameResourceDescriptor.ResourceType.FullName,
                    SecondaryId = x.NameResourceDescriptor.ResourceKey,
                    Value = x.NameResourceDescriptor.GetString()
                })
                    .Distinct()
                    .Select(x => new ThreeStringsTempTable
                    {
                        Id = x.Id,
                        SecondaryId = x.SecondaryId,
                        Value = x.Value
                    })
                .ToList();

            localizedNotificationDefinitionTypes.BulkInsert(dbContext);

            var clientNotificationDefinitionIds = clientNotificationDefinitions
                              .Select(g => g.Id)
                              .ToList();

            var notifications = notificationService.GetNotifications()
                .Where(n => clientNotificationDefinitionIds.Contains(n.NotificationDefinitionId));

            //When we mark all notifications as sent, we don't want to apply site filter.
            if (isApplySiteFilter)
            {
                var siteFilter = GetSiteFilter();
                if (siteFilter.Any())
                {
                    notifications = notifications.Where(x => siteFilter.Contains(x.SiteId));
                }
            }

            var isUnblinded = dbContext.Users
                .Where(x => x.Id == UserId)
                .Select(x => x.Role.IsUnblinded)
                .SingleOrDefault();

            var clientNotificationData = notifications
                    .Where(x => !x.NotificationDefinition.IsUnblinded
                              || isUnblinded
                              || (x.NotificationDefinition.CanOverrideBlindingForTransactionUser && x.OriginatorUserId == UserId))
                    .LeftJoin(dbContext.ThreeStringsTemp,
                        x => new { x.NotificationDefinition.ResourceManagerName, x.NotificationDefinition.ResourceKey },
                        y => new { ResourceManagerName = y.Id, ResourceKey = y.SecondaryId },
                        (x, y) => new NotificationTempDto
                        {
                            Id = x.Id,
                            NotificationDefinitionName = x.NotificationDefinition.ResourceManagerName != typeof(DynamicNotificationDefinition).FullName ? y.Value : x.NotificationDefinition.InvariantName,
                            NotificationDefinitionId = x.NotificationDefinitionId,
                            Title = x.NotificationLocalizedContentEntries
                                .Where(z => z.LanguageId == CultureInfo.CurrentUICulture.Name)
                                .OrderByDescending(z => z.GeneratedUtcDateTime)
                                .Select(z => z.Title)
                                .FirstOrDefault(),
                            Body = x.NotificationLocalizedContentEntries
                                              .Where(z => z.LanguageId == GlobalizationConstants.DefaultLanguage)
                                              .OrderByDescending(z => z.GeneratedUtcDateTime).Select(z => z.Body)
                                              .FirstOrDefault(),
                            Status = x.IsNotificationSent
                                        ? Resources.GenericNotificationsController.NotificationStatusSent
                                        : Resources.GenericNotificationsController.NotificationStatusPending,
                            SubjectVisitId = x.SubjectVisitId,
                            SubjectId = x.SubjectId,
                            IsNotificationSent = x.IsNotificationSent,
                            SentUtcDateTime = x.SentUtcDateTime,
                            GeneratedUtcDateTime = x.GeneratedUtcDateTime,
                            FailedSendAttempts = x.FailedSendAttempts ?? 0,
                            AdditionalInfo = x.AdditionalInfo
                        });

            return clientNotificationData;
        }

        private IEnumerable<DropDownItemViewModel<Guid>> GetNotificationDefinitionsDropDownItems()
        {
            NotificationDefinition[] allNotificationDefinitions = NotificationDefinitionRegistry.GetNotificationDefinitions(null);
            var clientNotificationDefinitions = new List<NotificationDefinition>();

            foreach (var senderService in container.ResolveAll<INotificationSenderService>())
            {
                clientNotificationDefinitions.AddRange(allNotificationDefinitions.Where(n => n.PendingNotificationSenderTypes.Contains(senderService.GetType())));
            }

            var notificationDefinitionItems = clientNotificationDefinitions
                .Select(x => new DropDownItemViewModel<Guid> { Text = x.Name, Value = x.Id })
                .OrderBy(x => x.Text)
                .ToArray();

            return notificationDefinitionItems;
        }

        private bool ValidateSubject(Guid? subjectID)
        {
            var siteFilter = GetSiteFilter();
            if (!siteFilter.Any())
            {
                return true;
            }

            var site = dbContext.Subjects.Find(subjectID).SiteId;
            var valid = siteFilter.Contains(site);

            return valid;
        }

        private bool IsVeevaClient(NotificationDefinition def)
        {
            return def != null && def.PendingNotificationSenderTypes.All(x => typeof(IVeevaWebApiNotificationSenderService).IsAssignableFrom(x));
        }
    }
}