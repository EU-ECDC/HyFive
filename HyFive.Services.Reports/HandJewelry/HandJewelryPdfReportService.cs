using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using HyFive.Services.Reports.QuickChart;
using HyFive.Services.Reports.Pdf;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;
using Image = iTextSharp.text.Image;
using Microsoft.Extensions.Hosting;
using iTextSharp.text;

namespace HyFive.Services.Reports.HandJewelry
{
    public class HandJewelryPdfReportService
    {
        private const int PixelWidth = 800;
        private const string TotalForAllRoles = "All professions";
        private readonly List<RoleToColorMap> _roleToColorMapList = new();

        public PdfResult GenerateReportForDepartment(JewelryReportForJewelryTypeAndRole report)
        {
            CreateRoleToColorMap(report);

            var grafForDepartment = CreateGraph(report.ReportForDepartment, "Report for department");
            var graphForFacility = CreateGraph(report.ReportForFacility, "Report for facility");
            var pdf = CreatePdf(grafForDepartment, graphForFacility, report);

            return pdf;
        }

        /// <summary>
        /// Find all roles and assign a color to each of them.
        /// Add 'roles' TotalForAllRoles first in the list.
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        private void CreateRoleToColorMap(JewelryReportForJewelryTypeAndRole report)
        {
            var allRolesDepartment = report.ReportForDepartment.RoleJewelrySummaryList.SelectMany(p => p.CountByRoleList.Select(q => q.Role)).ToList();
            var allRolesFacility = report.ReportForFacility.RoleJewelrySummaryList.SelectMany(p => p.CountByRoleList.Select(q => q.Role)).ToList();

            var allRoles = allRolesDepartment;
            allRoles.AddRange(allRolesFacility);
            allRoles = allRoles.Distinct().ToList();
            allRoles.Insert(0, TotalForAllRoles);

            foreach (var role in allRoles.Select((navn, index) => (navn, index)))
            {
                _roleToColorMapList.Add(new RoleToColorMap { Role = role.navn, Color = SelectColor(role.index) });
            }
        }

        private class RoleToColorMap
        {
            public string Role { get; init; }
            public string Color { get; init; }
        }

        private byte[] CreateGraph(ReportForUnit reportForUnit, string header)
        {
            var jewelryTypes = reportForUnit.RoleJewelrySummaryList
                .Select(p => p.JewelryType.Name)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            var role = reportForUnit.RoleJewelrySummaryList
                .SelectMany(p => p.CountByRoleList)
                .Select(p => p.Role)
                .Distinct()
                .ToList();

            var datasets = CreateDatasets(role, jewelryTypes);
            var data = LagData(reportForUnit, jewelryTypes, datasets);
            var options = CreateOptions(header);

            var chartconfig = new QuickChartConfig
            {
                Type = "bar",
                Data = data,
                Options = options
            };

            var graf = Helpers.CreateChart(chartconfig, PixelWidth).Result;

            return graf;
        }

        /// <summary>
        /// Create a dataset for the total for all roles + one for each roles, with space for values for each jewelry type
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="jewelryTypes"></param>
        /// <returns></returns>
        private List<Dataset> CreateDatasets(List<string> roles, ICollection jewelryTypes)
        {
            var datasets = new List<Dataset>();

            var datasetTotalForAlleRoller = new Dataset
            {
                Label = TotalForAllRoles,
                Stack = TotalForAllRoles,
                BackgroundColor = FindColorForRole(TotalForAllRoles),
                Data = new List<double>(new double[jewelryTypes.Count])
            };

            datasets.Add(datasetTotalForAlleRoller);

            foreach (var role in roles.OrderBy(p => p))
            {
                var dataset = new Dataset
                {
                    Label = role,
                    Stack = role,
                    BackgroundColor = FindColorForRole(role),
                    Data = new List<double>(new double[jewelryTypes.Count])
                };

                datasets.Add(dataset);
            }

            return datasets;
        }

        /// <summary>
        /// Find color for roles in roleToColorMapList
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private string FindColorForRole(string role)
        {
            var color = _roleToColorMapList.First(p => p.Role == role).Color;
            return color;
        }

