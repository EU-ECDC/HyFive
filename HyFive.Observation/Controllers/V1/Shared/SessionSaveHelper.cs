using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HyFive.Observation.Controllers.V1.Shared
{
    public static class SessionSaveHelper
    {
        public static async Task<ActionResult<Guid>> SaveSessionAsync<TCommand>(
        ControllerBase controller,
        ISessionWithObservations session,
        string routeName,
        IMediator mediator,
        IUserService userService,
        IStringLocalizer validation,
        Func<ISessionWithObservations, string, TCommand> commandFactory)
        where TCommand : IRequest<Guid>
        {
            if (session.Observations == null || !session.Observations.Any())
            {
                return controller.BadRequest(validation["SessionMustHaveObservation"]);
            }

            // Validate facility access
            if (!userService.IsObserverForFacility(session.Department.FacilityId))
                return controller.Unauthorized();

            // Build the correct SaveSession.Command (FiveIndication, HandJewelry, ProtectiveEquipment...)
            var command = commandFactory(session, userService.GetEmail());

            // Send the command
            var result = await mediator.Send(command);

            return controller.CreatedAtRoute(routeName, new { sessionId = session.Id }, result);
        }
    }
}
