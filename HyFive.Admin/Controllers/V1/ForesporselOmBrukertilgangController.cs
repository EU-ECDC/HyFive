using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.ForesporselOmBrukertilgang;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ForesporselOmBrukertilgang = HyFive.Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.Koordinator)]
    [Route("api/v1/foresporselombrukertilgang")]
    public class ForesporselOmBrukertilgangController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public ForesporselOmBrukertilgangController(IMediator mediator, IBrukerService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Henter alle Forespørsler om brukertilgang.
        /// </summary>
        /// <returns></returns>
        [HttpGet("alleforesporsler")]
        [Authorize(HandhygienePolicy.Koordinator)]
        [ProducesResponseType(typeof(List<ForesporselOmBrukertilgang>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ForesporselOmBrukertilgang>> HentAlleForesporsler([FromQuery] int institusjonId)
        {
            try
            {
                var bruker = await _brukerservice.HentBruker();
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
        [Authorize(HandhygienePolicy.Koordinator)]
        [ProducesResponseType(typeof(List<ForesporselOmBrukertilgang>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ForesporselOmBrukertilgang>> HentForesporslerSomVenterPaGodkjenning([FromQuery] int institusjonId)
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
        [Authorize(HandhygienePolicy.Koordinator)]
        public async Task<ActionResult<bool>> GodkjennForesporsel([FromQuery] int foresporselId)
        {
            try
            {
                var bruker = await _brukerservice.HentBruker();
                
                var response = await _mediator.Send(new OpprettBrukerFraForesporsel.Command()
                {
                    ForespørselId = foresporselId,
                    IdentPseudonym = bruker.IdentPseudonym,
                    HPRNummer = bruker.HPRNummer
                });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("avvisforesporsel")]
        [Authorize(HandhygienePolicy.Koordinator)]
        public async Task<ActionResult<bool>> AvvisForesporsel([FromQuery] int foresporselId)
        {
            try
            {
                var bruker = await _brukerservice.HentBruker();

                var response = await _mediator.Send(new AvvisForesporsel.Command()
                {
                    ForespørselId = foresporselId,
                    IdentPseudonym = bruker.IdentPseudonym,
                    HPRNummer = bruker.HPRNummer
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