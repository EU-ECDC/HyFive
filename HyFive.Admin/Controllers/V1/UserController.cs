using System;
using System.Threading.Tasks;
using HyFive.Domain.User;
using HyFive.Models.V1.User;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.User;
using HyFive.Services.UserServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using User = HyFive.Models.V1.User.User;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public UserController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        #region Observer

        /// <summary>
        /// Updating an observer.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("observer/update")]
        public async Task<IActionResult> UpdateObserver([FromBody] CreateUpdateUserRequest request)
        {
            if (await _userService.IsCoordinatorForFacilityOrAdmin(request.FacilityId))
            {
                var updatedObserver = await _mediator.Send(new UpdateObserver.Command() { Request = request });
                return Ok(updatedObserver);
            }

            return Unauthorized();
        }


        /// <summary>
        ///Creating an observer.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("observer/create")]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        public async Task<ActionResult<User>> CreateObserver([FromBody] CreateUpdateUserRequest request)
        {
            if (await _userService.IsCoordinatorForFacilityOrAdmin(request.FacilityId))
            {
                var response = await _mediator.Send(new CreateObserver.Command() { Request = request });
                return CreatedAtRoute("GetObservers", new { id = request.FacilityId }, response);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Deleting an observer.
        /// </summary>
        /// <param name="observerId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpDelete("observer/delete")]
        public async Task<ActionResult<bool>> DeleteObserver([FromQuery] int observerId)
        {
            if (await _userService.IsAdmin())
            {
                var result = await _mediator.Send(new DeleteUser.Command()
                {
                    UserId = observerId,
                    UserType = typeof(Observer)
                });
                return Ok(result);
            }

            return Unauthorized();
        }

        [Route("observer/HasTransferredSessionToAdmin")]
        [HttpGet]
        public async Task<IActionResult> HasTransferredSessionToAdmin([FromQuery] int observatorId)
        {
            var result = await _mediator.Send(new HasTransferredSessionToAdmin.Command
            {
                ObserverId = observatorId
            });

            return Ok(result);
        }

        #endregion

        #region Coordinator

        /// <summary>
        /// Create Coordinator.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("coordinator/create")]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        public async Task<ActionResult<User>> CreateCoordinator([FromBody] CreateUpdateUserRequest request)
        {
            if (await _userService.IsCoordinatorForFacilityOrAdmin(request.FacilityId))
            {
                var response = await _mediator.Send(new CreateCoordinator.Command() { Request = request });
                return CreatedAtRoute("GetCoordinators", new { id = request.FacilityId }, response);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Updating a Coordinator.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("coordinator/update")]
        public async Task<ActionResult<User>> UpdateCoordinator([FromBody] CreateUpdateUserRequest request)
        {
            if (await _userService.IsCoordinatorForFacilityOrAdmin(request.FacilityId))
            {
                var updatedUser = await _mediator.Send(new UpdateCoordinator.Command() { Request = request });
                return updatedUser;
            }

            return Unauthorized();
        }


        /// <summary>
        /// deleting a coordinator.
        /// </summary>
        /// <param name="coordinatorId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpDelete("coordinator/delete")]
        public async Task<ActionResult<bool>> DeleteCoordinator([FromQuery] int coordinatorId)
        {
            if (await _userService.IsAdmin())
            {
                var result = await _mediator.Send(new DeleteUser.Command()
                {
                    UserId = coordinatorId,
                    UserType = typeof(Coordinator)
                });
                return result;
            }

            return Unauthorized();
        }

        #endregion

        #region Admin

        /// <summary>
        /// Getting all FhiAdmins.
        /// </summary>
        /// <returns></returns>
        [HttpGet("admin")]
        [Authorize(HandhygienePolicy.Admin)]
        [ProducesResponseType(typeof(User[]), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> GetAdmin()
        {
            var response = await _mediator.Send(new GetAdmin.Query() { });
            return Ok(response);
            
        }

        /// <summary>
        /// Creating an Administrator.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("admin")]
        [Authorize(HandhygienePolicy.Admin)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            var response = await _mediator.Send(new CreateAdmin.Command() { Request = request });
            return Ok(response);            
        }

        /// <summary>
        /// Updating a Administrator.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("admin")]
        [Authorize(HandhygienePolicy.Admin)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> UpdateAdmin([FromBody] User user)
        {
            var response = await _mediator.Send(new UpdateAdmin.Command() { User = user });
            return Ok(response);            
        }

        #endregion
    }
}
