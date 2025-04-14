using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.ForesporselOmBrukertilgang;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAccessRequest = HyFive.Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.Coordinator)]
    [Route("api/v1/foresporselombrukertilgang")]
    public class ForesporselOmBrukertilgangController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _brukerservice;

        public ForesporselOmBrukertilgangController(IMediator mediator, IUserService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Henter alle Forespørsler om brukertilgang.
        /// </summary>
        /// <returns></returns>
        [HttpGet("alleforesporsler")]
        [Authorize(HandhygienePolicy.Coordinator)]
        [ProducesResponseType(typeof(List<UserAccessRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserAccessRequest>> HentAlleForesporsler([FromQuery] int institusjonId)
        {
            try
            {
                var bruker = await _brukerservice.GetUser();
                var response = await _mediator.Send(new HentAlleForesporsler.Query
                {
                    InstitusjonId = institusjonId
                });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Henter Forespørsler som er ikke godkjent.
        /// </summary>
        /// <returns></returns>
        [HttpGet("foresporslersomventerpagodkjenning")]
        [Authorize(HandhygienePolicy.Coordinator)]
        [ProducesResponseType(typeof(List<UserAccessRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserAccessRequest>> HentForesporslerSomVenterPaGodkjenning([FromQuery] int institusjonId)
        {
            try
            {
                var response = await _mediator.Send(new HentForesporslerSomVenterPaGodkjenning.Query()
                {
                    InstitusjonId = institusjonId
                });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("godkjennforesporsel")]
        [Authorize(HandhygienePolicy.Coordinator)]
        public async Task<ActionResult<bool>> GodkjennForesporsel([FromQuery] int foresporselId)
        {
            try
            {
                var bruker = await _brukerservice.GetUser();
                
                var response = await _mediator.Send(new OpprettBrukerFraForesporsel.Command()
                {
                    ForespørselId = foresporselId,
                    IdentPseudonym = bruker.IdentityPseudonym,
                    HPRNummer = bruker.HPRNumber
                });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("avvisforesporsel")]
        [Authorize(HandhygienePolicy.Coordinator)]
        public async Task<ActionResult<bool>> AvvisForesporsel([FromQuery] int foresporselId)
        {
            try
            {
                var bruker = await _brukerservice.GetUser();

                var response = await _mediator.Send(new AvvisForesporsel.Command()
                {
                    ForespørselId = foresporselId,
                    IdentPseudonym = bruker.IdentityPseudonym,
                    HPRNummer = bruker.HPRNumber
                });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}