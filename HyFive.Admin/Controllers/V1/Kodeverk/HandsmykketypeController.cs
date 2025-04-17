using HyFive.Modeller.V1.Observation;
using HyFive.Services.HandJewelry;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/handsmykketype")]
    public class HandsmykketypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HandsmykketypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent alle tilgjengelige handsmykketyper <see cref="HandJewelryType"/>
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IEnumerable<HandJewelryType>> HentHandsmykketyper()
            => await _mediator.Send(new GetHandJewelryType.Query());

        /// <summary>
        /// Oppdaterer en handsmykketype
        /// </summary>
        /// <param name="handsmykketype"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<HandJewelryType> OppdaterHandsmykketype([FromBody] HandJewelryType handsmykketype)
        {
            var result = await _mediator.Send(new UpdateHandJewelryType.Command() { HandJewelryType = handsmykketype });
            return result;
        }
    }
}
