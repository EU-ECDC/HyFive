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
        [ProducesResponseType(typeof(List<Models.V1.OrganisationUnit.City>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Models.V1.OrganisationUnit.City>>> GetAllCities()
        {
            var allCities = await _mediator.Send(new GetAllCities.Query());
            return Ok(allCities);
        }        

        /// <summary>
        /// Retrieves the coordinators for the specified city.
        /// </summary>
        /// <param name="cityId">The identifier of the city. Must be greater than zero.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the coordinators:
        /// - 200 OK with an array of <see cref="CityCoordinator"/> when successful.
        /// - 400 Bad Request when <paramref name="cityId"/> is invalid.
        /// - 401 Unauthorized when the caller is not authorized to access coordinators for the city.
        /// </returns>
        [HttpGet("{cityId}/coordinators")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<CityCoordinator[]>> GetCoordinatorsForCity(int cityId)
        {
            if (cityId > 0)
                return BadRequest("City is required.");

            if (!await _userService.IsCoordinatorForCityOrAdmin(cityId))
                return Unauthorized();

            var coordinators = await _mediator.Send(new GetCoordinatorsForCity.Query
            {
                CityId = cityId
            });

            return Ok(coordinators);
        }

        /// <summary>
        /// Retrieves the facilities report for the specified city.
        /// </summary>
        /// <param name="cityId">The identifier of the city. Must be greater than zero.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the facility reports:
        /// - 200 OK with an array of <see cref="FacilityReport"/> when successful.
        /// - 400 Bad Request when <paramref name="cityId"/> is invalid.
        /// - 401 Unauthorized when the caller is not authorized to access facilities for the city.
        /// </returns>
        [HttpGet("{cityId}/facilities")]
        [ProducesResponseType(typeof(FacilityReport[]), StatusCodes.Status200OK)]
        public async Task<ActionResult<FacilityReport[]>> GetFacilitiesForCity(int cityId)
        {
            if (cityId > 0)
                return BadRequest("City is required.");

            if (!await _userService.IsCoordinatorForCityOrAdmin(cityId))
                return Unauthorized();

            var facilities = await _mediator.Send(new GetCity.Query
            {
                CityId = cityId
            });

            return Ok(facilities);
        }


        /// <summary>
        /// Updates a coordinator for the specified city.
        /// </summary>
        /// <param name="coordinator">The coordinator details from the request body.</param>
        /// <param name="cityId">The identifier of the city. Must be greater than zero.</param>
        /// <returns>
        /// - 200 OK with <see cref="Status"/> when update succeeds.
        /// - 400 Bad Request when inputs are invalid.
        /// - 401 Unauthorized when the caller is not authorized to update coordinators for the city.
        /// </returns>
        [HttpPut("{cityId}/updateCoordinator")]
        [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> UpdateCoordinator([FromBody] CityCoordinator coordinator, [FromRoute] int cityId)
        {
            if (cityId > 0)
                return BadRequest("City is required.");

            if (coordinator == null)
                return BadRequest("Coordinator is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _userService.IsCoordinatorForCityOrAdmin(cityId))
                return Unauthorized();

            var updatedStatus = await _mediator.Send(new UpdateCoordinatorForCity.Command
            {
                Coordinator = coordinator,
                CityId = cityId
            });

            return Ok(updatedStatus);
        }

        /// <summary>
        /// Creates a coordinator for the specified city.
        /// </summary>
        /// <param name="coordinator">The coordinator details from the request body.</param>
        /// <param name="cityId">The identifier of the city. Must be greater than zero.</param>
        /// <returns>
        /// - 200 OK with a <see cref="Status"/> when creation succeeds.
        /// - 400 Bad Request when inputs are invalid.
        /// - 401 Unauthorized when the caller is not authorized to create coordinators for the city.
        /// </returns>
        [HttpPost("{cityId}/createCoordinator")]
        [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> CreateCoordinator([FromBody] CityCoordinator coordinator, [FromRoute] int cityId)
        {
            if (cityId > 0)
                return BadRequest("City is required.");

            if (!await _userService.IsCoordinatorForCityOrAdmin(cityId))
                return Unauthorized();

            var createdStatus = await _mediator.Send(new CreateCoordinatorForCity.Command
            {
                Coordinator = coordinator,
                CityId = cityId
            });

            return Ok(createdStatus);
        }
    }
}
