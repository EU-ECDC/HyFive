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
                Email = _userService.GetEmail()
            });
            return result;
        }

        [HttpGet("fiveIndications", Name = "GetFiveIndicationsSession")]
        public async Task<FiveIndicationsSession> GetFiveIndicationsSession([FromQuery] Guid sessionId)
        {
            var session = await _mediator.Send(new GetFiveIndicationsSession.Query()
            {
                Email = _userService.GetEmail(),  
                SessionId = sessionId
            });
            return session;
        }

        [HttpGet("handJewelry", Name = "GetHandJewelrySession")]
        public async Task<HandJewelrySession> GetHandJewelrySession([FromQuery] Guid sessionId)
        {
            var session = await _mediator.Send(new GetHandJewelrySession.Query()
            {
                Email = _userService.GetEmail(), 
                SessionId = sessionId
            });
            return session;
        }

        [HttpGet("protectiveEquipment", Name = "GetProtectiveEquipmentSession")]
        public async Task<ProtectiveEquipmentSession> GetProtectiveEquipmentSession([FromQuery] Guid sessionId)
        {
            var session = await _mediator.Send(new GetProtectiveEquipmentSession.Query()
            {
                Email = _userService.GetEmail(),
                SessionId = sessionId
            });
            return session;
        }

        [HttpGet("glove", Name = "GetGloveSession")]
        public async Task<GloveSession> GetGloveSession([FromQuery] Guid sessionId)
        {
            var session = await _mediator.Send(new GetGloveSession.Query
            {
                Email = _userService.GetEmail(),
                SessionId = sessionId
            });

            return session;
        }
    }
}
