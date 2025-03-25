using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Observasjon.Hansker;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.Hanske;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/hanskemedindikasjontype")]
    public class HanskeMedIndikasjonTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HanskeMedIndikasjonTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent HanskeMedIndikasjonTyper
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<HanskeMedIndikasjonType>> HentHanskeMedIndikasjonTyper()
        {
            var hanskeVedIndikasjonTyper = await _mediator.Send(new HentHanskeMedIndikasjonTyper.Query());
            return hanskeVedIndikasjonTyper;
        }

        /// <summary>
        /// Oppdater HanskeVedIndikasjonType
        /// </summary>
        /// <param name="hanskeMedIndikasjonType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<HanskeMedIndikasjonType> OppdaterHanskeMedIndikasjonType([FromBody] HanskeMedIndikasjonType hanskeMedIndikasjonType)
        {
            var erOppdatert = await _mediator.Send(new OppdaterHanskeMedIndikasjonType.Command
            {
                HanskeMedIndikasjonType = hanskeMedIndikasjonType
            });

            return erOppdatert;
        }
    }
}
