using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.Facility;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Facility;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    /// <summary>
    /// FacilityTypes
    /// </summary>
    [Authorize(HandhygienePolicy.Admin)]
    [Route("api/v1/facilityTypes")]
    public class FacilityTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FacilityTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get FacilityTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<FacilityType>> GetFacilityTypes()
        {
            var facilityTypes = await _mediator.Send(new GetFacilityTypes.Query());
            return facilityTypes;
        }

        /// <summary>
        /// update FacilityType
        /// </summary>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<FacilityType> UpdateFacilityType([FromBody] FacilityType facilityType)
        {
            return await _mediator.Send(new UpdateFacilityType.Command()
            {
                FacilityType = facilityType
            });
        }


        /// <summary>
        /// Create FacilityType
        /// </summary>
        /// <param name="facilityType"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(FacilityType), StatusCodes.Status201Created)]
        public async Task<ActionResult<FacilityType>> CreateFacilityType([FromBody] CreateFacilityTypeRequest facilityType)
        {
            try
            {
                var response = await _mediator.Send(new CreateFacilityType.Command()
                {
                    FacilityType = facilityType
                });

                return CreatedAtRoute("GetFacilityTypes", response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}