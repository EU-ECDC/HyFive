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
            /// <inheritdoc />
            migrationBuilder.Sql(@"INSERT INTO ""ActivityType"" (""Id"", ""Code"", ""Name"") VALUES (1, N'NOT_REGISTERED', N'Not registered') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ActivityType"" (""Id"", ""Code"", ""Name"") VALUES (2, N'NOT_PERFORMED', N'Not performed') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ActivityType"" (""Id"", ""Code"", ""Name"") VALUES (3, N'HAND_WASH', N'Hand wash') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ActivityType"" (""Id"", ""Code"", ""Name"") VALUES (4, N'DISINFECTION', N'Disinfection') ON CONFLICT (""Id"") DO NOTHING");


            migrationBuilder.Sql(@"INSERT INTO ""Address"" (""Id"", ""City"", ""Street"", ""PostalCode"") VALUES (1, 'Birgu', NULL, NULL) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Address"" (""Id"", ""City"", ""Street"", ""PostalCode"") VALUES (2, 'Valletta', NULL, NULL) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Address"" (""Id"", ""City"", ""Street"", ""PostalCode"") VALUES (3, 'Victoria', NULL, NULL) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Address"" (""Id"", ""City"", ""Street"", ""PostalCode"") VALUES (4, 'Bormla', NULL, NULL) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Address"" (""Id"", ""City"", ""Street"", ""PostalCode"") VALUES (5, 'Qormi', NULL, NULL) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Address"" (""Id"", ""City"", ""Street"", ""PostalCode"") VALUES (6, 'Senglea', NULL, NULL) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Address"" (""Id"", ""City"", ""Street"", ""PostalCode"") VALUES (7, 'Mdina', NULL, NULL) ON CONFLICT (""Id"") DO NOTHING");

            migrationBuilder.Sql(@"INSERT INTO ""OrganisationUnitLevel"" (""Id"", ""Level"", ""Description"") VALUES (1, N'Facility', N'Level 1 - Facility') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""OrganisationUnitLevel"" (""Id"", ""Level"", ""Description"") VALUES (2, N'Department', N'Level 2 - Department') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""OrganisationUnitLevel"" (""Id"", ""Level"", ""Description"") VALUES (3, N'Unit', N'Level 3 - Unit') ON CONFLICT (""Id"") DO NOTHING");

            migrationBuilder.Sql(@"INSERT INTO ""OrganisationUnitType"" (""Id"", ""Code"", ""Name"", ""Description"") VALUES
      (1, N'F_PRIMARY_CARE', N'Primary care', N'Primary care'),
      (2, N'F_SECONDARY_CARE', N'Secondary care', N'Secondary care'),
      (3, N'F_TERTIARY_CARE', N'Tertiary care', N'Tertiary care'),
      (4, N'F_SPECIALISED_CARE', N'Specialised care', N'Specialised care'),
      (5, N'F_LONG_TERM_CARE', N'Long-term care', N'Long-term care'),
      (6, N'D_BONE_MARROW_TRANSPLANTATION', N'Bone Marrow Transplantation', N'Bone Marrow Transplantation'),
        (7, N'D_BURNS_CARE', N'Burns care', N'Burns care'),
        (8, N'D_CARDIO_VASCULAR_SURGERY', N'Cardio/vascular surgery', N'Cardio/vascular surgery'),
        (9, N'D_CARDIOLOGY', N'Cardiology', N'Cardiology'),
        (10, N'D_DERMATOLOGY', N'Dermatology', N'Dermatology'),
        (11, N'D_DIGESTIVE_TRACT_SURGERY', N'Digestive tract surgery', N'Digestive tract surgery'),
        (12, N'D_EAR_NOSE_AND_THROAT', N'Ear, nose and throat', N'Ear, nose and throat'),
        (13, N'D_ENDOCRINOLOGY', N'Endocrinology', N'Endocrinology'),
        (14, N'D_GASTRO_ENTEROLOGY', N'Gastro-enterology', N'Gastro-enterology'),
        (15, N'D_GENERAL_MEDICINE', N'General medicine', N'General medicine'),
        (16, N'D_GENERAL_SURGERY', N'General surgery', N'General surgery'),
        (17, N'D_GERIATRICS', N'Geriatrics', N'Geriatrics'),
        (18, N'D_GYNAECOLOGY', N'Gynaecology', N'Gynaecology'),
        (19, N'D_HAEMATOLOGY', N'Haematology', N'Haematology'),
        (20, N'D_HAEMATOLOGY_BMT', N'Haematology/BMT', N'Haematology/BMT'),
        (21, N'D_HEALTHY_NEONATES_MATERNITY', N'Healthy neonates/maternity', N'Healthy neonates/maternity'),
        (22, N'D_HEPATOLOGY', N'Hepatology', N'Hepatology'),
        (23, N'D_INFECTIOUS_DISEASES', N'Infectious diseases', N'Infectious diseases'),
        (24, N'D_LONG_TERM_CARE', N'Long-term care', N'Long-term care'),
        (25, N'D_MAXILLO_FACIAL_SURGERY', N'Maxillo-facial surgery', N'Maxillo-facial surgery'),
        (26, N'D_MEDICAL_ICU', N'Medical ICU', N'Medical ICU'),
        (27, N'D_MEDICAL_TRAUMATOLOGY', N'Medical traumatology', N'Medical traumatology'),
        (28, N'D_MIXED_ICU', N'Mixed ICU', N'Mixed ICU'),
        (29, N'D_NEONATAL_ICU', N'Neonatal ICU', N'Neonatal ICU'),
        (30, N'D_NEONATOLOGY', N'Neonatology', N'Neonatology'),
        (31, N'D_NEPHROLOGY', N'Nephrology', N'Nephrology'),
        (32, N'D_NEUROLOGY', N'Neurology', N'Neurology'),
        (33, N'D_NEUROSURGERY', N'Neurosurgery', N'Neurosurgery'),
        (34, N'D_OBSTETRICS_MATERNITY', N'Obstetrics/Maternity', N'Obstetrics/Maternity'),
        (35, N'D_ONCOLOGY', N'Oncology', N'Oncology'),
        (36, N'D_OPHTHALMOLOGY', N'Ophthalmology', N'Ophthalmology'),
        (37, N'D_ORTHOPAEDICS', N'Orthopaedics', N'Orthopaedics'),
        (38, N'D_ORTHOPAEDICS_AND_SURGICAL_TRAUMATOLOGY', N'Orthopaedics and surgical traumatology', N'Orthopaedics and surgical traumatology'),
        (39, N'D_OTHER', N'Other', N'Other'),
        (40, N'D_OTHER_SURGERY', N'Other surgery', N'Other surgery'),
        (41, N'D_PAEDIATRIC_ICU', N'Paediatric ICU', N'Paediatric ICU'),
        (42, N'D_PAEDIATRIC_GENERAL_SURGERY', N'Paediatric general surgery', N'Paediatric general surgery'),
        (43, N'D_PAEDIATRICS', N'Paediatrics', N'Paediatrics'),
        (44, N'D_PLASTIC_AND_RECONSTRUCTIVE_SURGERY', N'Plastic and reconstructive surgery', N'Plastic and reconstructive surgery'),
        (45, N'D_PNEUMOLOGY', N'Pneumology', N'Pneumology'),
        (46, N'D_PSYCHIATRICS', N'Psychiatrics', N'Psychiatrics'),
        (47, N'D_REHABILITATION', N'Rehabilitation', N'Rehabilitation'),
        (48, N'D_RHEUMATOLOGY', N'Rheumatology', N'Rheumatology'),
        (49, N'D_SPECIALISED_ICU', N'Specialised ICU', N'Specialised ICU'),
        (50, N'D_STOMATOLOGY_DENTISTRY', N'Stomatology/ Dentistry', N'Stomatology/ Dentistry'),
        (51, N'D_SURGERY_FOR_CANCER', N'Surgery for cancer', N'Surgery for cancer'),
        (52, N'D_SURGICAL_ICU', N'Surgical ICU', N'Surgical ICU'),
        (53, N'D_THORACIC_SURGERY', N'Thoracic surgery', N'Thoracic surgery'),
        (54, N'D_TRANSPLANTATION_SURGERY', N'Transplantation surgery', N'Transplantation surgery'),
        (55, N'D_TRAUMATOLOGY', N'Traumatology', N'Traumatology'),
        (56, N'D_UROLOGY', N'Urology', N'Urology'),
        (57, N'D_VASCULAR_SURGERY', N'Vescular surgery', N'Vescular surgery')
        ON CONFLICT(""Id"") DO NOTHING");

            migrationBuilder.Sql(@" INSERT INTO ""OrganisationUnit""(""Id"", ""ParentId"", ""Name"", ""Abbreviation"", ""Description"", ""AddressId"", ""TypeId"", ""LevelId"") VALUES
    (1, NULL, N'Hospital A', N'HOSA', NULL, 1, 1, 1),
    (2, NULL, N'Hospital B', N'HOSB', NULL, 2, 3, 1),
    (3, NULL, N'Hospital C', N'HOSC', NULL, 3, 4, 1) ,               
    (4, 1, N'Cardiology A', NULL, NULL, NULL, 6, 2),
    (5, 1, N'General surgery A', NULL, NULL, NULL, 7, 2),
    (6, 1, N'Haematology A', NULL, NULL, NULL, 8, 2),
    (7, 1, N'Haematology B', NULL, NULL, NULL, 8, 2),
    (8, 1, N'Neurology A', NULL, NULL, NULL, 9, 2),
    (9, 1, N'Neurology B', NULL, NULL, NULL, 9, 2),
    (10, 1, N'Neurology C', NULL, NULL, NULL, 9, 2),
    (11, 1, N'Paediatrics A', NULL, NULL, NULL, 10, 2),
    (12, 1, N'Paediatrics B', NULL, NULL, NULL, 10, 2),
    (13, 2, N'ICU Burn', NULL, NULL, NULL, 11, 2),
    (14, 2, N'Med ICU', NULL, NULL, NULL, 12, 2),
    (15, 2, N'Lung ICU', NULL, NULL, NULL, 12, 2),
    (16, 2, N'Neo full term', NULL, NULL, NULL, 13, 2),
    (17, 2, N'Neo pre term', NULL, NULL, NULL, 13, 2),
    (18, 2, N'Onco', NULL, NULL, NULL, 14, 2),
    (19, 2, N'Neonates', NULL, NULL, NULL, 15, 2),
    (20, 3, N'Haematology A', NULL, NULL, NULL, 8, 2),
    (21, 3, N'Neurology A', NULL, NULL, NULL, 9, 2),
    (22, 3, N'Paediatrics A', NULL, NULL, NULL, 10, 2),
    (23, 3, N'Paediatrics B', NULL, NULL, NULL, 10, 2),
    (24, 3, N'Onco', NULL, NULL, NULL, 14, 2),
    (25, 4, N'Cardio 1', NULL, NULL, NULL, 6, 3),
    (26, 5, N'Surgical 1', NULL, NULL, NULL, 7, 3),
    (27, 6, N'Haema 1', NULL, NULL, NULL, 8, 3),
    (28, 7, N'Haema 2', NULL, NULL, NULL, 8, 3),
    (29, 8, N'Neuro elderly', NULL, NULL, NULL, 9, 3),
    (30, 9, N'Neuro paed', NULL, NULL, NULL, 9, 3),
    (31, 10, N'Neuro palliative', NULL, NULL, NULL, 9, 3),
    (32, 11, N'Paed 1', NULL, NULL, NULL, 10, 3),
    (33, 12, N'Paed 2', NULL, NULL, NULL, 10, 3),
    (34, 13, N'ICU Burn A', NULL, NULL, NULL, 11, 3),
    (35, 14, N'Med ICU A', NULL, NULL, NULL, 12, 3),
    (36, 14, N'Med ICU B', NULL, NULL, NULL, 12, 3),
    (37, 15, N'Lung ICU A', NULL, NULL, NULL, 12, 3),
    (38, 16, N'NFT', NULL, NULL, NULL, 13, 3),
    (39, 17, N'NPT', NULL, NULL, NULL, 13, 3),
    (40, 18, N'Onco A', NULL, NULL, NULL, 14, 3),
    (41, 19, N'Neonates A', NULL, NULL, NULL, 13, 3),
    (42, 19, N'Neonates B', NULL, NULL, NULL, 13, 3),
    (43, 20, N'Haema 1', NULL, NULL, NULL, 8, 3),
    (44, 21, N'Neuro elderly', NULL, NULL, NULL, 9, 3),
    (45, 22, N'Paed 1', NULL, NULL, NULL, 10, 3),
    (46, 23, N'Paed 2', NULL, NULL, NULL, 10, 3),
    (47, 24, N'Onco A', NULL, NULL, NULL, 14, 3)
ON CONFLICT (""Id"") DO NOTHING");

            migrationBuilder.Sql(@"INSERT INTO ""Role"" (""Id"", ""Name"", ""Description"") VALUES (1, N'Doctor', NULL) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Role"" (""Id"", ""Name"", ""Description"") VALUES (2, N'Nurse', N'Nurses and nurses with special education') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Role"" (""Id"", ""Name"", ""Description"") VALUES (3, N'Student', NULL) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Role"" (""Id"", ""Name"", ""Description"") VALUES (4, N'Auxiliary staff', NULL) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""Role"" (""Id"", ""Name"", ""Description"") VALUES (5, N'Other', NULL) ON CONFLICT (""Id"") DO NOTHING");


            migrationBuilder.Sql(@"
    INSERT INTO ""OrganisationUnitRole"" (""OrganisationUnitId"", ""RoleId"")
    SELECT ou.""Id"", r.""Id""
    FROM ""OrganisationUnit"" ou
    CROSS JOIN ""Role"" r
    WHERE ou.""LevelId"" = 2
    ON CONFLICT DO NOTHING;
    ");


            migrationBuilder.Sql(@" INSERT INTO ""User"" (""Id"", ""LastName"", ""FirstName"", ""CreatedTime"", ""IsDeactivated"", ""IdentityPseudonym"", ""Email"")
            VALUES
            (1,  'Vits', 'Grønn', TIMESTAMP '2021-08-05T07:12:21.6585229', FALSE, NULL, 'epipulse_ecdc_o@ecdc.europa.eu'),
            (2,  'Grevling', 'Kvart', TIMESTAMP '2021-08-05T07:12:21.6402513', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (3,  'Kjeltring', 'Virkelig', TIMESTAMP '2021-08-05T07:12:21.5567643', FALSE, NULL, 'epipulsetest.ext.at@test.ecdc.europa.eu'),
            (4,  'Grevling', 'Kvart', TIMESTAMP '2021-08-24T07:27:19.3604512', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (5,  'Grevling', 'Kvart', TIMESTAMP '2021-09-07T09:19:49.4211127', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (6,  'Grevling', 'Kvart', TIMESTAMP '2021-09-09T11:35:38.1485530', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (7,  'Grevling', 'Kvart', TIMESTAMP '2021-09-10T13:40:14.5199901', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (8,  'Kjeltring', 'Virkelig', TIMESTAMP '2021-09-15T09:50:13.4933279', FALSE, NULL, 'epipulsetest.ext.at@test.ecdc.europa.eu'),
            (9,  'Jadczak', 'Ursula', TIMESTAMP '2021-09-21T11:56:46.7907555', FALSE, NULL, 'epipulsetest.ext.it.fwd@test.ecdc.europa.eu'),
            (10, 'Moe Sæther', 'Guri', TIMESTAMP '2021-09-22T08:25:36.4117626', FALSE, NULL, 'epipulsetest.ext.demo@test.ecdc.europa.eu'),
            (11, 'Røen Skogland', 'Trygve', TIMESTAMP '2021-09-22T08:25:49.8159852', FALSE, NULL, 'epipulsetest.ext.eg@test.ecdc.europa.eu'),
            (12, 'Skeide', 'Rikke', TIMESTAMP '2021-09-22T08:26:04.5476963', FALSE, NULL, 'epipulsetest.ext.baa@test.ecdc.europa.eu'),
            (13, 'Alfsen', 'Jim', TIMESTAMP '2021-09-22T08:26:23.0400767', FALSE, NULL, 'epipulsetest.wgssal@test.ecdc.europa.eu'),
            (14, 'Aunan', 'Knut ', TIMESTAMP '2021-09-22T08:26:32.4522064', FALSE, NULL, 'epipulsetest.efsa2@test.ecdc.europa.eu'),
            (15, 'Jama Herstad Engum', 'Bjørnar Eivind ', TIMESTAMP '2021-09-22T08:26:45.2173199', FALSE, NULL, 'epipulsetest.eip@test.ecdc.europa.eu'),
            (16, 'Multi', 'Inga ', TIMESTAMP '2021-09-22T08:26:57.0966088', FALSE, NULL, 'epipulsetest.ext.be1@test.ecdc.europa.eu'),
            (17, 'Thon Synnes', 'Sigrunn ', TIMESTAMP '2021-09-22T08:27:11.4323779', FALSE, NULL, 'epipulsetest.ext.mk@test.ecdc.europa.eu'),
            (18, 'Psa Elstad', 'Ella', TIMESTAMP '2021-09-22T08:27:22.8837230', FALSE, NULL, 'epipulsetest.ext.whoeuro@test.ecdc.europa.eu'),
            (19, 'Schultz', 'Helle ', TIMESTAMP '2021-09-22T08:27:32.2884050', FALSE, NULL, 'epipulsetest.tld@test.ecdc.europa.eu'),
            (20, 'Saue', 'Isabell', TIMESTAMP '2021-10-18T08:48:27.8776442', FALSE, NULL, 'epipulsetest.ecdc.ror@test.ecdc.europa.eu'),
            (21, 'LANDSVERK', 'ENDRE', TIMESTAMP '2022-09-23T06:49:34.0795519', FALSE, NULL, 'epipulsetest.ext.tr@test.ecdc.europa.eu'),
            (22, 'Saue', 'Isabell', TIMESTAMP '2022-10-06T09:37:36.0221489', FALSE, NULL, 'epipulsetest.ecdc.ror@test.ecdc.europa.eu'),
            (23, 'STANGELAND', 'STIAN PÅL', TIMESTAMP '2022-10-10T13:45:57.4291769', FALSE, NULL, 'epipulsetest@test.ecdc.europa.eu'),
            (24, 'STANGELAND', 'STIAN PÅL', TIMESTAMP '2022-10-10T13:46:40.8363856', FALSE, NULL, 'epipulsetest2@test.ecdc.europa.eu'),
            (25, 'STANGELAND', 'Stein', TIMESTAMP '2022-10-10T13:47:50.0660310', FALSE, NULL, 'epipulsetest3@test.ecdc.europa.eu'),
            (26, 'JENSEN', 'KARIN MARIANNE', TIMESTAMP '2022-10-10T15:18:51.6032173', FALSE, NULL, 'epipulsetest.ext.uk@test.ecdc.europa.eu'),
            (27, 'Gundersen', 'Roland ', TIMESTAMP '2022-10-11T08:09:41.4974289', FALSE, NULL, 'epipulsetest5@test.ecdc.europa.eu'),
            (28, 'DAHLE', 'MARTHA', TIMESTAMP '2022-10-11T08:13:35.1173136', FALSE, NULL, 'epipulsetest6@test.ecdc.europa.eu'),
            (29, 'SAUE', 'ISABELL', TIMESTAMP '2022-10-24T14:41:08.7444832', FALSE, NULL, 'epipulsetest7@test.ecdc.europa.eu'),
            (30, 'Grevling', 'Kvart', TIMESTAMP '2023-01-17T10:00:56.9213927', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (31, 'Lin', 'Rita', TIMESTAMP '2023-01-17T10:22:45.7949409', FALSE, NULL, 'rita@lin.no'),
            (32, 'Lin', 'Rita', TIMESTAMP '2023-01-17T10:22:45.7972551', FALSE, NULL, 'epipulsetest9@test.ecdc.europa.eu'),
            (33, 'ROLFSEN', 'KARSTEIN', TIMESTAMP '2023-01-17T13:12:30.9625445', FALSE, NULL, ''),
            (34, 'SAUE', 'ISABELL', TIMESTAMP '2023-04-12T14:54:39.0953161', FALSE, NULL, ''),
            (35, 'Grevling', 'Kvart', TIMESTAMP '2023-05-10T10:01:53.5217328', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (36, 'Grevling', 'Kvart', TIMESTAMP '2023-05-10T10:02:08.6688450', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (37, 'Grevling', 'Kvart', TIMESTAMP '2023-05-10T10:02:21.0808744', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (38, 'Grevling', 'Kvart', TIMESTAMP '2023-05-10T10:02:46.7122389', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (39, 'Grevling', 'Kvart', TIMESTAMP '2023-05-10T10:03:00.4634366', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (40, 'Grevling', 'Kvart', TIMESTAMP '2023-05-10T10:03:25.0876650', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (41, 'Grevling', 'Kvart', TIMESTAMP '2023-05-10T12:48:46.4324948', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (42, 'Grevling', 'Kvart', TIMESTAMP '2023-05-11T14:41:43.1701505', FALSE, NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (43, 'Grevling', 'Kvart', TIMESTAMP '2023-05-11T14:41:43.9228467', TRUE,  NULL, 'epipulsetest.ecdc.p@test.ecdc.europa.eu'),
            (44, 'GLAD', 'PÅL', TIMESTAMP '2023-11-16T13:17:38.2308257', FALSE, NULL, 'epipulsetest.ext.italyy@test.ecdc.europa.eu'),
            (45, 'LOGARITME', 'VEIK', TIMESTAMP '2024-05-14T13:35:45.1739428', FALSE, NULL, 'epipulsetest.ext.us@test.ecdc.europa.eu'),
            (46, 'Haugenes', 'Ståle', TIMESTAMP '2024-08-01T09:23:21.6943638', FALSE, NULL, ''),
            (47, 'Haugenes', 'Ståle', TIMESTAMP '2024-08-06T14:54:31.6742594', FALSE, NULL, 'epipulsetest.ext.ecror@test.ecdc.europa.eu'),
            (48, 'Fager', 'Mette ', TIMESTAMP '2024-09-20T11:01:27.2226989', FALSE, NULL, ''),
            (49, 'b', 'a', TIMESTAMP '2024-09-20T11:39:14.7007139', FALSE, NULL, 'a@b'),
            (50, 'h', 'cD', TIMESTAMP '2024-09-20T11:42:30.1366469', FALSE, NULL, ''),
            (51, 'Vits', 'Grønn', TIMESTAMP '2021-08-05T07:12:21.6585229', FALSE, NULL, 'epipulse_ecdc_o@ecdc.europa.eu'),
            (52, 'Vits', 'Grønn', TIMESTAMP '2021-08-05T07:12:21.6585229', FALSE, NULL, 'epipulse_ecdc_o@ecdc.europa.eu'),
            (53, 'Vits', 'Grønn', TIMESTAMP '2021-08-05T07:12:21.6585229', FALSE, NULL, 'epipulse_ecdc_o@ecdc.europa.eu'),
            (54, 'Admin1', 'HyFive', TIMESTAMP '2025-09-12T12:39:21.6585229', FALSE, NULL, 'hyfiveadmin1@test.ecdc.europa.eu'),
            (55, 'Admin2', 'HyFive', TIMESTAMP '2025-09-12T12:42:21.6585229', FALSE, NULL, 'hyfiveadmin2@test.ecdc.europa.eu'),
            (56, 'Wash', 'Coordinator', TIMESTAMP '2025-09-12T12:44:21.6585229', FALSE, NULL, 'coordinatorwash@test.ecdc.europa.eu'),
            (57, 'Clean', 'Coordinator', TIMESTAMP '2025-09-12T12:46:21.6585229', FALSE, NULL, 'coordinatorclean@test.ecdc.europa.eu'),
            (58, 'Dry', 'Coordinator', TIMESTAMP '2025-09-12T12:48:21.6585229', FALSE, NULL, 'coordinatordry@test.ecdc.europa.eu'),
            (59, 'Wash', 'Observer', TIMESTAMP '2025-09-12T12:50:21.6585229', FALSE, NULL, 'observerwash@test.ecdc.europa.eu'),
            (60, 'Clean', 'Observer', TIMESTAMP '2025-09-12T12:52:21.6585229', FALSE, NULL, 'observerclean@test.ecdc.europa.eu'),
            (61, 'Dry', 'Observer', TIMESTAMP '2025-09-12T12:54:21.6585229', FALSE, NULL, 'observerdry@test.ecdc.europa.eu')
            ON CONFLICT (""Id"") DO NOTHING
            ");

            migrationBuilder.Sql(@"INSERT INTO ""UserPermission"" (""Id"", ""UserId"", ""PermissionLevel"", ""OrganisationUnitId"")
            VALUES
                (1,54, N'Administrator', NULL),
                (2,55, N'Administrator', NULL), 
                (3,56, N'Coordinator', 1),                
                (4,57, N'Coordinator', 2),
                (5,58, N'Coordinator', 3),
                (6,59, N'Observer', 1),
                (7,60, N'Observer', 2),
                (8,61, N'Observer', 3)
            ON CONFLICT (""Id"") DO NOTHING;
            ");

            migrationBuilder.Sql(@"INSERT INTO ""TransferStatusType"" (""Id"", ""Code"", ""Name"") VALUES (1, N'TRANSFERRED_TO_ADMIN', N'Transferred To Admin') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""TransferStatusType"" (""Id"", ""Code"", ""Name"") VALUES (2, N'TRANSFERRED_TO_COORDINATOR', N'Transferred To Coordinator') ON CONFLICT (""Id"") DO NOTHING");


            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingType"" (""Id"", ""Code"", ""Name"") VALUES (1, N'BASIC_ISOLATION_ROUTINES', N'Basic isolation routines') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingType"" (""Id"", ""Code"", ""Name"") VALUES (2, N'CONTACT_TRANSMISSION', N'Contact transmission') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingType"" (""Id"", ""Code"", ""Name"") VALUES (3, N'DROPLET_TRANSMISSION', N'Droplet transmission') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingType"" (""Id"", ""Code"", ""Name"") VALUES (4, N'AIRBORNE_TRANSMISSION', N'Airborne Transmission') ON CONFLICT (""Id"") DO NOTHING");


            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentType"" (""Id"", ""Code"", ""Name"") VALUES (1, N'GOWN', N'Gown (Care)') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentType"" (""Id"", ""Code"", ""Name"") VALUES (2, N'TRANSMISSION_CONTROL_GOWN', N'Transmission Control Gown') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentType"" (""Id"", ""Code"", ""Name"") VALUES (3, N'FACE_MASK', N'Face Mask') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentType"" (""Id"", ""Code"", ""Name"") VALUES (4, N'RESPIRATORY_PROTECTION', N'Respiratory Protection') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentType"" (""Id"", ""Code"", ""Name"") VALUES (5, N'EYE_PROTECTION', N'Eye Protection') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentType"" (""Id"", ""Code"", ""Name"") VALUES (6, N'HOOD', N'Hood') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentType"" (""Id"", ""Code"", ""Name"") VALUES (7, N'PLASTIC_APRON', N'Plastic Apron') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentType"" (""Id"", ""Code"", ""Name"") VALUES (8, N'GLOVES', N'Gloves') ON CONFLICT (""Id"") DO NOTHING");


            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (1, N'Incorrect use during donning', 1) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (2, N'Incorrect technique during donning', 8) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (3, N'Incorrect technique during doffing', 7) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (4, N'Not properly closed', 7) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (5, N'Incorrect use during donning', 7) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (6, N'Incorrect technique during donning', 6) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (7, N'Incorrect technique during doffing', 6) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (8, N'Incorrect technique during donning', 5) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (9, N'Incorrect technique during doffing', 5) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (10, N'Incorrect technique during donning', 4) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (11, N'Not fitted/performed fit check', 4) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (12, N'Incorrect technique during doffing', 4) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (13, N'Incorrect technique during donning', 3) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (14, N'Loosely fastened around nose/mouth', 3) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (15, N'Not fastened over nose', 3) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (16, N'Not pulled under chin', 3) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (17, N'Incorrect technique during doffing', 3) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (18, N'Incorrect use during donning', 2) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (19, N'Not properly closed', 2) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (20, N'Incorrect technique during doffing', 2) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (21, N'Incorrect technique during doffing', 1) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (22, N'Not properly closed', 1) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (23, N'Incorrect technique during doffing', 8) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""MisuseType"" (""Id"", ""Name"", ""ProtectiveEquipmentTypeId"") VALUES (24, N'Not fastened over cuff', 8) ON CONFLICT (""Id"") DO NOTHING");


            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (1, 1, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (1, 2, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (1, 3, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (1, 4, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (2, 1, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (2, 2, TRUE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (2, 3, TRUE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (2, 4, TRUE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (3, 1, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (3, 2, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (3, 3, TRUE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (3, 4, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (4, 1, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (4, 2, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (4, 3, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (4, 4, TRUE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (5, 1, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (5, 2, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (5, 3, TRUE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (5, 4, TRUE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (6, 1, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (6, 2, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (6, 3, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (6, 4, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (7, 1, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (7, 2, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (7, 3, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (7, 4, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (8, 1, FALSE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (8, 2, TRUE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (8, 3, TRUE) ON CONFLICT  DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""ProtectiveEquipmentSettingTypeProtectiveEquipmentType"" (""ProtectiveEquipmentTypeId"", ""ProtectiveEquipmentSettingTypeId"", ""IsDefault"") VALUES (8, 4, TRUE) ON CONFLICT  DO NOTHING");


            migrationBuilder.Sql(@"INSERT INTO ""IndicationType"" (""Id"", ""Name"", ""Code"", ""Number"") VALUES (1, N'After patient', N'AFTER_PATIENT', N'4') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""IndicationType"" (""Id"", ""Name"", ""Code"", ""Number"") VALUES (2, N'Body fluid', N'BODY_FLUID', N'3') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""IndicationType"" (""Id"", ""Name"", ""Code"", ""Number"") VALUES (3, N'Aseptic', N'ASEPTIC_PROCEDURES', N'2') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""IndicationType"" (""Id"", ""Name"", ""Code"", ""Number"") VALUES (4, N'Before patient', N'BEFORE_PATIENT', N'1') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""IndicationType"" (""Id"", ""Name"", ""Code"", ""Number"") VALUES (5, N'Patient`s Surroundings', N'PATIENTS_SURROUNDINGS', N'5') ON CONFLICT (""Id"") DO NOTHING");


            migrationBuilder.Sql(@"INSERT INTO ""HandJewelryType"" (""Id"", ""Code"", ""Name"", ""Order"", ""IsActive"") VALUES (1, N'ARTIFICIAL_NAIL_SHELLAC', N'Artificial nails/Shellac', 1, TRUE) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""HandJewelryType"" (""Id"", ""Code"", ""Name"", ""Order"", ""IsActive"") VALUES (2, N'LONG_NAILS', N'Long nails', 2, TRUE) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""HandJewelryType"" (""Id"", ""Code"", ""Name"", ""Order"", ""IsActive"") VALUES (3, N'WATCH_BRACELET', N'Watch/Bracelet', 3, TRUE) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""HandJewelryType"" (""Id"", ""Code"", ""Name"", ""Order"", ""IsActive"") VALUES (4, N'RING', N'Ring', 4, TRUE) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""HandJewelryType"" (""Id"", ""Code"", ""Name"", ""Order"", ""IsActive"") VALUES (5, N'ALL_OK', N'All is ok', 99, TRUE) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""HandJewelryType"" (""Id"", ""Code"", ""Name"", ""Order"", ""IsActive"") VALUES (6, N'SHORT_SLEEVES', N'Short sleeves', 5, FALSE) ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""HandJewelryType"" (""Id"", ""Code"", ""Name"", ""Order"", ""IsActive"") VALUES (7, N'LONG_SLEEVES', N'Long sleeves', 6, TRUE) ON CONFLICT (""Id"") DO NOTHING");




            migrationBuilder.Sql(@"INSERT INTO ""GloveWithIndicationType"" (""Id"", ""Code"", ""Name"") VALUES (1, N'OTHER', N'Other') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""GloveWithIndicationType"" (""Id"", ""Code"", ""Name"") VALUES (2, N'CONTACT_PRECAUTIONS', N'Contact precautions') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""GloveWithIndicationType"" (""Id"", ""Code"", ""Name"") VALUES (3, N'BODY_FLUIDS', N'Body fluids') ON CONFLICT (""Id"") DO NOTHING");

            migrationBuilder.Sql(@"INSERT INTO ""GloveWithoutIndicationType"" (""Id"", ""Code"", ""Name"") VALUES (1, N'CARE_WITHOUT_BODY_FLUIDS', N'Care without body fluids') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""GloveWithoutIndicationType"" (""Id"", ""Code"", ""Name"") VALUES (2, N'FOOD', N'Food') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""GloveWithoutIndicationType"" (""Id"", ""Code"", ""Name"") VALUES (3, N'OTHER', N'Other') ON CONFLICT (""Id"") DO NOTHING");


            migrationBuilder.Sql(@"INSERT INTO ""HandHygieneAfterGloveUseType"" (""Id"", ""Code"", ""Name"") VALUES (1, N'YES', N'Yes') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""HandHygieneAfterGloveUseType"" (""Id"", ""Code"", ""Name"") VALUES (2, N'NO', N'No') ON CONFLICT (""Id"") DO NOTHING");
            migrationBuilder.Sql(@"INSERT INTO ""HandHygieneAfterGloveUseType"" (""Id"", ""Code"", ""Name"") VALUES (3, N'NOT_INDICATED', N'Not indicated') ON CONFLICT (""Id"") DO NOTHING");


            // Sequence fixes after inserts
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""ActivityType""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""ActivityType"";");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Activity""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""Activity"";");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Role""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""Role"";");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""User""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""User"";");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""ProtectiveEquipment""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""ProtectiveEquipment"";");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""OrganisationUnit""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""OrganisationUnit"";");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""OrganisationUnitLevel""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""OrganisationUnitLevel"";");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""OrganisationUnitType""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""OrganisationUnitType"";");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""UserPermission""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""UserPermission"";");
            migrationBuilder.Sql(@"SELECT setval(pg_get_serial_sequence('""Address""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""Address"";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
