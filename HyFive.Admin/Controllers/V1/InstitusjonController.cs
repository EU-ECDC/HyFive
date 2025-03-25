using HyFive.Modeller.V1.Bruker;
using HyFive.Modeller.V1.Institusjon;
using HyFive.Tjenester.Institusjon;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.Avdeling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/institusjon")]
    public class InstitusjonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public InstitusjonController(IMediator mediator, IBrukerService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Hent alle tilgjengelige institusjoner <see cref="InstitusjonRapport"/>
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IEnumerable<InstitusjonRapport>> HentInstitusjoner()
        {
            if (_brukerservice.ErFhiAdmin())
            {
                return await _mediator.Send(new HentInstitusjoner.Query());
            }
            return await _mediator.Send(new HentInstitusjonerForKoordinator.Query() { KoordinatorHprNummer = _brukerservice.HentHprnummer(), KoordinatorPseudonym = _brukerservice.HentPseudonym()});
        }

        [Authorize(HandhygienePolicy.Koordinator)]
        [HttpGet("hentInstitusjonerForKoordinator")]
        public async Task<IEnumerable<InstitusjonRapport>> HentInstitusjonerForKoordinator()
        {
            return await _mediator.Send(new HentInstitusjonerForKoordinator.Query() { KoordinatorHprNummer = _brukerservice.HentHprnummer(), KoordinatorPseudonym = _brukerservice.HentPseudonym() });
        }


        /// <summary>
        /// Hent én institusjon
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "HentInstitusjon")]
        public async Task<IActionResult> HentInstitusjon(int id)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(id))
            {
                var resultat = await _mediator.Send(new HentInstitusjon.Query() { InstitusjonId = id });
                return Ok(resultat);
            }
            return Unauthorized();
        }



        /// <summary>
        /// Hent alle avdelinger tilhørende institusjon
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/avdelinger", Name = "HentAvdelinger")]
        public async Task<ActionResult<IEnumerable<Avdeling>>> HentAvdelinger(int id)
        {
            if (_brukerservice.ErKoordinatorForInstitusjon(id))
            {
                var resultat = await _mediator.Send(new HentAvdelingerForInstitusjon.Query() { InstitusjonId = id });
                return Ok(resultat);
            }

            return Unauthorized();
        }


        /// <summary>
        /// Hent alle observatører tilhørende institusjon
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/observatorer", Name = "HentObservatorer")]
        public async Task<ActionResult<IEnumerable<Bruker>>> HentObservatorer(int id)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(id))
            {
                var resultat = await _mediator.Send(new HentObservatorerForInstitusjon.Query() { InstitusjonId = id });
                return Ok(resultat);
            }

            return Unauthorized();
        }



        /// <summary>
        /// Hent alle koordinatorer tilhørende institusjon
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/koordinatorer", Name = "HentKoordinatorer")]
        public async Task<ActionResult<IEnumerable<Bruker>>> HentKoordinatorer(int id)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(id))
            {
                var resultat = await _mediator.Send(new HentKoordinatorerForInstitusjon.Query() { InstitusjonId = id });
                return Ok(resultat);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Hent  <see cref="InstitusjonType"/>r
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpGet("typer")]
        public async Task<IEnumerable<InstitusjonType>> HentInstitusjontyper()
        {
            var result = await _mediator.Send(new HentInstitusjonstyper.Query());
            return result;
        }

        /// <summary>
        /// Opprett institusjon med koordinator
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPost("opprett")]
        [ProducesResponseType(typeof(Institusjon), StatusCodes.Status201Created)]
        public async Task<ActionResult<Institusjon>> OpprettInstitusjon([FromBody] OpprettInstitusjonRequest request)
        {
            var result = await _mediator.Send(new OpprettInstitusjon.Command() { Request = request });
            return CreatedAtRoute("HentInstitusjon", new { id = result.Id }, result);
        }

        /// <summary>
        /// Oppdaterer en institusjon. Ignorerer eventuelle avdelinger eller roller som sendes med.
        /// </summary>
        /// <param name="institusjon"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<Institusjon> OppdaterInstitusjon([FromBody] Institusjon institusjon)
        {
            var result = await _mediator.Send(new OppdaterInstitusjon.Command() { Institusjon = institusjon });
            return result;
        }

        /// <summary>
        /// Slett institusjon og alle tilhørende sesjoner, observasjoner, avdelinger, roller og brukere
        /// </summary>
        /// <param name="institusjonId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpDelete("slett")]
        public async Task<bool> SlettInstitusjon([FromQuery] int institusjonId)
        {
            var result = await _mediator.Send(new SlettInstitusjon.Command()
            {
                InstitusjonId = institusjonId
            });
            return result;
        }
    }
}
