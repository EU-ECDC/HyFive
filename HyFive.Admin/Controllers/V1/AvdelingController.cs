using System.Collections.Generic;
using HyFive.Modeller.V1.Institusjon;
using HyFive.Tjenester.Avdeling;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Observasjon;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.Roller;
using System;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/avdeling")]
    public class AvdelingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public AvdelingController(IMediator mediator, IBrukerService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Hent avdeling
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Avdeling), StatusCodes.Status200OK)]
        public async Task<ActionResult<Avdeling>> HentAvdeling(int id)
        {
            if (_brukerservice.ErKoordinatorForAvdelingEllerFhiAdmin(id))
            {
                return await _mediator.Send(new HentAvdeling.Query() { Id = id });
            }
            return Unauthorized();

        }

        /// <summary>
        /// Opprett avdeling med roller
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("opprett")]
        [ProducesResponseType(typeof(Avdeling), StatusCodes.Status201Created)]
        public async Task<ActionResult<Avdeling>> OpprettAvdeling([FromBody] OpprettAvdelingRequest request)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(request.InstitusjonId))
            {
                var result = await _mediator.Send(new OpprettAvdeling.Command() { Request = request });
                return CreatedAtRoute("HentAvdelinger", new { id = result.InstitusjonId }, result);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Oppdater avdeling 
        /// </summary>
        /// <param name="avdeling"></param>
        /// <returns></returns>
        [HttpPut("oppdater")]
        public async Task<ActionResult<Avdeling>> OppdaterAvdeling([FromBody] Avdeling avdeling)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(avdeling.InstitusjonId))
            {
                var result = await _mediator.Send(new OppdaterAvdeling.Command()
                {
                    Id = avdeling.Id,
                    AvdelingTypeId = avdeling.AvdelingTypeId,
                    Navn = avdeling.Navn,
                    Roller = avdeling.Roller
                });
                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpGet("avdelingstyper")]
        public async Task<ActionResult<List<AvdelingType>>> HentAvdelingstyper()
        {
            var result = await _mediator.Send(new HentAvdelingTyper.Query() { });
            return Ok(result);
        }

        /// <summary>
        /// Opprett avdelingstype
        /// </summary>
        /// <param name="avdelingType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPost("avdelingstyper/opprett")]
        [ProducesResponseType(typeof(AvdelingType), StatusCodes.Status201Created)]
        public async Task<ActionResult<AvdelingType>> OpprettAvdelingType([FromBody] AvdelingType avdelingType)
        {
            try
            {
                var result = await _mediator.Send(new OpprettAvdelingType.Command() { AvdelingType = avdelingType });

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Oppdater avdelingstype
        /// </summary>
        /// <param name="avdelingType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("avdelingstyper/oppdater")]
        [ProducesResponseType(typeof(AvdelingType), StatusCodes.Status200OK)]
        public async Task<ActionResult<AvdelingType>> OppdaterAvdelingType([FromBody] AvdelingType avdelingType)
        {
            var result = await _mediator.Send(new OppdaterAvdelingType.Command() { AvdelingType = avdelingType });
            return Ok(result);
        }

        /// <summary>
        /// Hent roller
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/roller")]
        [ProducesResponseType(typeof(Rolle), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Rolle>>> HentRoller(int id)
        {
            if (_brukerservice.ErKoordinatorForAvdelingEllerFhiAdmin(id))
            {
                return await _mediator.Send(new HentRollerForAvdeling.Query { AvdelingId = id });
            }
            return Unauthorized();

        }


        /// <summary>
        /// Slett avdeling
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
        [HttpDelete("slett/{id}")]
        public async Task<bool> SlettAvdeling(int id)
        {
            var result = await _mediator.Send(new SlettAvdeling.Command()
            {
                AvdelingId = id
            });
            return result;
        }

        /// <summary>
        /// Sjekker om avdeling har sesjoner overført til FHI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("harOverfortSesjonTilFHI/{id}")]
        public async Task<IActionResult> HarOverfortSesjonTilFHI(int id)
        {

            var result = await _mediator.Send(new HarOverfortSesjonTilFHI.Command
            {
                AvdelingId = id
            });

            return Ok(result);
        }


    }
}
