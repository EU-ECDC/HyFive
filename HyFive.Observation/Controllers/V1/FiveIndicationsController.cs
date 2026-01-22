using HyFive.Api.Common.ExtensionMethods;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Report.FiveIndications;
using HyFive.Models.V1.Session;
using HyFive.Observation.Controllers.V1.Shared;
using HyFive.Services;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Authentication.User;
using HyFive.Services.FiveIndication;
using HyFive.Services.Localization;
using HyFive.Services.Report.Observations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace HyFive.Observation.Controllers.V1
{

    [Route("api/v1/fiveIndications")]
    public class FiveIndicationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<Services.Localization.Validation> _validation;

        public FiveIndicationsController(IMediator mediator, IUserService userService, IStringLocalizer<Services.Localization.Validation> validation)
        {
            _mediator = mediator;
            _userService = userService;
            _validation = validation;
        }

        /// <summary>
        /// Save a Five Indications session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        //[Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> SaveSession([FromBody] FiveIndicationsSession session)
        {
            return await SessionSaveHelper.SaveSessionAsync(
                controller: this,
                session: session,
                routeName: "GetFiveIndicationsSession",
                mediator: _mediator,
                userService: _userService,
                validation: _validation,
                commandFactory: (s, email) => new HyFive.Services.FiveIndication.SaveSession.Command
                {
                    Email = email,
                    Session = (FiveIndicationsSession)s
                }
            );
        }

        [HttpGet("indicationTypes")]
        public async Task<IEnumerable<IndicationType>> GetIndicationTypes()
        {
            var result = await _mediator.Send(new GetIndicationTypes.Query());
            return result;
        }

        [HttpGet("activityTypes")]
        public async Task<IEnumerable<ActivityType>> GetActivityTypes()
        {
            var result = await _mediator.Send(new GetActivityTypes.Query());
            return result;
        }

        [HttpGet("myObservations")]
        public async Task<IEnumerable<FiveIndicationsObservationReport>> GetMyObservations(int facilityId, Guid? sessionId = null)
        {
            var observerIdForFacility = _userService.GetObserverIdForFacility(facilityId);
            if (observerIdForFacility > 0)
            {
                var query = new GetFiveIndicationsObservations.Query()
                {
                    ObserverId = observerIdForFacility,
                    FacilityId = facilityId,
                    SessionId = sessionId,
                    Role = AuthorizedRole.Coordinator
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
