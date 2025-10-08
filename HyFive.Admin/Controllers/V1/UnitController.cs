using System.Collections.Generic;
using HyFive.Models.V1.Facility;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HyFive.Services.Unit;
//using HyFive.Domain.Place;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/unit")]
    public class UnitController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public UnitController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Get Unit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Models.V1.Facility.Unit), StatusCodes.Status200OK)]
        public async Task<ActionResult<Models.V1.Facility.Unit>> GetUnit(int id, int facilityId)
        {
            if (_userService.IsCoordinatorForFacilityOrAdmin(facilityId))
            {
                return await _mediator.Send(new GetUnit.Query() { Id = id, FacilityId = facilityId });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Get Units For Facility
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        [HttpGet("facility/{facilityId}")]
        [ProducesResponseType(typeof(IEnumerable<Models.V1.Facility.Unit>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Models.V1.Facility.Unit>>> GetUnitsForFacility(int facilityId)
        {
            if (_userService.IsCoordinatorForFacilityOrAdmin(facilityId))
            {
                var units = await _mediator.Send(new GetUnitsForFacility.Query() { FacilityId = facilityId });
                return Ok(units);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Create unit
        /// </summary>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(Models.V1.Facility.Unit), StatusCodes.Status201Created)]
        public async Task<ActionResult<Models.V1.Facility.Unit>> CreateUnit([FromBody] Models.V1.Facility.Unit unit)
        {
            if (_userService.IsCoordinatorForFacilityOrAdmin(unit.FacilityId))
            {
                return await _mediator.Send(new CreateUnit.Command() { Unit = unit });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Update unit
        /// </summary>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(typeof(Models.V1.Facility.Unit), StatusCodes.Status201Created)]
        public async Task<ActionResult<Models.V1.Facility.Unit>> UpdateUnit([FromBody] Models.V1.Facility.Unit unit)
        {
            if (_userService.IsCoordinatorForFacilityOrAdmin(unit.FacilityId))
            {
                return await _mediator.Send(new UpdateUnit.Command() { Unit = unit });
            }

            return Unauthorized();
        }
    }
}
