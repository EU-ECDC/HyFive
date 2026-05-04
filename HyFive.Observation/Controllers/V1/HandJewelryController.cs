using HyFive.Api.Common.ExtensionMethods;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Report.HandJewelry;
using HyFive.Models.V1.Session;
using HyFive.Observation.Controllers.V1.Shared;
using HyFive.Services;
using HyFive.Services.Authentication.User;
using HyFive.Services.HandJewelry;
using HyFive.Services.Report.Observations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyFive.Observation.Controllers.V1
{
    [Route("api/v1/handJewelry")]
    public class HandJewelryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<Services.Localization.Validation> _validation;

        public HandJewelryController(IMediator mediator, IUserService userService, IStringLocalizer<Services.Localization.Validation> validation)
        {
            _mediator = mediator;
            _userService = userService;
            _validation = validation;
        }

        /// <summary>
        /// Save a Hand Jewelry session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        //[Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> SaveSession([FromBody] HandJewelrySession session)
        {
            return await SessionSaveHelper.SaveSessionAsync(
                controller: this,
                session: session,
                routeName: "GetHandJewelrySession",
                mediator: _mediator,
                userService: _userService,
                validation: _validation,
                commandFactory: (s, email) => new HyFive.Services.HandJewelry.SaveSession.Command
                {
                    Email = email,
                    Session = (HandJewelrySession)s
                }
            );
        }

        [HttpGet("getHandJewelryTypes")]
        public async Task<IEnumerable<HandJewelryType>> GetHandJewelryTypes()
        {
            var result = await _mediator.Send(new GetHandJewelryTypes.Query());
            return result;
        }

        [HttpGet("myObservations")]
        public async Task<IEnumerable<HandJewelryObservationReport>> GetMyObservations(int facilityId, Guid? sessionId = null)
        {
            var observerIdForFacility = await _userService.GetObserverIdIfHasAccessToFacility(facilityId);
            if (observerIdForFacility > 0)
            {
                var query = new GetHandJewelryObservations.Query
                {
                    ObserverId = observerIdForFacility,
                    FacilityId = facilityId,
                    SessionId = sessionId,
                    Role = AuthorizedRole.Observer
                };

                var observations = await _mediator.Send(query);
                return observations;
            }

            throw new DomainException("FacilityAccessDenied");
        }

        [HttpGet("myObservations/excel")]
        public async Task<IActionResult> GetMyObservationsAsExcel(int facilityId, Guid? sessionId = null)
        {
            var observations = await GetMyObservations(facilityId, sessionId);
            return await this.ExcelFileContentResult(observations, "Observations");
        }
    }
}
