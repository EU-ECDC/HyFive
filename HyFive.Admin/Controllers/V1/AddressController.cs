using HyFive.Models.V1.OrganisationUnit;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Authentication.User;
using HyFive.Services.City;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/addresses")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public AddressController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        [Authorize(HandhygienePolicy.Admin)]
        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CreateAddress([FromBody] CreateAddressRequest request)
        {
            var isCreated = await _mediator.Send(new CreateAddress.Command
            {
                Address = request
            });
            return Ok(isCreated);
        }

        [Authorize(HandhygienePolicy.Admin)]
        [HttpPut("update")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> UpdateAddress([FromBody] UpdateAddressRequest request, int id)
        {
            var isUpdated = await _mediator.Send(new UpdateAddress.Command
            {
                Id = id,
                Address = request
            });
            return Ok(isUpdated);
        }
    }
}
