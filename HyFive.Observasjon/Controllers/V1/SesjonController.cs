using HyFive.Models.Session;
using HyFive.Models.V1.Session;
using HyFive.Services.Sesjon;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;

namespace HyFive.Observasjon.Controllers.V1
{
    [Authorize(HandhygienePolicy.Observer)]
    [Route("api/v1/sesjon")]
    public class SesjonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _brukerservice;

        public SesjonController(IMediator mediator, IUserService brukerservice)
        {
            _mediator = mediator;
            _brukerservice = brukerservice;
        }

        [HttpGet]
        public async Task<List<SessionReport>> HentSesjoner()
        {
            
            var resultat = await _mediator.Send(new HentMineSesjoner.Query()
            {
                HPRNummer = _brukerservice.GetHprNumber(),
                Pseudonym = _brukerservice.GetPseudonym()
            });
            return resultat;
        }

        [HttpGet("fireindikasjoner", Name = "HentFireIndikasjonerSesjon")]
        public async Task<FourIndicationsSession> HentFireIndikasjonerSesjon([FromQuery] Guid sesjonId)
        {
            var sesjon = await _mediator.Send(new HentFireIndikasjonerSesjon.Query()
            {
                HPRNummer = _brukerservice.GetHprNumber(),  
                Pseudonym =  _brukerservice.GetPseudonym(), 
                SesjonId = sesjonId
            });
            return sesjon;
        }

        [HttpGet("handsmykker", Name = "HentHandsmykkeSesjon")]
        public async Task<HandJewelrySession> HentHandsmykkeSesjon([FromQuery] Guid sesjonId)
        {
            var sesjon = await _mediator.Send(new HentHandsmykkeSesjon.Query()
            {
                HPRNummer = _brukerservice.GetHprNumber(), 
                Pseudonym = _brukerservice.GetPseudonym(),
                SesjonId = sesjonId
            });
            return sesjon;
        }

        [HttpGet("beskyttelsesutstyr", Name = "HentBeskyttelsesutstyrSesjon")]
        public async Task<ProtectiveEquipmentSession> HentBeskyttelsesutstyrSesjon([FromQuery] Guid sesjonId)
        {
            var sesjon = await _mediator.Send(new HentBeskyttelsesutstyrSesjon.Query()
            {
                HPRNummer = _brukerservice.GetHprNumber(),
                Pseudonym = _brukerservice.GetPseudonym(),
                SesjonId = sesjonId
            });
            return sesjon;
        }

        [HttpGet("hanske", Name = "HentHanskeSesjon")]
        public async Task<GloveSession> HentHanskeSesjon([FromQuery] Guid sesjonId)
        {
            var sesjon = await _mediator.Send(new HentHanskeSesjon.Query
            {
                HPRNummer = _brukerservice.GetHprNumber(),
                Pseudonym = _brukerservice.GetPseudonym(),
                SesjonId = sesjonId
            });

            return sesjon;
        }
    }
}
