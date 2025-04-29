using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.Observation;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Institution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using HyFive.Services.Roles;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/role")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Get all roles in the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<Role>), StatusCodes.Status201Created)]
        public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles()
        {
            var result = await _mediator.Send(new GetRoles.Query() {});
            return Ok(result);
        }

        /// <summary>
        /// Create role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPost()]
        [ProducesResponseType(typeof(Role), StatusCodes.Status201Created)]
        public async Task<ActionResult<Role>> CreateRole([FromBody] CreateRoleRequest request)
        {
            var result = await _mediator.Send(new OpprettRolle.Command() { Request = request });
            return result;
        }

        /// <summary>
        /// Update role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut()]
        public async Task<Role> UpdateRole([FromBody] UpdateRoleRequest request)
        {
            var result = await _mediator.Send(new OppdaterRolle.Command() { Request = request });
            return result;
        }
    }
}
