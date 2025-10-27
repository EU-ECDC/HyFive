using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Models.V1.Constants;
using HyFive.Services.Session;
using NUnit.Framework;

namespace HyFive.Services.Tests.Session
{
    public class DeleteSessionTests : ServiceTests
    {
        //[Test]
        //public async Task SlettFireIndikasjonerSesjon_SkalFeileHvisOverføringsstatusErFeil()
        //{
        //    // Arrange
        //    var sessionId = Guid.NewGuid();
        //    var sesjon = await CreateFiveIndicatorsSession(sessionId, Guid.NewGuid(), DatabaseContext.Department.First(), Seed.SeedObservatorEmail);
        //    var avdeling = DatabaseContext.Department.First();
        //    // Act
        //    var handler = new DeleteSession.Handler(DatabaseContext);
        //    var slettRequest = new DeleteSession.Command()
        //    {
        //        TransferStatusCode = OverforingstatusTypeKonstanter.TransferredToAdmin,
        //        SessionId = sessionId,
        //        CityId = avdeling.CityId
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
        //    var sessionId = Guid.NewGuid();
        //    var avdeling = DatabaseContext.Department.First();
        //    var sesjon = await CreateFiveIndicatorsSession(sessionId, Guid.NewGuid(), avdeling, Seed.SeedObservatorEmail);

        //    Assert.That(DatabaseContext.Session.FirstOrDefault(s => s.Id == sesjon), Is.Not.Null); 
            
        //    // Act
        //    var handler = new DeleteSession.Handler(DatabaseContext);
        //    var slettRequest = new DeleteSession.Command()
        //    {
        //        TransferStatusCode = OverforingstatusTypeKonstanter.TransferredToCoordinator,
        //        SessionId = sessionId,
        //        CityId = avdeling.CityId
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
