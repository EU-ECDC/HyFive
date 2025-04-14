using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Observation.Gloves;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Hanske;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
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
        public async Task<IEnumerable<PostGloveHandHygieneType>> HentHandhygieneEtterHanskebrukTyper()
        {
            var handhygieneEtterHanskebrukTyper = await _mediator.Send(new HentHandhygieneEtterHanskebrukTyper.Query());
            return handhygieneEtterHanskebrukTyper;
        }

        /// <summary>
        /// Oppdater PostGloveHandHygiene
        /// </summary>
        /// <param name="handhygieneEtterHanskebrukType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<PostGloveHandHygieneType> OppdaterHandhygieneEtterHanskebrukType([FromBody] PostGloveHandHygieneType handhygieneEtterHanskebrukType)
        {
            var erOppdatert = await _mediator.Send(new OppdaterHandhygieneEtterHanskebrukType.Command
            {
                HandhygieneEtterHanskebrukType = handhygieneEtterHanskebrukType
            });

            return erOppdatert;
        }
    }
}
