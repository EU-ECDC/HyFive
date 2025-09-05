using HyFive.Models.V1.Institution;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Institution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Observation.Controllers.V1
{
    // [Authorize(HandhygienePolicy.Observer)]
    [Route("api/v1/institution")]
    public class InstitutionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public InstitutionController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Get the user's available institutions
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IEnumerable<Institution>> GetObserverInstitutions()
        {
            var result = await _mediator.Send(new GetInstitutionsForObserver.Query() { Email = _userService.GetEmail()});
            return result;
        }

        /// <summary>
        /// Get predefined comments for given institution and session type
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="sessionType"></param>
        /// <returns></returns>
        [HttpGet("predefinedComments")]
        public async Task<ActionResult<IEnumerable<string>>> GetPredefinedComments([FromQuery] int institutionId, [FromQuery] SessionType sessionType)
        {
            if (_userService.IsObserverForInstitution(institutionId))
            {
                var result = await _mediator.Send(new GetPredefinedComments.Query
                {
                    InstitutionId = institutionId,
                    SessionType = sessionType
                });
                return Ok(result);
            }

            return Unauthorized();
        }
    }
}
