using HyFive.Models.V1.Observation;
using HyFive.Services.HandJewelry;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/handJewelryType")]
    public class HandJewelryTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HandJewelryTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all available hand jewelry types <see cref="HandJewelryType"/>
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IEnumerable<HandJewelryType>> GetAllHandJewelryTypes()
            => await _mediator.Send(new GetHandJewelryTypes.Query());

        /// <summary>
        /// Update HandJewelryType
        /// </summary>
        /// <param name="handJewelryType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpPut("update")]
        public async Task<HandJewelryType> UpdateHandJewelryType([FromBody] HandJewelryType handJewelryType)
        {
            var result = await _mediator.Send(new UpdateHandJewelryType.Command() { HandJewelryType = handJewelryType });
            return result;
        }
    }
}
