using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Api.Common.ExtensionMethods;
using HyFive.Modeller.V1.Observasjon;
using HyFive.Modeller.V1.Rapport.Handsmykke;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HyFive.Tjenester.Handsmykke;
using HyFive.Tjenester.Rapport.Observasjoner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HyFive.Observasjon.Controllers.V1
{
    [Route("api/v1/handsmykke")]
    public class HandsmykkerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public HandsmykkerController(IMediator mediator, IBrukerService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Lagre en Håndsmykke-sesjon
        /// </summary>
        /// <param name="sesjon"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observator)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> LagreSesjon([FromBody] HandsmykkeSesjon sesjon)
        {
            if (!sesjon.Observasjoner.Any())
            {
                return BadRequest("Sesjonen må ha minst én observasjon");
            }
            if (_brukerservice.ErObservatorForInstitusjon(sesjon.Avdeling.InstitusjonId))
            {
                var resultat = await _mediator.Send(new LagreSesjon.Command()
                {
                    HPRNummer = _brukerservice.HentHprnummer(),
                    Pseudonym = _brukerservice.HentPseudonym(),
                    Sesjon = sesjon
                });

                return CreatedAtRoute("HentHandsmykkeSesjon", new { sesjonId = sesjon.Id }, resultat);
            }

            return Unauthorized();
        }

        [HttpGet("handsmykketyper")]
        public async Task<IEnumerable<HandJewelryType>> HentHandsmykkeTyper()
        {
            var resultat = await _mediator.Send(new HentHandsmykkeTyper.Query());
            return resultat;
        }

        [HttpGet("mineobservasjoner")]
        public async Task<IEnumerable<HandsmykkeObservasjonRapport>> HentMineObservasjoner(int institusjonId, Guid? sesjonId = null)
        {
            var observatorIdForInstitusjon = _brukerservice.HentObservatorIdForInstitusjon(institusjonId);
            if (observatorIdForInstitusjon > 0)
            {
                var query = new HentHandsmykkeObservasjoner.Query
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
