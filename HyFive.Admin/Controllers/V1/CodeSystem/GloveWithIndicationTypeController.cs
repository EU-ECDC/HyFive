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
    [Route("api/v1/gloveWithIndicationType")]
    public class GloveWithIndicationTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GloveWithIndicationTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get IndicatedGloveTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<GloveWithIndicationType>> GetGloveWithIndicationTypes()
        {
            var gloveForIndicationTypes = await _mediator.Send(new GetGloveWithIndicationTypes.Query());
            return gloveForIndicationTypes;
        }

        /// <summary>
        /// Update GloveForIndicationType
        /// </summary>
        /// <param name="gloveWithIndicationType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("update")]
        public async Task<GloveWithIndicationType> UpdateGloveWithIndicationType([FromBody] GloveWithIndicationType gloveWithIndicationType)
        {
            var isUpdated = await _mediator.Send(new UpdateGloveWithIndicationType.Command
            {
                GloveWithIndicationType = gloveWithIndicationType
            });

            return isUpdated;
        }
    }
}
