using HyFive.Modeller.V1.Observasjon;
using HyFive.Tjenester.FireIndikasjoner;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Tjenester.Autentisering.Requirements;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/aktivitettype")]
    public class AktivitettypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AktivitettypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent alle tilgjengelige aktivitettyper <see cref="ActivityType"/>
        /// </summary>
        /// <returns></returns>

        [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
        [HttpGet]
        public async Task<IEnumerable<ActivityType>> HentAktivitettyper()
            => await _mediator.Send(new HentAktivitetTyper.Query());

        /// <summary>
        /// Oppdaterer en aktivitettype
        /// </summary>
        /// <param name="aktivitettype"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<ActivityType> OppdaterAktivitettype([FromBody] ActivityType aktivitettype)
        {
            var result = await _mediator.Send(new OppdaterAktivitetType.Command() { Aktivitettype = aktivitettype });
            return result;
        }
    }
}
