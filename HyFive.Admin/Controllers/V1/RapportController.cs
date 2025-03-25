using System;
using HyFive.Tjenester.Autentisering.Bruker;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using HyFive.Api.Common.ExtensionMethods;
using HyFive.Tjenester;
using HyFive.Tjenester.Autentisering.Requirements;
using HyFive.Tjenester.Rapport.Observasjoner;
using HyFive.Tjenester.Rapporter.FireIndikasjoner;
using HyFive.Tjenester.Rapporter.Handsmykker;
using HyFive.Tjenester.Rapporter.Pdf;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminEllerKoordinator)]
    [Route("api/v1/rapport")]
    public class RapportController : ControllerBase
    {
        private readonly FireIndikasjonerPdfRapportService _fireIndikasjonerPdfRapportService;
        private readonly IBrukerService _brukerservice;
        private readonly IMediator _mediator;
        private readonly HandsmykkePdfRapportService _handsmykkePdfRapportService;

        public RapportController(
            FireIndikasjonerPdfRapportService fireIndikasjonerPdfRapportService,
            IBrukerService brukerservice,
            IMediator mediator,
            HandsmykkePdfRapportService handsmykkePdfRapportService)
        {
            _fireIndikasjonerPdfRapportService = fireIndikasjonerPdfRapportService;
            _brukerservice = brukerservice;
            _mediator = mediator;
            _handsmykkePdfRapportService = handsmykkePdfRapportService;
        }

        /// <summary>
        /// Lag en Excel-rapport for Hanske-observasjoner for avdeling
        /// </summary>
        /// <param name="institusjonId"></param>
        /// <param name="avdelingId"></param>
        /// <param name="fraTid"></param>
        /// <param name="tilTid"></param>
        /// <param name="rolle"></param>
        /// <returns></returns>
        [HttpGet("avdeling/hansker/excel")]
        public async Task<IActionResult> LagHanskeRapportSomExcel(
            [FromQuery] int institusjonId,
            [FromQuery] int avdelingId,
            [FromQuery] DateTime? fraTid,
            [FromQuery] DateTime? tilTid,
            [FromQuery] AuthorizedRole rolle)
        {
            if (!BrukerErAutorisert(institusjonId))
                return Unauthorized();

            var query = new HentHanskeObservasjoner.Query
            {
                AvdelingId = avdelingId,
                InstitusjonId = institusjonId,
                FraTid = fraTid,
                TilTid = tilTid,
                Rolle = rolle
            };

            var rapportData = await _mediator.Send(query);
            var fil = await this.ExcelFileContentResult(rapportData, "Hanskeobservasjoner");

            return fil;
        }

        /// <summary>
        /// Lag en Excel-rapport for Handsmykke-observasjoner for avdeling
        /// </summary>
        /// <param name="institusjonId"></param>
        /// <param name="avdelingId"></param>
        /// <param name="fraTid"></param>
        /// <param name="tilTid"></param>
        /// <param name="rolle"></param>
        /// <returns></returns>
        [HttpGet("avdeling/handsmykker/excel")]
        public async Task<IActionResult> LagHandsmykkeRapportSomExcel(
            [FromQuery] int institusjonId,
            [FromQuery] int avdelingId,
            [FromQuery] DateTime? fraTid,
            [FromQuery] DateTime? tilTid,
            [FromQuery] AuthorizedRole rolle)
        {
            if (!BrukerErAutorisert(institusjonId))
                return Unauthorized();

            var query = new HentHandsmykkeObservasjoner.Query
            {
                AvdelingId = avdelingId,
                InstitusjonId = institusjonId,
                FraTid = fraTid,
                TilTid = tilTid,
                Rolle = rolle
            };

            var rapportData = await _mediator.Send(query);
            var fil = await this.ExcelFileContentResult(rapportData, "Handsmykkeobservasjoner");

            return fil;
        }

        /// <summary>
        /// Lag en Excel-rapport for Beskyttelsesutstyr-observasjoner for avdeling
        /// </summary>
        /// <param name="institusjonId"></param>
        /// <param name="avdelingId"></param>
        /// <param name="fraTid"></param>
        /// <param name="tilTid"></param>
        /// <param name="rolle"></param>
        /// <returns></returns>
        [HttpGet("avdeling/beskyttelsesutstyr/excel")]
        public async Task<IActionResult> LagBeskyttelsesutstyrRapportSomExcel(
            [FromQuery] int institusjonId,
            [FromQuery] int avdelingId,
            [FromQuery] DateTime? fraTid,
            [FromQuery] DateTime? tilTid,
            [FromQuery] AuthorizedRole rolle)
        {
            if (!BrukerErAutorisert(institusjonId))
                return Unauthorized();

            var query = new HentBeskyttelsesutstyrObservasjoner.Query
            {
                AvdelingId = avdelingId,
                InstitusjonId = institusjonId,
                FraTid = fraTid,
                TilTid = tilTid,
                Rolle = rolle
            };

            var rapportData = await _mediator.Send(query);
            var fil = await this.ExcelFileContentResult(rapportData, "Beskyttelsesutstyrobservasjoner");

            return fil;
        }

        /// <summary>
        /// Lag en Excel-rapport for Fire Indikasjoner-observasjoner for avdeling
        /// </summary>
        /// <param name="institusjonId"></param>
        /// <param name="avdelingId"></param>
        /// <param name="fraTid"></param>
        /// <param name="tilTid"></param>
        /// <param name="rolle"></param>
        /// <param name="sesjonId"></param>
        /// <returns></returns>
        [HttpGet("avdeling/fireindikasjoner/excel")]
        public async Task<IActionResult> LagFireindikasjonerRapportSomExcel(
            [FromQuery] int institusjonId,
            [FromQuery] int avdelingId,
            [FromQuery] DateTime fraTid,
            [FromQuery] DateTime tilTid,
            [FromQuery] AuthorizedRole rolle)
        {
            if (!BrukerErAutorisert(institusjonId))
                return Unauthorized();

            var query = new HentFireIndikasjonerObservasjoner.Query
            {
                AvdelingId = avdelingId,
                InstitusjonId = institusjonId,
                FraTid = fraTid,
                TilTid = tilTid,
                Rolle = rolle
            };

            var rapportData = await _mediator.Send(query);
            var fil = await this.ExcelFileContentResult(rapportData, "Fireindikasjoner-observasjoner");

            return fil;
        }

        /// <summary>
        /// Lag fire indikasjoner rapport for avdeling på pdf format
        /// </summary>
        /// <param name="avdelingId"></param>
        /// <param name="institusjonId"></param>
        /// <param name="fraTid"></param>
        /// <param name="tilTid"></param>
        /// <param name="rolle"></param>
        /// <returns></returns>
        [HttpGet("fireindikasjoner/avdeling/pdf")]
        public async Task<IActionResult> LagFireIndikasjnerRapportForAvdelingPdf(
            [FromQuery] int avdelingId,
            [FromQuery] int institusjonId,
            [FromQuery] DateTime fraTid,
            [FromQuery] DateTime tilTid,
            [FromQuery] AuthorizedRole rolle)
        {
            if (!BrukerErAutorisert(institusjonId))
                return Unauthorized();

            var query = new HentFireIndikasjonerRapportForAvdeling.Query
            {
                FraTidspunkt = fraTid,
                TilTidspunkt = tilTid,
                AvdelingId = avdelingId,
                Rolle = rolle
            };

            var rapportData = await _mediator.Send(query);
            var pdf = await _fireIndikasjonerPdfRapportService.LagRapportForAvdeling(rapportData);
            var fil = LagFil(pdf);

            return fil;
        }

        /// <summary>
        /// Lag håndsmykkerapport for avdeling på pdf format
        /// </summary>
        /// <param name="avdelingId"></param>
        /// <param name="institusjonId"></param>
        /// <param name="fraTid"></param>
        /// <param name="tilTid"></param>
        /// <param name="rolle"></param>
        /// <returns></returns>
        [HttpGet("handsmykke/avdeling/pdf")]
        public async Task<IActionResult> LagHandsmykkeRapportForAvdelingPdf(
            [FromQuery] int avdelingId,
            [FromQuery] int institusjonId,
            [FromQuery] DateTime fraTid,
            [FromQuery] DateTime tilTid,
            [FromQuery] AuthorizedRole rolle)
        {
            if (!BrukerErAutorisert(institusjonId))
                return Unauthorized();

            var query = new HentHandsmykkeRapportForAvdeling.Query
            {
                AvdelingId = avdelingId,
                InstiusjonId = institusjonId,
                FraTidspunkt = fraTid,
                TilTidspunkt = tilTid,
                Rolle = rolle
            };

            var rapportData = await _mediator.Send(query);
            var pdf = _handsmykkePdfRapportService.LagRapportForAvdeling(rapportData);
            var fil = LagFil(pdf);

            return fil;
        }

        [HttpGet("rapportforsesjontypehardata")]
        public async Task<IActionResult> RapportForSesjonTypeHarData(
            [FromQuery] int sesjonType,
            [FromQuery] int institusjonId,
            [FromQuery] int? avdelingId,
            [FromQuery] DateTime fraDato,
            [FromQuery] DateTime tilDato,
            [FromQuery] AuthorizedRole rolleId)
        {
            if (!BrukerErAutorisert(institusjonId))
                return Unauthorized();

            var query = new RapportForSesjonTypeHarData.Query
            {
                SesjonType = sesjonType,
                InstitusjonId = institusjonId,
                AvdelingId = avdelingId,
                FraDato = fraDato,
                TilDato = tilDato,
                Rolle = rolleId
            };

            var harData = await _mediator.Send(query);

            return Ok(harData);
        }

        [HttpGet("fireindikasjoner/etterlevelse")]
        public async Task<IActionResult> FireIndikasjonerEtterlevelse([FromQuery] int institusjonId, [FromQuery] string intervall, [FromQuery] int fraManed, [FromQuery] int fraAr, [FromQuery] int tilManed, [FromQuery] int tilAr, [FromQuery] int? rolleId, [FromQuery] int? avdelingId)
        {
            if (!BrukerErAutorisert(institusjonId))
                return Unauthorized();

            var query = new Etterlevelse.Query
            {
                InstitusjonId = institusjonId,
                Intervall = intervall,
                FraManed = fraManed,
                FraAr = fraAr,
                TilManed = tilManed,
                TilAr = tilAr,
                RolleId = rolleId,
                AvdelingId = avdelingId
            };

            var grafListe = await _mediator.Send(query);
            return Ok(grafListe);
        }

        private bool BrukerErAutorisert(int institusjonId)
        {
            if (_brukerservice.ErFhiAdmin())
                return true;

            if (_brukerservice.ErKoordinatorForInstitusjon(institusjonId))
                return true;

            return false;
        }

        private FileStreamResult LagFil(PdfResult pdf)
        {
            var fil = File(new MemoryStream(pdf.Content), "application/pdf", pdf.Filename);
            return fil;
        }
    }
}
