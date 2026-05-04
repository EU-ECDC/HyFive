using HyFive.Domain.User;
using HyFive.Models.V1;
using HyFive.Models.V1.User;
using HyFive.Models.V1.OrganisationUnit;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.City;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/cities")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public CityController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        [Authorize(HandhygienePolicy.Admin)]
        [HttpGet]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<string>>> GetAllCities()
        {
            var allCities = await _mediator.Send(new GetAllCities.Query());
            return Ok(allCities);
        }        

        [HttpGet("{city}/coordinators")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<CityCoordinator[]>> GetCoordinatorsForCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City is required.");

            if (!await _userService.IsCoordinatorForCityOrAdmin(city))
                return Unauthorized();

            var coordinators = await _mediator.Send(new GetCoordinatorsForCity.Query
            {
                City = city
            });

            return Ok(coordinators);
        }

        [HttpGet("{city}/Facilities")]
        public async Task<ActionResult<FacilityReport[]>> GetFacilitiesForCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City is required.");

            if (!await _userService.IsCoordinatorForCityOrAdmin(city))
                return Unauthorized();

            var facilities = await _mediator.Send(new GetCity.Query
            {
                City = city
            });

            return Ok(facilities);
        }


        [HttpPut("{city}/updateCoordinator")]
        [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> UpdateCoordinator([FromBody] CityCoordinator coordinator, string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City is required.");

            if (!await _userService.IsCoordinatorForCityOrAdmin(city))
                return Unauthorized();

            var updatedStatus = await _mediator.Send(new UpdateCoordinatorForCity.Command
            {
                Coordinator = coordinator,
                City = city
            });

            return Ok(updatedStatus);
        }

        [HttpPost("{city}/createCoordinator")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> CreateCoordinator([FromBody] CityCoordinator coordinator, string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("City is required.");

            if (!await _userService.IsCoordinatorForCityOrAdmin(city))
                return Unauthorized();

            var createdStatus = await _mediator.Send(new CreateCoordinatorForCity.Command
            {
                Coordinator = coordinator,
                City = city
            });

            return Ok(createdStatus);
        }
    }
}
