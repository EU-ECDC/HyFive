using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.Institution;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Institution;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    /// <summary>
    /// InstitutionTypes
    /// </summary>
    [Authorize(HandhygienePolicy.FhiAdmin)]
    [Route("api/v1/institutionTypes")]
    public class InstitutionTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InstitutionTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get InstitutionTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetInstitutionTypes")]
        public async Task<IEnumerable<InstitutionType>> GetInstitutionTypes()
        {
            var institutionTypes = await _mediator.Send(new GetInstitutionTypes.Query());
            return institutionTypes;
        }

        /// <summary>
        /// update InstitutionType
        /// </summary>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<InstitutionType> UpdateInstitutionType([FromBody] InstitutionType institutionType)
        {
            return await _mediator.Send(new UpdateInstitutionType.Command()
            {
                InstitutionType = institutionType
            });
        }


        /// <summary>
        /// Create InstitutionType
        /// </summary>
        /// <param name="institutionType"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(InstitutionType), StatusCodes.Status201Created)]
        public async Task<ActionResult<InstitutionType>> CreateInstitutionType([FromBody] CreateInstitutionTypeRequest institutionType)
        {
            try
            {
                var response = await _mediator.Send(new CreateInstitutionType.Command()
                {
                    InstitutionType = institutionType
                });

                return CreatedAtRoute("GetInstitutionTypes", response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}