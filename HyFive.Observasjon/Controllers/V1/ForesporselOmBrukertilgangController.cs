using HyFive.Models.V1.UserAccessRequest;
using HyFive.Services.Authentication.User;
using HyFive.Services.ForesporselOmBrukertilgang;
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
        private readonly IUserService _brukerService;

        public ForesporselOmBrukertilgangController(IMediator mediator, IUserService brukerService)
        {
            _mediator = mediator;
            _brukerService = brukerService;
        }

        [HttpGet("institusjoner")]
        [ProducesResponseType(typeof(List<InstitutionForUserAccessRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionForUserAccessRequest>>> HentInstitusjoner()
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
        public async Task<ActionResult<bool>> SendForesporselOmBrukertilgang([FromBody] CreateUserAccessRequest foresporselOmBrukertilgang)
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
        public async Task<ActionResult<UserAccessRequest>> HentForesporselSomSendtAllerede()
        {
            try
            {
                var bruker = await _brukerService.GetUser();
                var foresporsel = await _mediator.Send(new HentForesporselSomSendtAllerede.Query
                {
                    HprNummer = bruker.HPRNumber,
                    IdentPseudonym = bruker.IdentityPseudonym
                });

                return Ok(foresporsel);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("institusjon")]
        [ProducesResponseType(typeof(List<InstitutionForUserAccessRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionForUserAccessRequest>>> HentInstitusjon(int institusjonId)
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