        /// <summary>
        /// Fill out the percentage count for each jewelry type, both for all roles combined and for each role.
        ///
        /// Percent for roles for a jewelry type: number of registrations with jewelry type for roles / number of observations for role
        /// Example: Of 20 observations of a Doctor, 10 have a ring = 50%, 5 have a watch = 25%, etc.
        ///
        /// Percent for all roles for a jewelry type: the sum of jewelry type usage for all roles / the sum of the number of observations for roles that have used the jewelry type
        /// Example: There are 100 observations of nurses, 40 use a ring (so 40%), there are 100 observations of doctors, 20 use a ring, so 20%. 
        /// In total, 200 observations, of which 60 (40+20) use a ring (so 30% of all).
        /// </summary>
        /// <param name="reportForUnit"></param>
        /// <param name="jewelryTypes"></param>
        /// <param name="datasets"></param>
        /// <returns></returns>
        private static Data LagData(ReportForUnit reportForUnit, IReadOnlyCollection<string> jewelryTypes, List<Dataset> datasets)
        {
            foreach (var jewelryType in jewelryTypes.Select((navn, index) => (navn, index)))
            {
                var roleListCount = reportForUnit.RoleJewelrySummaryList
                    .Where(p => p.JewelryType.Name == jewelryType.navn)
                    .SelectMany(p => p.CountByRoleList)
                    .ToList();

                var totalObservationsForAllRoles = 0;
                foreach (var countRole in roleListCount)
                {
                    var numberOfObservationsForRole = reportForUnit.ListOfObservationsByRole.First(p => p.Role == countRole.Role).Count;
                    var percent = countRole.Count * 100 / numberOfObservationsForRole;
                    var dataset = datasets.First(p => p.Label == countRole.Role);
                    dataset.Data[jewelryType.index] = percent;

                    totalObservationsForAllRoles += numberOfObservationsForRole;
                }

                // // Percent for all roles = sum of usage of jewelryType for all roles / sum of number of observations for roles that have used jewelryType
                var totalCountForRoles = roleListCount.Select(p => p.Count).Sum();
                var prosentTotalForRoller = totalCountForRoles * 100 / totalObservationsForAllRoles;
                var datasetForAlleRoller = datasets.First(p => p.Label == TotalForAllRoles);
                datasetForAlleRoller.Data[jewelryType.index] = prosentTotalForRoller;
            }

            var labels = jewelryTypes.Select(p => new[] { p }).ToList();
            var data = new Data
            {
                Labels = labels,
                Datasets = datasets
            };

            return data;
        }

        private static Options CreateOptions(string header)
        {
            var options = new Options
            {
                Plugins = new Plugins
                {
                    Datalabels = new Datalabels
                    {
                        Display = false
                    }
                },
                Title = new Title
                {
                    Display = true,
                    Text = header
                },
                Scales = new Scales
                {
                    XAxes = new[] { new Xax { Stacked = false } },
                    YAxes = new[] { new Yax { Stacked = false, Ticks = new Ticks { BeginAtZero = true }, ScaleLabel = new ScaleLabel { LabelString = "Compliance (%)", Display = true } } }
                }
            };

            return options;
        }

        /// <summary>
        /// One color for each roles in the graph + one for the sum of all roles.
        /// There should be at least as many colors as there are roles.
        /// </summary>
        private static readonly List<string> Colors = new()
        {
            ColorTranslator.ToHtml(Color.FromArgb(Color.Teal.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.LimeGreen.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Navy.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Magenta.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Purple.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.GreenYellow.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.DodgerBlue.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Plum.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Fuchsia.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.SlateGray.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Peru.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Turquoise.ToArgb())),
        };

        /// <summary>
        /// If the index is greater than the last color in the list, the last one is chosen.
        /// </summary>
        /// <returns></returns>
        private static string SelectColor(int index)
        {
            var color = index < Colors.Count ? Colors[index] : Colors[^1];
            return color;
        }

        private const int YStartWithHeader = 400;
        private const int YStartWithoutHeader = 450;
        private const int ImageWidth = 500;
        private const int ImageHeight = 250;
        private const int YSpacing = 40;
        private const int LeftMargin = 50;

        private static PdfResult CreatePdf(byte[] graphForDepartment, byte[] graphForFacility, JewelryReportForJewelryTypeAndRole report)
        {
            var copyOfTemplate = Helpers.ReadCopyOfPdfTemplateFromFile("HyFive.Services.Reports.Assets.Report-template.pdf");
            using var pdfMemoryStream = new MemoryStream();
            var pdfStamper = new PdfStamper(copyOfTemplate, pdfMemoryStream);

            FillOutHeader(report, pdfStamper);

            const int sideNr = 1; // ✅ draw on the first page

            float yStart = AddReportLabel(pdfStamper, sideNr, report);
            yStart -= 210f;
            //var yStart = YStartWithHeader; // ✅ start lower to avoid overlapping the header
            var pdfContent = pdfStamper.GetOverContent(sideNr);

            // Draw first graph (department)
            var image = CreateImage(graphForDepartment, (int)yStart);
            pdfContent.AddImage(image);

            // Draw second graph below the first
            yStart -= ImageHeight + YSpacing;
            image = CreateImage(graphForFacility, (int)yStart);
            pdfContent.AddImage(image);

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();

            var pdfResult = new PdfResult
            {
                Content = pdfMemoryStream.ToArray(),
                Filename = $"{DateTime.UtcNow.ToString(Helpers.FileNamePrefix)}-ECDC hyFive Hand Jewelry.pdf"
            };

            return pdfResult;
        }

