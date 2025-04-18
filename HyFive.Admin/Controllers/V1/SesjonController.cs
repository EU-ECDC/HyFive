using System;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Constants;
using HyFive.Modeller.V1.Session;
using HyFive.Services.Authentication.User;
using HyFive.Services.Session;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Route("api/v1/sesjon")]
    public class SesjonController : ControllerBase
    {
        private readonly IUserService _brukerservice;
        private readonly IMediator _mediator;

        public SesjonController(IUserService brukerservice, IMediator mediator)
        {
            _brukerservice = brukerservice;
            _mediator = mediator;
        }

        [Route("slett/{sesjonId}")]
        [HttpDelete]
        public async Task<IActionResult> SlettSesjon(Guid sesjonId, [FromQuery] int institusjonId)
        {
            if (_brukerservice.IsFhiAdminOrCoordinator(institusjonId))
            {
                var resultat = await _mediator.Send(new DeleteSession.Command
                {
                    InstitutionId = institusjonId,
                    TransferStatusCode = TransferStatusTypeConstants.TransferredToCoordinator,
                    SesjonId = sesjonId
                });

                return Ok(resultat.Suksess);
            }

            return Forbid();
        }

        [Route("oppdater")]
        [HttpPut]
        public async Task<IActionResult> OppdaterSesjon([FromBody] OppdaterSesjonRequest sesjon)
        {
            if (_brukerservice.ErKoordinatorForInstitusjon(sesjon.InstitutionId))
            {
                var resultat = await _mediator.Send(new UpdateSession.Command
                {
                    SesjonId = sesjon.SesjonId,
                    InstitutionId = sesjon.InstitutionId,
                    Comment = sesjon.Kommentar
                });

                return Ok(resultat.Suksess);
            }

            return Forbid();
        }
    }
}
