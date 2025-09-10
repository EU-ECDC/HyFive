using HyFive.Models.V1.Observation;
using HyFive.Services.FiveIndication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.Requirements;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/activityType")]
    public class ActivityTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActivityTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all available activity types <see cref="ActivityType"/>
        /// </summary>
        /// <returns></returns>

        [Authorize(HandhygienePolicy.AdminOrCoordinator)]
        [HttpGet]
        public async Task<IEnumerable<ActivityType>> GetAllActivityTypes()
            => await _mediator.Send(new GetActivityTypes.Query());

        /// <summary>
        /// Updating an activity type
        /// </summary>
        /// <param name="activityType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpPut("update")]
        public async Task<ActivityType> UpdateActivityType([FromBody] ActivityType activityType)
        {
            var result = await _mediator.Send(new UpdateActivityType.Command() { ActivityType = activityType });
            return result;
        }
    }
}
