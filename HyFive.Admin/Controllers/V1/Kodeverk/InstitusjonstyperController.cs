using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institusjon;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.Institusjon;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    /// <summary>
    /// Indikasjonstyper
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
        public async Task<IEnumerable<InstitusjonType>> HentInstitusjonstyper()
        {
            var institusjonstyper = await _mediator.Send(new HentInstitusjonstyper.Query());
            return institusjonstyper;
        }

        /// <summary>
        /// Oppdater indikasjonstype
        /// </summary>
        /// <returns></returns>
        [HttpPut("oppdater")]
        public async Task<InstitusjonType> OppdaterInstitusjonstype([FromBody] InstitusjonType institusjonstype)
        {
            return await _mediator.Send(new OppdaterInstitusjonstype.Command()
            {
                Institusjonstype = institusjonstype
            });
        }


        /// <summary>
        /// Opprett institusjonstype
        /// </summary>
        /// <param name="institusjonstype"></param>
        /// <returns></returns>
        [HttpPost("opprett")]
        [ProducesResponseType(typeof(InstitusjonType), StatusCodes.Status201Created)]
        public async Task<ActionResult<InstitusjonType>> OpprettInstitusjonstype([FromBody] OpprettInstitusjonstypeRequest institusjonstype)
        {
            try
            {
                var response = await _mediator.Send(new OpprettInstitusjonstype.Command()
                {
                    Institusjonstype = institusjonstype
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