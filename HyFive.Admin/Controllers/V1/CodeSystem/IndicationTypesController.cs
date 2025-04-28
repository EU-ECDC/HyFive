using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.FourIndication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IndicationType = HyFive.Models.V1.Observation.IndicationType;

namespace HyFive.Admin.Controllers.V1
{
    /// <summary>
    /// IndicationTypes
    /// </summary>
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/indicationTypes")]
    public class IndicationTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IndicationTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get IndicationTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<IndicationType>> GetIndicationTypes()
        {
            var indicationTypes = await _mediator.Send(new GetIndicationTypes.Query());
            return indicationTypes;
        }

        /// <summary>
        /// Update IndicationTypes
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("update")]
        public async Task<IndicationType> UpdateIndicationType([FromBody] IndicationType indicationType)
        {
            return await _mediator.Send(new UpdateIndicationType.Command() { IndicationType = indicationType });
        }
    }
}