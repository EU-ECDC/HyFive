using System.Collections.Generic;
using HyFive.Models.V1.Institution;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HyFive.Services.Clinic;
//using HyFive.Domain.Place;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/clinic")]
    public class ClinicController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public ClinicController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Get Clinic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Clinic), StatusCodes.Status200OK)]
        public async Task<ActionResult<Clinic>> GetClinic(int id, int institutionId)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(institutionId))
            {
                return await _mediator.Send(new GetClinic.Query() { Id = id, InstitutionId = institutionId });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Get Clinics For Institution
        /// </summary>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        [HttpGet("institution/{institutionId}")]
        [ProducesResponseType(typeof(IEnumerable<Clinic>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Clinic>>> GetClinicsForInstitution(int institutionId)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(institutionId))
            {
                var clinics = await _mediator.Send(new GetClinicsForInstitution.Query() { InstitutionId = institutionId });
                return Ok(clinics);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Create clinic
        /// </summary>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(Clinic), StatusCodes.Status201Created)]
        public async Task<ActionResult<Clinic>> CreateClinic([FromBody] Clinic clinic)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(clinic.InstitutionId))
            {
                return await _mediator.Send(new CreateClinic.Command() { Clinic = clinic });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Update clinic
        /// </summary>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(typeof(Clinic), StatusCodes.Status201Created)]
        public async Task<ActionResult<Clinic>> UpdateClinic([FromBody] Clinic clinic)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(clinic.InstitutionId))
            {
                return await _mediator.Send(new UpdateClinic.Command() { Clinic = clinic });
            }

            return Unauthorized();
        }
    }
}
