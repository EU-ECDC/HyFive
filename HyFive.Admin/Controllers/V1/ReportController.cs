using HyFive.Api.Common.ExtensionMethods;
using HyFive.Models.V1.Report.FiveIndications;
using HyFive.Models.V1.Report.ProtectiveEquipment;
using HyFive.Models.V1.Session;
using HyFive.Services;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Authentication.User;
using HyFive.Services.Report.Observations;
using HyFive.Services.Reports.FiveIndicators;
using HyFive.Services.Reports.HandJewelry;
using HyFive.Services.Reports.Pdf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/report")]
    public class ReportController : ControllerBase
    {
        private readonly FiveIndicationsPDFReportService _fiveIndicationsPDFReportService;
        private readonly IUserService _userService;
        private readonly IMediator _mediator;
        private readonly HandJewelryPdfReportService _handJewelryPdfReportService;

        public ReportController(
            FiveIndicationsPDFReportService fiveIndicationsPdfReportService,
            IUserService userService,
            IMediator mediator,
            HandJewelryPdfReportService handJewelryPdfReportService)
        {
            _fiveIndicationsPDFReportService = fiveIndicationsPdfReportService;
            _userService = userService;
            _mediator = mediator;
            _handJewelryPdfReportService = handJewelryPdfReportService;
        }

        /// <summary>
        /// Create an Excel report for Glove observations for department
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("department/gloves/excel")]
        public async Task<IActionResult> CreateGloveReportAsExcel([FromBody] GloveReportRequest request)
        {
            foreach (var facilityId in request.FacilityIds)
            {
                if (!UserIsAuthorized(facilityId))
                    return Unauthorized();
            }

            var role = (AuthorizedRole)request.Role;

            var query = new GetGloveObservations.Query
            {
                DepartmentIds = request.DepartmentIds,
                FacilityIds = request.FacilityIds,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var file = await this.ExcelFileContentResult(reportData, "GloveObservations.");

            return file;
        }

        /// <summary>
        /// Create an Excel report for HandJewelries observations for department
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("department/handJewelry/excel")]
        public async Task<IActionResult> CreateHandJewelryReportAsExcel([FromBody] HandJewelryReportRequest request)
        {
            foreach (var facilityId in request.FacilityIds)
            {
                if (!UserIsAuthorized(facilityId))
                    return Unauthorized();
            }

            var role = (AuthorizedRole)request.Role;

            var query = new GetHandJewelryObservations.Query
            {
                DepartmentIds = request.DepartmentIds,
                FacilityIds = request.FacilityIds,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var file = await this.ExcelFileContentResult(reportData, "HandJewelryObservations");

            return file;
        }

        /// <summary>
        /// Create an Excel report for ProtectiveEquipment observations for department
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("department/protectiveEquipment/excel")]
        public async Task<IActionResult> CreateProtectiveEquipmentReportAsExcel([FromBody] ProtectiveEquipmentReport request)
        {
            foreach (var facilityId in request.FacilityIds)
            {
                if (!UserIsAuthorized(facilityId))
                    return Unauthorized();
            }

            var role = (AuthorizedRole)request.Role;

            var query = new GetProtectiveEquipmentObservations.Query
            {
                DepartmentIds = request.DepartmentIds,
                FacilityIds = request.FacilityIds,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var file = await this.ExcelFileContentResult(reportData, "ProtectiveEquipmentObservations");

            return file;
        }

        /// <summary>
        /// Lag en Excel-rapport for Fire Indications-observasjoner for avdeling
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("department/fiveIndications/excel")]
        public async Task<IActionResult> CreateFiveIndicationsReportAsExcel([FromBody] FiveIndicationsReportRequest request)
        {
            foreach (var facilityId in request.FacilityIds)
            {
                if (!UserIsAuthorized(facilityId))
                    return Unauthorized();
            }

            var role = (AuthorizedRole)request.Role;

            var query = new GetFiveIndicationsObservations.Query
            {
                DepartmentIds = request.DepartmentIds,
                FacilityIds = request.FacilityIds,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var file = await this.ExcelFileContentResult(reportData, "FiveIndicationsObservations");

            return file;
        }

        /// <summary>
        /// Create five indications report for department in PDF format
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("fiveIndications/department/pdf")]
        public async Task<IActionResult> CreateFiveIndicationsReportForDepartmentPdf([FromBody] FiveIndicationsReportRequest request)
        {
            foreach (var facilityId in request.FacilityIds)
            {
                if (!UserIsAuthorized(facilityId))
                    return Unauthorized();
            }

            var role = (AuthorizedRole)request.Role;

            var query = new GetFiveIndicatorsReportForDepartment.Query
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                DepartmentIds = request.DepartmentIds,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var pdf = await _fiveIndicationsPDFReportService.CreateDepartmentReport(reportData);
            var file = CreateFile(pdf);

            return file;
        }

        /// <summary>
        /// Create hand jewelry report for department in PDF format
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("handJewelry/department/pdf")]
        public async Task<IActionResult> CreateHandJewelryReportForDepartmentPdf([FromBody] HandJewelryReportRequest request)
        {
            foreach (var facilityId in request.FacilityIds)
            {
                if (!UserIsAuthorized(facilityId))
                    return Unauthorized();
            }

            var role = (AuthorizedRole)request.Role;

            var query = new GetHandJewelryReportForDepartment.Query
            {
                DepartmentIds = request.DepartmentIds,
                FacilityIds = request.FacilityIds,
                FromDateTime = request.FromDate,
                ToDateTime = request.ToDate,
                Role = role
            };

            var reportData = await _mediator.Send(query);
            var pdf = _handJewelryPdfReportService.GenerateReportForDepartment(reportData);
            var file = CreateFile(pdf);

            return file;
        }

        /// <summary>
        /// Checks if there is report data for a given session type, facilities, and (optionally) departments.
        /// </summary>
        /// <param name="request">The request body.</param>
        /// <returns>True if data exists, otherwise false.</returns>
        [HttpPost("reportForSessionTypeHasData")]
        public async Task<IActionResult> ReportForSessionTypeHasData([FromBody] ReportForSessionTypeHasDataRequest request)
        {
            foreach (var facilityId in request.FacilityIds)
            {
                if (!UserIsAuthorized(facilityId))
                    return Unauthorized();
            }

            var role = (AuthorizedRole)request.RoleId;

            var query = new ReportForSessionTypeHasData.Query
            {
                SessionType = request.SessionType,
                FacilityIds = request.FacilityIds,
                DepartmentIds = request.DepartmentIds,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Role = role
            };

            var hasData = await _mediator.Send(query);

            return Ok(hasData);
        }

        [HttpPost("fiveIndications/compliance")]
        public async Task<IActionResult> FiveIndicationsCompliance([FromBody] FiveIndicationsComplianceRequest request)
        {
            foreach(var facilityId in request.FacilityIds)
            {
                if (!UserIsAuthorized(facilityId))
                    return Unauthorized();
            }

            var query = new Compliance.Query
            {
                FacilityIds = request.FacilityIds,
                Interval = request.Interval,
                FromMonth = request.FromMonth,
                FromYear = request.FromYear,
                FromQuarter = request.FromQuarter,
                ToMonth = request.ToMonth,
                ToYear = request.ToYear,
                ToQuarter = request.ToQuarter,
                RoleIds = request.RoleIds,
                DepartmentIds = request.DepartmentIds,
                DepartmentTypeIds = request.DepartmentTypeIds,
                FacilityTypeIds = request.FacilityTypeIds,
                TranferredTo = request.TransferredTo
            };

            var graphList = await _mediator.Send(query);
            return Ok(graphList);
        }

        private bool UserIsAuthorized(int facilityId)
        {
            if (_userService.IsAdmin())
                return true;

            if (_userService.IsCoordinatorForFacility(facilityId))
                return true;

            return false;
        }

        private FileStreamResult CreateFile(PdfResult pdf)
        {
            var file = File(new MemoryStream(pdf.Content), "application/pdf", pdf.Filename);
            return file;
        }
    }
}
