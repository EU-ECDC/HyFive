using HyFive.Services.Authentication.Requirements;
using HyFive.Services.RegionalHealthOrganization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdmin)]
    [Route("api/v1/regionalHealthcareOrganizations")]
    [ApiController]
    public class RegionalHealthcareOrganizationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegionalHealthcareOrganizationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Models.V1.Institution.RegionalHealthcareOrganization>>> GetAllRegionalHealthcareOrganizations()
        {
            var result = await  _mediator.Send(new GetAllRegionalHealthOrganization.Query());

            return Ok(result);
        }
    }
}
