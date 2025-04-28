using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.Observation.Gloves;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Glove;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/handHygieneAfterGloveUseTypes")]
    public class HandHygieneAfterGloveUseTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HandHygieneAfterGloveUseTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get HandHygieneAfterGloveUseTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<PostGloveHandHygieneType>> GetHandHygieneAfterGloveUseTypes()
        {
            var handHygieneAfterGloveUseTypes = await _mediator.Send(new GetHandHygieneAfterGloveUseTypes.Query());
            return handHygieneAfterGloveUseTypes;
        }

        /// <summary>
        /// Update  PostGloveHandHygiene
        /// </summary>
        /// <param name="handHygieneAfterGloveUseType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("update ")]
        public async Task<PostGloveHandHygieneType> UpdateHandHygieneAfterGloveUseType([FromBody] PostGloveHandHygieneType handHygieneAfterGloveUseType)
        {
            var IsUpdated = await _mediator.Send(new UpdateHandHygieneAfterGloveUseType.Command
            {
                HandHygieneAfterGloveUseType = handHygieneAfterGloveUseType
            });

            return IsUpdated;
        }
    }
}
