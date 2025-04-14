using HyFive.Services.Rapporter.FireIndikasjoner;
using HyFive.Services.Rapporter.QuickChart;
using iTextSharp.text.pdf;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace HyFive.Services.Rapporter
{
    public class Hjelpere
    {
        public const string Datoformat = "dd.MM.yyyy";
        public const string FilnavnPrefiks = "dd-MM-yyyy-HH-mm-ss";

        public static PdfReader LesInnKopiAvPdfTemplateFraFil(string resourceName)
        {
            var avdelingTemplateStream = typeof(FireIndikasjonerPdfRapportService).Assembly.GetManifestResourceStream(resourceName);
            var originalPdfLeser = new PdfReader(avdelingTemplateStream);
            var uavhengigDuplikatLeser = new PdfReader(originalPdfLeser);

            return uavhengigDuplikatLeser;
        }

        public static async Task<byte[]> LagChart(QuickChartConfig chartconfig, int pixelWidth)
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