        private static void FillOutHeader(JewelryReportForJewelryTypeAndRole report, PdfStamper pdfStamper)
        {
            pdfStamper.AcroFields.SetField("title", "Report on Observations of Transmission Prevention Measures");
            pdfStamper.AcroFields.SetField("subtitle", "Module 2: Jewelry, Watches, and Nails");
            pdfStamper.AcroFields.SetField("facility", $"Facility: {report.Facility}");
            pdfStamper.AcroFields.SetField("department", $"Department: {report.Department}");
            pdfStamper.AcroFields.SetField("time period", "Registered time period: " +
                                                      $"{report.FromDate.ToString(Helpers.DateFormat, CultureInfo.InvariantCulture)} - " +
                                                      $"{report.ToDate.ToString(Helpers.DateFormat, CultureInfo.InvariantCulture)}");
            pdfStamper.AcroFields.SetField("report date", $"Report date: {DateTime.Today.ToString(Helpers.DateFormat)}");

            FillOutUnitInfo(report.ReportForDepartment, pdfStamper, "Number of observations - Department", "Department info");
            FillOutUnitInfo(report.ReportForFacility, pdfStamper, "Number of observations - Facility", "Facility info");
        }

        private static void FillOutUnitInfo(ReportForUnit reportForUnit, PdfStamper pdfStamper, string header, string field)
        {
            var totalNumberOfObservations = reportForUnit.ListOfObservationsByRole
                .Select(p => p.Count)
                .Sum();

            var info = header + Environment.NewLine;
            info += Environment.NewLine;
            info += $"Total, all professions: {totalNumberOfObservations}" + Environment.NewLine;
            foreach (var observationsForRole in reportForUnit.ListOfObservationsByRole.OrderBy(p => p.Role))
            {
                info += $"{observationsForRole.Role}: {observationsForRole.Count}" + Environment.NewLine;
            }

            pdfStamper.AcroFields.SetField(field, $"{info}");
        }

        private static Image CreateImage(byte[] graf, int yPos)
        {
            var image = Image.GetInstance(graf);
            image.SetAbsolutePosition(LeftMargin, yPos);
            image.ScaleAbsolute(ImageWidth, ImageHeight);

            return image;
        }

        private static float AddReportLabel(PdfStamper pdfStamper, int page, JewelryReportForJewelryTypeAndRole report)
        {
            var pdfContent = pdfStamper.GetOverContent(page);
            var font = LoadRobotoFont();

            pdfContent.BeginText();
            pdfContent.SetFontAndSize(font, 7);
            pdfContent.SetColorFill(BaseColor.Black);

            float startX = 50;
            float startY = 730; // Position below the logo/header
            float maxWidth = 600f; // available width for text before wrapping
            float lineHeight = 14f;

            // Prepare text lines
            string timePeriod = $"{report.FromDate:dd.MM.yyyy} - {report.ToDate:dd.MM.yyyy}";
            var lines = new[]
            {
                $"Departments: {report.Department}",
                $"Facilities: {report.Facility}",
                $"Time Period: {timePeriod}"
            };

            // Draw each line with word wrapping
            foreach (var line in lines)
            {
                var wrappedLines = WrapText(line, font, 10, maxWidth);
                foreach (var wrapped in wrappedLines)
                {
                    pdfContent.ShowTextAligned(iTextSharp.text.Element.ALIGN_LEFT, wrapped, startX, startY, 0);
                    startY -= lineHeight;
                }
                startY -= 5; // small spacing between sections
            }

            pdfContent.EndText();

            // Return Y position below the label block (with a bit of padding)
            return startY - 20;
        }

        private static List<string> WrapText(string text, BaseFont font, float fontSize, float maxWidth)
        {
            var result = new List<string>();
            var words = text.Split(' ');
            var currentLine = "";

            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                float width = font.GetWidthPoint(testLine, fontSize);

                if (width > maxWidth)
                {
                    result.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                result.Add(currentLine);

            return result;
        }

        private static BaseFont LoadRobotoFont()
        {
            var assembly = typeof(HandJewelryPdfReportService).Assembly;
            var resourcePath = "HyFive.Services.Reports.Assets.Fonts.Roboto-Regular.ttf";

            using var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
                throw new Exception("Roboto font not found in embedded resources.");

            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            var fontData = ms.ToArray();

            return BaseFont.CreateFont("Roboto-Regular.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED, true, fontData, null);
        }
    }
}
