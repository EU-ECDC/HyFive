using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Constants;
using HyFive.Services.Sesjon;
using NUnit.Framework;

namespace HyFive.Services.Tests.Sesjon
{
    public class SlettSesjonTests : TjenesteTests
    {
        //[Test]
        //public async Task SlettFireIndikasjonerSesjon_SkalFeileHvisOverføringsstatusErFeil()
        //{
        //    // Arrange
        //    var sesjonId = Guid.NewGuid();
        //    var sesjon = await OpprettFireIndikasjonerSesjon(sesjonId, Guid.NewGuid(), DatabaseContext.Department.First(), Seed.SeedObservatorHprNummer);
        //    var avdeling = DatabaseContext.Department.First();
        //    // Act
        //    var handler = new SlettSesjon.Handler(DatabaseContext);
        //    var slettRequest = new SlettSesjon.Command()
        //    {
        //        OverforingstatusKode = OverforingstatusTypeKonstanter.TransferredToFhi,
        //        SessionId = sesjonId,
        //        InstitutionId = avdeling.InstitutionId
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
        //    var avdeling = DatabaseContext.Department.First();
        //    var sesjon = await OpprettFireIndikasjonerSesjon(sesjonId, Guid.NewGuid(), avdeling, Seed.SeedObservatorHprNummer);

        //    Assert.That(DatabaseContext.Session.FirstOrDefault(s => s.Id == sesjon), Is.Not.Null); 
            
        //    // Act
        //    var handler = new SlettSesjon.Handler(DatabaseContext);
        //    var slettRequest = new SlettSesjon.Command()
        //    {
        //        OverforingstatusKode = OverforingstatusTypeKonstanter.TransferredToCoordinator,
        //        SessionId = sesjonId,
        //        InstitutionId = avdeling.InstitutionId
        //    };
        //    var resultat = await handler.Handle(slettRequest, CancellationToken.None);

        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(resultat.Suksess, Is.True);
        //        Assert.That(DatabaseContext.Session.FirstOrDefault(s => s.Id == sesjon), Is.Null);
        //    });
        //}

    }
}
