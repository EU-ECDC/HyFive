using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Tjenester.Sesjon;
using NUnit.Framework;

namespace HyFive.Tjenester.Tests.Sesjon
{
    public class SlettSesjonTests : TjenesteTests
    {
        //[Test]
        //public async Task SlettFireIndikasjonerSesjon_SkalFeileHvisOverføringsstatusErFeil()
        //{
        //    // Arrange
        //    var sesjonId = Guid.NewGuid();
        //    var sesjon = await OpprettFireIndikasjonerSesjon(sesjonId, Guid.NewGuid(), DatabaseContext.Avdeling.First(), Seed.SeedObservatorHprNummer);
        //    var avdeling = DatabaseContext.Avdeling.First();
        //    // Act
        //    var handler = new SlettSesjon.Handler(DatabaseContext);
        //    var slettRequest = new SlettSesjon.Command()
        //    {
        //        OverforingstatusKode = OverforingstatusTypeKonstanter.OverfortTilFhi,
        //        SesjonId = sesjonId,
        //        InstitusjonId = avdeling.InstitusjonId
        //    };
            
        //    // Assert
        //    Assert.ThrowsAsync<ArgumentException>(async () =>
        //    {
        //        await handler.Handle(slettRequest, CancellationToken.None);
        //    });
        //}

        //[Test]
        //public async Task SlettFireIndikasjonerSesjon_SkalGåOkHvisOverføringsstatusErRiktig()
        //{
        //    // Arrange
        //    var sesjonId = Guid.NewGuid();
        //    var avdeling = DatabaseContext.Avdeling.First();
        //    var sesjon = await OpprettFireIndikasjonerSesjon(sesjonId, Guid.NewGuid(), avdeling, Seed.SeedObservatorHprNummer);

        //    Assert.That(DatabaseContext.Sesjon.FirstOrDefault(s => s.Id == sesjon), Is.Not.Null); 
            
        //    // Act
        //    var handler = new SlettSesjon.Handler(DatabaseContext);
        //    var slettRequest = new SlettSesjon.Command()
        //    {
        //        OverforingstatusKode = OverforingstatusTypeKonstanter.OverfortTilKoordinator,
        //        SesjonId = sesjonId,
        //        InstitusjonId = avdeling.InstitusjonId
        //    };
        //    var resultat = await handler.Handle(slettRequest, CancellationToken.None);

        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(resultat.Suksess, Is.True);
        //        Assert.That(DatabaseContext.Sesjon.FirstOrDefault(s => s.Id == sesjon), Is.Null);
        //    });
        //}

    }
}
