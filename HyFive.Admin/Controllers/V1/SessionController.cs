using System;
using System.Threading.Tasks;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Session;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Route("api/v1/session")]
    public class SessionController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMediator _mediator;

        public SessionController(IUserService userService, IMediator mediator)
        {
            _userService = userService;
            _mediator = mediator;
        }

        [Route("delete/{sessionId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSession(Guid sesjonId, [FromQuery] int institutionId)
        {
            if (_userService.IsCoordinatorForInstitution(institutionId))
            {
                var result = await _mediator.Send(new DeleteSession.Command
                {
                    InstitutionId = institutionId,
                    TransferStatusCode = TransferStatusTypeConstants.TransferredToCoordinator,
                    SessionId = sesjonId
                });

                return Ok(result.Success);
            }

            return Forbid();
        }

        [Route("update")]
        [HttpPut]
        public async Task<IActionResult> UpdateSession([FromBody] UpdateSessionRequest session)
        {
            if (_userService.IsCoordinatorForInstitution(session.InstitutionId))
            {
                var resultat = await _mediator.Send(new UpdateSession.Command
                {
                    SessionId = session.SessionId,
                    InstitutionId = session.InstitutionId,
                    Comment = session.Comment
                });

                return Ok(resultat.Success);
            }

            return Forbid();
        }
    }
}
