using HyFive.Modeller.V1.Observasjon;
using HyFive.Tjenester.Handsmykke;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Tjenester.Autentisering.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/handsmykketype")]
    public class HandsmykketypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HandsmykketypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent alle tilgjengelige handsmykketyper <see cref="HandsmykkeType"/>
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IEnumerable<HandsmykkeType>> HentHandsmykketyper()
            => await _mediator.Send(new HentHandsmykkeTyper.Query());

        /// <summary>
        /// Oppdaterer en handsmykketype
        /// </summary>
        /// <param name="handsmykketype"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<HandsmykkeType> OppdaterHandsmykketype([FromBody] HandsmykkeType handsmykketype)
        {
            var result = await _mediator.Send(new OppdaterHandsmykkeType.Command() { Handsmykketype = handsmykketype });
            return result;
        }
    }
}
