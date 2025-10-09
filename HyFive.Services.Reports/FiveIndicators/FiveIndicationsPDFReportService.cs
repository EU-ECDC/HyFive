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
        const int IMAGE_HEIGHT = 180;
        const int IMAGE_WIDTH = 200;
        const int Y_SPACER = 95;
        const int X_SPACER = 50;
        const int STARTING_PAGE = 1;

        private const int PixelWidth = 500;

        public async Task<PdfResult> CreateDepartmentReport(FiveIndicatorsReportForDepartment departmentReport)
        {
            foreach (var role in departmentReport.Department.Roles)
            {
                var roleGraphChartConfig = MakeChartConfigForRole(role);
                var roleGraph = await Helpers.CreateChart(roleGraphChartConfig, PixelWidth);
                role.Chart = roleGraph;
            }

            foreach (var role in departmentReport.Facility.Roles)
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

            foreach (var role in departmentReport.Units.SelectMany(r => r.Roles))
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
                        BackgroundColor = "#65B32E",
                        Stack = "Stack 0",
                        Data = roleReport.Combinations.Select(r => Math.Round(r.PercentComplied)).ToList(),
                    },
                    new Dataset()
                    {
                        Label = "Not complied with",
                        BackgroundColor = "#e0271a",
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
                    YAxes = new Yax[] { new Yax() { Stacked = true, Ticks = new Ticks { BeginAtZero = true }, ScaleLabel = new ScaleLabel { LabelString = "Compliance (%)", Display = true } } }
                }
            };

            return chartconfig;
        }

        private static PdfResult CreateDepartmentPdf(FiveIndicatorsReportForDepartment reportForDepartment)
        {
            var reportsToBeCombined = new List<PdfResult>();
            var departmentReport = CreatePdfReport(reportForDepartment.Department, "Departments:");
            var fileName = departmentReport.Filename;
            reportsToBeCombined.Add(departmentReport);

            if (reportForDepartment.Facility?.NumberOfObservations > 0)
            {
                var facilityReport = CreatePdfReport(reportForDepartment.Facility, "Facilities:");
                reportsToBeCombined.Add(facilityReport);
            }

            if (reportForDepartment.ComparableDepartments?.NumberOfObservations > 0)
            {
                var comparableDepartmentsReport = CreatePdfReport(reportForDepartment.ComparableDepartments, "Comparable Departments:");
                reportsToBeCombined.Add(comparableDepartmentsReport);
            }

            foreach (var unit in reportForDepartment.Units)
            {
                if (unit.NumberOfObservations > 0)
                {
                    var unitReport = CreatePdfReport(unit, null);
                    reportsToBeCombined.Add(unitReport);
                }
            }

            // Combine all the files
            var entirePdfMemoryStream = new MemoryStream();
            var document = new Document();
            var copy = new PdfCopy(document, entirePdfMemoryStream) { CloseStream = false };
            document.Open();

            foreach (var report in reportsToBeCombined)
            {
                var reportReader = new PdfReader(report.Content);
                for (int pageIndex = 1; pageIndex <= reportReader.NumberOfPages; pageIndex++)
                {
                    copy.AddPage(copy.GetImportedPage(reportReader, pageIndex));
                }
            }
            document.Close();

            return new PdfResult
            {
                Content = entirePdfMemoryStream.ToArray(),
                Filename = fileName
            };

        }
        private static PdfResult CreatePdfReport(FiveIndicatorsReport report, string label)
        {
            var copyOfDepartmentTemplate = Helpers.ReadCopyOfPdfTemplateFromFile("HyFive.Services.Reports.Assets.FHI-five-indications-report-template.pdf");
            using var pdfMemoryStream = new MemoryStream();

            var pdfStamper = new PdfStamper(copyOfDepartmentTemplate, pdfMemoryStream);

            var timePeriod = $"{report.FromDate.ToString(Helpers.DateFormat, CultureInfo.InvariantCulture)} - " +
                                $"{report.ToDate.ToString(Helpers.DateFormat, CultureInfo.InvariantCulture)}";

            pdfStamper.AcroFields.SetField("reportName", $"{report.Name}" + "\n" + $"{timePeriod}");

            var page = STARTING_PAGE;
            var pdfContentOver = pdfStamper.GetOverContent(page);   // <-- for label (visible)
            var pdfContentUnder = pdfStamper.GetUnderContent(page);
            var graphsPlaced = 0;

            var x1 = X_SPACER;
            var x2 = IMAGE_WIDTH + X_SPACER * 2;

            var labelNames = $"{report.Name}\nTime Period: {timePeriod}  |  Number of Observations: {report.NumberOfObservations}";

            float labelY = pdfStamper.Reader.GetPageSize(1).Height - 170f;
            float startingYposition = AddSectionLabel(pdfContentOver, label, labelNames, X_SPACER, labelY, 450f);
            startingYposition -= 180f;
            foreach (var graf in report.Roles.Select(r => r.Chart))
            {
                if (graphsPlaced % 6 == 0 && graphsPlaced != 0)
                {
                    // "flip page" and create a blank page after 6 rendered graphs
                    graphsPlaced = 0;
                    page++;
                    pdfStamper.InsertPage(page, copyOfDepartmentTemplate.GetPageSize(1));
                    pdfContentUnder = pdfStamper.GetUnderContent(page);
                    pdfContentOver = pdfStamper.GetOverContent(page);

                    startingYposition = pdfStamper.Reader.GetPageSize(1).Height - 170f;

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
                pdfContentUnder.AddImage(image);

            }

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();

            var pdfResult = new PdfResult
            {
                Content = pdfMemoryStream.ToArray(),
                Filename = $"{DateTime.UtcNow.ToString(Helpers.FileNamePrefix)}-ECDC hyFive Five Indications - DepartmentReport.pdf"
            };

            return pdfResult;
        }

        private static float AddSectionLabel(PdfContentByte pdfContent, string sectionTitle, string labelNames, float x, float y, float maxWidth = 500f)
        {
            if (string.IsNullOrEmpty(labelNames)) return y;

            // Build the full label text
            var labelText = $"{sectionTitle} {labelNames}";

            // Create font
            var font = LoadRobotoFont();
            float fontSize = 7f;
            pdfContent.SetColorFill(BaseColor.Black);

            var manualLines = labelText.Split(new[] { '\n' }, StringSplitOptions.None);
            var lines = new List<string>();

            foreach (var manualLine in manualLines)
            {
                var wrapped = WrapText(manualLine, font, fontSize, maxWidth);
                lines.AddRange(wrapped);
            }

            float lineHeight = fontSize + 2; // vertical spacing
            float currentY = y;

            pdfContent.BeginText();
            pdfContent.SetFontAndSize(font, fontSize);

            foreach (var line in lines)
            {
                pdfContent.ShowTextAligned(Element.ALIGN_LEFT, line, x, currentY, 0);
                currentY -= lineHeight;
            }

            pdfContent.EndText();

            return currentY - 20f;
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

        private static BaseFont LoadRobotoFont(string fontFileName = "Roboto-Regular.ttf")
        {
            var assembly = typeof(FiveIndicationsPDFReportService).Assembly;
            var resourcePath = $"HyFive.Services.Reports.Assets.Fonts.{fontFileName}";

            using var fontStream = assembly.GetManifestResourceStream(resourcePath);
            if (fontStream == null)
                throw new Exception($"Font resource '{resourcePath}' not found.");

            using var ms = new MemoryStream();
            fontStream.CopyTo(ms);
            var fontData = ms.ToArray();

            return BaseFont.CreateFont(fontFileName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, true, fontData, null);
        }
    }
}
