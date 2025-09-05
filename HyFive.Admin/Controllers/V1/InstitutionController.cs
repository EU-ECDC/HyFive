using HyFive.Models.V1.User;
using HyFive.Models.V1.Institution;
using HyFive.Services.Institution;
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
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/institution")]
    public class InstitutionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public InstitutionController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Get all available institutions <see cref="InstitutionReport"/>
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IEnumerable<InstitutionReport>> GetInstitutions()
        {
            if (_userService.IsFhiAdmin())
            {
                return await _mediator.Send(new GetInstitutions.Query());
            }
            return await _mediator.Send(new GetInstitutionsForCoordinator.Query() {  CoordinatorEmail = _userService.GetEmail() });
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpGet("getInstitutionsForCoordinator")]
        public async Task<IEnumerable<InstitutionReport>> GetInstitutionsForCoordinator()
        {
            return await _mediator.Send(new GetInstitutionsForCoordinator.Query() {CoordinatorEmail = _userService.GetEmail() });
        }


        /// <summary>
        /// Get institution
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetInstitution")]
        public async Task<IActionResult> GetInstitution(int id)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(id))
            {
                var result = await _mediator.Send(new GetInstitution.Query() { InstitutionId = id });
                return Ok(result);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Get compliance institutions
        /// </summary>
        /// <param name="ids">The list of institution IDs to fetch.</param>
        /// <returns>Returns a list of matching institutions if authorized; otherwise Unauthorized.</returns>
        [HttpGet("getComplianceInstitutions")]
        public async Task<IActionResult> GetComplianceInstitutions([FromQuery] List<int> institutionIds)
        {
            if (_userService.IsCoordinatorForInstitutionsOrAdmin(institutionIds))
            {
                var result = await _mediator.Send(new GetComplianceInstitution.Query() { InstitutionIds = institutionIds });
                return Ok(result);
            }
            return Unauthorized();
        }



        /// <summary>
        /// Get all departments belonging to the institution
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/departments", Name = "GetDepartments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments(int id)
        {
            if (!UserIsAuthorized(id))
                return Unauthorized();
            
                var result = await _mediator.Send(new GetDepartmentsForInstitution.Query() { InstitutionId = id });
                return Ok(result);
            
        }


        /// <summary>
        /// Get all observers belonging to the institution
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/observers", Name = "GetObservers")]
        public async Task<ActionResult<IEnumerable<User>>> GetObservators(int id)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(id))
            {
                var result = await _mediator.Send(new GetObserversForInstitution.Query() { InstitutionId = id });
                return Ok(result);
            }

            return Unauthorized();
        }



        /// <summary>
        /// Get all coordinators belonging to the institution
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/coordinators", Name = "GetCoordinators")]
        public async Task<ActionResult<IEnumerable<User>>> GetCoordinators(int id)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(id))
            {
                var result = await _mediator.Send(new GetCoordinatorsForInstitution.Query() { InstitutionId = id });
                return Ok(result);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Get Institution Types  <see cref="InstitutionType"/>r
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpGet("types")]
        public async Task<IEnumerable<InstitutionType>> GetInstitutionTypes()
        {
            var result = await _mediator.Send(new GetInstitutionTypes.Query());
            return result;
        }

        /// <summary>
        /// Create institution with coordinator.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPost("create")]
        [ProducesResponseType(typeof(Institution), StatusCodes.Status201Created)]
        public async Task<ActionResult<Institution>> CreateInstitution([FromBody] CreateInstitutionRequest request)
        {
            var result = await _mediator.Send(new CreateInstitution.Command() { Request = request });
            return CreatedAtRoute("GetInstitution", new { id = result.Id }, result);
        }

        /// <summary>
        /// Updating an institution. Ignoring any departments or roles that are sent with it.
        /// </summary>
        /// <param name="institution"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("update")]
        public async Task<Institution> UpdateInstitution([FromBody] Institution institution)
        {
            var result = await _mediator.Send(new UpdateInstitution.Command() { Institution = institution });
            return result;
        }

        /// <summary>
        /// Delete institution and all associated sessions, observations, departments, roles, and users
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpDelete("delete")]
        public async Task<bool> DeleteInstitution([FromQuery] int institutionId)
        {
            var result = await _mediator.Send(new DeleteInstitution.Command()
            {
                InstitutionId = institutionId
            });
            return result;
        }

        private bool UserIsAuthorized(int institutionId)
        {
            if (_userService.IsFhiAdmin())
                return true;

            if (_userService.IsCoordinatorForInstitution(institutionId))
                return true;

            return false;
        }
    }
}
