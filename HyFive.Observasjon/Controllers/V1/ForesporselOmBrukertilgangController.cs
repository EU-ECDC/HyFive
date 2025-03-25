using HyFive.Modeller.V1.ForesporselOmBrukertilgang;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.ForesporselOmBrukertilgang;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Observasjon.Controllers.V1
{
    [Route("api/v1/foresporselombrukertilgang")]
    public class ForesporselOmBrukertilgangController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerService;

        public ForesporselOmBrukertilgangController(IMediator mediator, IBrukerService brukerService)
        {
            _mediator = mediator;
            _brukerService = brukerService;
        }

        [HttpGet("institusjoner")]
        [ProducesResponseType(typeof(List<InstitusjonForForesporselOmBrukertilgang>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitusjonForForesporselOmBrukertilgang>>> HentInstitusjoner()
        {
            try
            {
                var result = await _mediator.Send(new HentInstitusjoner.Query());
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("send")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        public async Task<ActionResult<bool>> SendForesporselOmBrukertilgang([FromBody] OpprettForesporselOmBrukertilgangRequest foresporselOmBrukertilgang)
        {
            try
            {
                var result = await _mediator.Send(new OpprettForesporselOmBrukertilgang.Command
                {
                    ForesporselOmBrukertilgang = foresporselOmBrukertilgang
                });

                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<ForesporselOmBrukertilgang>> HentForesporselSomSendtAllerede()
        {
            try
            {
                var bruker = await _brukerService.HentBruker();
                var foresporsel = await _mediator.Send(new HentForesporselSomSendtAllerede.Query
                {
                    HprNummer = bruker.HPRNummer,
                    IdentPseudonym = bruker.IdentPseudonym
                });

                return Ok(foresporsel);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("institusjon")]
        [ProducesResponseType(typeof(List<InstitusjonForForesporselOmBrukertilgang>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitusjonForForesporselOmBrukertilgang>>> HentInstitusjon(int institusjonId)
        {
            try
            {
                var result = await _mediator.Send(new HentInstitusjon.Query()
                {
                    InstitusjonId = institusjonId
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
