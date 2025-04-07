using HyFive.Tjenester.Rapporter.Pdf;
using HyFive.Tjenester.Rapporter.QuickChart;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Rapporter.FireIndikasjoner
{
    /// <summary>
    /// En service som genererer PDF'er med bar-charts basert på input-data
    /// </summary>
    public class FireIndikasjonerPdfRapportService
    {
        const int STARTPOSITION_WITH_HEADER = 470;
        const int STARTPOSITION_WITHOUT_HEADER = 550;
        const int IMAGE_HEIGHT = 200;
        const int IMAGE_WIDTH = 200;
        const int Y_SPACER = 110;
        const int X_SPACER = 50;
        const int STARTING_PAGE = 1;

        private const int PixelWidth = 300;

        public async Task<PdfResult> LagRapportForAvdeling(FireIndikasjonerRapportForAvdeling rapportForAvdeling)
        {
            foreach (var rolle in rapportForAvdeling.Avdeling.Roller)
            {
                var rollegrafChartConfig = MakeChartConfigForRolle(rolle);
                var rollegraf = await Hjelpere.LagChart(rollegrafChartConfig, PixelWidth);
                rolle.Chart = rollegraf;
            }

            foreach (var rolle in rapportForAvdeling.Institusjon.Roller)
            {
                var rollegrafChartConfig = MakeChartConfigForRolle(rolle);
                var rollegraf = await Hjelpere.LagChart(rollegrafChartConfig, PixelWidth);
                rolle.Chart = rollegraf;
            }
            foreach (var rolle in rapportForAvdeling.SammenlignbareAvdelinger.Roller)
            {
                var rollegrafChartConfig = MakeChartConfigForRolle(rolle);
                var rollegraf = await Hjelpere.LagChart(rollegrafChartConfig, PixelWidth);
                rolle.Chart = rollegraf;
            }

            foreach (var rolle in rapportForAvdeling.Klinikker.SelectMany(r => r.Roller))
            {
                var rollegrafChartConfig = MakeChartConfigForRolle(rolle);
                var rollegraf = await Hjelpere.LagChart(rollegrafChartConfig, PixelWidth);
                rolle.Chart = rollegraf;
            }

            var pdf = LagPdfForAvdeling(rapportForAvdeling);
            return pdf;
        }

        private static QuickChartConfig MakeChartConfigForRolle(RolleMedKombinasjonerRapport rapportForRolle)
        {
            var chartconfig = new QuickChartConfig();
            chartconfig.Type = "bar";

            chartconfig.Data = new Data()
            {
                Labels = rapportForRolle.Kombinasjoner.Select(k => new[] { k.Navn, $"[N={k.AntallObservasjoner}]" }).ToList(),
                Datasets = new List<Dataset>()
                {
                    new Dataset()
                    {
                        Label = "Etterlevd",
                        BackgroundColor = "#393C61",
                        Stack = "Stack 0",
                        Data = rapportForRolle.Kombinasjoner.Select(r => Math.Round(r.ProsentEtterlevd)).ToList(),
                    },
                    new Dataset()
                    {
                        Label = "Ikke etterlevd",
                        BackgroundColor = "#FC5F56",
                        Stack = "Stack 0",
                        Data = rapportForRolle.Kombinasjoner.Select(r => Math.Round(r.ProsentIkkeEtterlevd)).ToList(),
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
                    Text = $"{rapportForRolle.Navn} [N={rapportForRolle.Kombinasjoner.Sum(k => k.AntallObservasjoner)}]"
                },
                Scales = new Scales()
                {
                    XAxes = new Xax[] { new Xax() { Stacked = true } },
                    YAxes = new Yax[] { new Yax() { Stacked = true, Ticks = new Ticks { BeginAtZero = true }, ScaleLabel = new ScaleLabel { LabelString = "[%]", Display = true } } }
                }
            };

            return chartconfig;
        }

        private static PdfResult LagPdfForAvdeling(FireIndikasjonerRapportForAvdeling rapportForAvdeling)
        {
            var rapporterSomSkalKombineres = new List<PdfResult>();
            var avdelingRapport = LagPdfForRapport(rapportForAvdeling.Avdeling);
            var filnavn = avdelingRapport.Filename;
            rapporterSomSkalKombineres.Add(avdelingRapport);

            if (rapportForAvdeling.Institusjon?.AntallObservasjoner > 0)
            {
                var institusjonRapport = LagPdfForRapport(rapportForAvdeling.Institusjon);
                rapporterSomSkalKombineres.Add(institusjonRapport);
            }

            if (rapportForAvdeling.SammenlignbareAvdelinger?.AntallObservasjoner > 0)
            {
                var sammenlignbareAvdelingerRapport = LagPdfForRapport(rapportForAvdeling.SammenlignbareAvdelinger);
                rapporterSomSkalKombineres.Add(sammenlignbareAvdelingerRapport);
            }

            foreach (var klinikk in rapportForAvdeling.Klinikker)
            {
                if (klinikk.AntallObservasjoner > 0)
                {
                    var klinikkRapport = LagPdfForRapport(klinikk);
                    rapporterSomSkalKombineres.Add(klinikkRapport);
                }
            }

            // Kombiner alle filene
            var entirePdfMemoryStream = new MemoryStream();
            var document = new Document();
            var copy = new PdfCopy(document, entirePdfMemoryStream) { CloseStream = false };
            document.Open();

            foreach (var rapport in rapporterSomSkalKombineres)
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
                Filename = filnavn
            };

        }
        private static PdfResult LagPdfForRapport(FireIndikasjonerRapport rapport)
        {
            var kopiAvAvdelingTemplate = Hjelpere.LesInnKopiAvPdfTemplateFraFil("HyFive.Tjenester.Rapporter.Assets.FHI-fire-indikasjoner-rapport-template.pdf");
            using var pdfMemoryStream = new MemoryStream();

            var pdfStamper = new PdfStamper(kopiAvAvdelingTemplate, pdfMemoryStream);

            var tidsrom = $"{rapport.FraTidspunkt.ToString(Hjelpere.Datoformat, CultureInfo.InvariantCulture)} - " +
                                $"{rapport.TilTidspunkt.ToString(Hjelpere.Datoformat, CultureInfo.InvariantCulture)}";

            pdfStamper.AcroFields.SetField("rapportnavn", $"{rapport.Navn}" + "\n" + $"{tidsrom}");

            var page = STARTING_PAGE;
            var pdfContent = pdfStamper.GetOverContent(page);
            var startingYposition = STARTPOSITION_WITH_HEADER;
            var graferPlassert = 0;

            var x1 = X_SPACER;
            var x2 = IMAGE_WIDTH + X_SPACER * 2;

            foreach (var graf in rapport.Roller.Select(r => r.Chart))
            {
                if (graferPlassert % 6 == 0 && graferPlassert != 0)
                {
                    // "bla om" og lag blank side etter 6 skrevne grafer
                    graferPlassert = 0;
                    page++;
                    startingYposition = STARTPOSITION_WITHOUT_HEADER;
                    pdfStamper.InsertPage(page, kopiAvAvdelingTemplate.GetPageSize(1));
                    pdfContent = pdfStamper.GetOverContent(page);
                }

                graferPlassert++;
                var image = Image.GetInstance(graf);
                var y1 = startingYposition - (graferPlassert - 1) * Y_SPACER;
                var y2 = startingYposition - (graferPlassert - 2) * Y_SPACER;
                var absX = graferPlassert % 2 != 0
                    ? x1 : x2;
                var absY = graferPlassert % 2 != 0
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
                Filename = $"{DateTime.UtcNow.ToString(Hjelpere.FilnavnPrefiks)}-Fire-indikasjoner-Avdelingsrapport-{rapport.Navn}.pdf"
            };

            return pdfResult;
        }
    }
}
