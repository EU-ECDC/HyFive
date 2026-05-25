using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HyFive.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCityTable : Migration
    {
        private const string IntegerType = "integer";
        private const string NowSql = "NOW()";
        private const string TimestampWithTimeZoneType = "timestamp with time zone";
        private const string CityIdColumn = "CityId";
        private const string AddressTable = "Address";
        private const string CreatedAtColumn = "CreatedAt";
        private const string CurrentTimestampSql = "CURRENT_TIMESTAMP";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
        name: "City",
        columns: table => new
        {
            Id = table.Column<int>(type: IntegerType, nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
            CreatedAt = table.Column<DateTime>(type: TimestampWithTimeZoneType, nullable: false, defaultValueSql: NowSql),
            CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
            LastModified = table.Column<DateTime>(type: TimestampWithTimeZoneType, nullable: true),
            LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_City", x => x.Id);
        });

            migrationBuilder.CreateIndex(
                name: "IX_City_Name",
                table: "City",
                column: "Name",
                unique: true);

            migrationBuilder.AddColumn<int>(
                name: CityIdColumn,
                table: AddressTable,
                type: IntegerType,
                nullable: true);

            migrationBuilder.Sql(@"
                INSERT INTO ""City"" (""Name"", ""CreatedAt"")
                SELECT DISTINCT TRIM(""City""), NOW()
                FROM ""Address""
                WHERE ""City"" IS NOT NULL
                  AND TRIM(""City"") <> ''
                ON CONFLICT (""Name"") DO NOTHING;
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Address"" a
                SET ""CityId"" = c.""Id""
                FROM ""City"" c
                WHERE a.""City"" IS NOT NULL
                  AND TRIM(a.""City"") = c.""Name"";
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Address"" a
                SET ""CityId"" = (
                    SELECT ""Id"" FROM ""City"" WHERE ""Name"" = 'Unknown' LIMIT 1
                )
                WHERE a.""CityId"" IS NULL
                   OR NOT EXISTS (
                       SELECT 1 FROM ""City"" c WHERE c.""Id"" = a.""CityId""
                   );
            ");

            migrationBuilder.AlterColumn<int>(
                name: CityIdColumn,
                table: AddressTable,
                type: IntegerType,
                nullable: false,
                oldClrType: typeof(int),
                oldType: IntegerType,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "City",
                table: AddressTable);

            migrationBuilder.CreateIndex(
                name: "IX_Address_CityId",
                table: AddressTable,
                column: CityIdColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_City_CityId",
                table: AddressTable,
                column: CityIdColumn,
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_City_CityId",
                table: AddressTable);

            migrationBuilder.DropIndex(
                name: "IX_Address_CityId",
                table: AddressTable);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: AddressTable,
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""Address"" a
                SET ""City"" = c.""Name""
                FROM ""City"" c
                WHERE a.""CityId"" = c.""Id"";
            ");

            migrationBuilder.DropColumn(
                name: CityIdColumn,
                table: AddressTable);

            migrationBuilder.DropTable(
                name: "City");

            RevertCreatedAtDefault(migrationBuilder, "UserPermission");
            RevertCreatedAtDefault(migrationBuilder, "UserIdentifierType");
            RevertCreatedAtDefault(migrationBuilder, "UserIdentifier");
            RevertCreatedAtDefault(migrationBuilder, "OrganisationUnitType");
            RevertCreatedAtDefault(migrationBuilder, "OrganisationUnitRole");
            RevertCreatedAtDefault(migrationBuilder, "OrganisationUnitLevel");
            RevertCreatedAtDefault(migrationBuilder, "OrganisationUnit");
            RevertCreatedAtDefault(migrationBuilder, AddressTable);
        }

        private static void RevertCreatedAtDefault(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: CreatedAtColumn,
                table: table,
                type: TimestampWithTimeZoneType,
                nullable: false,
                defaultValueSql: NowSql,
                oldClrType: typeof(DateTime),
                oldType: TimestampWithTimeZoneType,
                oldDefaultValueSql: CurrentTimestampSql);
        }
    }
}
