using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.FireIndikasjoner;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IndikasjonType = HyFive.Modeller.V1.Observasjon.IndikasjonType;

namespace HyFive.Admin.Controllers.V1
{
    /// <summary>
    /// Indikasjonstyper
    /// </summary>
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
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
        public async Task<List<IndikasjonType>> HentIndikasjonstyper()
        {
            var indikasjonstyper = await _mediator.Send(new HentIndikasjonstyper.Query());
            return indikasjonstyper;
        }

        /// <summary>
        /// Oppdater indikasjonstype
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<IndikasjonType> OppdaterIndikasjonstype([FromBody] IndikasjonType indikasjonstype)
        {
            return await _mediator.Send(new OppdaterIndikasjonstype.Command() { Indikasjonstype = indikasjonstype });
        }
    }
}