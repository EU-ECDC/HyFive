using HyFive.Modeller.V1.User;
using HyFive.Modeller.V1.Institution;
using HyFive.Services.Institution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
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
        /// Hent alle tilgjengelige institusjoner <see cref="InstitutionReport"/>
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IEnumerable<InstitutionReport>> HentInstitusjoner()
        {
            if (_brukerservice.IsFhiAdmin())
            {
                return await _mediator.Send(new GetInstitutions.Query());
            }
            return await _mediator.Send(new GetInstitutionsForCoordinator.Query() { CoordinatorHprNumber = _brukerservice.GetHprNumber(), CoordinatorPseudonym = _brukerservice.GetPseudonym()});
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpGet("hentInstitusjonerForKoordinator")]
        public async Task<IEnumerable<InstitutionReport>> HentInstitusjonerForKoordinator()
        {
            return await _mediator.Send(new GetInstitutionsForCoordinator.Query() { CoordinatorHprNumber = _brukerservice.GetHprNumber(), CoordinatorPseudonym = _brukerservice.GetPseudonym() });
        }


        /// <summary>
        /// Hent én institusjon
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "HentInstitusjon")]
        public async Task<IActionResult> HentInstitusjon(int id)
        {
            if (_brukerservice.IsCoordinatorForInstitutionOrFhiAdmin(id))
            {
                var resultat = await _mediator.Send(new GetInstitution.Query() { InstitutionId = id });
                return Ok(resultat);
            }
            return Unauthorized();
        }



        /// <summary>
        /// Hent alle avdelinger tilhørende institusjon
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/avdelinger", Name = "HentAvdelinger")]
        public async Task<ActionResult<IEnumerable<Department>>> HentAvdelinger(int id)
        {
            if (_brukerservice.IsFhiAdminOrCoordinator(id))
            {
                var resultat = await _mediator.Send(new GetDepartmentsForInstitution.Query() { InstitutionId = id });
                return Ok(resultat);
            }

            return Unauthorized();
        }


        /// <summary>
        /// Hent alle observatører tilhørende institusjon
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/observatorer", Name = "HentObservatorer")]
        public async Task<ActionResult<IEnumerable<User>>> HentObservatorer(int id)
        {
            if (_brukerservice.IsCoordinatorForInstitutionOrFhiAdmin(id))
            {
                var resultat = await _mediator.Send(new GetObserversForInstitution.Query() { InstitutionId = id });
                return Ok(resultat);
            }

            return Unauthorized();
        }



        /// <summary>
        /// Hent alle koordinatorer tilhørende institusjon
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/koordinatorer", Name = "HentKoordinatorer")]
        public async Task<ActionResult<IEnumerable<User>>> HentKoordinatorer(int id)
        {
            if (_brukerservice.IsCoordinatorForInstitutionOrFhiAdmin(id))
            {
                var resultat = await _mediator.Send(new GetCoordinatorsForInstitution.Query() { InstitutionId = id });
                return Ok(resultat);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Hent  <see cref="InstitutionType"/>r
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpGet("typer")]
        public async Task<IEnumerable<InstitutionType>> HentInstitusjontyper()
        {
            var result = await _mediator.Send(new GetInstitutionTypes.Query());
            return result;
        }

        /// <summary>
        /// Opprett institusjon med koordinator
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPost("opprett")]
        [ProducesResponseType(typeof(Institution), StatusCodes.Status201Created)]
        public async Task<ActionResult<Institution>> OpprettInstitusjon([FromBody] CreateInstitutionRequest request)
        {
            var result = await _mediator.Send(new CreateInstitution.Command() { Request = request });
            return CreatedAtRoute("HentInstitusjon", new { id = result.Id }, result);
        }

        /// <summary>
        /// Oppdaterer en institusjon. Ignorerer eventuelle avdelinger eller roller som sendes med.
        /// </summary>
        /// <param name="institusjon"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<Institution> OppdaterInstitusjon([FromBody] Institution institusjon)
        {
            var result = await _mediator.Send(new UpdateInstitution.Command() { Institution = institusjon });
            return result;
        }

        /// <summary>
        /// Slett institusjon og alle tilhørende sessions, observasjoner, avdelinger, roller og brukere
        /// </summary>
        /// <param name="institusjonId"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpDelete("slett")]
        public async Task<bool> SlettInstitusjon([FromQuery] int institusjonId)
        {
            var result = await _mediator.Send(new DeleteInstitution.Command()
            {
                InstitutionId = institusjonId
            });
            return result;
        }
    }
}
