using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.ProtectiveEquipment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdmin)]
    [Route("api/v1/protectiveEquipmentTypes")]
    public class ProtectiveEquipmentTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProtectiveEquipmentTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get ProtectiveEquipmentTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ProtectiveEquipmentType>> GetProtectiveEquipmentTypes()
        {
            return await _mediator.Send(new GetProtectiveEquipmentTypes.Query());
        }

        /// <summary>
        /// Update ProtectiveEquipmentType
        /// </summary>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<ProtectiveEquipmentType> UpdateProtectiveEquipmentType([FromBody] ProtectiveEquipmentType equipmentType)
        {
            return await _mediator.Send(new UpdateProtectiveEquipmentType.Command
            {
                EquipmentType = equipmentType
            });
        }

        /// <summary>
        /// Get MisuseType
        /// </summary>
        /// <param name="equipmentTypeId"></param>
        /// <returns></returns>
        [HttpGet("misuseTypes", Name = "GetMisuseTypes")]
        public async Task<List<MisuseType>> GetMisuseTypes([FromQuery] int equipmentTypeId)
        {
            return await _mediator.Send(new GetMisuseTypes.Query
            {
                EquipmentTypeId = equipmentTypeId
            });
        }

        /// <summary>
        /// Update MisuseType
        /// </summary>
        /// <param name="equipmentTypeId"></param>
        /// <param name="misuseType"></param>
        /// <returns></returns>
        [HttpPut("misuseTypes/update")]
        public async Task<MisuseType> UpdateMisuseType([FromQuery] int equipmentTypeId, [FromBody] MisuseType misuseType)
        {
            return await _mediator.Send(new UpdateMisuseType.Command
            {
                EquipmentTypeId = equipmentTypeId,
                MisuseType = misuseType
            });
        }

        /// <summary>
        /// Create MisuseType
        /// </summary>
        /// <param name="equipmentTypeId"></param>
        /// <param name="misuseType"></param>
        /// <returns></returns>
        [HttpPost("misuseTypes/create")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        public async Task<ActionResult<bool>> CreateMisuseType([FromQuery] int equipmentTypeId, [FromBody] CreateIncorrectUseTypeRequest misuseType)
        {
            var isCreated =  await _mediator.Send(new CreateMisuseType.Command
            {
                EquipmentTypeId = equipmentTypeId,
                MisuseType = misuseType
            });

            return CreatedAtRoute("GetMisuseTypes", new { equipmentTypeId = equipmentTypeId }, isCreated);
        }
    }
}
