using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.FourIndication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyFive.Api.Common.ExtensionMethods;
using HyFive.Models.V1.Report.FourIndications;
using HyFive.Services;
using HyFive.Services.Rapport.Observasjoner;

namespace HyFive.Observasjon.Controllers.V1
{

    [Route("api/v1/fireindikasjoner")]
    public class FireIndikasjonerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _brukerservice;

        public FireIndikasjonerController(IMediator mediator, IUserService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Lagre en Fire Indications-sesjon
        /// </summary>
        /// <param name="sesjon"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> LagreSesjon([FromBody] FourIndicationsSession sesjon)
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

                return CreatedAtRoute("HentFireIndikasjonerSesjon", new { sesjonId = sesjon.Id }, resultat);
            }

            return Unauthorized();
        }

        [HttpGet("indikasjonstyper")]
        public async Task<IEnumerable<IndicationType>> HentIndikasjonstyper()
        {
            var resultat = await _mediator.Send(new GetIndicationTypes.Query());
            return resultat;
        }

        [HttpGet("aktivitettyper")]
        public async Task<IEnumerable<ActivityType>> HentAktivitetTyper()
        {
            var resultat = await _mediator.Send(new GetActivityTypes.Query());
            return resultat;
        }

        [HttpGet("mineobservasjoner")]
        public async Task<IEnumerable<FourIndicationsObservationReport>> HentMineObservasjoner(int institusjonId, Guid? sesjonId = null)
        {
            var observatorIdForInstitusjon = _brukerservice.GetObserverIdForInstitution(institusjonId);
            if (observatorIdForInstitusjon > 0)
            {
                var query = new HentFireIndikasjonerObservasjoner.Query()
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
