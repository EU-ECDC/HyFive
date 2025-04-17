using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institution;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Institution;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/predefinertkommentar")]
    public class PredefinertKommentarController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _brukerservice;

        public PredefinertKommentarController(IMediator mediator, IUserService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        /// <summary>
        /// Hent predefinert kommentarer
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "HentPredefinertKommentarer")]
        public async Task<ActionResult<List<PredefinedComment>>> HentPredefinertKommentar(int institusjonId)
        {
            if (_brukerservice.IsCoordinatorForInstitutionOrFhiAdmin(institusjonId))
            {
                return await _mediator.Send(new GetPredefinedCommentsForCoordinator.Query
                {
                    InstitutionId = institusjonId
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
            [FromBody] PredefinedComment predefinertKommentar)
        {
            if (_brukerservice.IsCoordinatorForInstitutionOrFhiAdmin(institusjonId))
            {
                var erOppdatert = await _mediator.Send(new UpdatePredefinedComment.Command
                {
                    PredefinedComment = predefinertKommentar,
                    InstitutionId = institusjonId
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
            [FromBody] CreatePredefinedCommentRequest nyPredefinertKommentar)
        {
            if (_brukerservice.IsCoordinatorForInstitutionOrFhiAdmin(institusjonId))
            {
                var erOpprettet = await _mediator.Send(new CreatePredefinedComment.Command
                {
                    NewPredefinedComment = nyPredefinertKommentar,
                    InstitutionId = institusjonId
                });

                return CreatedAtRoute("HentPredefinertKommentarer", erOpprettet);
            }
            return Unauthorized();
        }
    }
}
