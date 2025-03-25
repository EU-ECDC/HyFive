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
    [Route("api/v1/handhygieneetterhanskebruktype")]
    public class HandhygieneEtterHanskebrukTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HandhygieneEtterHanskebrukTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent HandhygieneEtterHanskebrukTyper
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<HandhygieneEtterHanskebrukType>> HentHandhygieneEtterHanskebrukTyper()
        {
            var handhygieneEtterHanskebrukTyper = await _mediator.Send(new HentHandhygieneEtterHanskebrukTyper.Query());
            return handhygieneEtterHanskebrukTyper;
        }

        /// <summary>
        /// Oppdater HandhygieneEtterHanskebrukType
        /// </summary>
        /// <param name="handhygieneEtterHanskebrukType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<HandhygieneEtterHanskebrukType> OppdaterHandhygieneEtterHanskebrukType([FromBody] HandhygieneEtterHanskebrukType handhygieneEtterHanskebrukType)
        {
            var erOppdatert = await _mediator.Send(new OppdaterHandhygieneEtterHanskebrukType.Command
            {
                HandhygieneEtterHanskebrukType = handhygieneEtterHanskebrukType
            });

            return erOppdatert;
        }
    }
}
