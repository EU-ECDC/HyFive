using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institution;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Region;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdmin)]
    [Route("api/v1/regioner")]
    public class RegionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent Regiontyper
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Region>>> HentRegionTyper()
        {
            var regioner = await _mediator.Send(new HentRegioner.Query());
            return Ok(regioner);
        }

        /// <summary>
        /// Hent Regiontype
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpGet("{id}", Name = "HentRegion")]
        public async Task<ActionResult<Region>> HentRegion(int id)
        {
            var region = await _mediator.Send(new HentRegion.Query() { Id = id });
            return Ok(region);
        }

        /// <summary>
        /// Opprett Regiontype
        /// </summary>
        /// <param name="nyRegionsType"></param>
        /// <returns></returns>
        [HttpPost("opprett")]
        public async Task<ActionResult<Region>> OpprettRegionType([FromBody] CreateRegionRequest nyRegionsType)
        {
            try
            {
                var opprettetRegion = await _mediator.Send(new OpprettRegion.Command
                {
                    NyRegion = nyRegionsType
                });

                return CreatedAtRoute("HentRegion", new { id = opprettetRegion.Id }, opprettetRegion);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Oppdater Regiontype
        /// </summary>
        /// <param name="regionType"></param>
        /// <returns></returns>
        [HttpPut("oppdater")]
        public async Task<ActionResult<Region>> OppdaterRegionType([FromBody] Region regionType)
        {
            var oppdatertRegion = await _mediator.Send(new OppdaterRegion.Command
            {
                RegionType = regionType
            });
            return Ok(oppdatertRegion);
        }
    }
}
