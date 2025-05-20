using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Api.Common.ExtensionMethods;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Report.HandJewelry;
using HyFive.Models.V1.Session;
using HyFive.Services;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HyFive.Services.HandJewelry;
using HyFive.Services.Rapport.Observations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HyFive.Observation.Controllers.V1
{
    [Route("api/v1/handJewelry")]
    public class HandJewelryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public HandJewelryController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Save a Hand Jewelry session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> SaveSession([FromBody] HandJewelrySession session)
        {
            if (!session.Observations.Any())
            {
                return BadRequest("The session must have at least one observation");
            }
            if (_userService.IsObserverForInstitution(session.Department.InstitutionId))
            {
                var result = await _mediator.Send(new SaveSession.Command()
                {
                    HprNumber = _userService.GetHprNumber(),
                    Pseudonym = _userService.GetPseudonym(),
                    Session = session
                });

                return CreatedAtRoute("GetHandJewelrySession", new { sessionId = session.Id }, result);
            }

            return Unauthorized();
        }

        [HttpGet("getHandJewelryTypes")]
        public async Task<IEnumerable<HandJewelryType>> GetHandJewelryTypes()
        {
            var result = await _mediator.Send(new GetHandJewelryTypes.Query());
            return result;
        }

        [HttpGet("myObservations")]
        public async Task<IEnumerable<HandJewelryObservationReport>> GetMyObservations(int institutionId, Guid? sessionId = null)
        {
            var observerIdForInstitution = _userService.GetObserverIdForInstitution(institutionId);
            if (observerIdForInstitution > 0)
            {
                var query = new GetHandJewelryObservations.Query
                {
                    ObserverId = observerIdForInstitution,
                    InstitutionId = institutionId,
                    SessionId = sessionId,
                    Role = AuthorizedRole.Observer
                };

                var observations = await _mediator.Send(query);
                return observations;
            }

            throw new UnauthorizedAccessException("You do not have access to query the observations for this institution");
        }

        [HttpGet("myObservations/excel")]
        public async Task<IActionResult> GetMyObservationsAsExcel(int institutionId, Guid? sessionId = null)
        {
            var observations = await GetMyObservations(institutionId, sessionId);
            return await this.ExcelFileContentResult(observations, "Observations");
        }
    }
}
