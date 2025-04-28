using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Municipality;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdmin)]
    [Route("api/v1/municipality")]
    [ApiController]
    public class MunicipalityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MunicipalityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Models.V1.Institution.Comment>>> GetMunicipalities()
        {
            var result = await _mediator.Send(new GetMunicipalities.Query());

            return Ok(result);
        }
    }
}
