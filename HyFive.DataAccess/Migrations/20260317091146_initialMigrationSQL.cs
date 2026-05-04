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
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    City = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Street = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
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
                name: "OrganisationUnitLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationUnitLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationUnitType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationUnitType", x => x.Id);
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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IdentityPseudonym = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentifierType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentifierType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SecondsUsed = table.Column<int>(type: "integer", nullable: false),
                    TimingWasPerformed = table.Column<bool>(type: "boolean", nullable: false),
                    GlovesUsed = table.Column<bool>(type: "boolean", nullable: true),
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
                name: "OrganisationUnit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AddressId = table.Column<int>(type: "integer", nullable: true),
                    TypeId = table.Column<int>(type: "integer", nullable: true),
                    LevelId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationUnit_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrganisationUnit_OrganisationUnitLevel_LevelId",
                        column: x => x.LevelId,
                        principalTable: "OrganisationUnitLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganisationUnit_OrganisationUnitType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "OrganisationUnitType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganisationUnit_OrganisationUnit_ParentId",
                        column: x => x.ParentId,
                        principalTable: "OrganisationUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "UserIdentifier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserIdentifierTypeId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserIdentifier_UserIdentifierType_UserIdentifierTypeId",
                        column: x => x.UserIdentifierTypeId,
                        principalTable: "UserIdentifierType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserIdentifier_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationUnitAssociation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceOrganisationUnitId = table.Column<int>(type: "integer", nullable: false),
                    TargetOrganisationUnitId = table.Column<int>(type: "integer", nullable: false),
                    AssociationType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationUnitAssociation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationUnitAssociation_OrganisationUnit_SourceOrganisa~",
                        column: x => x.SourceOrganisationUnitId,
                        principalTable: "OrganisationUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganisationUnitAssociation_OrganisationUnit_TargetOrganisa~",
                        column: x => x.TargetOrganisationUnitId,
                        principalTable: "OrganisationUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationUnitRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganisationUnitId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationUnitRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationUnitRole_OrganisationUnit_OrganisationUnitId",
                        column: x => x.OrganisationUnitId,
                        principalTable: "OrganisationUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationUnitRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PredefinedComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SessionType = table.Column<int>(type: "integer", nullable: false),
                    OrganisationUnitId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredefinedComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredefinedComment_OrganisationUnit_OrganisationUnitId",
                        column: x => x.OrganisationUnitId,
                        principalTable: "OrganisationUnit",
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
                    OrganisationUnitId = table.Column<int>(type: "integer", nullable: false),
                    ObserverId = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    TransferStatusId = table.Column<int>(type: "integer", nullable: true),
                    TransferStatusTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Session_OrganisationUnit_OrganisationUnitId",
                        column: x => x.OrganisationUnitId,
                        principalTable: "OrganisationUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Session_TransferStatusType_TransferStatusId",
                        column: x => x.TransferStatusId,
                        principalTable: "TransferStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Session_TransferStatusType_TransferStatusTypeId",
                        column: x => x.TransferStatusTypeId,
                        principalTable: "TransferStatusType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Session_User_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PermissionLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OrganisationUnitId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermission_OrganisationUnit_OrganisationUnitId",
                        column: x => x.OrganisationUnitId,
                        principalTable: "OrganisationUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermission_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    GlovesUsed = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "IX_IndicationType_Code",
                table: "IndicationType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndicationType_Name",
                table: "IndicationType",
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
                name: "IX_OrganisationUnit_AddressId",
                table: "OrganisationUnit",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUnit_LevelId",
                table: "OrganisationUnit",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUnit_ParentId_Name",
                table: "OrganisationUnit",
                columns: new[] { "ParentId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUnit_TypeId",
                table: "OrganisationUnit",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUnitAssociation_SourceOrganisationUnitId_Target~",
                table: "OrganisationUnitAssociation",
                columns: new[] { "SourceOrganisationUnitId", "TargetOrganisationUnitId", "AssociationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUnitAssociation_TargetOrganisationUnitId",
                table: "OrganisationUnitAssociation",
                column: "TargetOrganisationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUnitRole_OrganisationUnitId_RoleId",
                table: "OrganisationUnitRole",
                columns: new[] { "OrganisationUnitId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUnitRole_RoleId",
                table: "OrganisationUnitRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUnitType_Code",
                table: "OrganisationUnitType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PredefinedComment_OrganisationUnitId",
                table: "PredefinedComment",
                column: "OrganisationUnitId");

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
                name: "IX_Role_Name",
                table: "Role",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Session_CreatedDate",
                table: "Session",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Session_Discriminator",
                table: "Session",
                column: "Discriminator");

            migrationBuilder.CreateIndex(
                name: "IX_Session_ObserverId",
                table: "Session",
                column: "ObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_OrganisationUnitId",
                table: "Session",
                column: "OrganisationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_StartDate",
                table: "Session",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Session_TransferStatusId",
                table: "Session",
                column: "TransferStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_TransferStatusTypeId",
                table: "Session",
                column: "TransferStatusTypeId");

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
                name: "IX_User_IdentityPseudonym",
                table: "User",
                column: "IdentityPseudonym");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifier_UserId_UserIdentifierTypeId",
                table: "UserIdentifier",
                columns: new[] { "UserId", "UserIdentifierTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifier_UserIdentifierTypeId",
                table: "UserIdentifier",
                column: "UserIdentifierTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifierType_Code",
                table: "UserIdentifierType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermission_OrganisationUnitId",
                table: "UserPermission",
                column: "OrganisationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermission_UserId_OrganisationUnitId",
                table: "UserPermission",
                columns: new[] { "UserId", "OrganisationUnitId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "OrganisationUnitAssociation");

            migrationBuilder.DropTable(
                name: "OrganisationUnitRole");

            migrationBuilder.DropTable(
                name: "PredefinedComment");

            migrationBuilder.DropTable(
                name: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType");

            migrationBuilder.DropTable(
                name: "UserIdentifier");

            migrationBuilder.DropTable(
                name: "UserPermission");

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
                name: "UserIdentifierType");

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
                name: "OrganisationUnit");

            migrationBuilder.DropTable(
                name: "TransferStatusType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "OrganisationUnitLevel");

            migrationBuilder.DropTable(
                name: "OrganisationUnitType");
        }
    }
}
