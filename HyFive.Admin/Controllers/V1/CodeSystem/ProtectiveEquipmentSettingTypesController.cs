using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.ProtectiveEquipment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{

    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/protectiveEquipmentSettingTypes")]
    public class ProtectiveEquipmentSettingTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProtectiveEquipmentSettingTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get ProtectiveEquipmentSettingTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ProtectiveEquipmentSettingType>> GetProtectiveEquipmentSettingTypes()
        {
            return await _mediator.Send(new GetProtectiveEquipmentSettingTypes.Query());
        }

        /// <summary>
        /// Update ProtectiveEquipmentSettingType
        /// </summary>
        /// <param name="settingType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpPut("update")]
        public async Task<ProtectiveEquipmentSettingType> UpdateProtectionEquipmentSettingType([FromBody] ProtectiveEquipmentSettingType settingType)
        {
            return await _mediator.Send(new UpdateProtectiveEquipmentSettingType.Command
            {
                SettingType = settingType
            });
        }
    }
}
