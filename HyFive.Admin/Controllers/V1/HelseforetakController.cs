using HyFive.Domain.Bruker;
using HyFive.Modeller.V1;
using HyFive.Modeller.V1.User;
using HyFive.Modeller.V1.Institution;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Helseforetak;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/helseforetak")]
    [ApiController]
    public class HelseforetakController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _brukerservice;

        public HelseforetakController(IMediator mediator, IUserService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpGet]
        [ProducesResponseType(typeof(List<HealthcareEnterprise>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<HealthcareEnterprise>>> HentAlleHelseforetak()
        {
            var alleHelseforetak = await _mediator.Send(new HentAlleHelseforetak.Query());
            return Ok(alleHelseforetak);
        }

        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPost("opprett")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> OpprettEtHelseforetak([FromBody] CreateHealthEnterpriseRequest helseforetakRequest)
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
        public async Task<ActionResult<bool>> OppdaterHeleforetaket([FromBody] HealthcareEnterprise helseforetak)
        {
            var erOppdatert = await _mediator.Send(new OppdaterHelseforetaket.Command
            {
                HealthcareProvider = helseforetak
            });
            return Ok(erOppdatert);
        }

        [HttpGet("{id}/koordinatorer")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<HealthcareInstitutionCoordinator[]>> HentKoordinatorForHelseforetak(int id)
        {
            if (_brukerservice.IsCoordinatorForHealthcareProviderOrFhiAdmin(id))
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
        public async Task<ActionResult<InstitutionReport[]>> HentInstitiusjonerForHelseforetak(int id)
        {
            if (_brukerservice.IsCoordinatorForHealthcareProviderOrFhiAdmin(id))
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
        public async Task<ActionResult<Status>> OppdaterKoordinator([FromBody] HealthcareInstitutionCoordinator koordinator, int id)
        {
            if (_brukerservice.IsCoordinatorForHealthcareProviderOrFhiAdmin(id))
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
        public async Task<ActionResult<Status>> OpprettKoordinator([FromBody] HealthcareInstitutionCoordinator koordinator, int id)
        {
            if (_brukerservice.IsCoordinatorForHealthcareProviderOrFhiAdmin(id))
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
