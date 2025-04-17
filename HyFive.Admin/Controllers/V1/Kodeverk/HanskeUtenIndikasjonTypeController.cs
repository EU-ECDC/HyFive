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
    [Route("api/v1/hanskeutenindikasjontype")]
    public class HanskeUtenIndikasjonTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HanskeUtenIndikasjonTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent GeneralPurposeGloveTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<GeneralPurposeGloveType>> HentHanskeUtenIndikasjonTyper()
        {
            var hanskeVedIndikasjonTyper = await _mediator.Send(new GetGloveWithoutIndicationTypes.Query());
            return hanskeVedIndikasjonTyper;
        }

        /// <summary>
        /// Oppdater GeneralPurposeGloveType
        /// </summary>
        /// <param name="hanskeUtenIndikasjonType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<GeneralPurposeGloveType> OppdaterHanskeUtenIndikasjonType([FromBody] GeneralPurposeGloveType hanskeUtenIndikasjonType)
        {
            var erOppdatert = await _mediator.Send(new UpdateGloveWithoutIndicationType.Command
            {
                HanskeUtenIndikasjonType = hanskeUtenIndikasjonType
            });

            return erOppdatert;
        }
    }
}
