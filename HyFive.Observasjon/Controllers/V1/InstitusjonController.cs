using HyFive.Models.V1.Institution;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Institusjon;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyFive.Observasjon.Controllers.V1
{
    // [Authorize(HandhygienePolicy.Observer)]
    [Route("api/v1/institusjon")]
    public class InstitusjonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _brukerservice;

        public InstitusjonController(IMediator mediator, IUserService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Hent brukerens tilgjengelige institusjoner
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IEnumerable<Institution>> HentObservatorensInstitusjoner()
        {
            var result = await _mediator.Send(new HentInstitusjonerForObservator.Query() { HPRNummer = _brukerservice.GetHprNumber(), Pseudonym = _brukerservice.GetPseudonym()});
            return result;
        }

        /// <summary>
        /// Hent predefinerte kommentarer for gitt institusjon og sesjonstype
        /// </summary>
        /// <param name="institusjonid"></param>
        /// <param name="sesjontype"></param>
        /// <returns></returns>
        [HttpGet("predefinertekommentarer")]
        public async Task<ActionResult<IEnumerable<string>>> HentPredefinerteKommentarer([FromQuery] int institusjonid, [FromQuery] SessionType sesjontype)
        {
            if (_brukerservice.IsObserverForInstitution(institusjonid))
            {
                var result = await _mediator.Send(new HentPredefinerteKommentarer.Query
                {
                    InstitusjonId = institusjonid,
                    Sesjontype = sesjontype
                });
                return Ok(result);
            }

            return Unauthorized();
        }
    }
}
