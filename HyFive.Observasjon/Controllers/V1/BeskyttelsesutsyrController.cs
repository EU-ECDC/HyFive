using HyFive.Api.Common.ExtensionMethods;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using HyFive.Modeller.V1.Rapport.Beskyttelsesutstyr;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.Beskyttelsesutstyr;
using HyFive.Tjenester.Rapport.Observasjoner;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Tjenester;

namespace HyFive.Observasjon.Controllers.V1
{

    [Route("api/v1/beskyttelsesutstyr")]
    public class BeskyttelsesutsyrController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public BeskyttelsesutsyrController(IMediator mediator, IBrukerService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }
        
        [HttpGet]
        public async Task<IEnumerable<ProtectiveEquipmentSettingType>> HentBeskyttelsesutstyrsettingtyper()
        {
            return await _mediator.Send(new HentBeskyttelsesutstyrsettingTyper.Query());
        }

        /// <summary>
        /// Lagre en ProtectiveEquipment-sesjon
        /// </summary>
        /// <param name="sesjon"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observator)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> LagreSesjon([FromBody] BeskyttelsesutstyrSesjon sesjon)
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

                return CreatedAtRoute("HentBeskyttelsesutstyrSesjon", new { sesjonId = sesjon.Id }, resultat);
            }

            return Unauthorized();
        }

        [HttpGet("mineobservasjoner")]
        public async Task<IEnumerable<BeskyttelsesutstyrObservasjonRapport>> HentMineObservasjoner(int institusjonId, Guid? sesjonId = null)
        {
            var observatorIdForInstitusjon = _brukerservice.HentObservatorIdForInstitusjon(institusjonId);
            if (observatorIdForInstitusjon > 0)
            {
                var query = new HentBeskyttelsesutstyrObservasjoner.Query()
                {
                    ObservatorId = observatorIdForInstitusjon,
                    InstitusjonId = institusjonId,
                    SesjonId = sesjonId,
                    Rolle = AuthorizedRole.Koordinator
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
