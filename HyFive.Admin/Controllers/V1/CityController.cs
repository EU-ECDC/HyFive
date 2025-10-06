using HyFive.Domain.User;
using HyFive.Models.V1;
using HyFive.Models.V1.User;
using HyFive.Models.V1.Facility;
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
    [Route("api/v1/city")]
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
        [ProducesResponseType(typeof(List<City>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<City>>> GetAllCities()
        {
            var allCities = await _mediator.Send(new GetAllCities.Query());
            return Ok(allCities);
        }

        [Authorize(HandhygienePolicy.Admin)]
        [HttpPost("create")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CreateCity([FromBody] CreateCityRequest cityRequest)
        {
            var isCreated = await _mediator.Send(new CreateCity.Command
            {
                City = cityRequest
            });
            return Ok(isCreated);
        }

        [Authorize(HandhygienePolicy.Admin)]
        [HttpPut("update")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> UpdateCity([FromBody] City city)
        {
            var isUpdated = await _mediator.Send(new UpdateCity.Command
            {
                City = city
            });
            return Ok(isUpdated);
        }

        [HttpGet("{id}/coordinators")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<CityCoordinator[]>> GetCoordinatorsForCity(int id)
        {
            if (_userService.IsCoordinatorForCityOrAdmin(id))
            {
                var coordinatorsWithFacilitiesList = await _mediator.Send(new GetCoordinatorsForCity.Query
                {
                    CityId = id
                });
                return Ok(coordinatorsWithFacilitiesList);
            }

            return Unauthorized();
        }

        [HttpGet("{id}/Facilities")]
        public async Task<ActionResult<FacilityReport[]>> GetFacilitiesForCity(int id)
        {
            if (_userService.IsCoordinatorForCityOrAdmin(id))
            {
                return await _mediator.Send(new GetCity.Query
                {
                    CityId = id
                });
            }

            return Unauthorized();
        }


        [HttpPut("{id}/updateCoordinator")]
        [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> UpdateCoordinator([FromBody] CityCoordinator coordinator, int id)
        {
            if (_userService.IsCoordinatorForCityOrAdmin(id))
            {
                var updatedStatus = await _mediator.Send(new UpdateCoordinatorForCity.Command
                {
                    Coordinator = coordinator,
                    CityId = id
                });
                return Ok(updatedStatus);
            }

            return Unauthorized();
        }

        [HttpPost("{id}/createCoordinator")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> CreateCoordinator([FromBody] CityCoordinator coordinator, int id)
        {
            if (_userService.IsCoordinatorForCityOrAdmin(id))
            {
                var createdStatus = await _mediator.Send(new CreateCoordinatorForCity.Command
                {
                    Coordinator = coordinator,
                    CityId = id
                });
                return Ok(createdStatus);
            }

            return Unauthorized();
        }
    }
}
