using HyFive.Modeller.V1.Institution;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Institusjon;
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
        private readonly IBrukerService _brukerservice;

        public InstitusjonController(IMediator mediator, IBrukerService brukerservice)
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
            var result = await _mediator.Send(new HentInstitusjonerForObservator.Query() { HPRNummer = _brukerservice.HentHprnummer(), Pseudonym = _brukerservice.HentPseudonym()});
            return result;
        }

        /// <summary>
        /// Hent predefinerte kommentarer for gitt institusjon og sesjonstype
        /// </summary>
        /// <param name="institusjonid"></param>
        /// <param name="sesjontype"></param>
        /// <returns></returns>
        [HttpGet("predefinertekommentarer")]
        public async Task<ActionResult<IEnumerable<string>>> HentPredefinerteKommentarer([FromQuery] int institusjonid, [FromQuery] SesjonType sesjontype)
        {
            if (_brukerservice.ErObservatorForInstitusjon(institusjonid))
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
