using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.FourIndication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IndicationType = HyFive.Modeller.V1.Observation.IndicationType;

namespace HyFive.Admin.Controllers.V1
{
    /// <summary>
    /// IndicationTypes
    /// </summary>
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/indikasjonstyper")]
    public class IndikasjonstyperController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IndikasjonstyperController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent indikasjonstyper
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<IndicationType>> HentIndikasjonstyper()
        {
            var indikasjonstyper = await _mediator.Send(new GetIndicationTypes.Query());
            return indikasjonstyper;
        }

        /// <summary>
        /// Oppdater indikasjonstype
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<IndicationType> OppdaterIndikasjonstype([FromBody] IndicationType indikasjonstype)
        {
            return await _mediator.Send(new UpdateIndicationType.Command() { IndicationType = indikasjonstype });
        }
    }
}