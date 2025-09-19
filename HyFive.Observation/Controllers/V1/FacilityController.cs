using HyFive.Models.V1.Facility;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Facility;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Observation.Controllers.V1
{
    // [Authorize(HandhygienePolicy.Observer)]
    [Route("api/v1/facility")]
    public class FacilityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public FacilityController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Get the user's available facilities
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IEnumerable<Facility>> GetObserverFacilities()
        {
            var result = await _mediator.Send(new GetFacilitiesForObserver.Query() { Email = _userService.GetEmail()});
            return result;
        }

        /// <summary>
        /// Get predefined comments for given facility and session type
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="sessionType"></param>
        /// <returns></returns>
        [HttpGet("predefinedComments")]
        public async Task<ActionResult<IEnumerable<string>>> GetPredefinedComments([FromQuery] int facilityId, [FromQuery] SessionType sessionType)
        {
            if (_userService.IsObserverForFacility(facilityId))
            {
                var result = await _mediator.Send(new GetPredefinedComments.Query
                {
                    FacilityId = facilityId,
                    SessionType = sessionType
                });
                return Ok(result);
            }

            return Unauthorized();
        }
    }
}
