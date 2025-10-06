using HyFive.Models.V1.User;
using HyFive.Models.V1.Facility;
using HyFive.Services.Facility;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/facility")]
    public class FacilityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public FacilityController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Get all available facilities <see cref="FacilityReport"/>
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IEnumerable<FacilityReport>> GetFacilities()
        {
            if (_userService.IsAdmin())
            {
                return await _mediator.Send(new GetFacilities.Query());
            }
            return await _mediator.Send(new GetFacilitiesForCoordinator.Query() {  CoordinatorEmail = _userService.GetEmail() });
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpGet("getFacilitiesForCoordinator")]
        public async Task<IEnumerable<FacilityReport>> GetFacilitiesForCoordinator()
        {
            return await _mediator.Send(new GetFacilitiesForCoordinator.Query() {CoordinatorEmail = _userService.GetEmail() });
        }


        /// <summary>
        /// Get facility
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetFacility")]
        public async Task<IActionResult> GetFacility(int id)
        {
            if (_userService.IsCoordinatorForFacilityOrAdmin(id))
            {
                var result = await _mediator.Send(new GetFacility.Query() { FacilityId = id });
                return Ok(result);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Get compliance facilities
        /// </summary>
        /// <param name="ids">The list of facility IDs to fetch.</param>
        /// <returns>Returns a list of matching facilities if authorized; otherwise Unauthorized.</returns>
        [HttpGet("getComplianceFacilities")]
        public async Task<IActionResult> GetComplianceFacilities([FromQuery] List<int> facilityIds)
        {
            if (_userService.IsCoordinatorForFacilitiesOrAdmin(facilityIds))
            {
                var result = await _mediator.Send(new GetComplianceFacilities.Query() { FacilityIds = facilityIds });
                return Ok(result);
            }
            return Unauthorized();
        }



        /// <summary>
        /// Get all departments belonging to the facility
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/departments", Name = "GetDepartments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments(int id)
        {
            if (!UserIsAuthorized(id))
                return Unauthorized();
            
                var result = await _mediator.Send(new GetDepartmentsForFacility.Query() { FacilityId = id });
                return Ok(result);
            
        }


        /// <summary>
        /// Get all observers belonging to the facility
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/observers", Name = "GetObservers")]
        public async Task<ActionResult<IEnumerable<User>>> GetObservators(int id)
        {
            if (_userService.IsCoordinatorForFacilityOrAdmin(id))
            {
                var result = await _mediator.Send(new GetObserversForFacility.Query() { FacilityId = id });
                return Ok(result);
            }

            return Unauthorized();
        }



        /// <summary>
        /// Get all coordinators belonging to the facility
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/coordinators", Name = "GetCoordinators")]
        public async Task<ActionResult<IEnumerable<User>>> GetCoordinators(int id)
        {
            if (_userService.IsCoordinatorForFacilityOrAdmin(id))
            {
                var result = await _mediator.Send(new GetCoordinatorsForFacility.Query() { FacilityId = id });
                return Ok(result);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Get Facility Types  <see cref="FacilityType"/>r
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpGet("types")]
        public async Task<IEnumerable<FacilityType>> GetFacilityTypes()
        {
            var result = await _mediator.Send(new GetFacilityTypes.Query());
            return result;
        }

        /// <summary>
        /// Create facility with coordinator.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpPost("create")]
        [ProducesResponseType(typeof(Facility), StatusCodes.Status201Created)]
        public async Task<ActionResult<Facility>> CreateFacility([FromBody] CreateFacilityRequest request)
        {
            var result = await _mediator.Send(new CreateFacility.Command() { Request = request });
            return CreatedAtRoute("GetFacility", new { id = result.Id }, result);
        }

        /// <summary>
        /// Updating an facility. Ignoring any departments or roles that are sent with it.
        /// </summary>
        /// <param name="facility"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpPut("update")]
        public async Task<Facility> UpdateFacility([FromBody] Facility facility)
        {
            var result = await _mediator.Send(new UpdateFacility.Command() { Facility = facility });
            return result;
        }

        /// <summary>
        /// Delete facility and all associated sessions, observations, departments, roles, and users
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpDelete("delete")]
        public async Task<bool> DeleteFacility([FromQuery] int facilityId)
        {
            var result = await _mediator.Send(new DeleteFacility.Command()
            {
                FacilityId = facilityId
            });
            return result;
        }

        private bool UserIsAuthorized(int facilityId)
        {
            if (_userService.IsAdmin())
                return true;

            if (_userService.IsCoordinatorForFacility(facilityId))
                return true;

            return false;
        }
    }
}
