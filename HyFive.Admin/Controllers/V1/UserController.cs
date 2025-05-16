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
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
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
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("observer/update")]
        public async Task<IActionResult> UpdateObserver([FromBody] User user)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(user.InstitutionId))
            {
                var updatedObserver = await _mediator.Send(new UpdateObserver.Command() { User = user });
                return Ok(updatedObserver);
            }

            return Unauthorized();
        }


        /// <summary>
        ///Creating an observer.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("observer/create")]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        public async Task<ActionResult<User>> CreateObserver([FromBody] User user)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(user.InstitutionId))
            {
                var response = await _mediator.Send(new CreateObserver.Command() { User = user });
                return CreatedAtRoute("GetObservers", new { id = response.InstitutionId }, response);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Deleting an observer.
        /// </summary>
        /// <param name="observerId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpDelete("observer/delete")]
        public async Task<ActionResult<bool>> DeleteObserver([FromQuery] int observerId)
        {
            if (_userService.IsFhiAdmin())
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

        [Route("observer/HasTransferredSessionToFHI")]
        [HttpGet]
        public async Task<IActionResult> HasTransferredSessionToFHI([FromQuery] int observatorId)
        {
            var result = await _mediator.Send(new HasTransferredSessionToFHI.Command
            {
                ObservationId = observatorId
            });

            return Ok(result);
        }

        #endregion

        #region Coordinator

        /// <summary>
        /// Create Coordinator.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("coordinator/create")]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        public async Task<ActionResult<User>> CreateCoordinator([FromBody] User user)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(user.InstitutionId))
            {
                var response = await _mediator.Send(new CreateCoordinator.Command() { USer = user });
                return CreatedAtRoute("GetCoordinators", new { id = response.InstitutionId }, response);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Updating a Coordinator.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("coordinator/update")]
        public async Task<ActionResult<User>> UpdateCoordinator([FromBody] User user)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(user.InstitutionId))
            {
                var updatedUser = await _mediator.Send(new UpdateCoordinator.Command() { User = user });
                return updatedUser;
            }

            return Unauthorized();
        }


        /// <summary>
        /// deleting a coordinator.
        /// </summary>
        /// <param name="coordinatorId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpDelete("coordinator/delete")]
        public async Task<ActionResult<bool>> DeleteCoordinator([FromQuery] int coordinatorId)
        {
            if (_userService.IsFhiAdmin())
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

        #region FhiAdmin

        /// <summary>
        /// Getting all FhiAdmins.
        /// </summary>
        /// <returns></returns>
        [HttpGet("fhiadmin")]
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> GetFhiAdmin()
        {
            try
            {
                var response = await _mediator.Send(new GetFhiAdmin.Query() { });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Creating an FhiAdmin.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("fhiadmin")]
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        public async Task<ActionResult<User>> CreateFhiAdmin([FromBody] CreateFhiAdminRequest request)
        {
            try
            {
                var response = await _mediator.Send(new CreateFhiAdmin.Command() { Request = request });
                return Ok(response);
                //return CreatedAtRoute("GetFhiAdmin", new { id = response.HealthcareOrganizationId }, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Updating a FhiAdmin.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("fhiadmin")]
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        public async Task<ActionResult<User>> UpdateFhiAdmin([FromBody] User user)
        {
            try
            {
                var response = await _mediator.Send(new UpdateFhiAdmin.Command() { User = user });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion
    }
}
