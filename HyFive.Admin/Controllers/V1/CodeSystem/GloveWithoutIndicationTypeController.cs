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
    [Route("api/v1/gloveWithoutIndicationType")]
    public class GloveWithoutIndicationTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GloveWithoutIndicationTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get GloveWithoutIndicationTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<GloveWithoutIndicationType>> GetGloveWithoutIndicationTypes()
        {
            var gloveWithoutIndicationTypes = await _mediator.Send(new GetGloveWithoutIndicationTypes.Query());
            return gloveWithoutIndicationTypes;
        }

        /// <summary>
        /// Update GloveWithoutIndicationType
        /// </summary>
        /// <param name="gloveWithoutIndicationType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("update")]
        public async Task<GloveWithoutIndicationType> UpdateGloveWithoutIndicationType([FromBody] GloveWithoutIndicationType gloveWithoutIndicationType)
        {
            var isUpdated = await _mediator.Send(new UpdateGloveWithoutIndicationType.Command
            {
                GloveWithoutIndicationType = gloveWithoutIndicationType
            });

            return isUpdated;
        }
    }
}
