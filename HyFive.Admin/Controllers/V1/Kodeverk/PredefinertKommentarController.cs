using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institusjon;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.Institusjon;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/predefinertkommentar")]
    public class PredefinertKommentarController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public PredefinertKommentarController(IMediator mediator, IBrukerService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Hent predefinert kommentarer
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "HentPredefinertKommentarer")]
        public async Task<ActionResult<List<PredefinertKommentar>>> HentPredefinertKommentar(int institusjonId)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(institusjonId))
            {
                return await _mediator.Send(new HentPredefinertKommentarerForKoordinator.Query
                {
                    Institusjonid = institusjonId
                });
            }
            return Unauthorized();
        }

        /// <summary>
        /// Oppdater predefinert kommentar
        /// </summary>
        /// <param name="predefinertKommentar"></param>
        /// <param name="institusjonId"></param>
        /// <returns></returns>
        [HttpPut("{institusjonId}/oppdater")]
        public async Task<ActionResult<bool>> OppdaterPredefinertKommentar(
            int institusjonId,
            [FromBody] PredefinertKommentar predefinertKommentar)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(institusjonId))
            {
                var erOppdatert = await _mediator.Send(new OppdaterPredefinertKommentar.Command
                {
                    PredefinertKommentar = predefinertKommentar,
                    Institusjonid = institusjonId
                });
                return erOppdatert;
            }

            return Unauthorized();
        }

        /// <summary>
        /// Opprett predefinert kommentar
        /// </summary>
        /// <param name="nyPredefinertKommentar"></param>
        /// <param name="institusjonId"></param>
        /// <returns></returns>
        [HttpPost("{institusjonId}/Opprett")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        public async Task<ActionResult<bool>> OpprettPredefinertKommentar(
            int institusjonId,
            [FromBody] OpprettPredefinertKommentarRequest nyPredefinertKommentar)
        {
            if (_brukerservice.ErKoordinatorForInstitusjonEllerFhiAdmin(institusjonId))
            {
                var erOpprettet = await _mediator.Send(new OpprettPredefinertKommentar.Command
                {
                    NyPredefinertKommentar = nyPredefinertKommentar,
                    Institusjonid = institusjonId
                });

                return CreatedAtRoute("HentPredefinertKommentarer", erOpprettet);
            }
            return Unauthorized();
        }
    }
}
