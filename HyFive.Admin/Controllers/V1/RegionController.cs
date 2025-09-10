using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.Institution;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Region;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.Admin)]
    [Route("api/v1/regions")]
    public class RegionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Region Types
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Region>>> GetRegionTypes()
        {
            var regions = await _mediator.Send(new GetRegions.Query());
            return Ok(regions);
        }

        /// <summary>
        /// Get Region type
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpGet("{id}", Name = "GetRegion")]
        public async Task<ActionResult<Region>> GetRegion(int id)
        {
            var region = await _mediator.Send(new GetRegion.Query() { Id = id });
            return Ok(region);
        }

        /// <summary>
        /// Create Region type
        /// </summary>
        /// <param name="newRegionType"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult<Region>> CreateRegionType([FromBody] CreateRegionRequest newRegionType)
        {
            try
            {
                var createdRegion = await _mediator.Send(new CreateRegion.Command
                {
                    NewRegion = newRegionType
                });

                return CreatedAtRoute("GetRegion", new { id = createdRegion.Id }, createdRegion);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update Region type
        /// </summary>
        /// <param name="regionType"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<ActionResult<Region>> OppdaterRegionType([FromBody] Region regionType)
        {
            var updatedRegion = await _mediator.Send(new UpdateRegion.Command
            {
                RegionType = regionType
            });
            return Ok(updatedRegion);
        }
    }
}
