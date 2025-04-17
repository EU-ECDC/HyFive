using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institution;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Institution;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    /// <summary>
    /// IndicationTypes
    /// </summary>
    [Authorize(HandhygienePolicy.FhiAdmin)]
    [Route("api/v1/institusjonstyper")]
    public class InstitusjonstyperController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InstitusjonstyperController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Hent indikasjonstyper
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "HentInstitusjonstyper")]
        public async Task<IEnumerable<InstitutionType>> HentInstitusjonstyper()
        {
            var institusjonstyper = await _mediator.Send(new GetInstitutionTypes.Query());
            return institusjonstyper;
        }

        /// <summary>
        /// Oppdater indikasjonstype
        /// </summary>
        /// <returns></returns>
        [HttpPut("oppdater")]
        public async Task<InstitutionType> OppdaterInstitusjonstype([FromBody] InstitutionType institusjonstype)
        {
            return await _mediator.Send(new UpdateInstitutionType.Command()
            {
                InstitutionType = institusjonstype
            });
        }


        /// <summary>
        /// Opprett institusjonstype
        /// </summary>
        /// <param name="institusjonstype"></param>
        /// <returns></returns>
        [HttpPost("opprett")]
        [ProducesResponseType(typeof(InstitutionType), StatusCodes.Status201Created)]
        public async Task<ActionResult<InstitutionType>> OpprettInstitusjonstype([FromBody] CreateInstitutionTypeRequest institusjonstype)
        {
            try
            {
                var response = await _mediator.Send(new CreateInstitutionType.Command()
                {
                    InstitutionType = institusjonstype
                });

                return CreatedAtRoute("HentInstitusjonstyper", response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}