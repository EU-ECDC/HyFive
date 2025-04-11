using System.Collections.Generic;
using HyFive.Modeller.V1.Institution;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HyFive.Tjenester.Klinikk;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/klinikk")]
    public class KlinikkController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public KlinikkController(IMediator mediator, IBrukerService brukerService)
        {
            _mediator = mediator;
            _brukerservice = brukerService;
        }

        /// <summary>
        /// Hent klinikk
        /// </summary>
        /// <param name="id"></param>
        /// <param name="institusjonsId"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Clinic), StatusCodes.Status200OK)]
        public async Task<ActionResult<Clinic>> HentKlinikk(int id, int institusjonsId)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(institusjonsId))
            {
                return await _mediator.Send(new HentKlinikk.Query() { Id = id, InstitusjonId = institusjonsId });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Hent klinikker for institusjon
        /// </summary>
        /// <param name="institusjonId"></param>
        /// <returns></returns>
        [HttpGet("institusjon/{institusjonId}")]
        [ProducesResponseType(typeof(IEnumerable<Clinic>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Clinic>>> HentKlinikkerForInstitusjon(int institusjonId)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(institusjonId))
            {
                var klinikker = await _mediator.Send(new HentKlinikkerForInstitusjon.Query() { InstitusjonId = institusjonId });
                return Ok(klinikker);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Opprett klinikk
        /// </summary>
        /// <returns></returns>
        [HttpPost("opprett")]
        [ProducesResponseType(typeof(Clinic), StatusCodes.Status201Created)]
        public async Task<ActionResult<Clinic>> OpprettKlinikk([FromBody] Clinic klinikk)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(klinikk.InstitutionId))
            {
                return await _mediator.Send(new OpprettKlinikk.Command() { Klinikk = klinikk });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Opprett klinikk
        /// </summary>
        /// <returns></returns>
        [HttpPut("oppdater")]
        [ProducesResponseType(typeof(Clinic), StatusCodes.Status201Created)]
        public async Task<ActionResult<Clinic>> OppdaterKlinikk([FromBody] Clinic klinikk)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(klinikk.InstitutionId))
            {
                return await _mediator.Send(new OppdaterKlinikk.Command() { Klinikk = klinikk });
            }

            return Unauthorized();
        }
    }
}
