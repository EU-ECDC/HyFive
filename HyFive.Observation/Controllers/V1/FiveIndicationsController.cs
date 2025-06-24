using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.FiveIndication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Api.Common.ExtensionMethods;
using HyFive.Models.V1.Report.FiveIndications;
using HyFive.Services;
using HyFive.Services.Rapport.Observations;

namespace HyFive.Observation.Controllers.V1
{

    [Route("api/v1/fiveIndications")]
    public class FiveIndicationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public FiveIndicationsController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Save a Five Indications session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> SaveSession([FromBody] FiveIndicationsSession session)
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

                return CreatedAtRoute("GetFiveIndicationsSession", new { sessionId = session.Id }, result);
            }

            return Unauthorized();
        }

        [HttpGet("indicationTypes")]
        public async Task<IEnumerable<IndicationType>> GetIndicationTypes()
        {
            var result = await _mediator.Send(new GetIndicationTypes.Query());
            return result;
        }

        [HttpGet("activityTypes")]
        public async Task<IEnumerable<ActivityType>> GetActivityTypes()
        {
            var result = await _mediator.Send(new GetActivityTypes.Query());
            return result;
        }

        [HttpGet("myObservations")]
        public async Task<IEnumerable<FiveIndicationsObservationReport>> GetMyObservations(int institutionId, Guid? sessionId = null)
        {
            var observerIdForInstitution = _userService.GetObserverIdForInstitution(institutionId);
            if (observerIdForInstitution > 0)
            {
                var query = new GetFiveIndicationsObservations.Query()
                {
                    ObserverId = observerIdForInstitution,
                    InstitutionId = institutionId,
                    SessionId = sessionId,
                    Role = AuthorizedRole.Coordinator
                };
                var observations = await _mediator.Send(query);
                return observations;
            }
            throw new UnauthorizedAccessException("You do not have access to inquire about the observations of this institution");

        }

        [HttpGet("myObservations/excel")]
        public async Task<IActionResult> GetMyObservationsAsExcel(int institutionId, Guid? sessionId = null)
        {
            var observations = await GetMyObservations(institutionId, sessionId);
            return await this.ExcelFileContentResult(observations, "Observations");
        }

        
    }
}
