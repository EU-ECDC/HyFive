using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Observation.ProtectiveEquipment;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.ProtectiveEquipment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{

    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/beskyttelsesutstyrsettingtyper")]
    public class BeskyttelsesutstyrsettingtyperController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BeskyttelsesutstyrsettingtyperController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent beskyttelsesutstyrsettingtyper
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ProtectiveEquipmentSettingType>> HentBeskyttelsesutstyrsettingTyper()
        {
            return await _mediator.Send(new GetProtectiveEquipmentSettingTypes.Query());
        }

        /// <summary>
        /// Oppdater beskyttelsesutstyrsettingType
        /// </summary>
        /// <param name="settingType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("oppdater")]
        public async Task<ProtectiveEquipmentSettingType> OppdaterBeskyttelsesutstyrsettingType([FromBody] ProtectiveEquipmentSettingType settingType)
        {
            return await _mediator.Send(new UpdateProtectiveEquipmentSettingType.Command
            {
                SettingType = settingType
            });
        }
    }
}
