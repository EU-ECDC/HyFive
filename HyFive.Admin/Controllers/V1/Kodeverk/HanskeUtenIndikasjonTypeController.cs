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
    [Route("api/v1/hanskeutenindikasjontype")]
    public class HanskeUtenIndikasjonTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HanskeUtenIndikasjonTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent HanskeUtenIndikasjonTyper
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<HanskeUtenIndikasjonType>> HentHanskeUtenIndikasjonTyper()
        {
            var hanskeVedIndikasjonTyper = await _mediator.Send(new HentHanskeUtenIndikasjonTyper.Query());
            return hanskeVedIndikasjonTyper;
        }

        /// <summary>
        /// Oppdater HanskeUtenIndikasjonType
        /// </summary>
        /// <param name="hanskeUtenIndikasjonType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<HanskeUtenIndikasjonType> OppdaterHanskeUtenIndikasjonType([FromBody] HanskeUtenIndikasjonType hanskeUtenIndikasjonType)
        {
            var erOppdatert = await _mediator.Send(new OppdaterHanskeUtenIndikasjonType.Command
            {
                HanskeUtenIndikasjonType = hanskeUtenIndikasjonType
            });

            return erOppdatert;
        }
    }
}
