using System.Collections.Generic;
using HyFive.Models.V1.Facility;
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
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
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
        /// <param name="facilityId"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Clinic), StatusCodes.Status200OK)]
        public async Task<ActionResult<Clinic>> GetClinic(int id, int facilityId)
        {
            if (_userService.IsCoordinatorForFacilityOrFhiAdmin(facilityId))
            {
                return await _mediator.Send(new GetClinic.Query() { Id = id, FacilityId = facilityId });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Get Clinics For Facility
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        [HttpGet("facility/{facilityId}")]
        [ProducesResponseType(typeof(IEnumerable<Clinic>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Clinic>>> GetClinicsForFacility(int facilityId)
        {
            if (_userService.IsCoordinatorForFacilityOrFhiAdmin(facilityId))
            {
                var clinics = await _mediator.Send(new GetClinicsForFacility.Query() { FacilityId = facilityId });
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
            if (_userService.IsCoordinatorForFacilityOrFhiAdmin(clinic.FacilityId))
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
            if (_userService.IsCoordinatorForFacilityOrFhiAdmin(clinic.FacilityId))
            {
                return await _mediator.Send(new UpdateClinic.Command() { Clinic = clinic });
            }

            return Unauthorized();
        }
    }
}
