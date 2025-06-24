using HyFive.Services.Reports.Pdf;
using HyFive.Services.Reports.QuickChart;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HyFive.Services.Reports.FiveIndicators
{
    /// <summary>
    /// A service that generates PDFs with bar charts based on input data.
    /// </summary>
    public class FiveIndicationsPDFReportService
    {
        const int STARTPOSITION_WITH_HEADER = 470;
        const int STARTPOSITION_WITHOUT_HEADER = 550;
        const int IMAGE_HEIGHT = 200;
        const int IMAGE_WIDTH = 200;
        const int Y_SPACER = 110;
        const int X_SPACER = 50;
        const int STARTING_PAGE = 1;

        private const int PixelWidth = 300;

        public async Task<PdfResult> CreateDepartmentReport(FiveIndicatorsReportForDepartment departmentReport)
        {
            foreach (var role in departmentReport.Department.Roles)
            {
                var roleGraphChartConfig = MakeChartConfigForRole(role);
                var roleGraph = await Helpers.CreateChart(roleGraphChartConfig, PixelWidth);
                role.Chart = roleGraph;
            }

            foreach (var role in departmentReport.Institution.Roles)
            {
                var roleGraphChartConfig = MakeChartConfigForRole(role);
                var roleGraph = await Helpers.CreateChart(roleGraphChartConfig, PixelWidth);
                role.Chart = roleGraph;
            }
            foreach (var role in departmentReport.ComparableDepartments.Roles)
            {
                var roleGraphChartConfig = MakeChartConfigForRole(role);
                var roleGraph = await Helpers.CreateChart(roleGraphChartConfig, PixelWidth);
                role.Chart = roleGraph;
            }

            foreach (var role in departmentReport.Clinics.SelectMany(r => r.Roles))
            {
                var roleGraphChartConfig = MakeChartConfigForRole(role);
                var roleGraph = await Helpers.CreateChart(roleGraphChartConfig, PixelWidth);
                role.Chart = roleGraph;
            }

            var pdf = CreateDepartmentPdf(departmentReport);
            return pdf;
        }

        private static QuickChartConfig MakeChartConfigForRole(RoleWithCombinationsReport roleReport)
        {
            var chartconfig = new QuickChartConfig();
            chartconfig.Type = "bar";

            chartconfig.Data = new Data()
            {
                Labels = roleReport.Combinations.Select(k => new[] { k.Name, $"[N={k.NumberOfObservations}]" }).ToList(),
                Datasets = new List<Dataset>()
                {
                    new Dataset()
                    {
                        Label = "Complied",
                        BackgroundColor = "#393C61",
                        Stack = "Stack 0",
                        Data = roleReport.Combinations.Select(r => Math.Round(r.PercentComplied)).ToList(),
                    },
                    new Dataset()
                    {
                        Label = "Not complied with",
                        BackgroundColor = "#FC5F56",
                        Stack = "Stack 0",
                        Data = roleReport.Combinations.Select(r => Math.Round(r.PercentNotComplied)).ToList(),
                    }
                }
            };

            chartconfig.Options = new Options()
            {
                Plugins = new Plugins()
                {
                    Datalabels = new Datalabels()
                    {
                        Anchor = "center",
                        Align = "center",
                        Color = "white",
                        Display = true,
                        Font = new ChartFont()
                        {
                            Weight = "normal"
                        }
                    }
                },
                Title = new Title()
                {
                    Display = true,
                    Text = $"{roleReport.Name} [N={roleReport.Combinations.Sum(k => k.NumberOfObservations)}]"
                },
                Scales = new Scales()
                {
                    XAxes = new Xax[] { new Xax() { Stacked = true } },
                    YAxes = new Yax[] { new Yax() { Stacked = true, Ticks = new Ticks { BeginAtZero = true }, ScaleLabel = new ScaleLabel { LabelString = "[%]", Display = true } } }
                }
            };

            return chartconfig;
        }

        private static PdfResult CreateDepartmentPdf(FiveIndicatorsReportForDepartment reportForDepartment)
        {
            var reportsToBeCombined = new List<PdfResult>();
            var departmentReport = CreatePdfReport(reportForDepartment.Department);
            var fileName = departmentReport.Filename;
            reportsToBeCombined.Add(departmentReport);

            if (reportForDepartment.Institution?.NumberOfObservations > 0)
            {
                var institutionReport = CreatePdfReport(reportForDepartment.Institution);
                reportsToBeCombined.Add(institutionReport);
            }

            if (reportForDepartment.ComparableDepartments?.NumberOfObservations > 0)
            {
                var sammenlignbareAvdelingerRapport = CreatePdfReport(reportForDepartment.ComparableDepartments);
                reportsToBeCombined.Add(sammenlignbareAvdelingerRapport);
            }

            foreach (var klinikk in reportForDepartment.Clinics)
            {
                if (klinikk.NumberOfObservations > 0)
                {
                    var klinikkRapport = CreatePdfReport(klinikk);
                    reportsToBeCombined.Add(klinikkRapport);
                }
            }

            // Kombiner alle filene
            var entirePdfMemoryStream = new MemoryStream();
            var document = new Document();
            var copy = new PdfCopy(document, entirePdfMemoryStream) { CloseStream = false };
            document.Open();

            foreach (var rapport in reportsToBeCombined)
            {
                var rapportReader = new PdfReader(rapport.Content);
                for (int pageIndex = 1; pageIndex <= rapportReader.NumberOfPages; pageIndex++)
                {
                    copy.AddPage(copy.GetImportedPage(rapportReader, pageIndex));
                }
            }
            document.Close();

            return new PdfResult
            {
                Content = entirePdfMemoryStream.ToArray(),
                Filename = fileName
            };

        }
        private static PdfResult CreatePdfReport(FiveIndicatorsReport report)
        {
            var copyOfDepartmentTemplate = Helpers.ReadCopyOfPdfTemplateFromFile("HyFive.Tjenester.Rapporter.Assets.FHI-fire-indikasjoner-rapport-template.pdf");
            using var pdfMemoryStream = new MemoryStream();

            var pdfStamper = new PdfStamper(copyOfDepartmentTemplate, pdfMemoryStream);

            var timePeriod = $"{report.FromDate.ToString(Helpers.DateFormat, CultureInfo.InvariantCulture)} - " +
                                $"{report.ToDate.ToString(Helpers.DateFormat, CultureInfo.InvariantCulture)}";

            pdfStamper.AcroFields.SetField("reportName", $"{report.Name}" + "\n" + $"{timePeriod}");

            var page = STARTING_PAGE;
            var pdfContent = pdfStamper.GetOverContent(page);
            var startingYposition = STARTPOSITION_WITH_HEADER;
            var graphsPlaced = 0;

            var x1 = X_SPACER;
            var x2 = IMAGE_WIDTH + X_SPACER * 2;

            foreach (var graf in report.Roles.Select(r => r.Chart))
            {
                if (graphsPlaced % 6 == 0 && graphsPlaced != 0)
                {
                    // "flip page" and create a blank page after 6 rendered graphs
                    graphsPlaced = 0;
                    page++;
                    startingYposition = STARTPOSITION_WITHOUT_HEADER;
                    pdfStamper.InsertPage(page, copyOfDepartmentTemplate.GetPageSize(1));
                    pdfContent = pdfStamper.GetOverContent(page);
                }

                graphsPlaced++;
                var image = Image.GetInstance(graf);
                var y1 = startingYposition - (graphsPlaced - 1) * Y_SPACER;
                var y2 = startingYposition - (graphsPlaced - 2) * Y_SPACER;
                var absX = graphsPlaced % 2 != 0
                    ? x1 : x2;
                var absY = graphsPlaced % 2 != 0
                    ? y1 : y2;

                image.SetAbsolutePosition(absX, absY);
                image.ScaleAbsolute(IMAGE_WIDTH, IMAGE_HEIGHT);
                pdfContent.AddImage(image);

            }

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();

            var pdfResult = new PdfResult
            {
                Content = pdfMemoryStream.ToArray(),
                Filename = $"{DateTime.UtcNow.ToString(Helpers.FileNamePrefix)}-Fire-indikasjoner-Avdelingsrapport-{report.Name}.pdf"
            };

            return pdfResult;
        }
    }
}
