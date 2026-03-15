using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRT.Modules.DataTransfer.Generic.Migrations
{
    /// <inheritdoc />
    public partial class IRTModulesDataTransferGeneric810 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenericItemGroupRepeatKeys");

            migrationBuilder.DropTable(
                name: "GenericItemGroupRepeatKeysLastUsed");

            migrationBuilder.DropTable(
                name: "GenericNotificationDefinitions");

            migrationBuilder.DropTable(
                name: "GenericStudyEventRepeatKeys");

            migrationBuilder.DropTable(
                name: "GenericStudyEventRepeatKeysLastUsed");

            migrationBuilder.CreateTable(
                name: "DynamicDataTransferSettings",
                columns: table => new
                {
                    EntityTypeName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExtendedPropertiesTypeName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExtendedPropertyName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicDataTransferSettings", x => new { x.EntityTypeName, x.ExtendedPropertiesTypeName, x.EntityId, x.ExtendedPropertyName });
                });

            migrationBuilder.CreateTable(
                name: "DynamicSettings",
                columns: table => new
                {
                    DynamicSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    EntityTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtendedPropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSettings", x => x.DynamicSettingId);
                });

            migrationBuilder.CreateTable(
                name: "NotificationDependency",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DependsOnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDependency", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_NotificationDependency_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicDataTransferSettings");

            migrationBuilder.DropTable(
                name: "DynamicSettings");

            migrationBuilder.DropTable(
                name: "NotificationDependency");

            migrationBuilder.CreateTable(
                name: "GenericItemGroupRepeatKeys",
                columns: table => new
                {
                    RepeatKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DrugUnitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrugUnitIdToRestore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDrugUnitReplaced = table.Column<bool>(type: "bit", nullable: true),
                    IsSelfSupportDrugUnit = table.Column<bool>(type: "bit", nullable: true),
                    LogicalSubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LogicalVisitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepeatKey = table.Column<int>(type: "int", nullable: true),
                    ReplacedDrugUnitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemGroupRepeatKeys", x => x.RepeatKeyId);
                });

            migrationBuilder.CreateTable(
                name: "GenericItemGroupRepeatKeysLastUsed",
                columns: table => new
                {
                    RepeatKeyLastUsedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogicalSubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LogicalVisitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VisitId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemGroupRepeatKeysLastUsed", x => x.RepeatKeyLastUsedId);
                });

            migrationBuilder.CreateTable(
                name: "GenericNotificationDefinitions",
                columns: table => new
                {
                    NotificationDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApacheHopResourceEndpoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UseDefaultView = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericNotificationDefinitions", x => x.NotificationDefinitionId);
                    table.ForeignKey(
                        name: "FK_GenericNotificationDefinitions_NotificationDefinitions_NotificationDefinitionId",
                        column: x => x.NotificationDefinitionId,
                        principalTable: "NotificationDefinitions",
                        principalColumn: "NotificationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericStudyEventRepeatKeys",
                columns: table => new
                {
                    RepeatKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InformedConsentRepeatKey = table.Column<int>(type: "int", nullable: true),
                    NotificationDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepeatKey = table.Column<int>(type: "int", nullable: true),
                    ReplacementRepeatKey = table.Column<int>(type: "int", nullable: true),
                    ScheduledRepeatKey = table.Column<int>(type: "int", nullable: true),
                    ScreenFailRepeatKey = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnscheduledRepeatKey = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericStudyEventRepeatKeys", x => x.RepeatKeyId);
                });

            migrationBuilder.CreateTable(
                name: "GenericStudyEventRepeatKeysLastUsed",
                columns: table => new
                {
                    RepeatKeyLastUsedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InformedConsentRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    NotificationDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    ReplacementRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    ScheduledRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    ScreenFailRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnscheduledRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericStudyEventRepeatKeysLastUsed", x => x.RepeatKeyLastUsedId);
                });
        }
    }
}
