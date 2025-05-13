using HyFive.Models.Session;
using HyFive.Models.V1.Session;
using HyFive.Services.Session;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;

namespace HyFive.Observation.Controllers.V1
{
    [Authorize(HandhygienePolicy.Observer)]
    [Route("api/v1/session")]
    public class SessionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public SessionController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        [HttpGet]
        public async Task<List<SessionReport>> GetSessions()
        {
            
            var result = await _mediator.Send(new GetMySessions.Query()
            {
                HPRNumber = _userService.GetHprNumber(),
                Pseudonym = _userService.GetPseudonym()
            });
            return result;
        }

        [HttpGet("fourIndications", Name = "GetFourIndicationsSession")]
        public async Task<FourIndicationsSession> GetFourIndicationsSession([FromQuery] Guid sesjonId)
        {
            var sesion = await _mediator.Send(new GetFourIndicationsSession.Query()
            {
                HPRNumber = _userService.GetHprNumber(),  
                Pseudonym =  _userService.GetPseudonym(), 
                SessionId = sesjonId
            });
            return sesion;
        }

        [HttpGet("handJewelry", Name = "GetHandJewelrySession")]
        public async Task<HandJewelrySession> GetHandJewelrySession([FromQuery] Guid sessionId)
        {
            var sesjon = await _mediator.Send(new GetHandJewelrySession.Query()
            {
                HPRNumber = _userService.GetHprNumber(), 
                Pseudonym = _userService.GetPseudonym(),
                SessionId = sessionId
            });
            return sesjon;
        }

        [HttpGet("protectiveEquipment", Name = "GetProtectiveEquipmentSession")]
        public async Task<ProtectiveEquipmentSession> GetProtectiveEquipmentSession([FromQuery] Guid sesjonId)
        {
            var sesjon = await _mediator.Send(new GetProtectiveEquipmentSession.Query()
            {
                HPRNumber = _userService.GetHprNumber(),
                Pseudonym = _userService.GetPseudonym(),
                SessionId = sesjonId
            });
            return sesjon;
        }

        [HttpGet("glove", Name = "GetGloveSession")]
        public async Task<GloveSession> GetGloveSession([FromQuery] Guid sessionId)
        {
            var session = await _mediator.Send(new GetGloveSession.Query
            {
                HPRNumber = _userService.GetHprNumber(),
                Pseudonym = _userService.GetPseudonym(),
                SessionId = sessionId
            });

            return session;
        }
    }
}
