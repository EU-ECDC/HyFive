using HyFive.Api.Common.ExtensionMethods;
using HyFive.Services;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Authentication.User;
using HyFive.Services.Rapport.Observations;
using HyFive.Services.Reports.FourIndicators;
using HyFive.Services.Reports.HandJewelry;
using HyFive.Services.Reports.Pdf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/report")]
    public class ReportController : ControllerBase
    {
        private readonly FourIndicationsPDFReportService _fourIndicationsPDFReportService;
        private readonly IUserService _userService;
        private readonly IMediator _mediator;
        private readonly HandJewelryPdfReportService _handJewelryPdfReportService;

        public ReportController(
            FourIndicationsPDFReportService fourIndicationsPdfReportService,
            IUserService userService,
            IMediator mediator,
            HandJewelryPdfReportService handJewelryPdfReportService)
        {
            _fourIndicationsPDFReportService = fourIndicationsPdfReportService;
            _userService = userService;
            _mediator = mediator;
            _handJewelryPdfReportService = handJewelryPdfReportService;
        }

        /// <summary>
        /// Create an Excel report for Glove observations for department
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="departmentId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet("department/hansker/excel")]
        public async Task<IActionResult> CreateGloveReportAsExcel(
            [FromQuery] int institutionId,
            [FromQuery] int departmentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] AuthorizedRole role)
        {
            if (!UserIsAuthorized(institutionId))
                return Unauthorized();

            var query = new GetGloveObservations.Query
            {
                DepartmentId = departmentId,
                InstitutionId = institutionId,
                FromDate = fromDate,
                ToTime = toDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var file = await this.ExcelFileContentResult(reportData, "GloveObservations.");

            return file;
        }

        /// <summary>
        /// Create an Excel report for HandJewelry observations for department
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="departmentId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet("department/handJewelry/excel")]
        public async Task<IActionResult> CreateHandJewelryReportAsExcel(
            [FromQuery] int institutionId,
            [FromQuery] int departmentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] AuthorizedRole role)
        {
            if (!UserIsAuthorized(institutionId))
                return Unauthorized();

            var query = new GetHandJewelryObservations.Query
            {
                DepartmentId = departmentId,
                InstitutionId = institutionId,
                FromDate = fromDate,
                ToTime = toDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var file = await this.ExcelFileContentResult(reportData, "HandJewelryObservations");

            return file;
        }

        /// <summary>
        /// Create an Excel report for ProtectiveEquipment observations for department
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="departmentId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet("department/protectiveEquipment/excel")]
        public async Task<IActionResult> CreateProtectiveEquipmentReportAsExcel(
            [FromQuery] int institutionId,
            [FromQuery] int departmentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] AuthorizedRole role)
        {
            if (!UserIsAuthorized(institutionId))
                return Unauthorized();

            var query = new GetProtectiveEquipmentObservations.Query
            {
                DepartmentId = departmentId,
                InstitutionId = institutionId,
                FromDate = fromDate,
                ToTime = toDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var file = await this.ExcelFileContentResult(reportData, "ProtectiveEquipmentObservations");

            return file;
        }

        /// <summary>
        /// Lag en Excel-rapport for Fire Indications-observasjoner for avdeling
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="departmentId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="role"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [HttpGet("department/fourIndications/excel")]
        public async Task<IActionResult> CreateFourIndicationsReportAsExcel(
            [FromQuery] int institutionId,
            [FromQuery] int departmentId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] AuthorizedRole role)
        {
            if (!UserIsAuthorized(institutionId))
                return Unauthorized();

            var query = new GetFourIndicationsObservations.Query
            {
                DepartmentId = departmentId,
                InstitutionId = institutionId,
                FromDate = fromDate,
                ToTime = toDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var file = await this.ExcelFileContentResult(reportData, "FourIndicationsObservations");

            return file;
        }

        /// <summary>
        /// Create four indications report for department in PDF format
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="institutionId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet("fourIndications/department/pdf")]
        public async Task<IActionResult> CreateFourIndicationsReportForDepartmentPdf(
            [FromQuery] int departmentId,
            [FromQuery] int institutionId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] AuthorizedRole role)
        {
            if (!UserIsAuthorized(institutionId))
                return Unauthorized();

            var query = new GetFourIndicatorsReportForDepartment.Query
            {
                FromDate = fromDate,
                ToTime = toDate,
                DepartmentId = departmentId,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var pdf = await _fourIndicationsPDFReportService.CreateDepartmentReport(reportData);
            var file = CreateFile(pdf);

            return file;
        }

        /// <summary>
        /// Create hand jewelry report for department in PDF format
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="institutionId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet("handJewelry/department/pdf")]
        public async Task<IActionResult> CreateHandJewelryReportForDepartmentPdf(
            [FromQuery] int departmentId,
            [FromQuery] int institutionId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] AuthorizedRole role)
        {
            if (!UserIsAuthorized(institutionId))
                return Unauthorized();

            var query = new GetHandJewelryReportForDepartment.Query
            {
                DepartmentId = departmentId,
                InstitutionId = institutionId,
                FromDateTime = fromDate,
                ToDateTime = toDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var pdf = _handJewelryPdfReportService.GenerateReportForDepartment(reportData);
            var file = CreateFile(pdf);

            return file;
        }

        [HttpGet("reportForSessionTypeHasData")]
        public async Task<IActionResult> ReportForSessionTypeHasData(
            [FromQuery] int sessionType,
            [FromQuery] int institutionId,
            [FromQuery] int? departmentId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] AuthorizedRole roleId)
        {
            if (!UserIsAuthorized(institutionId))
                return Unauthorized();

            var query = new ReportForSessionTypeHasData.Query
            {
                SessionType = sessionType,
                InstitutionId = institutionId,
                DepartmentId = departmentId,
                FromDate = fromDate,
                ToDate = toDate,
                Role = roleId
            };

            var hasData = await _mediator.Send(query);

            return Ok(hasData);
        }

        [HttpGet("fourΙndications/compliance")]
        public async Task<IActionResult> FourIndicationsCompliance([FromQuery] int institutionId, [FromQuery] string interval, [FromQuery] int fromMonth, [FromQuery] int fromYear, [FromQuery] int ToMonth, [FromQuery] int toYear, [FromQuery] int? roleId, [FromQuery] int? departmentId)
        {
            if (!UserIsAuthorized(institutionId))
                return Unauthorized();

            var query = new Compliance.Query
            {
                InstitutionId = institutionId,
                Interval = interval,
                FromMonth = fromMonth,
                FromYear = fromYear,
                ToMonth = ToMonth,
                ToYear = toYear,
                RoleId = roleId,
                DepartmentId = departmentId
            };

            var graphList = await _mediator.Send(query);
            return Ok(graphList);
        }

        private bool UserIsAuthorized(int institutionId)
        {
            if (_userService.IsFhiAdmin())
                return true;

            if (_userService.IsCoordinatorForInstitution(institutionId))
                return true;

            return false;
        }

        private FileStreamResult CreateFile(PdfResult pdf)
        {
            var fil = File(new MemoryStream(pdf.Content), "application/pdf", pdf.Filename);
            return fil;
        }
    }
}
