using System;
using System.Threading.Tasks;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1.Bruker;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.Bruker;
using HyFive.Tjenester.BrukerTjenester;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bruker = HyFive.Modeller.V1.Bruker.Bruker;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/bruker")]
    public class BrukerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public BrukerController(IMediator mediator, IBrukerService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        #region Observator

        /// <summary>
        /// Oppdaterer en observatør.
        /// </summary>
        /// <param name="bruker"></param>
        /// <returns></returns>
        [HttpPut("observator/oppdater")]
        public async Task<IActionResult> OppdaterObservator([FromBody] Bruker bruker)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(bruker.InstitusjonId))
            {
                var oppdatertObservator = await _mediator.Send(new OppdaterObservator.Command() { Bruker = bruker });
                return Ok(oppdatertObservator);
            }

            return Unauthorized();
        }


        /// <summary>
        /// Oppretter en observatør.
        /// </summary>
        /// <param name="bruker"></param>
        /// <returns></returns>
        [HttpPost("observator/opprett")]
        [ProducesResponseType(typeof(Bruker), StatusCodes.Status201Created)]
        public async Task<ActionResult<Bruker>> OpprettObservator([FromBody] Bruker bruker)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(bruker.InstitusjonId))
            {
                var response = await _mediator.Send(new OpprettObservator.Command() { Bruker = bruker });
                return CreatedAtRoute("HentObservatorer", new { id = response.InstitusjonId }, response);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Sletter en observatør.
        /// </summary>
        /// <param name="observatorId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpDelete("observator/slett")]
        public async Task<ActionResult<bool>> SlettObservator([FromQuery] int observatorId)
        {
            if (_brukerservice.ErFhiAdmin())
            {
                var result = await _mediator.Send(new SlettBruker.Command()
                {
                    BrukerId = observatorId,
                    Brukertype = typeof(Observator)
                });
                return Ok(result);
            }

            return Unauthorized();
        }

        [Route("observator/harOverfortSesjonTilFHI")]
        [HttpGet]
        public async Task<IActionResult> HarOverfortSesjonTilFHI([FromQuery] int observatorId)
        {
            var resultat = await _mediator.Send(new HarOverfortSesjonTilFHI.Command
            {
                ObervasjonsId = observatorId
            });

            return Ok(resultat);
        }

        #endregion

        #region Koordinator

        /// <summary>
        /// Oppretter en koordinator.
        /// </summary>
        /// <param name="bruker"></param>
        /// <returns></returns>
        [HttpPost("koordinator/opprett")]
        [ProducesResponseType(typeof(Bruker), StatusCodes.Status201Created)]
        public async Task<ActionResult<Bruker>> OpprettKoordinator([FromBody] Bruker bruker)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(bruker.InstitusjonId))
            {
                var response = await _mediator.Send(new OpprettKoordinator.Command() { Bruker = bruker });
                return CreatedAtRoute("HentKoordinatorer", new { id = response.InstitusjonId }, response);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Oppdaterer en koordinator.
        /// </summary>
        /// <param name="bruker"></param>
        /// <returns></returns>
        [HttpPut("koordinator/oppdater")]
        public async Task<ActionResult<Bruker>> OppdaterKoordinator([FromBody] Bruker bruker)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(bruker.InstitusjonId))
            {
                var oppdatertBruker = await _mediator.Send(new OppdaterKoordinator.Command() { Bruker = bruker });
                return oppdatertBruker;
            }

            return Unauthorized();
        }


        /// <summary>
        /// Sletter en koordinator.
        /// </summary>
        /// <param name="koordinatorId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpDelete("koordinator/slett")]
        public async Task<ActionResult<bool>> SlettKoordinator([FromQuery] int koordinatorId)
        {
            if (_brukerservice.ErFhiAdmin())
            {
                var result = await _mediator.Send(new SlettBruker.Command()
                {
                    BrukerId = koordinatorId,
                    Brukertype = typeof(Koordinator)
                });
                return result;
            }

            return Unauthorized();
        }

        #endregion

        #region FhiAdmin

        /// <summary>
        /// Henter alle FhiAdmin.
        /// </summary>
        /// <returns></returns>
        [HttpGet("fhiadmin")]
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [ProducesResponseType(typeof(Bruker), StatusCodes.Status200OK)]
        public async Task<ActionResult<Bruker>> HentFhiAdmin()
        {
            try
            {
                var response = await _mediator.Send(new HentFhiAdmin.Query() { });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Oppretter en FhiAdmin.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("fhiadmin")]
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [ProducesResponseType(typeof(Bruker), StatusCodes.Status201Created)]
        public async Task<ActionResult<Bruker>> OpprettFhiAdmin([FromBody] OpprettFhiAdminRequest request)
        {
            try
            {
                var response = await _mediator.Send(new OpprettFhiAdmin.Command() { Request = request });
                return Ok(response);
                //return CreatedAtRoute("HentFhiAdmin", new { id = response.InstitusjonId }, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Oppdaterer en FhiAdmin.
        /// </summary>
        /// <param name="bruker"></param>
        /// <returns></returns>
        [HttpPut("fhiadmin")]
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [ProducesResponseType(typeof(Bruker), StatusCodes.Status201Created)]
        public async Task<ActionResult<Bruker>> OpprettFhiAdmin([FromBody] Bruker bruker)
        {
            try
            {
                var response = await _mediator.Send(new OppdaterFhiAdmin.Command() { Bruker = bruker });
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion
    }
}
