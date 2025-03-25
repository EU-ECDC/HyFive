using HyFive.Modeller.Sesjon;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Sesjon;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Requirements;

namespace HyFive.Observasjon.Controllers.V1
{
    [Authorize(HandhygienePolicy.Observator)]
    [Route("api/v1/sesjon")]
    public class SesjonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBrukerService _brukerservice;

        public SesjonController(IMediator mediator, IBrukerService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        [HttpGet]
        public async Task<List<SesjonRapport>> HentSesjoner()
        {
            
            var resultat = await _mediator.Send(new HentMineSesjoner.Query()
            {
                HPRNummer = _brukerservice.HentHprnummer(),
                Pseudonym = _brukerservice.HentPseudonym()
            });
            return resultat;
        }

        [HttpGet("fireindikasjoner", Name = "HentFireIndikasjonerSesjon")]
        public async Task<FireIndikasjonerSesjon> HentFireIndikasjonerSesjon([FromQuery] Guid sesjonId)
        {
            var sesjon = await _mediator.Send(new HentFireIndikasjonerSesjon.Query()
            {
                HPRNummer = _brukerservice.HentHprnummer(),  
                Pseudonym =  _brukerservice.HentPseudonym(), 
                SesjonId = sesjonId
            });
            return sesjon;
        }

        [HttpGet("handsmykker", Name = "HentHandsmykkeSesjon")]
        public async Task<HandsmykkeSesjon> HentHandsmykkeSesjon([FromQuery] Guid sesjonId)
        {
            var sesjon = await _mediator.Send(new HentHandsmykkeSesjon.Query()
            {
                HPRNummer = _brukerservice.HentHprnummer(), 
                Pseudonym = _brukerservice.HentPseudonym(),
                SesjonId = sesjonId
            });
            return sesjon;
        }

        [HttpGet("beskyttelsesutstyr", Name = "HentBeskyttelsesutstyrSesjon")]
        public async Task<BeskyttelsesutstyrSesjon> HentBeskyttelsesutstyrSesjon([FromQuery] Guid sesjonId)
        {
            var sesjon = await _mediator.Send(new HentBeskyttelsesutstyrSesjon.Query()
            {
                HPRNummer = _brukerservice.HentHprnummer(),
                Pseudonym = _brukerservice.HentPseudonym(),
                SesjonId = sesjonId
            });
            return sesjon;
        }

        [HttpGet("hanske", Name = "HentHanskeSesjon")]
        public async Task<HanskeSesjon> HentHanskeSesjon([FromQuery] Guid sesjonId)
        {
            var sesjon = await _mediator.Send(new HentHanskeSesjon.Query
            {
                HPRNummer = _brukerservice.HentHprnummer(),
                Pseudonym = _brukerservice.HentPseudonym(),
                SesjonId = sesjonId
            });

            return sesjon;
        }
    }
}
