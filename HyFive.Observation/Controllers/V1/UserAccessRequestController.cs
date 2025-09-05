using HyFive.Models.V1.UserAccessRequest;
using HyFive.Services.Authentication.User;
using HyFive.Services.UserAccessRequest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Observation.Controllers.V1
{
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

        [HttpGet("institutions")]
        [ProducesResponseType(typeof(List<InstitutionForUserAccessRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionForUserAccessRequest>>> GetInstitutions()
        {
            try
            {
                var result = await _mediator.Send(new GetInstitutions.Query());
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("send")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        public async Task<ActionResult<bool>> SendUserAccessRequest([FromBody] Models.V1.UserAccessRequest.CreateUserAccessRequest userAccessRequest)
        {
            try
            {
                var result = await _mediator.Send(new Services.UserAccessRequest.CreateUserAccessRequest.Command
                {
                    UserAccessRequest = userAccessRequest
                });

                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserAccessRequest>> GetAlreadySentUserAccessRequest()
        {
            try
            {
                var user = await _userService.GetUser();
                var request = await _mediator.Send(new GetAlreadySentRequest.Query
                {
                    Email = user.Email
                });

                return Ok(request);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("institution")]
        [ProducesResponseType(typeof(List<InstitutionForUserAccessRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionForUserAccessRequest>>> GetInstitution(int institutionId)
        {
            try
            {
                var result = await _mediator.Send(new GetInstitution.Query()
                {
                    InstitutionId = institutionId
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
