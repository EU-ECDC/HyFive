using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.UserAccessRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAccessRequest = HyFive.Models.V1.UserAccessRequest.UserAccessRequest;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.Coordinator)]
    [Route("api/v1/userAccessRequest")]
    public class UserAccessRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public UserAccessRequestController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Getting all requests for user access.
        /// </summary>
        /// <returns></returns>
        [HttpGet("allRequests")]
        [Authorize(HandhygienePolicy.Coordinator)]
        [ProducesResponseType(typeof(List<UserAccessRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserAccessRequest>> GetAllRequests([FromQuery] int institutionId)
        {
            try
            {
                var user = await _userService.GetUser();
                var response = await _mediator.Send(new GetAllRequests.Query
                {
                    InstitutionId = institutionId
                });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Getting requests that are not approved.
        /// </summary>
        /// <returns></returns>
        [HttpGet("pendingApprovalRequests")]
        [Authorize(HandhygienePolicy.Coordinator)]
        [ProducesResponseType(typeof(List<UserAccessRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserAccessRequest>> GetPendingApprovalRequests([FromQuery] int institutionId)
        {
            try
            {
                var response = await _mediator.Send(new GetPendingApprovalRequests.Query()
                {
                    InstitutionId = institutionId
                });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("approveRequest")]
        [Authorize(HandhygienePolicy.Coordinator)]
        public async Task<ActionResult<bool>> ApproveRequest([FromQuery] int requestId)
        {
            try
            {
                var user = await _userService.GetUser();
                
                var response = await _mediator.Send(new CreateUserRequest.Command()
                {
                    RequestId = requestId,
                    Email = user.Email
                });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("rejectRequest")]
        [Authorize(HandhygienePolicy.Coordinator)]
        public async Task<ActionResult<bool>> RejectRequest([FromQuery] int requestId)
        {
            try
            {
                var user = await _userService.GetUser();

                var response = await _mediator.Send(new RejectRequest.Command()
                {
                    RequestId = requestId,
                    Email = user.Email
                });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}