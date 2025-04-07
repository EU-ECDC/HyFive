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
        //    var enAvdeling = (await hentAvdelingHandler.Handle(new HentAvdelingerForInstitusjon.Query(){InstitusjonId = 1}, CancellationToken.None)).First();

        //    var hentAktivitettyperHandler =
        //        new HentAktivitetTyper.Handler(DatabaseContext, Mapper);
        //    var aktivitettyper =
        //        await hentAktivitettyperHandler.Handle(new HentAktivitetTyper.Query(), CancellationToken.None);

        //    var hentIndikasjonstyperHandler =
        //        new HentIndikasjonstyper.Handler(DatabaseContext, Mapper);
        //    var indikasjonstyper =
        //        await hentIndikasjonstyperHandler.Handle(new HentIndikasjonstyper.Query(), CancellationToken.None);

        //    var lagreSesjonHandler = new LagreSesjon.Handler(DatabaseContext, Mapper, logger.Object, BrukerService);

        //    var etterlevdObservasjonKombinasjonA = new FireIndikasjonerObservasjon()
        //    {
        //        Aktivitet = new Aktivitet()
        //        {
        //            AktivitetType = aktivitettyper.First(a => a.Kode == AktivitetTypeKonstanter.Desinfeksjon),
        //            BenyttetHanske = false,
        //            SekunderBrukt = 4,
        //            TidtakingBleUtfort = false
        //        },
        //        Indikasjonstyper = new List<IndikasjonType>()
        //        {
        //            indikasjonstyper.First(i => i.Kode == IndikasjonTypeKonstanter.FoerPasient),
        //            indikasjonstyper.First(i => i.Kode == IndikasjonTypeKonstanter.AseptiskeProsedyrer)
        //        },
        //        Registrerttidspunkt = DateTime.UtcNow,
        //        Rolle = enAvdeling.Roller.First(r => r.Navn == rolleSomTestes),
        //    };
            
        //    var ikkeEtterlevdObservasjonKombinasjonA = new FireIndikasjonerObservasjon()
        //    {
        //        Aktivitet = new Aktivitet()
        //        {
        //            AktivitetType = aktivitettyper.First(a => a.Kode == AktivitetTypeKonstanter.IkkeUtfort),
        //            BenyttetHanske = false,
        //            SekunderBrukt = 4,
        //            TidtakingBleUtfort = false
        //        },
        //        Indikasjonstyper = new List<IndikasjonType>()
        //        {
        //            indikasjonstyper.First(i => i.Kode == IndikasjonTypeKonstanter.FoerPasient),
        //            indikasjonstyper.First(i => i.Kode == IndikasjonTypeKonstanter.AseptiskeProsedyrer)
        //        },
        //        Registrerttidspunkt = DateTime.UtcNow,
        //        Rolle = enAvdeling.Roller.First(r => r.Navn == rolleSomTestes),
        //    };


        //    var lagreSesjonQuery = new LagreSesjon.Command()
        //    {
        //        HPRNummer = Seed.SeedObservatorHprNummer,
        //        Sesjon = new FireIndikasjonerSesjon()
        //        {
        //            Avdeling = enAvdeling,
        //            Starttidspunkt = DateTime.UtcNow,
        //            Observasjoner = new List<FireIndikasjonerObservasjon>()
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
        //        FraTidspunkt = DateTime.UtcNow.AddDays(-1),
        //        TilTidspunkt = DateTime.UtcNow.AddDays(1),
        //        Rolle = AuthorizedRole.Koordinator,
        //    };
        //    var rapport = await rapportHandler.Handle(lagRapportQuery, new System.Threading.CancellationToken());

        //    var kombinasjonArapport = rapport
        //        .Avdeling
        //        .Roller
        //        .First(r => r.Navn == rolleSomTestes)
        //        .Kombinasjoner
        //        .First(k => k.Navn == "A");
            
        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(kombinasjonArapport.AntallObservasjoner, Is.EqualTo(3));
        //        Assert.That(Math.Round(kombinasjonArapport.ProsentIkkeEtterlevd), Is.EqualTo(33));
        //        Assert.That(Math.Round(kombinasjonArapport.ProsentEtterlevd), Is.EqualTo(67));
        //    });
        //}
    }
}