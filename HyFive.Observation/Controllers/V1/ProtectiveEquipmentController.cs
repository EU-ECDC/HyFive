using HyFive.Api.Common.ExtensionMethods;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Models.V1.Report.Beskyttelsesutstyr;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.ProtectiveEquipment;
using HyFive.Services.Report.Observations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Services;

namespace HyFive.Observation.Controllers.V1
{

    [Route("api/v1/protectiveEquipment")]
    public class ProtectiveEquipmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public ProtectiveEquipmentController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<IEnumerable<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType>> GetProtectiveEquipmentSettingTypes()
        {
            return await _mediator.Send(new GetProtectiveEquipmentSettingTypes.Query());
        }

        /// <summary>
        /// Save a ProtectiveEquipment session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> SaveSession([FromBody] ProtectiveEquipmentSession session)
        {
            if (!session.Observations.Any())
            {
                return BadRequest("The session must have at least one observation");
            }

            if (_userService.IsObserverForFacility(session.Department.FacilityId))
            {
                var result = await _mediator.Send(new SaveSession.Command()
                {
                    Email = _userService.GetEmail(),
                    Session = session
                });

                return CreatedAtRoute("GetProtectiveEquipmentSession", new { sessionId = session.Id }, result);
            }

            return Unauthorized();
        }

        [HttpGet("myObservations.")]
        public async Task<IEnumerable<PPEObservationReport>> GetMyObservations(int facilityId, Guid? sessionId = null)
        {
            var observerIdForFacility = _userService.GetObserverIdForFacility(facilityId);
            if (observerIdForFacility > 0)
            {
                var query = new GetProtectiveEquipmentObservations.Query()
                {
                    ObserverId = observerIdForFacility,
                    FacilityId = facilityId,
                    SessionId = sessionId,
                    Role = AuthorizedRole.Coordinator
                };
                var observations = await _mediator.Send(query);
                return observations;
            }
            throw new UnauthorizedAccessException("You do not have access to inquire about the observations of this facility.");

        }

        [HttpGet("myObservations/excel")]
        public async Task<IActionResult> GetMyObservationsAsExcel(int facilityId, Guid? sessionId = null)
        {
            var observations = await GetMyObservations(facilityId, sessionId);
            return await this.ExcelFileContentResult(observations, "Observations");
        }
    }
}
