using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HyFive.Dataaksess.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AktivitetType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AktivitetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvdelingType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvdelingType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeskyttelsesutstyrsettingType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeskyttelsesutstyrsettingType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeskyttelsesutstyrType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeskyttelsesutstyrType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForesporselOmBrukertilgang",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstitusjonId = table.Column<int>(type: "integer", nullable: true),
                    BrukerFornavn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BrukerEtternavn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdentPseudonym = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HPRNummer = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Opprettettidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BehandletTidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BehandletAvBrukerId = table.Column<int>(type: "integer", nullable: true),
                    BehandletAvBrukernavn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForesporselOmBrukertilgang", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HandhygieneEtterHanskebrukType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandhygieneEtterHanskebrukType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HandsmykkeType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Rekkefolge = table.Column<int>(type: "integer", nullable: false),
                    ErAktiv = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandsmykkeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HanskeMedIndikasjonType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HanskeMedIndikasjonType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HanskeUtenIndikasjonType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HanskeUtenIndikasjonType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndikasjonType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Nummer = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndikasjonType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstitusjonType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitusjonType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kommune",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nummer = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kommune", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OverforingstatusType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverforingstatusType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionaltHelseforetak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Navn = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionaltHelseforetak", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rolle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Navn = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Beskrivelse = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rolle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aktivitet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SekunderBrukt = table.Column<int>(type: "integer", nullable: false),
                    TidtakingBleUtfort = table.Column<bool>(type: "boolean", nullable: false),
                    BenyttetHanske = table.Column<bool>(type: "boolean", nullable: true),
                    AktivitetTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aktivitet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aktivitet_AktivitetType_AktivitetTypeId",
                        column: x => x.AktivitetTypeId,
                        principalTable: "AktivitetType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType",
                columns: table => new
                {
                    BeskyttelsesutstyrTypeId = table.Column<int>(type: "integer", nullable: false),
                    BeskyttelsesutstyrsettingTypeId = table.Column<int>(type: "integer", nullable: false),
                    ErDefaultIndikert = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType", x => new { x.BeskyttelsesutstyrTypeId, x.BeskyttelsesutstyrsettingTypeId });
                    table.ForeignKey(
                        name: "FK_BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType_Beskytt~",
                        column: x => x.BeskyttelsesutstyrTypeId,
                        principalTable: "BeskyttelsesutstyrType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType_Beskyt~1",
                        column: x => x.BeskyttelsesutstyrsettingTypeId,
                        principalTable: "BeskyttelsesutstyrsettingType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeilbrukType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Navn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BeskyttelsesutstyrTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeilbrukType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeilbrukType_BeskyttelsesutstyrType_BeskyttelsesutstyrTypeId",
                        column: x => x.BeskyttelsesutstyrTypeId,
                        principalTable: "BeskyttelsesutstyrType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Helseforetak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Navn = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    RegionaltHelseforetakId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Helseforetak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Helseforetak_RegionaltHelseforetak_RegionaltHelseforetakId",
                        column: x => x.RegionaltHelseforetakId,
                        principalTable: "RegionaltHelseforetak",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Institusjon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HERId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Opprettettidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Navn = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Forkortelse = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    InstitusjontypeId = table.Column<int>(type: "integer", nullable: true),
                    RegionId = table.Column<int>(type: "integer", nullable: true),
                    HelseforetakId = table.Column<int>(type: "integer", nullable: true),
                    KommuneId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institusjon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Institusjon_Helseforetak_HelseforetakId",
                        column: x => x.HelseforetakId,
                        principalTable: "Helseforetak",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Institusjon_InstitusjonType_InstitusjontypeId",
                        column: x => x.InstitusjontypeId,
                        principalTable: "InstitusjonType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Institusjon_Kommune_KommuneId",
                        column: x => x.KommuneId,
                        principalTable: "Kommune",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Institusjon_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Avdeling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstitusjonId = table.Column<int>(type: "integer", nullable: false),
                    Navn = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AvdelingtypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avdeling", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avdeling_AvdelingType_AvdelingtypeId",
                        column: x => x.AvdelingtypeId,
                        principalTable: "AvdelingType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Avdeling_Institusjon_InstitusjonId",
                        column: x => x.InstitusjonId,
                        principalTable: "Institusjon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bruker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstitusjonId = table.Column<int>(type: "integer", nullable: true),
                    Opprettettidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Fornavn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Etternavn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Epost = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IdentPseudonym = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ErDeaktivert = table.Column<bool>(type: "boolean", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    HPRNummer = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bruker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bruker_Institusjon_InstitusjonId",
                        column: x => x.InstitusjonId,
                        principalTable: "Institusjon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Klinikk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Navn = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    InstitusjonId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klinikk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Klinikk_Institusjon_InstitusjonId",
                        column: x => x.InstitusjonId,
                        principalTable: "Institusjon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PredefinertKommentar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kommentar = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SesjonType = table.Column<int>(type: "integer", nullable: false),
                    InstitusjonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredefinertKommentar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredefinertKommentar_Institusjon_InstitusjonId",
                        column: x => x.InstitusjonId,
                        principalTable: "Institusjon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvdelingRolle",
                columns: table => new
                {
                    AvdelingerId = table.Column<int>(type: "integer", nullable: false),
                    RollerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvdelingRolle", x => new { x.AvdelingerId, x.RollerId });
                    table.ForeignKey(
                        name: "FK_AvdelingRolle_Avdeling_AvdelingerId",
                        column: x => x.AvdelingerId,
                        principalTable: "Avdeling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvdelingRolle_Rolle_RollerId",
                        column: x => x.RollerId,
                        principalTable: "Rolle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sesjon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Opprettettidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Starttidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AvdelingId = table.Column<int>(type: "integer", nullable: true),
                    ObservatorId = table.Column<int>(type: "integer", nullable: true),
                    Kommentar = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    OverforingstatusId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sesjon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sesjon_Avdeling_AvdelingId",
                        column: x => x.AvdelingId,
                        principalTable: "Avdeling",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sesjon_Bruker_ObservatorId",
                        column: x => x.ObservatorId,
                        principalTable: "Bruker",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sesjon_OverforingstatusType_OverforingstatusId",
                        column: x => x.OverforingstatusId,
                        principalTable: "OverforingstatusType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AvdelingKlinikk",
                columns: table => new
                {
                    AvdelingerId = table.Column<int>(type: "integer", nullable: false),
                    KlinikkerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvdelingKlinikk", x => new { x.AvdelingerId, x.KlinikkerId });
                    table.ForeignKey(
                        name: "FK_AvdelingKlinikk_Avdeling_AvdelingerId",
                        column: x => x.AvdelingerId,
                        principalTable: "Avdeling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvdelingKlinikk_Klinikk_KlinikkerId",
                        column: x => x.KlinikkerId,
                        principalTable: "Klinikk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeskyttelsesutstyrObservasjon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SettingtypeId = table.Column<int>(type: "integer", nullable: true),
                    BeskyttelsesutstyrSesjonId = table.Column<Guid>(type: "uuid", nullable: true),
                    Opprettettidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RolleId = table.Column<int>(type: "integer", nullable: true),
                    Registrerttidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Kommentar = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeskyttelsesutstyrObservasjon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeskyttelsesutstyrObservasjon_BeskyttelsesutstyrsettingType~",
                        column: x => x.SettingtypeId,
                        principalTable: "BeskyttelsesutstyrsettingType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BeskyttelsesutstyrObservasjon_Rolle_RolleId",
                        column: x => x.RolleId,
                        principalTable: "Rolle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BeskyttelsesutstyrObservasjon_Sesjon_BeskyttelsesutstyrSesj~",
                        column: x => x.BeskyttelsesutstyrSesjonId,
                        principalTable: "Sesjon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FireIndikasjonerObservasjon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FireIndikasjonerSesjonId = table.Column<Guid>(type: "uuid", nullable: true),
                    AktivitetId = table.Column<int>(type: "integer", nullable: true),
                    Opprettettidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RolleId = table.Column<int>(type: "integer", nullable: true),
                    Registrerttidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Kommentar = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireIndikasjonerObservasjon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FireIndikasjonerObservasjon_Aktivitet_AktivitetId",
                        column: x => x.AktivitetId,
                        principalTable: "Aktivitet",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FireIndikasjonerObservasjon_Rolle_RolleId",
                        column: x => x.RolleId,
                        principalTable: "Rolle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FireIndikasjonerObservasjon_Sesjon_FireIndikasjonerSesjonId",
                        column: x => x.FireIndikasjonerSesjonId,
                        principalTable: "Sesjon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HandsmykkeObservasjon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HandsmykkeSesjonId = table.Column<Guid>(type: "uuid", nullable: true),
                    Opprettettidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RolleId = table.Column<int>(type: "integer", nullable: true),
                    Registrerttidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Kommentar = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandsmykkeObservasjon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandsmykkeObservasjon_Rolle_RolleId",
                        column: x => x.RolleId,
                        principalTable: "Rolle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HandsmykkeObservasjon_Sesjon_HandsmykkeSesjonId",
                        column: x => x.HandsmykkeSesjonId,
                        principalTable: "Sesjon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HanskeObservasjon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BenyttetHanske = table.Column<bool>(type: "boolean", nullable: false),
                    HandhygieneEtterHanskebrukTypeId = table.Column<int>(type: "integer", nullable: true),
                    HanskeSesjonId = table.Column<Guid>(type: "uuid", nullable: true),
                    Opprettettidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RolleId = table.Column<int>(type: "integer", nullable: true),
                    Registrerttidspunkt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Kommentar = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HanskeObservasjon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HanskeObservasjon_HandhygieneEtterHanskebrukType_Handhygien~",
                        column: x => x.HandhygieneEtterHanskebrukTypeId,
                        principalTable: "HandhygieneEtterHanskebrukType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HanskeObservasjon_Rolle_RolleId",
                        column: x => x.RolleId,
                        principalTable: "Rolle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HanskeObservasjon_Sesjon_HanskeSesjonId",
                        column: x => x.HanskeSesjonId,
                        principalTable: "Sesjon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Beskyttelsesutstyr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BleBenyttet = table.Column<bool>(type: "boolean", nullable: false),
                    ErIndikert = table.Column<bool>(type: "boolean", nullable: false),
                    UtstyrstypeId = table.Column<int>(type: "integer", nullable: true),
                    BleBenyttetRiktig = table.Column<bool>(type: "boolean", nullable: false),
                    Kommentar = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    BeskyttelsesutstyrObservasjonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beskyttelsesutstyr", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beskyttelsesutstyr_BeskyttelsesutstyrObservasjon_Beskyttels~",
                        column: x => x.BeskyttelsesutstyrObservasjonId,
                        principalTable: "BeskyttelsesutstyrObservasjon",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Beskyttelsesutstyr_BeskyttelsesutstyrType_UtstyrstypeId",
                        column: x => x.UtstyrstypeId,
                        principalTable: "BeskyttelsesutstyrType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FireIndikasjonerObservasjonIndikasjonType",
                columns: table => new
                {
                    IndikasjonstyperId = table.Column<int>(type: "integer", nullable: false),
                    ObservasjonerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireIndikasjonerObservasjonIndikasjonType", x => new { x.IndikasjonstyperId, x.ObservasjonerId });
                    table.ForeignKey(
                        name: "FK_FireIndikasjonerObservasjonIndikasjonType_FireIndikasjonerO~",
                        column: x => x.ObservasjonerId,
                        principalTable: "FireIndikasjonerObservasjon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FireIndikasjonerObservasjonIndikasjonType_IndikasjonType_In~",
                        column: x => x.IndikasjonstyperId,
                        principalTable: "IndikasjonType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HandsmykkeObservasjonHandsmykkeType",
                columns: table => new
                {
                    HandsmykkerId = table.Column<int>(type: "integer", nullable: false),
                    ObservasjonerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandsmykkeObservasjonHandsmykkeType", x => new { x.HandsmykkerId, x.ObservasjonerId });
                    table.ForeignKey(
                        name: "FK_HandsmykkeObservasjonHandsmykkeType_HandsmykkeObservasjon_O~",
                        column: x => x.ObservasjonerId,
                        principalTable: "HandsmykkeObservasjon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HandsmykkeObservasjonHandsmykkeType_HandsmykkeType_Handsmyk~",
                        column: x => x.HandsmykkerId,
                        principalTable: "HandsmykkeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HanskeMedIndikasjonTypeHanskeObservasjon",
                columns: table => new
                {
                    HanskeMedIndikasjonTyperId = table.Column<int>(type: "integer", nullable: false),
                    ObservasjonerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HanskeMedIndikasjonTypeHanskeObservasjon", x => new { x.HanskeMedIndikasjonTyperId, x.ObservasjonerId });
                    table.ForeignKey(
                        name: "FK_HanskeMedIndikasjonTypeHanskeObservasjon_HanskeMedIndikasjo~",
                        column: x => x.HanskeMedIndikasjonTyperId,
                        principalTable: "HanskeMedIndikasjonType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HanskeMedIndikasjonTypeHanskeObservasjon_HanskeObservasjon_~",
                        column: x => x.ObservasjonerId,
                        principalTable: "HanskeObservasjon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HanskeObservasjonHanskeUtenIndikasjonType",
                columns: table => new
                {
                    HanskeUtenIndikasjonTyperId = table.Column<int>(type: "integer", nullable: false),
                    ObservasjonerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HanskeObservasjonHanskeUtenIndikasjonType", x => new { x.HanskeUtenIndikasjonTyperId, x.ObservasjonerId });
                    table.ForeignKey(
                        name: "FK_HanskeObservasjonHanskeUtenIndikasjonType_HanskeObservasjon~",
                        column: x => x.ObservasjonerId,
                        principalTable: "HanskeObservasjon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HanskeObservasjonHanskeUtenIndikasjonType_HanskeUtenIndikas~",
                        column: x => x.HanskeUtenIndikasjonTyperId,
                        principalTable: "HanskeUtenIndikasjonType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeskyttelsesutstyrFeilbrukType",
                columns: table => new
                {
                    BeskyttelsesutstyrId = table.Column<int>(type: "integer", nullable: false),
                    FeilbruktyperId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeskyttelsesutstyrFeilbrukType", x => new { x.BeskyttelsesutstyrId, x.FeilbruktyperId });
                    table.ForeignKey(
                        name: "FK_BeskyttelsesutstyrFeilbrukType_Beskyttelsesutstyr_Beskyttel~",
                        column: x => x.BeskyttelsesutstyrId,
                        principalTable: "Beskyttelsesutstyr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeskyttelsesutstyrFeilbrukType_FeilbrukType_FeilbruktyperId",
                        column: x => x.FeilbruktyperId,
                        principalTable: "FeilbrukType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aktivitet_AktivitetTypeId",
                table: "Aktivitet",
                column: "AktivitetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AktivitetType_Kode",
                table: "AktivitetType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AktivitetType_Navn",
                table: "AktivitetType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_Avdeling_AvdelingtypeId",
                table: "Avdeling",
                column: "AvdelingtypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Avdeling_InstitusjonId",
                table: "Avdeling",
                column: "InstitusjonId");

            migrationBuilder.CreateIndex(
                name: "IX_Avdeling_Navn",
                table: "Avdeling",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_AvdelingKlinikk_KlinikkerId",
                table: "AvdelingKlinikk",
                column: "KlinikkerId");

            migrationBuilder.CreateIndex(
                name: "IX_AvdelingRolle_RollerId",
                table: "AvdelingRolle",
                column: "RollerId");

            migrationBuilder.CreateIndex(
                name: "IX_AvdelingType_Kode",
                table: "AvdelingType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvdelingType_Navn",
                table: "AvdelingType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_Beskyttelsesutstyr_BeskyttelsesutstyrObservasjonId",
                table: "Beskyttelsesutstyr",
                column: "BeskyttelsesutstyrObservasjonId");

            migrationBuilder.CreateIndex(
                name: "IX_Beskyttelsesutstyr_UtstyrstypeId",
                table: "Beskyttelsesutstyr",
                column: "UtstyrstypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrFeilbrukType_FeilbruktyperId",
                table: "BeskyttelsesutstyrFeilbrukType",
                column: "FeilbruktyperId");

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrObservasjon_BeskyttelsesutstyrSesjonId",
                table: "BeskyttelsesutstyrObservasjon",
                column: "BeskyttelsesutstyrSesjonId");

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrObservasjon_Opprettettidspunkt",
                table: "BeskyttelsesutstyrObservasjon",
                column: "Opprettettidspunkt");

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrObservasjon_Registrerttidspunkt",
                table: "BeskyttelsesutstyrObservasjon",
                column: "Registrerttidspunkt");

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrObservasjon_RolleId",
                table: "BeskyttelsesutstyrObservasjon",
                column: "RolleId");

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrObservasjon_SettingtypeId",
                table: "BeskyttelsesutstyrObservasjon",
                column: "SettingtypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrsettingType_Kode",
                table: "BeskyttelsesutstyrsettingType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrsettingType_Navn",
                table: "BeskyttelsesutstyrsettingType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType_Beskytt~",
                table: "BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType",
                column: "BeskyttelsesutstyrsettingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrType_Kode",
                table: "BeskyttelsesutstyrType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BeskyttelsesutstyrType_Navn",
                table: "BeskyttelsesutstyrType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_Bruker_HPRNummer",
                table: "Bruker",
                column: "HPRNummer");

            migrationBuilder.CreateIndex(
                name: "IX_Bruker_IdentPseudonym",
                table: "Bruker",
                column: "IdentPseudonym");

            migrationBuilder.CreateIndex(
                name: "IX_Bruker_InstitusjonId",
                table: "Bruker",
                column: "InstitusjonId");

            migrationBuilder.CreateIndex(
                name: "IX_FeilbrukType_BeskyttelsesutstyrTypeId",
                table: "FeilbrukType",
                column: "BeskyttelsesutstyrTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeilbrukType_Navn",
                table: "FeilbrukType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_FireIndikasjonerObservasjon_AktivitetId",
                table: "FireIndikasjonerObservasjon",
                column: "AktivitetId");

            migrationBuilder.CreateIndex(
                name: "IX_FireIndikasjonerObservasjon_FireIndikasjonerSesjonId",
                table: "FireIndikasjonerObservasjon",
                column: "FireIndikasjonerSesjonId");

            migrationBuilder.CreateIndex(
                name: "IX_FireIndikasjonerObservasjon_Opprettettidspunkt",
                table: "FireIndikasjonerObservasjon",
                column: "Opprettettidspunkt");

            migrationBuilder.CreateIndex(
                name: "IX_FireIndikasjonerObservasjon_Registrerttidspunkt",
                table: "FireIndikasjonerObservasjon",
                column: "Registrerttidspunkt");

            migrationBuilder.CreateIndex(
                name: "IX_FireIndikasjonerObservasjon_RolleId",
                table: "FireIndikasjonerObservasjon",
                column: "RolleId");

            migrationBuilder.CreateIndex(
                name: "IX_FireIndikasjonerObservasjonIndikasjonType_ObservasjonerId",
                table: "FireIndikasjonerObservasjonIndikasjonType",
                column: "ObservasjonerId");

            migrationBuilder.CreateIndex(
                name: "IX_ForesporselOmBrukertilgang_Status",
                table: "ForesporselOmBrukertilgang",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HandhygieneEtterHanskebrukType_Kode",
                table: "HandhygieneEtterHanskebrukType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HandhygieneEtterHanskebrukType_Navn",
                table: "HandhygieneEtterHanskebrukType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_HandsmykkeObservasjon_HandsmykkeSesjonId",
                table: "HandsmykkeObservasjon",
                column: "HandsmykkeSesjonId");

            migrationBuilder.CreateIndex(
                name: "IX_HandsmykkeObservasjon_Opprettettidspunkt",
                table: "HandsmykkeObservasjon",
                column: "Opprettettidspunkt");

            migrationBuilder.CreateIndex(
                name: "IX_HandsmykkeObservasjon_Registrerttidspunkt",
                table: "HandsmykkeObservasjon",
                column: "Registrerttidspunkt");

            migrationBuilder.CreateIndex(
                name: "IX_HandsmykkeObservasjon_RolleId",
                table: "HandsmykkeObservasjon",
                column: "RolleId");

            migrationBuilder.CreateIndex(
                name: "IX_HandsmykkeObservasjonHandsmykkeType_ObservasjonerId",
                table: "HandsmykkeObservasjonHandsmykkeType",
                column: "ObservasjonerId");

            migrationBuilder.CreateIndex(
                name: "IX_HandsmykkeType_Kode",
                table: "HandsmykkeType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HanskeMedIndikasjonType_Kode",
                table: "HanskeMedIndikasjonType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HanskeMedIndikasjonType_Navn",
                table: "HanskeMedIndikasjonType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_HanskeMedIndikasjonTypeHanskeObservasjon_ObservasjonerId",
                table: "HanskeMedIndikasjonTypeHanskeObservasjon",
                column: "ObservasjonerId");

            migrationBuilder.CreateIndex(
                name: "IX_HanskeObservasjon_HandhygieneEtterHanskebrukTypeId",
                table: "HanskeObservasjon",
                column: "HandhygieneEtterHanskebrukTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HanskeObservasjon_HanskeSesjonId",
                table: "HanskeObservasjon",
                column: "HanskeSesjonId");

            migrationBuilder.CreateIndex(
                name: "IX_HanskeObservasjon_Opprettettidspunkt",
                table: "HanskeObservasjon",
                column: "Opprettettidspunkt");

            migrationBuilder.CreateIndex(
                name: "IX_HanskeObservasjon_Registrerttidspunkt",
                table: "HanskeObservasjon",
                column: "Registrerttidspunkt");

            migrationBuilder.CreateIndex(
                name: "IX_HanskeObservasjon_RolleId",
                table: "HanskeObservasjon",
                column: "RolleId");

            migrationBuilder.CreateIndex(
                name: "IX_HanskeObservasjonHanskeUtenIndikasjonType_ObservasjonerId",
                table: "HanskeObservasjonHanskeUtenIndikasjonType",
                column: "ObservasjonerId");

            migrationBuilder.CreateIndex(
                name: "IX_HanskeUtenIndikasjonType_Kode",
                table: "HanskeUtenIndikasjonType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HanskeUtenIndikasjonType_Navn",
                table: "HanskeUtenIndikasjonType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_Helseforetak_RegionaltHelseforetakId",
                table: "Helseforetak",
                column: "RegionaltHelseforetakId");

            migrationBuilder.CreateIndex(
                name: "IX_IndikasjonType_Kode",
                table: "IndikasjonType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndikasjonType_Navn",
                table: "IndikasjonType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_Institusjon_Forkortelse",
                table: "Institusjon",
                column: "Forkortelse");

            migrationBuilder.CreateIndex(
                name: "IX_Institusjon_HelseforetakId",
                table: "Institusjon",
                column: "HelseforetakId");

            migrationBuilder.CreateIndex(
                name: "IX_Institusjon_HERId",
                table: "Institusjon",
                column: "HERId");

            migrationBuilder.CreateIndex(
                name: "IX_Institusjon_InstitusjontypeId",
                table: "Institusjon",
                column: "InstitusjontypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Institusjon_KommuneId",
                table: "Institusjon",
                column: "KommuneId");

            migrationBuilder.CreateIndex(
                name: "IX_Institusjon_Navn",
                table: "Institusjon",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_Institusjon_RegionId",
                table: "Institusjon",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_InstitusjonType_Kode",
                table: "InstitusjonType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstitusjonType_Navn",
                table: "InstitusjonType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_Klinikk_InstitusjonId",
                table: "Klinikk",
                column: "InstitusjonId");

            migrationBuilder.CreateIndex(
                name: "IX_Klinikk_Navn",
                table: "Klinikk",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_OverforingstatusType_Kode",
                table: "OverforingstatusType",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OverforingstatusType_Navn",
                table: "OverforingstatusType",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_PredefinertKommentar_InstitusjonId",
                table: "PredefinertKommentar",
                column: "InstitusjonId");

            migrationBuilder.CreateIndex(
                name: "IX_Region_Kode",
                table: "Region",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Region_Navn",
                table: "Region",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_Rolle_Navn",
                table: "Rolle",
                column: "Navn");

            migrationBuilder.CreateIndex(
                name: "IX_Sesjon_AvdelingId",
                table: "Sesjon",
                column: "AvdelingId");

            migrationBuilder.CreateIndex(
                name: "IX_Sesjon_Discriminator",
                table: "Sesjon",
                column: "Discriminator");

            migrationBuilder.CreateIndex(
                name: "IX_Sesjon_ObservatorId",
                table: "Sesjon",
                column: "ObservatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sesjon_Opprettettidspunkt",
                table: "Sesjon",
                column: "Opprettettidspunkt");

            migrationBuilder.CreateIndex(
                name: "IX_Sesjon_OverforingstatusId",
                table: "Sesjon",
                column: "OverforingstatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Sesjon_Starttidspunkt",
                table: "Sesjon",
                column: "Starttidspunkt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvdelingKlinikk");

            migrationBuilder.DropTable(
                name: "AvdelingRolle");

            migrationBuilder.DropTable(
                name: "BeskyttelsesutstyrFeilbrukType");

            migrationBuilder.DropTable(
                name: "BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType");

            migrationBuilder.DropTable(
                name: "FireIndikasjonerObservasjonIndikasjonType");

            migrationBuilder.DropTable(
                name: "ForesporselOmBrukertilgang");

            migrationBuilder.DropTable(
                name: "HandsmykkeObservasjonHandsmykkeType");

            migrationBuilder.DropTable(
                name: "HanskeMedIndikasjonTypeHanskeObservasjon");

            migrationBuilder.DropTable(
                name: "HanskeObservasjonHanskeUtenIndikasjonType");

            migrationBuilder.DropTable(
                name: "PredefinertKommentar");

            migrationBuilder.DropTable(
                name: "Klinikk");

            migrationBuilder.DropTable(
                name: "Beskyttelsesutstyr");

            migrationBuilder.DropTable(
                name: "FeilbrukType");

            migrationBuilder.DropTable(
                name: "FireIndikasjonerObservasjon");

            migrationBuilder.DropTable(
                name: "IndikasjonType");

            migrationBuilder.DropTable(
                name: "HandsmykkeObservasjon");

            migrationBuilder.DropTable(
                name: "HandsmykkeType");

            migrationBuilder.DropTable(
                name: "HanskeMedIndikasjonType");

            migrationBuilder.DropTable(
                name: "HanskeObservasjon");

            migrationBuilder.DropTable(
                name: "HanskeUtenIndikasjonType");

            migrationBuilder.DropTable(
                name: "BeskyttelsesutstyrObservasjon");

            migrationBuilder.DropTable(
                name: "BeskyttelsesutstyrType");

            migrationBuilder.DropTable(
                name: "Aktivitet");

            migrationBuilder.DropTable(
                name: "HandhygieneEtterHanskebrukType");

            migrationBuilder.DropTable(
                name: "BeskyttelsesutstyrsettingType");

            migrationBuilder.DropTable(
                name: "Rolle");

            migrationBuilder.DropTable(
                name: "Sesjon");

            migrationBuilder.DropTable(
                name: "AktivitetType");

            migrationBuilder.DropTable(
                name: "Avdeling");

            migrationBuilder.DropTable(
                name: "Bruker");

            migrationBuilder.DropTable(
                name: "OverforingstatusType");

            migrationBuilder.DropTable(
                name: "AvdelingType");

            migrationBuilder.DropTable(
                name: "Institusjon");

            migrationBuilder.DropTable(
                name: "Helseforetak");

            migrationBuilder.DropTable(
                name: "InstitusjonType");

            migrationBuilder.DropTable(
                name: "Kommune");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "RegionaltHelseforetak");
        }
    }
}
