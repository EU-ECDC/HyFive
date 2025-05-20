using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Api.Common.ExtensionMethods;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.Gloves;
using HyFive.Models.V1.Report.Glove;
using HyFive.Models.V1.Session;
using HyFive.Services;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Glove;
using HyFive.Services.Rapport.Observations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Observation.Controllers.V1
{
    [Route("api/v1/glove")]
    [ApiController]
    public class GloveController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public GloveController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Save a Glove session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> SaveSession([FromBody] GloveSession session)
        {
            if (!session.Observations.Any())
            {
                return BadRequest("The session must contain at least one observation");
            }

            if (_userService.IsObserverForInstitution(session.Department.InstitutionId))
            {
                var result = await _mediator.Send(new SaveSession.Command
                {
                    HPRNumber = _userService.GetHprNumber(),
                    Pseudonym = _userService.GetPseudonym(),
                    Session = session
                });

                return CreatedAtRoute("GetGloveSession", new { sessionId = session.Id }, result);
            }

            return Unauthorized();
        }

        [HttpGet("gloveWithIndicationType")]
        public async Task<IEnumerable<GloveWithIndicationType>> GetGloveWithIndicationTypes()
        {
            var result = await _mediator.Send(new GetGloveWithIndicationTypes.Query());
            return result;
        }

        [HttpGet("gloveWithoutIndicationType")]
        public async Task<IEnumerable<GloveWithoutIndicationType>> GetGloveWithoutIndicationTypes()
        {
            var result = await _mediator.Send(new GetGloveWithoutIndicationTypes.Query());
            return result;
        }

        [HttpGet("handHygieneAfterGloveUseType")]
        public async Task<IEnumerable<PostGloveHandHygieneType>> GetHandHygieneAfterGloveUseTypes()
        {
            var result = await _mediator.Send(new GetHandHygieneAfterGloveUseTypes.Query());
            return result;
        }

        [HttpGet("myObservations")]
        public async Task<IEnumerable<GloveObservationReport>> GetMyObservations(int institutionId, Guid? sessionId = null)
        {
            var observerIdForInstitution = _userService.GetObserverIdForInstitution(institutionId);
            if (observerIdForInstitution > 0)
            {
                var query = new GetGloveObservations.Query()
                {
                    ObserverId = observerIdForInstitution,
                    InstitutionId = institutionId,
                    SessionId = sessionId,
                    Role = AuthorizedRole.Observer
                };

                var observations = await _mediator.Send(query);
                return observations;
            }

            throw new UnauthorizedAccessException("You do not have access to inquire about the observations for this institution");
        }

        [HttpGet("myObservations/excel")]
        public async Task<IActionResult> GetMyObservationsAsExcel(int institutionId, Guid? sessionId = null)
        {
            var observations = await GetMyObservations(institutionId, sessionId);
            return await this.ExcelFileContentResult(observations, "Observations");
        }
    }
}