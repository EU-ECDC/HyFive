using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Kommune;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdmin)]
    [Route("api/v1/kommune")]
    [ApiController]
    public class KommuneController : ControllerBase
    {
        private readonly IMediator _mediator;

        public KommuneController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Modeller.V1.Institution.Comment>>> HentKommuner()
        {
            var result = await _mediator.Send(new HentKommuner.Query());

            return Ok(result);
        }
    }
}
