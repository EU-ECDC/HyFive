using System;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Sesjon;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Route("api/v1/sesjon")]
    public class SesjonController : ControllerBase
    {
        private readonly IBrukerService _brukerservice;
        private readonly IMediator _mediator;

        public SesjonController(IBrukerService brukerservice, IMediator mediator)
        {
            _brukerservice = brukerservice;
            _mediator = mediator;
        }

        [Route("slett/{sesjonId}")]
        [HttpDelete]
        public async Task<IActionResult> SlettSesjon(Guid sesjonId, [FromQuery] int institusjonId)
        {
            if (_brukerservice.ErKoordinatorForInstitusjon(institusjonId))
            {
                var resultat = await _mediator.Send(new SlettSesjon.Command
                {
                    InstitusjonId = institusjonId,
                    OverforingstatusKode = OverforingstatusTypeKonstanter.OverfortTilKoordinator,
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
            if (_brukerservice.ErKoordinatorForInstitusjon(sesjon.InstitusjonId))
            {
                var resultat = await _mediator.Send(new OppdaterSesjon.Command
                {
                    SesjonId = sesjon.SesjonId,
                    InstitusjonId = sesjon.InstitusjonId,
                    Kommentar = sesjon.Kommentar
                });

                return Ok(resultat.Suksess);
            }

            return Forbid();
        }
    }
}
