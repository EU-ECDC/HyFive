using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HyFive.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class initialMigrationSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GloveWithIndicationType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloveWithIndicationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GloveWithoutIndicationType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloveWithoutIndicationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HandHygieneAfterGloveUseType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandHygieneAfterGloveUseType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HandJewelryType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandJewelryType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndicationType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Number = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstitutionType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitutionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Municipality",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProtectiveEquipmentSettingType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtectiveEquipmentSettingType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProtectiveEquipmentType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtectiveEquipmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionalHealthcareOrganization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalHealthcareOrganization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransferStatusType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferStatusType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstitutionId = table.Column<int>(type: "integer", nullable: true),
                    UserFirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserLastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdentityPseudonym = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HPRNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProcessedByUserID = table.Column<int>(type: "integer", nullable: true),
                    ProcessedByUsername = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SecondsUsed = table.Column<int>(type: "integer", nullable: false),
                    TimingWasPerformed = table.Column<bool>(type: "boolean", nullable: false),
                    GloveUsed = table.Column<bool>(type: "boolean", nullable: true),
                    ActivityTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_ActivityType_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MisuseType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProtectiveEquipmentTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MisuseType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MisuseType_ProtectiveEquipmentType_ProtectiveEquipmentTypeId",
                        column: x => x.ProtectiveEquipmentTypeId,
                        principalTable: "ProtectiveEquipmentType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType",
                columns: table => new
                {
                    ProtectiveEquipmentTypeId = table.Column<int>(type: "integer", nullable: false),
                    ProtectiveEquipmentSettingTypeId = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType", x => new { x.ProtectiveEquipmentTypeId, x.ProtectiveEquipmentSettingTypeId });
                    table.ForeignKey(
                        name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prote~",
                        column: x => x.ProtectiveEquipmentSettingTypeId,
                        principalTable: "ProtectiveEquipmentSettingType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prot~1",
                        column: x => x.ProtectiveEquipmentTypeId,
                        principalTable: "ProtectiveEquipmentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthcareOrganization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    RegionalHealthcareOrganizationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthcareOrganization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthcareOrganization_RegionalHealthcareOrganization_Regio~",
                        column: x => x.RegionalHealthcareOrganizationId,
                        principalTable: "RegionalHealthcareOrganization",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Institution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HERId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Abbreviation = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    InstitutionTypeId = table.Column<int>(type: "integer", nullable: true),
                    RegionId = table.Column<int>(type: "integer", nullable: true),
                    HealthcareOrganizationId = table.Column<int>(type: "integer", nullable: true),
                    MunicipalityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Institution_HealthcareOrganization_HealthcareOrganizationId",
                        column: x => x.HealthcareOrganizationId,
                        principalTable: "HealthcareOrganization",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Institution_InstitutionType_InstitutionTypeId",
                        column: x => x.InstitutionTypeId,
                        principalTable: "InstitutionType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Institution_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalTable: "Municipality",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Institution_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clinic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    InstitutionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clinic_Institution_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institution",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstitutionId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DepartmentTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_DepartmentType_DepartmentTypeId",
                        column: x => x.DepartmentTypeId,
                        principalTable: "DepartmentType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Department_Institution_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PredefinedComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SessionType = table.Column<int>(type: "integer", nullable: false),
                    InstitutionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredefinedComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredefinedComment_Institution_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstitutionId = table.Column<int>(type: "integer", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IdentityPseudonym = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    HPRNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Institution_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institution",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClinicDepartment",
                columns: table => new
                {
                    ClinicsId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicDepartment", x => new { x.ClinicsId, x.DepartmentsId });
                    table.ForeignKey(
                        name: "FK_ClinicDepartment_Clinic_ClinicsId",
                        column: x => x.ClinicsId,
                        principalTable: "Clinic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClinicDepartment_Department_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentRole",
                columns: table => new
                {
                    DepartmentsId = table.Column<int>(type: "integer", nullable: false),
                    RolesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentRole", x => new { x.DepartmentsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_DepartmentRole_Department_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentRole_Role_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    ObserverId = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    TransferStatusId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Session_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Session_TransferStatusType_TransferStatusId",
                        column: x => x.TransferStatusId,
                        principalTable: "TransferStatusType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Session_User_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FiveIndicationsObservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FiveIndicationsSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivityId = table.Column<int>(type: "integer", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    RegisteredTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiveIndicationsObservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FiveIndicationsObservation_Activity_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FiveIndicationsObservation_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FiveIndicationsObservation_Session_FiveIndicationsSessionId",
                        column: x => x.FiveIndicationsSessionId,
                        principalTable: "Session",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GloveObservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GloveUsed = table.Column<bool>(type: "boolean", nullable: false),
                    PostGloveHandHygieneTypeId = table.Column<int>(type: "integer", nullable: true),
                    GloveSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    RegisteredTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloveObservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GloveObservation_HandHygieneAfterGloveUseType_PostGloveHand~",
                        column: x => x.PostGloveHandHygieneTypeId,
                        principalTable: "HandHygieneAfterGloveUseType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GloveObservation_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GloveObservation_Session_GloveSessionId",
                        column: x => x.GloveSessionId,
                        principalTable: "Session",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HandJewelryObservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HandJewelrySessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    RegisteredTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandJewelryObservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandJewelryObservation_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HandJewelryObservation_Session_HandJewelrySessionId",
                        column: x => x.HandJewelrySessionId,
                        principalTable: "Session",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProtectiveEquipmentObservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SettingTypeId = table.Column<int>(type: "integer", nullable: true),
                    ProtectiveEquipmentSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    RegisteredTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtectiveEquipmentObservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProtectiveEquipmentObservation_ProtectiveEquipmentSettingTy~",
                        column: x => x.SettingTypeId,
                        principalTable: "ProtectiveEquipmentSettingType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProtectiveEquipmentObservation_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProtectiveEquipmentObservation_Session_ProtectiveEquipmentS~",
                        column: x => x.ProtectiveEquipmentSessionId,
                        principalTable: "Session",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FiveIndicationsObservationIndicationTypes",
                columns: table => new
                {
                    IndicationTypesId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiveIndicationsObservationIndicationTypes", x => new { x.IndicationTypesId, x.ObservationsId });
                    table.ForeignKey(
                        name: "FK_FiveIndicationsObservationIndicationTypes_FiveIndicationsOb~",
                        column: x => x.ObservationsId,
                        principalTable: "FiveIndicationsObservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FiveIndicationsObservationIndicationTypes_IndicationType_In~",
                        column: x => x.IndicationTypesId,
                        principalTable: "IndicationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloveObservationGloveWithIndicationType",
                columns: table => new
                {
                    GloveWithIndicationTypesId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloveObservationGloveWithIndicationType", x => new { x.GloveWithIndicationTypesId, x.ObservationsId });
                    table.ForeignKey(
                        name: "FK_GloveObservationGloveWithIndicationType_GloveObservation_Ob~",
                        column: x => x.ObservationsId,
                        principalTable: "GloveObservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GloveObservationGloveWithIndicationType_GloveWithIndication~",
                        column: x => x.GloveWithIndicationTypesId,
                        principalTable: "GloveWithIndicationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloveObservationGloveWithoutIndicationType",
                columns: table => new
                {
                    GloveWithoutIndicationTypesId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloveObservationGloveWithoutIndicationType", x => new { x.GloveWithoutIndicationTypesId, x.ObservationsId });
                    table.ForeignKey(
                        name: "FK_GloveObservationGloveWithoutIndicationType_GloveObservation~",
                        column: x => x.ObservationsId,
                        principalTable: "GloveObservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GloveObservationGloveWithoutIndicationType_GloveWithoutIndi~",
                        column: x => x.GloveWithoutIndicationTypesId,
                        principalTable: "GloveWithoutIndicationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HandJewelryObservationHandJewelryType",
                columns: table => new
                {
                    HandJewelriesId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandJewelryObservationHandJewelryType", x => new { x.HandJewelriesId, x.ObservationsId });
                    table.ForeignKey(
                        name: "FK_HandJewelryObservationHandJewelryType_HandJewelryObservatio~",
                        column: x => x.ObservationsId,
                        principalTable: "HandJewelryObservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HandJewelryObservationHandJewelryType_HandJewelryType_HandJ~",
                        column: x => x.HandJewelriesId,
                        principalTable: "HandJewelryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProtectiveEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WasUsed = table.Column<bool>(type: "boolean", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    EquipmentTypeId = table.Column<int>(type: "integer", nullable: true),
                    WasUsedCorrectly = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ProtectiveEquipmentObservationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtectiveEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProtectiveEquipment_ProtectiveEquipmentObservation_Protecti~",
                        column: x => x.ProtectiveEquipmentObservationId,
                        principalTable: "ProtectiveEquipmentObservation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProtectiveEquipment_ProtectiveEquipmentType_EquipmentTypeId",
                        column: x => x.EquipmentTypeId,
                        principalTable: "ProtectiveEquipmentType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MisuseTypeProtectiveEquipment",
                columns: table => new
                {
                    MisuseTypesId = table.Column<int>(type: "integer", nullable: false),
                    ProtectiveEquipmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MisuseTypeProtectiveEquipment", x => new { x.MisuseTypesId, x.ProtectiveEquipmentId });
                    table.ForeignKey(
                        name: "FK_MisuseTypeProtectiveEquipment_MisuseType_MisuseTypesId",
                        column: x => x.MisuseTypesId,
                        principalTable: "MisuseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MisuseTypeProtectiveEquipment_ProtectiveEquipment_Protectiv~",
                        column: x => x.ProtectiveEquipmentId,
                        principalTable: "ProtectiveEquipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_ActivityTypeId",
                table: "Activity",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityType_Code",
                table: "ActivityType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityType_Name",
                table: "ActivityType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Clinic_InstitutionId",
                table: "Clinic",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinic_Name",
                table: "Clinic",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicDepartment_DepartmentsId",
                table: "ClinicDepartment",
                column: "DepartmentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_DepartmentTypeId",
                table: "Department",
                column: "DepartmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_InstitutionId",
                table: "Department",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_Name",
                table: "Department",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentRole_RolesId",
                table: "DepartmentRole",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentType_Code",
                table: "DepartmentType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentType_Name",
                table: "DepartmentType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FiveIndicationsObservation_ActivityId",
                table: "FiveIndicationsObservation",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_FiveIndicationsObservation_CreatedTime",
                table: "FiveIndicationsObservation",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_FiveIndicationsObservation_FiveIndicationsSessionId",
                table: "FiveIndicationsObservation",
                column: "FiveIndicationsSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_FiveIndicationsObservation_RegisteredTime",
                table: "FiveIndicationsObservation",
                column: "RegisteredTime");

            migrationBuilder.CreateIndex(
                name: "IX_FiveIndicationsObservation_RoleId",
                table: "FiveIndicationsObservation",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_FiveIndicationsObservationIndicationTypes_ObservationsId",
                table: "FiveIndicationsObservationIndicationTypes",
                column: "ObservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_GloveObservation_CreatedTime",
                table: "GloveObservation",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_GloveObservation_GloveSessionId",
                table: "GloveObservation",
                column: "GloveSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_GloveObservation_PostGloveHandHygieneTypeId",
                table: "GloveObservation",
                column: "PostGloveHandHygieneTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GloveObservation_RegisteredTime",
                table: "GloveObservation",
                column: "RegisteredTime");

            migrationBuilder.CreateIndex(
                name: "IX_GloveObservation_RoleId",
                table: "GloveObservation",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GloveObservationGloveWithIndicationType_ObservationsId",
                table: "GloveObservationGloveWithIndicationType",
                column: "ObservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_GloveObservationGloveWithoutIndicationType_ObservationsId",
                table: "GloveObservationGloveWithoutIndicationType",
                column: "ObservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_GloveWithIndicationType_Code",
                table: "GloveWithIndicationType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GloveWithIndicationType_Name",
                table: "GloveWithIndicationType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_GloveWithoutIndicationType_Code",
                table: "GloveWithoutIndicationType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GloveWithoutIndicationType_Name",
                table: "GloveWithoutIndicationType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_HandHygieneAfterGloveUseType_Code",
                table: "HandHygieneAfterGloveUseType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HandHygieneAfterGloveUseType_Name",
                table: "HandHygieneAfterGloveUseType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_HandJewelryObservation_CreatedTime",
                table: "HandJewelryObservation",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_HandJewelryObservation_HandJewelrySessionId",
                table: "HandJewelryObservation",
                column: "HandJewelrySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_HandJewelryObservation_RegisteredTime",
                table: "HandJewelryObservation",
                column: "RegisteredTime");

            migrationBuilder.CreateIndex(
                name: "IX_HandJewelryObservation_RoleId",
                table: "HandJewelryObservation",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_HandJewelryObservationHandJewelryType_ObservationsId",
                table: "HandJewelryObservationHandJewelryType",
                column: "ObservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_HandJewelryType_Code",
                table: "HandJewelryType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HealthcareOrganization_RegionalHealthcareOrganizationId",
                table: "HealthcareOrganization",
                column: "RegionalHealthcareOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicationType_Code",
                table: "IndicationType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndicationType_Name",
                table: "IndicationType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Institution_Abbreviation",
                table: "Institution",
                column: "Abbreviation");

            migrationBuilder.CreateIndex(
                name: "IX_Institution_HealthcareOrganizationId",
                table: "Institution",
                column: "HealthcareOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Institution_HERId",
                table: "Institution",
                column: "HERId");

            migrationBuilder.CreateIndex(
                name: "IX_Institution_InstitutionTypeId",
                table: "Institution",
                column: "InstitutionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Institution_MunicipalityId",
                table: "Institution",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Institution_Name",
                table: "Institution",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Institution_RegionId",
                table: "Institution",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_InstitutionType_Code",
                table: "InstitutionType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstitutionType_Name",
                table: "InstitutionType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MisuseType_Name",
                table: "MisuseType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MisuseType_ProtectiveEquipmentTypeId",
                table: "MisuseType",
                column: "ProtectiveEquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MisuseTypeProtectiveEquipment_ProtectiveEquipmentId",
                table: "MisuseTypeProtectiveEquipment",
                column: "ProtectiveEquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PredefinedComment_InstitutionId",
                table: "PredefinedComment",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipment_EquipmentTypeId",
                table: "ProtectiveEquipment",
                column: "EquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipment_ProtectiveEquipmentObservationId",
                table: "ProtectiveEquipment",
                column: "ProtectiveEquipmentObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentObservation_CreatedTime",
                table: "ProtectiveEquipmentObservation",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentObservation_ProtectiveEquipmentSessionId",
                table: "ProtectiveEquipmentObservation",
                column: "ProtectiveEquipmentSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentObservation_RegisteredTime",
                table: "ProtectiveEquipmentObservation",
                column: "RegisteredTime");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentObservation_RoleId",
                table: "ProtectiveEquipmentObservation",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentObservation_SettingTypeId",
                table: "ProtectiveEquipmentObservation",
                column: "SettingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentSettingType_Code",
                table: "ProtectiveEquipmentSettingType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentSettingType_Name",
                table: "ProtectiveEquipmentSettingType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prote~",
                table: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType",
                column: "ProtectiveEquipmentSettingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentType_Code",
                table: "ProtectiveEquipmentType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentType_Name",
                table: "ProtectiveEquipmentType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Region_Code",
                table: "Region",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Region_Name",
                table: "Region",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Session_CreatedDate",
                table: "Session",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Session_DepartmentId",
                table: "Session",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_Discriminator",
                table: "Session",
                column: "Discriminator");

            migrationBuilder.CreateIndex(
                name: "IX_Session_ObserverId",
                table: "Session",
                column: "ObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_StartDate",
                table: "Session",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Session_TransferStatusId",
                table: "Session",
                column: "TransferStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferStatusType_Code",
                table: "TransferStatusType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferStatusType_Name",
                table: "TransferStatusType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_User_HPRNumber",
                table: "User",
                column: "HPRNumber");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdentityPseudonym",
                table: "User",
                column: "IdentityPseudonym");

            migrationBuilder.CreateIndex(
                name: "IX_User_InstitutionId",
                table: "User",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessRequest_Status",
                table: "UserAccessRequest",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicDepartment");

            migrationBuilder.DropTable(
                name: "DepartmentRole");

            migrationBuilder.DropTable(
                name: "FiveIndicationsObservationIndicationTypes");

            migrationBuilder.DropTable(
                name: "GloveObservationGloveWithIndicationType");

            migrationBuilder.DropTable(
                name: "GloveObservationGloveWithoutIndicationType");

            migrationBuilder.DropTable(
                name: "HandJewelryObservationHandJewelryType");

            migrationBuilder.DropTable(
                name: "MisuseTypeProtectiveEquipment");

            migrationBuilder.DropTable(
                name: "PredefinedComment");

            migrationBuilder.DropTable(
                name: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType");

            migrationBuilder.DropTable(
                name: "UserAccessRequest");

            migrationBuilder.DropTable(
                name: "Clinic");

            migrationBuilder.DropTable(
                name: "FiveIndicationsObservation");

            migrationBuilder.DropTable(
                name: "IndicationType");

            migrationBuilder.DropTable(
                name: "GloveWithIndicationType");

            migrationBuilder.DropTable(
                name: "GloveObservation");

            migrationBuilder.DropTable(
                name: "GloveWithoutIndicationType");

            migrationBuilder.DropTable(
                name: "HandJewelryObservation");

            migrationBuilder.DropTable(
                name: "HandJewelryType");

            migrationBuilder.DropTable(
                name: "MisuseType");

            migrationBuilder.DropTable(
                name: "ProtectiveEquipment");

            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "HandHygieneAfterGloveUseType");

            migrationBuilder.DropTable(
                name: "ProtectiveEquipmentObservation");

            migrationBuilder.DropTable(
                name: "ProtectiveEquipmentType");

            migrationBuilder.DropTable(
                name: "ActivityType");

            migrationBuilder.DropTable(
                name: "ProtectiveEquipmentSettingType");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "TransferStatusType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "DepartmentType");

            migrationBuilder.DropTable(
                name: "Institution");

            migrationBuilder.DropTable(
                name: "HealthcareOrganization");

            migrationBuilder.DropTable(
                name: "InstitutionType");

            migrationBuilder.DropTable(
                name: "Municipality");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "RegionalHealthcareOrganization");
        }
    }
}
