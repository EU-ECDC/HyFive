using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyFive.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentRole_Department_DepartmentsId",
                table: "DepartmentRole");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentRole_Role_RolesId",
                table: "DepartmentRole");

            migrationBuilder.DropForeignKey(
                name: "FK_GloveObservation_HandHygieneAfterGloveUseType_HandHygiene~",
                table: "GloveObservation");

            migrationBuilder.DropForeignKey(
                name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prot~1",
                table: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType");

            migrationBuilder.DropForeignKey(
                name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prot~",
                table: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_TransferStatusType_TransferStatusId",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_User_ObserverId",
                table: "Session");

            migrationBuilder.DropTable(
                name: "DepartmentClinic");

            migrationBuilder.DropTable(
                name: "FiveIndicationsObservationIndicationType");

            migrationBuilder.DropTable(
                name: "GloveWithIndicationTypeGloveObservation");

            migrationBuilder.DropTable(
                name: "ProtectiveEquipmentMisuseType");

            migrationBuilder.RenameColumn(
                name: "ProcessedByUserId",
                table: "UserAccessRequest",
                newName: "ProcessedByUserID");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Session",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "CreatedTime",
                table: "Session",
                newName: "CreatedDate");

            migrationBuilder.RenameIndex(
                name: "IX_Session_StartTime",
                table: "Session",
                newName: "IX_Session_StartDate");

            migrationBuilder.RenameIndex(
                name: "IX_Session_CreatedTime",
                table: "Session",
                newName: "IX_Session_CreatedDate");

            migrationBuilder.RenameColumn(
                name: "IsIndicated",
                table: "ProtectiveEquipment",
                newName: "IsRequired");

            migrationBuilder.RenameColumn(
                name: "UsedGloves",
                table: "GloveObservation",
                newName: "GloveUsed");

            migrationBuilder.RenameColumn(
                name: "HandHygieneAfterGloveUseTypeId",
                table: "GloveObservation",
                newName: "PostGloveHandHygieneTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_GloveObservation_HandHygieneAfterGloveUseTypeId",
                table: "GloveObservation",
                newName: "IX_GloveObservation_PostGloveHandHygieneTypeId");

            migrationBuilder.RenameColumn(
                name: "UsedGloves",
                table: "Activity",
                newName: "GloveUsed");

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
                    IndicatedGloveTypesId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloveObservationGloveWithIndicationType", x => new { x.IndicatedGloveTypesId, x.ObservationsId });
                    table.ForeignKey(
                        name: "FK_GloveObservationGloveWithIndicationType_GloveObservation_Ob~",
                        column: x => x.ObservationsId,
                        principalTable: "GloveObservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GloveObservationGloveWithIndicationType_GloveWithIndication~",
                        column: x => x.IndicatedGloveTypesId,
                        principalTable: "GloveWithIndicationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloveObservationWithoutIndicationType",
                columns: table => new
                {
                    GeneralPurposeGloveTypesId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloveObservationWithoutIndicationType", x => new { x.GeneralPurposeGloveTypesId, x.ObservationsId });
                    table.ForeignKey(
                        name: "FK_GloveObservationWithoutIndicationType_GloveObservation~",
                        column: x => x.ObservationsId,
                        principalTable: "GloveObservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GloveObservationWithoutIndicationType_GloveWithoutIndi~",
                        column: x => x.GeneralPurposeGloveTypesId,
                        principalTable: "GloveWithoutIndicationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ClinicDepartment_DepartmentsId",
                table: "ClinicDepartment",
                column: "DepartmentsId");

            migrationBuilder.CreateIndex(
                name: "IX_FiveIndicationsObservationIndicationTypes_ObservationsId",
                table: "FiveIndicationsObservationIndicationTypes",
                column: "ObservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_GloveObservationGloveWithIndicationType_ObservationsId",
                table: "GloveObservationGloveWithIndicationType",
                column: "ObservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_MisuseTypeProtectiveEquipment_ProtectiveEquipmentId",
                table: "MisuseTypeProtectiveEquipment",
                column: "ProtectiveEquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentRole_Department_DepartmentsId",
                table: "DepartmentRole",
                column: "DepartmentsId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentRole_Role_RolesId",
                table: "DepartmentRole",
                column: "RolesId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GloveObservation_HandHygieneAfterGloveUseType_PostGloveHand~",
                table: "GloveObservation",
                column: "PostGloveHandHygieneTypeId",
                principalTable: "HandHygieneAfterGloveUseType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prote~",
                table: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType",
                column: "ProtectiveEquipmentSettingTypeId",
                principalTable: "ProtectiveEquipmentSettingType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prot~1",
                table: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType",
                column: "ProtectiveEquipmentTypeId",
                principalTable: "ProtectiveEquipmentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Session_TransferStatusType_TransferStatusId",
                table: "Session",
                column: "TransferStatusId",
                principalTable: "TransferStatusType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_User_ObserverId",
                table: "Session",
                column: "ObserverId",
                principalTable: "User",
                principalColumn: "Id");


           
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentRole_Department_DepartmentsId",
                table: "DepartmentRole");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentRole_Role_RolesId",
                table: "DepartmentRole");

            migrationBuilder.DropForeignKey(
                name: "FK_GloveObservation_HandHygieneAfterGloveUseType_PostGloveHand~",
                table: "GloveObservation");

            migrationBuilder.DropForeignKey(
                name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prote~",
                table: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType");

            migrationBuilder.DropForeignKey(
                name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prot~1",
                table: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_TransferStatusType_TransmissionStatusId",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_User_ObserverId",
                table: "Session");

            migrationBuilder.DropTable(
                name: "ClinicDepartment");

            migrationBuilder.DropTable(
                name: "FiveIndicationsObservationIndicationTypes");

            migrationBuilder.DropTable(
                name: "GloveObservationGloveWithIndicationType");

            migrationBuilder.DropTable(
                name: "GloveObservationGloveWithoutIndicationType");

            migrationBuilder.DropTable(
                name: "MisuseTypeProtectiveEquipment");

            migrationBuilder.RenameColumn(
                name: "ProcessedByUserID",
                table: "UserAccessRequest",
                newName: "ProcessedByUserId");

            migrationBuilder.RenameColumn(
                name: "TransmissionStatusId",
                table: "Session",
                newName: "TransferStatusTypeId");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Session",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "ObserverId",
                table: "Session",
                newName: "ObservatorId");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Session",
                newName: "CreatedTime");

            migrationBuilder.RenameIndex(
                name: "IX_Session_TransmissionStatusId",
                table: "Session",
                newName: "IX_Session_TransferStatusTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Session_StartDate",
                table: "Session",
                newName: "IX_Session_StartTime");

            migrationBuilder.RenameIndex(
                name: "IX_Session_ObserverId",
                table: "Session",
                newName: "IX_Session_ObservatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Session_CreatedDate",
                table: "Session",
                newName: "IX_Session_CreatedTime");

            migrationBuilder.RenameColumn(
                name: "IsRequired",
                table: "ProtectiveEquipment",
                newName: "IsIndicated");

            migrationBuilder.RenameColumn(
                name: "PostGloveHandHygieneTypeId",
                table: "GloveObservation",
                newName: "HandHygieneAfterGloveUseTypeId");

            migrationBuilder.RenameColumn(
                name: "GloveUsed",
                table: "GloveObservation",
                newName: "UsedGloves");

            migrationBuilder.RenameIndex(
                name: "IX_GloveObservation_PostGloveHandHygieneTypeId",
                table: "GloveObservation",
                newName: "IX_GloveObservation_HandHygieneAfterGloveUseTypeId");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "DepartmentRole",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "DepartmentsId",
                table: "DepartmentRole",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentRole_RolesId",
                table: "DepartmentRole",
                newName: "IX_DepartmentRole_RoleId");

            migrationBuilder.CreateTable(
                name: "DepartmentClinic",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    ClinicId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentClinic", x => new { x.DepartmentId, x.ClinicId });
                    table.ForeignKey(
                        name: "FK_DepartmentClinic_Clinic_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentClinic_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FiveIndicationsObservationIndicationType",
                columns: table => new
                {
                    IndicationTypesId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiveIndicationsObservationIndicationType", x => new { x.IndicationTypesId, x.ObservationsId });
                    table.ForeignKey(
                        name: "FK_FiveIndicationsObservationIndicationType_FiveIndicationsObs~",
                        column: x => x.ObservationsId,
                        principalTable: "FiveIndicationsObservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FiveIndicationsObservationIndicationType_IndicationType_Ind~",
                        column: x => x.IndicationTypesId,
                        principalTable: "IndicationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloveObservationWithoutIndicationType",
                columns: table => new
                {
                    GloveWithoutIndicationTypesId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloveObservationWithoutIndicationType", x => new { x.GloveWithoutIndicationTypesId, x.ObservationsId });
                    table.ForeignKey(
                        name: "FK_GloveObservationWithoutIndicationType_GloveObservation_Obse~",
                        column: x => x.ObservationsId,
                        principalTable: "GloveObservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GloveObservationWithoutIndicationType_GloveWithoutIndicatio~",
                        column: x => x.GloveWithoutIndicationTypesId,
                        principalTable: "GloveWithoutIndicationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloveWithIndicationTypeGloveObservation",
                columns: table => new
                {
                    GloveWithIndicationTypesId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloveWithIndicationTypeGloveObservation", x => new { x.GloveWithIndicationTypesId, x.ObservationsId });
                    table.ForeignKey(
                        name: "FK_GloveWithIndicationTypeGloveObservation_GloveObservation_Ob~",
                        column: x => x.ObservationsId,
                        principalTable: "GloveObservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GloveWithIndicationTypeGloveObservation_GloveWithIndication~",
                        column: x => x.GloveWithIndicationTypesId,
                        principalTable: "GloveWithIndicationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProtectiveEquipmentMisuseType",
                columns: table => new
                {
                    ProtectiveEquipmentId = table.Column<int>(type: "integer", nullable: false),
                    MisuseTypesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtectiveEquipmentMisuseType", x => new { x.ProtectiveEquipmentId, x.MisuseTypesId });
                    table.ForeignKey(
                        name: "FK_ProtectiveEquipmentMisuseType_MisuseType_MisuseTypesId",
                        column: x => x.MisuseTypesId,
                        principalTable: "MisuseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProtectiveEquipmentMisuseType_ProtectiveEquipment_Protectiv~",
                        column: x => x.ProtectiveEquipmentId,
                        principalTable: "ProtectiveEquipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentClinic_ClinicId",
                table: "DepartmentClinic",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_FiveIndicationsObservationIndicationType_ObservationsId",
                table: "FiveIndicationsObservationIndicationType",
                column: "ObservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_GloveObservationWithoutIndicationType_ObservationsId",
                table: "GloveObservationWithoutIndicationType",
                column: "ObservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_GloveWithIndicationTypeGloveObservation_ObservationsId",
                table: "GloveWithIndicationTypeGloveObservation",
                column: "ObservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectiveEquipmentMisuseType_MisuseTypesId",
                table: "ProtectiveEquipmentMisuseType",
                column: "MisuseTypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentRole_Department_DepartmentsId",
                table: "DepartmentRole",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentRole_Role_RolesId",
                table: "DepartmentRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GloveObservation_HandHygieneAfterGloveUseType_HandHygiene~",
                table: "GloveObservation",
                column: "HandHygieneAfterGloveUseTypeId",
                principalTable: "HandHygieneAfterGloveUseType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Beskyt~1",
                table: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType",
                column: "ProtectiveEquipmentSettingTypeId",
                principalTable: "ProtectiveEquipmentSettingType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProtectiveEquipmentSettingTypeProtectiveEquipmentType_Prote~",
                table: "ProtectiveEquipmentSettingTypeProtectiveEquipmentType",
                column: "ProtectiveEquipmentTypeId",
                principalTable: "ProtectiveEquipmentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Session_TransferStatusType_TransferStatusTypeId",
                table: "Session",
                column: "TransferStatusTypeId",
                principalTable: "TransferStatusType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_User_ObservatorId",
                table: "Session",
                column: "ObservatorId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
