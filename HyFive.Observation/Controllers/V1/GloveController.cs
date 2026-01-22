using HyFive.Api.Common.ExtensionMethods;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.Gloves;
using HyFive.Models.V1.Report.Glove;
using HyFive.Models.V1.Session;
using HyFive.Observation.Controllers.V1.Shared;
using HyFive.Services;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Authentication.User;
using HyFive.Services.Glove;
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

namespace HyFive.Observation.Controllers.V1
{
    [Route("api/v1/glove")]
    [ApiController]
    public class GloveController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<Services.Localization.Validation> _validation;


        public GloveController(IMediator mediator, IUserService userService, IStringLocalizer<Services.Localization.Validation> validation)
        {
            _mediator = mediator;
            _userService = userService;
            _validation = validation;
        }

        /// <summary>
        /// Save a Glove session
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        //[Authorize(HandhygienePolicy.Observer)]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> SaveSession([FromBody] GloveSession session)
        {
            return await SessionSaveHelper.SaveSessionAsync(
                controller: this,
                session: session,
                routeName: "GetGloveSession",
                mediator: _mediator,
                userService: _userService,
                validation: _validation,
                commandFactory: (s, email) => new HyFive.Services.Glove.SaveSession.Command
                {
                    Email = email,
                    Session = (GloveSession)s
                }
            );
        }

        [HttpGet("gloveWithIndicationType")]
        public async Task<IEnumerable<GloveWithIndicationType>> GetGloveWithIndicationTypes()
        {
            var result = await _mediator.Send(new GetGloveWithIndicationTypes.Query());
            return result;
        }

        [HttpGet("gloveWithoutIndicationType")]
        public async Task<IEnumerable<GloveWithoutIndicationType>> GetGloveWithoutIndicationTypes()
        {
            var result = await _mediator.Send(new GetGloveWithoutIndicationTypes.Query());
            return result;
        }

        [HttpGet("handHygieneAfterGloveUseType")]
        public async Task<IEnumerable<PostGloveHandHygieneType>> GetHandHygieneAfterGloveUseTypes()
        {
            var result = await _mediator.Send(new GetHandHygieneAfterGloveUseTypes.Query());
            return result;
        }

        [HttpGet("myObservations")]
        public async Task<IEnumerable<GloveObservationReport>> GetMyObservations(int facilityId, Guid? sessionId = null)
        {
            var observerIdForFacility = _userService.GetObserverIdForFacility(facilityId);
            if (observerIdForFacility > 0)
            {
                var query = new GetGloveObservations.Query()
                {
                    ObserverId = observerIdForFacility,
                    FacilityId = facilityId,
                    SessionId = sessionId,
                    Role = AuthorizedRole.Observer
                };

                var observations = await _mediator.Send(query);
                return observations;
            }

            throw new DomainException("InstitutionAccessDenied");
        }

        [HttpGet("myObservations/excel")]
        public async Task<IActionResult> GetMyObservationsAsExcel(int facilityId, Guid? sessionId = null)
        {
            var observations = await GetMyObservations(facilityId, sessionId);
            return await this.ExcelFileContentResult(observations, "Observations");
        }
    }
}