using HyFive.Services.Reports.FourIndicators;
using HyFive.Services.Reports.QuickChart;
using iTextSharp.text.pdf;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace HyFive.Services.Reports
{
    public class Helpers
    {
        public const string DateFormat = "dd.MM.yyyy";
        public const string FileNamePrefix = "dd-MM-yyyy-HH-mm-ss";

        public static PdfReader ReadCopyOfPdfTemplateFromFile(string resourceName)
        {
            var departmentTemplateStream = typeof(FourIndicationsPDFReportService).Assembly.GetManifestResourceStream(resourceName);
            var originalPdfReader = new PdfReader(departmentTemplateStream);
            var independentDuplicateReader = new PdfReader(originalPdfReader);

            return independentDuplicateReader;
        }

        public static async Task<byte[]> CreateChart(QuickChartConfig chartconfig, int pixelWidth)
        {
            var chartconfigJson = JsonSerializer.Serialize(chartconfig, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var chartBytes = await GetChartBytes(pixelWidth, chartconfigJson);
            return chartBytes;
        }

        private static async Task<byte[]> GetChartBytes(int pixelWidth, string chartconfigJson)
        {
            var apicl = new HttpClient();
            var uri = new Uri($"https://quickchart.io/chart?w={pixelWidth}&c=" + HttpUtility.UrlEncode(chartconfigJson));

            var chartBytes = await apicl.GetByteArrayAsync(uri);
            return chartBytes;
        }
    }
}
