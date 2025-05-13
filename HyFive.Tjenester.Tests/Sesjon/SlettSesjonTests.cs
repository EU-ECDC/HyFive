using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Models.V1.Constants;
using HyFive.Services.Session;
using NUnit.Framework;

namespace HyFive.Services.Tests.Sesjon
{
    public class SlettSesjonTests : ServiceTests
    {
        //[Test]
        //public async Task SlettFireIndikasjonerSesjon_SkalFeileHvisOverføringsstatusErFeil()
        //{
        //    // Arrange
        //    var sesjonId = Guid.NewGuid();
        //    var sesjon = await CreateFourIndicatorsSession(sesjonId, Guid.NewGuid(), DatabaseContext.Department.First(), Seed.SeedObservatorHprNummer);
        //    var avdeling = DatabaseContext.Department.First();
        //    // Act
        //    var handler = new DeleteSession.Handler(DatabaseContext);
        //    var slettRequest = new DeleteSession.Command()
        //    {
        //        TransferStatusCode = OverforingstatusTypeKonstanter.TransferredToFhi,
        //        SessionId = sesjonId,
        //        HealthcareOrganizationId = avdeling.HealthcareOrganizationId
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
        //    var sesjon = await CreateFourIndicatorsSession(sesjonId, Guid.NewGuid(), avdeling, Seed.SeedObservatorHprNummer);

        //    Assert.That(DatabaseContext.Session.FirstOrDefault(s => s.Id == sesjon), Is.Not.Null); 
            
        //    // Act
        //    var handler = new DeleteSession.Handler(DatabaseContext);
        //    var slettRequest = new DeleteSession.Command()
        //    {
        //        TransferStatusCode = OverforingstatusTypeKonstanter.TransferredToCoordinator,
        //        SessionId = sesjonId,
        //        HealthcareOrganizationId = avdeling.HealthcareOrganizationId
        //    };
        //    var resultat = await handler.Handle(slettRequest, CancellationToken.None);

        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(resultat.Success, Is.True);
        //        Assert.That(DatabaseContext.Session.FirstOrDefault(s => s.Id == sesjon), Is.Null);
        //    });
        //}

    }
}
