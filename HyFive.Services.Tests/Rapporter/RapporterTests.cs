using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Session;
using HyFive.Services.Department;
using HyFive.Services.FiveIndication;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Services.Reports.FiveIndicators;
using Moq;
using Microsoft.Extensions.Logging;

namespace HyFive.Services.Tests.Rapporter
{
    public class RapporterTests : ServiceTests
    {
        //[Test]
        //public async Task LagAvdelingsrapportTest()
        //{
        //    // Arrange
        //    var logger = new Mock<ILogger<SaveSession.Handler>>();
        //    var rolleSomTestes = "Lege";
        //    var hentAvdelingHandler =
        //        new GetDepartmentsForInstitution.Handler(DatabaseContext, Mapper);
        //    var enAvdeling = (await hentAvdelingHandler.Handle(new GetDepartmentsForInstitution.Query(){HealthcareOrganizationId = 1}, CancellationToken.None)).First();

        //    var hentAktivitettyperHandler =
        //        new GetActivityTypes.Handler(DatabaseContext, Mapper);
        //    var aktivitettyper =
        //        await hentAktivitettyperHandler.Handle(new GetActivityTypes.Query(), CancellationToken.None);

        //    var hentIndikasjonstyperHandler =
        //        new GetIndicationTypes.Handler(DatabaseContext, Mapper);
        //    var indikasjonstyper =
        //        await hentIndikasjonstyperHandler.Handle(new GetIndicationTypes.Query(), CancellationToken.None);

        //    var lagreSesjonHandler = new SaveSession.Handler(DatabaseContext, Mapper, logger.Object, UserService);

        //    var etterlevdObservasjonKombinasjonA = new FiveIndicationsObservation()
        //    {
        //        Activity = new Activity()
        //        {
        //            ActivityType = aktivitettyper.First(a => a.Code == AktivitetTypeKonstanter.Disinfection),
        //            GloveUsed = false,
        //            SecondsUsed = 4,
        //            TimingWasPerformed = false
        //        },
        //        IndicationTypes = new List<IndicationTypes>()
        //        {
        //            indikasjonstyper.First(i => i.Code == IndikasjonTypeKonstanter.PrePatient),
        //            indikasjonstyper.First(i => i.Code == IndikasjonTypeKonstanter.AsepticProcedures)
        //        },
        //        RegisteredTime = DateTime.UtcNow,
        //        Roles = enAvdeling.Roles.First(r => r.Name == rolleSomTestes),
        //    };
            
        //    var ikkeEtterlevdObservasjonKombinasjonA = new FiveIndicationsObservation()
        //    {
        //        Activity = new Activity()
        //        {
        //            ActivityType = aktivitettyper.First(a => a.Code == AktivitetTypeKonstanter.NotCompleted),
        //            GloveUsed = false,
        //            SecondsUsed = 4,
        //            TimingWasPerformed = false
        //        },
        //        IndicationTypes = new List<IndicationTypes>()
        //        {
        //            indikasjonstyper.First(i => i.Code == IndikasjonTypeKonstanter.PrePatient),
        //            indikasjonstyper.First(i => i.Code == IndikasjonTypeKonstanter.AsepticProcedures)
        //        },
        //        RegisteredTime = DateTime.UtcNow,
        //        Roles = enAvdeling.Roles.First(r => r.Name == rolleSomTestes),
        //    };


        //    var lagreSesjonQuery = new SaveSession.Command()
        //    {
        //        HPRNumber = Seed.SeedObservatorHprNummer,
        //        Session = new FiveIndicationsSession()
        //        {
        //            Department = enAvdeling,
        //            StartDate = DateTime.UtcNow,
        //            Observations = new List<FiveIndicationsObservation>()
        //            {
        //                etterlevdObservasjonKombinasjonA,
        //                etterlevdObservasjonKombinasjonA, 
        //                ikkeEtterlevdObservasjonKombinasjonA
        //            }
        //        }
        //    };


        //    // Act
        //    await lagreSesjonHandler.Handle(lagreSesjonQuery, CancellationToken.None);
            
        //    var rapportHandler = new GetFiveIndicatorsReportForDepartment.Handler(DatabaseContext);
        //    var lagRapportQuery = new GetFiveIndicatorsReportForDepartment.Query()
        //    {
        //        DepartmentId = enAvdeling.Id, 
        //        FromDate = DateTime.UtcNow.AddDays(-1),
        //        ToTime = DateTime.UtcNow.AddDays(1),
        //        Roles = AuthorizedRole.Coordinator,
        //    };
        //    var rapport = await rapportHandler.Handle(lagRapportQuery, new System.Threading.CancellationToken());

        //    var kombinasjonArapport = rapport
        //        .Department
        //        .Roles
        //        .First(r => r.Name == rolleSomTestes)
        //        .Combinations
        //        .First(k => k.Name == "A");
            
        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(kombinasjonArapport.NumberOfObservations, Is.EqualTo(3));
        //        Assert.That(Math.Round(kombinasjonArapport.PercentNotComplied), Is.EqualTo(33));
        //        Assert.That(Math.Round(kombinasjonArapport.PercentComplied), Is.EqualTo(67));
        //    });
        //}
    }
}