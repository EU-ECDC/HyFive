using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Observasjon;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Avdeling;
using HyFive.Tjenester.FireIndikasjoner;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Tjenester.Rapporter.FireIndikasjoner;
using Moq;
using Microsoft.Extensions.Logging;

namespace HyFive.Tjenester.Tests.Rapporter
{
    public class RapporterTests : TjenesteTests
    {
        //[Test]
        //public async Task LagAvdelingsrapportTest()
        //{
        //    // Arrange
        //    var logger = new Mock<ILogger<LagreSesjon.Handler>>();
        //    var rolleSomTestes = "Lege";
        //    var hentAvdelingHandler =
        //        new HentAvdelingerForInstitusjon.Handler(DatabaseContext, Mapper);
        //    var enAvdeling = (await hentAvdelingHandler.Handle(new HentAvdelingerForInstitusjon.Query(){InstitutionId = 1}, CancellationToken.None)).First();

        //    var hentAktivitettyperHandler =
        //        new HentAktivitetTyper.Handler(DatabaseContext, Mapper);
        //    var aktivitettyper =
        //        await hentAktivitettyperHandler.Handle(new HentAktivitetTyper.Query(), CancellationToken.None);

        //    var hentIndikasjonstyperHandler =
        //        new HentIndikasjonstyper.Handler(DatabaseContext, Mapper);
        //    var indikasjonstyper =
        //        await hentIndikasjonstyperHandler.Handle(new HentIndikasjonstyper.Query(), CancellationToken.None);

        //    var lagreSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);

        //    var etterlevdObservasjonKombinasjonA = new FourIndicationsObservation()
        //    {
        //        Activity = new Activity()
        //        {
        //            ActivityType = aktivitettyper.First(a => a.Code == AktivitetTypeKonstanter.Desinfeksjon),
        //            GloveUsed = false,
        //            TimeSpent = 4,
        //            TimeRecordingWasDone = false
        //        },
        //        IndicationTypes = new List<IndicationTypes>()
        //        {
        //            indikasjonstyper.First(i => i.Code == IndikasjonTypeKonstanter.FoerPasient),
        //            indikasjonstyper.First(i => i.Code == IndikasjonTypeKonstanter.AseptiskeProsedyrer)
        //        },
        //        RegistrationTime = DateTime.Now,
        //        Role = enAvdeling.Role.First(r => r.Name == rolleSomTestes),
        //    };
            
        //    var ikkeEtterlevdObservasjonKombinasjonA = new FourIndicationsObservation()
        //    {
        //        Activity = new Activity()
        //        {
        //            ActivityType = aktivitettyper.First(a => a.Code == AktivitetTypeKonstanter.IkkeUtfort),
        //            GloveUsed = false,
        //            TimeSpent = 4,
        //            TimeRecordingWasDone = false
        //        },
        //        IndicationTypes = new List<IndicationTypes>()
        //        {
        //            indikasjonstyper.First(i => i.Code == IndikasjonTypeKonstanter.FoerPasient),
        //            indikasjonstyper.First(i => i.Code == IndikasjonTypeKonstanter.AseptiskeProsedyrer)
        //        },
        //        RegistrationTime = DateTime.Now,
        //        Role = enAvdeling.Role.First(r => r.Name == rolleSomTestes),
        //    };


        //    var lagreSesjonQuery = new LagreSesjon.Command()
        //    {
        //        HPRNumber = Seed.SeedObservatorHprNummer,
        //        Session = new FourIndicationsSession()
        //        {
        //            Department = enAvdeling,
        //            StartTime = DateTime.Now,
        //            Observations = new List<FourIndicationsObservation>()
        //            {
        //                etterlevdObservasjonKombinasjonA,
        //                etterlevdObservasjonKombinasjonA, 
        //                ikkeEtterlevdObservasjonKombinasjonA
        //            }
        //        }
        //    };


        //    // Act
        //    await lagreSesjonHandler.Handle(lagreSesjonQuery, CancellationToken.None);
            
        //    var rapportHandler = new HentFireIndikasjonerRapportForAvdeling.Handler(DatabaseContext);
        //    var lagRapportQuery = new HentFireIndikasjonerRapportForAvdeling.Query()
        //    {
        //        AvdelingId = enAvdeling.Id, 
        //        FraTidspunkt = DateTime.Now.AddDays(-1),
        //        TilTidspunkt = DateTime.Now.AddDays(1),
        //        Role = AuthorizedRole.Coordinator,
        //    };
        //    var rapport = await rapportHandler.Handle(lagRapportQuery, new System.Threading.CancellationToken());

        //    var kombinasjonArapport = rapport
        //        .Department
        //        .Role
        //        .First(r => r.Name == rolleSomTestes)
        //        .Kombinasjoner
        //        .First(k => k.Name == "A");
            
        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(kombinasjonArapport.NumberOfObservations, Is.EqualTo(3));
        //        Assert.That(Math.Round(kombinasjonArapport.ProsentIkkeEtterlevd), Is.EqualTo(33));
        //        Assert.That(Math.Round(kombinasjonArapport.ProsentEtterlevd), Is.EqualTo(67));
        //    });
        //}
    }
}