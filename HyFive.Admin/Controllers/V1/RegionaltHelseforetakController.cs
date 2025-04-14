using HyFive.Services.Authentication.Requirements;
using HyFive.Services.RegionaltHelseforetak;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdmin)]
    [Route("api/v1/regionalthelseforetak")]
    [ApiController]
    public class RegionaltHelseforetakController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegionaltHelseforetakController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Modeller.V1.Institution.RegionalInstitution>>> HentAlleRegionaltHelseforetak()
        {
            var result = await  _mediator.Send(new HentAlleRegionaltHelseforetak.Query());

            return Ok(result);
        }
    }
}
