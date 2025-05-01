using HyFive.Domain.User;
using HyFive.Models.V1;
using HyFive.Models.V1.User;
using HyFive.Models.V1.Institution;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.HealthcareOrganization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/healthcareOrganization")]
    [ApiController]
    public class HealthcareOrganizationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public HealthcareOrganizationController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpGet]
        [ProducesResponseType(typeof(List<HealthcareOrganization>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<HealthcareOrganization>>> GetAllHealthcareOrganizations()
        {
            var allHealthcareOrganizations = await _mediator.Send(new GetAllHealthcareProviders.Query());
            return Ok(allHealthcareOrganizations);
        }

        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPost("create")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CreateHealthcareOrganization([FromBody] CreateHealthcareOrganizationRequest healthcareOrganizationRequest)
        {
            var isCreated = await _mediator.Send(new CreateHealthcareOrganization.Command
            {
                HealthcareOrganization = healthcareOrganizationRequest
            });
            return Ok(isCreated);
        }

        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("update")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> OppdaterHeleforetaket([FromBody] HealthcareOrganization healthcareOrganization)
        {
            var isUpdated = await _mediator.Send(new UpdateHealthcareOrganization.Command
            {
                HealthcareOrganization = healthcareOrganization
            });
            return Ok(isUpdated);
        }

        [HttpGet("{id}/coordinators")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<HealthcareOrganizationCoordinator[]>> GetCoordinatorsForHealthcareOrganization(int id)
        {
            if (_userService.IsCoordinatorForHealthcareProviderOrFhiAdmin(id))
            {
                var coordinatorsWithInstitutionsList = await _mediator.Send(new GetCoordinatorsForHealthcareOrganization.Query
                {
                    HealthcareOrganizationId = id
                });
                return Ok(coordinatorsWithInstitutionsList);
            }

            return Unauthorized();
        }

        [HttpGet("{id}/Institutions")]
        public async Task<ActionResult<InstitutionReport[]>> GetInstitutionsForHealthcareOrganization(int id)
        {
            if (_userService.IsCoordinatorForHealthcareProviderOrFhiAdmin(id))
            {
                return await _mediator.Send(new GetHealthcareOrganization.Query
                {
                    HealthcareOrganizationId = id
                });
            }

            return Unauthorized();
        }


        [HttpPut("{id}/updateCoordinator")]
        [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> OppdaterKoordinator([FromBody] HealthcareOrganizationCoordinator coordinator, int id)
        {
            if (_userService.IsCoordinatorForHealthcareProviderOrFhiAdmin(id))
            {
                var updatedStatus = await _mediator.Send(new UpdateCoordinatorForHealthcareOrganization.Command
                {
                    Coordinator = coordinator,
                    HealthcareOrganizationId = id
                });
                return Ok(updatedStatus);
            }

            return Unauthorized();
        }

        [HttpPost("{id}/createCoordinator")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> CreateCoordinator([FromBody] HealthcareOrganizationCoordinator coordinator, int id)
        {
            if (_userService.IsCoordinatorForHealthcareProviderOrFhiAdmin(id))
            {
                var createdStatus = await _mediator.Send(new CreateCoordinatorForHealthcareOrganization.Command
                {
                    Coordinator = coordinator,
                    HealthcareOrganizationId = id
                });
                return Ok(createdStatus);
            }

            return Unauthorized();
        }
    }
}
