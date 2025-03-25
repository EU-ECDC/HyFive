using HyFive.Domene.Bruker;
using HyFive.Modeller.V1;
using HyFive.Modeller.V1.Bruker;
using HyFive.Modeller.V1.Institusjon;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.Helseforetak;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/helseforetak")]
    [ApiController]
    public class HelseforetakController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public HelseforetakController(IMediator mediator, IBrukerService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpGet]
        [ProducesResponseType(typeof(List<Helseforetak>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Helseforetak>>> HentAlleHelseforetak()
        {
            var alleHelseforetak = await _mediator.Send(new HentAlleHelseforetak.Query());
            return Ok(alleHelseforetak);
        }

        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPost("opprett")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> OpprettEtHelseforetak([FromBody] OpprettHelseforetakRequest helseforetakRequest)
        {
            var erOpprettet = await _mediator.Send(new OpprettHelseforetak.Command
            {
                Helseforetak = helseforetakRequest
            });
            return Ok(erOpprettet);
        }

        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> OppdaterHeleforetaket([FromBody] Helseforetak helseforetak)
        {
            var erOppdatert = await _mediator.Send(new OppdaterHelseforetaket.Command
            {
                Helseforetak = helseforetak
            });
            return Ok(erOppdatert);
        }

        [HttpGet("{id}/koordinatorer")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<KoordinatorForHelseforetak[]>> HentKoordinatorForHelseforetak(int id)
        {
            if (_brukerservice.ErKoordinatorForHelseforetakEllerFhiAdmin(id))
            {
                var koordinatorMedInstitusjonerListe = await _mediator.Send(new HentKoordinatorerForHelseforetak.Query
                {
                    HelseforetakId = id
                });
                return Ok(koordinatorMedInstitusjonerListe);
            }

            return Unauthorized();
        }

        [HttpGet("{id}/institusjoner")]
        public async Task<ActionResult<InstitusjonRapport[]>> HentInstitiusjonerForHelseforetak(int id)
        {
            if (_brukerservice.ErKoordinatorForHelseforetakEllerFhiAdmin(id))
            {
                return await _mediator.Send(new HentInstitiusjonerForHelseforetak.Query
                {
                    HelseforetakId = id
                });
            }

            return Unauthorized();
        }


        [HttpPut("{id}/oppdaterkoordinator")]
        [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> OppdaterKoordinator([FromBody] KoordinatorForHelseforetak koordinator, int id)
        {
            if (_brukerservice.ErKoordinatorForHelseforetakEllerFhiAdmin(id))
            {
                var oppdatertStatus = await _mediator.Send(new OppdaterKoordinatorForHelseforetak.Command
                {
                    Koordinator = koordinator,
                    HelseforetakId = id
                });
                return Ok(oppdatertStatus);
            }

            return Unauthorized();
        }

        [HttpPost("{id}/opprettkoordinator")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<Status>> OpprettKoordinator([FromBody] KoordinatorForHelseforetak koordinator, int id)
        {
            if (_brukerservice.ErKoordinatorForHelseforetakEllerFhiAdmin(id))
            {
                var opprettetStatus = await _mediator.Send(new OpprettKoordinatorForHelseforetak.Command
                {
                    Koordinator = koordinator,
                    HelseforetakId = id
                });
                return Ok(opprettetStatus);
            }

            return Unauthorized();
        }
    }
}
