using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Api.Common.ExtensionMethods;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Report.HandJewelry;
using HyFive.Models.V1.Session;
using HyFive.Services;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HyFive.Services.HandJewelry;
using HyFive.Services.Rapport.Observasjoner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HyFive.Observasjon.Controllers.V1
{
    [Route("api/v1/handsmykke")]
    public class HandsmykkerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _brukerservice;

        public HandsmykkerController(IMediator mediator, IUserService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Lagre en Håndsmykke-sesjon
        /// </summary>
        /// <param name="sesjon"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> LagreSesjon([FromBody] HandJewelrySession sesjon)
        {
            if (!sesjon.Observations.Any())
            {
                return BadRequest("Sesjonen må ha minst én observasjon");
            }
            if (_brukerservice.ErObservatorForInstitusjon(sesjon.Department.InstitusjonId))
            {
                var resultat = await _mediator.Send(new SaveSession.Command()
                {
                    HprNumber = _brukerservice.GetHprNumber(),
                    Pseudonym = _brukerservice.GetPseudonym(),
                    Session = sesjon
                });

                return CreatedAtRoute("HentHandsmykkeSesjon", new { sesjonId = sesjon.Id }, resultat);
            }

            return Unauthorized();
        }

        [HttpGet("handsmykketyper")]
        public async Task<IEnumerable<HandJewelryType>> HentHandsmykkeTyper()
        {
            var resultat = await _mediator.Send(new GetHandJewelryType.Query());
            return resultat;
        }

        [HttpGet("mineobservasjoner")]
        public async Task<IEnumerable<HandJewelryObservationReport>> HentMineObservasjoner(int institusjonId, Guid? sesjonId = null)
        {
            var observatorIdForInstitusjon = _brukerservice.GetObserverIdForInstitution(institusjonId);
            if (observatorIdForInstitusjon > 0)
            {
                var query = new HentHandsmykkeObservasjoner.Query
                {
                    ObservatorId = observatorIdForInstitusjon,
                    InstitusjonId = institusjonId,
                    SesjonId = sesjonId,
                    Rolle = AuthorizedRole.Observer
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
