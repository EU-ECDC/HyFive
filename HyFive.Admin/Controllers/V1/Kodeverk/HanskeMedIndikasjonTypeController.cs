using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Observation.Gloves;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Glove;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/hanskemedindikasjontype")]
    public class HanskeMedIndikasjonTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HanskeMedIndikasjonTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent IndicatedGloveTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<IndicatedGloveType>> HentHanskeMedIndikasjonTyper()
        {
            var hanskeVedIndikasjonTyper = await _mediator.Send(new GetGloveWithIndicationTypes.Query());
            return hanskeVedIndikasjonTyper;
        }

        /// <summary>
        /// Oppdater HanskeVedIndikasjonType
        /// </summary>
        /// <param name="hanskeMedIndikasjonType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<IndicatedGloveType> OppdaterHanskeMedIndikasjonType([FromBody] IndicatedGloveType hanskeMedIndikasjonType)
        {
            var erOppdatert = await _mediator.Send(new UpdateGloveWithIndicationType.Command
            {
                GloveWithIndicationType = hanskeMedIndikasjonType
            });

            return erOppdatert;
        }
    }
}
