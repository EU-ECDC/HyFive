using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Api.Common.ExtensionMethods;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.Gloves;
using HyFive.Models.V1.Report.Glove;
using HyFive.Models.V1.Session;
using HyFive.Services;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Hanske;
using HyFive.Services.Rapport.Observasjoner;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Observasjon.Controllers.V1
{
    [Route("api/v1/hanske")]
    [ApiController]
    public class HanskeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _brukerservice;

        public HanskeController(IMediator mediator, IUserService brukerService)
        {
            _mediator = mediator;
            _brukerservice = brukerService;
        }

        /// <summary>
        /// Lagre en Glove-sesjon.
        /// </summary>
        /// <param name="sesjon"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> LagreSesjon([FromBody] HanskeSesjon sesjon)
        {
            if (!sesjon.Observasjoner.Any())
            {
                return BadRequest("Sesjonen må ha minst én observasjon");
            }

            if (_brukerservice.ErObservatorForInstitusjon(sesjon.Avdeling.InstitusjonId))
            {
                var resultat = await _mediator.Send(new LagreSesjon.Command
                {
                    HPRNummer = _brukerservice.GetHprNumber(),
                    Pseudonym = _brukerservice.GetPseudonym(),
                    Sesjon = sesjon
                });

                return CreatedAtRoute("HentHanskeSesjon", new { sesjonId = sesjon.Id }, resultat);
            }

            return Unauthorized();
        }

        [HttpGet("hanskemedindikasjontype")]
        public async Task<IEnumerable<IndicatedGloveType>> HentHanskeMedIndikasjonTyper()
        {
            var resultat = await _mediator.Send(new HentHanskeMedIndikasjonTyper.Query());
            return resultat;
        }

        [HttpGet("hanskeutenindikasjontype")]
        public async Task<IEnumerable<GeneralPurposeGloveType>> HentHanskeUtenIndikasjonTyper()
        {
            var resultat = await _mediator.Send(new HentHanskeUtenIndikasjonTyper.Query());
            return resultat;
        }

        [HttpGet("handhygieneetterhanskebruktype")]
        public async Task<IEnumerable<PostGloveHandHygieneType>> HentHandhygieneEtterHanskebrukTyper()
        {
            var resultat = await _mediator.Send(new HentHandhygieneEtterHanskebrukTyper.Query());
            return resultat;
        }

        [HttpGet("mineobservasjoner")]
        public async Task<IEnumerable<GloveObservationReport>> HentMineObservasjoner(int institusjonId, Guid? sesjonId = null)
        {
            var observatorIdForInstitusjon = _brukerservice.GetObserverIdForInstitution(institusjonId);
            if (observatorIdForInstitusjon > 0)
            {
                var query = new HentHanskeObservasjoner.Query()
                {
                    ObservatorId = observatorIdForInstitusjon,
                    InstitusjonId = institusjonId,
                    SesjonId = sesjonId,
                    Rolle = AuthorizedRole.Observator
                };

                var observasjoner = await _mediator.Send(query);
                return observasjoner;
            }

            throw new UnauthorizedAccessException("Du har ikke tilgang til å spørre om observasjonene til denne institusjonen");
        }

        [HttpGet("mineobservasjoner/excel")]
        public async Task<IActionResult> HentMineObservasjonerSomExcel(int institusjonId, Guid? sesjonId = null)
        {
            var observasjoner = await HentMineObservasjoner(institusjonId, sesjonId);
            return await this.ExcelFileContentResult(observasjoner, "Observasjoner");
        }
    }
}