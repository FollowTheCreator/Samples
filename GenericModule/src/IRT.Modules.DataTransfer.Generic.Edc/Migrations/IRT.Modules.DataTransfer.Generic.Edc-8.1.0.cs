using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRT.Modules.DataTransfer.Generic.Edc.Migrations
{
    /// <inheritdoc />
    public partial class IRTModulesDataTransferGenericEdc810 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GenericItemGroupRepeatKeys",
                columns: table => new
                {
                    RepeatKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LogicalSubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VisitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicalVisitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrugUnitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedDrugUnitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrugUnitIdToRestore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDrugUnitReplaced = table.Column<bool>(type: "bit", nullable: true),
                    IsSelfSupportDrugUnit = table.Column<bool>(type: "bit", nullable: true),
                    RepeatKey = table.Column<int>(type: "int", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    NotificationDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LogicalSubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VisitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicalVisitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemGroupRepeatKeysLastUsed", x => x.RepeatKeyLastUsedId);
                });

            migrationBuilder.CreateTable(
                name: "GenericStudyEventRepeatKeys",
                columns: table => new
                {
                    RepeatKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RepeatKey = table.Column<int>(type: "int", nullable: true),
                    ScheduledRepeatKey = table.Column<int>(type: "int", nullable: true),
                    UnscheduledRepeatKey = table.Column<int>(type: "int", nullable: true),
                    ReplacementRepeatKey = table.Column<int>(type: "int", nullable: true),
                    ScreenFailRepeatKey = table.Column<int>(type: "int", nullable: true),
                    InformedConsentRepeatKey = table.Column<int>(type: "int", nullable: true)
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
                    NotificationDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    ScheduledRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    UnscheduledRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    ReplacementRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    ScreenFailRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true),
                    InformedConsentRepeatKeyLastUsed = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericStudyEventRepeatKeysLastUsed", x => x.RepeatKeyLastUsedId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenericItemGroupRepeatKeys");

            migrationBuilder.DropTable(
                name: "GenericItemGroupRepeatKeysLastUsed");

            migrationBuilder.DropTable(
                name: "GenericStudyEventRepeatKeys");

            migrationBuilder.DropTable(
                name: "GenericStudyEventRepeatKeysLastUsed");
        }
    }
}
