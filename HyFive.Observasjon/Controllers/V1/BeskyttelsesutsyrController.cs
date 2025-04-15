using HyFive.Api.Common.ExtensionMethods;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Models.V1.Report.Beskyttelsesutstyr;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Beskyttelsesutstyr;
using HyFive.Services.Rapport.Observasjoner;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Services;

namespace HyFive.Observasjon.Controllers.V1
{

    [Route("api/v1/beskyttelsesutstyr")]
    public class BeskyttelsesutsyrController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _brukerservice;

        public BeskyttelsesutsyrController(IMediator mediator, IUserService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }
        
        [HttpGet]
        public async Task<IEnumerable<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType>> HentBeskyttelsesutstyrsettingtyper()
        {
            return await _mediator.Send(new HentBeskyttelsesutstyrsettingTyper.Query());
        }

        /// <summary>
        /// Lagre en ProtectiveEquipment-sesjon
        /// </summary>
        /// <param name="sesjon"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> LagreSesjon([FromBody] ProtectiveEquipmentSession sesjon)
        {
            if (!sesjon.Observasjoner.Any())
            {
                return BadRequest("Sesjonen må ha minst én observasjon");
            }

            if (_brukerservice.ErObservatorForInstitusjon(sesjon.Avdeling.InstitusjonId))
            {
                var resultat = await _mediator.Send(new LagreSesjon.Command()
                {
                    HPRNummer = _brukerservice.GetHprNumber(),
                    Pseudonym = _brukerservice.GetPseudonym(),
                    Sesjon = sesjon
                });

                return CreatedAtRoute("HentBeskyttelsesutstyrSesjon", new { sesjonId = sesjon.Id }, resultat);
            }

            return Unauthorized();
        }

        [HttpGet("mineobservasjoner")]
        public async Task<IEnumerable<PPEObservationReport>> HentMineObservasjoner(int institusjonId, Guid? sesjonId = null)
        {
            var observatorIdForInstitusjon = _brukerservice.GetObserverIdForInstitution(institusjonId);
            if (observatorIdForInstitusjon > 0)
            {
                var query = new HentBeskyttelsesutstyrObservasjoner.Query()
                {
                    ObservatorId = observatorIdForInstitusjon,
                    InstitusjonId = institusjonId,
                    SesjonId = sesjonId,
                    Rolle = AuthorizedRole.Coordinator
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
