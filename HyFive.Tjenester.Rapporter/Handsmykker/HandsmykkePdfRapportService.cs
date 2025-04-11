using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using HyFive.Tjenester.Rapporter.QuickChart;
using HyFive.Tjenester.Rapporter.Pdf;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;
using Image = iTextSharp.text.Image;
using Microsoft.Extensions.Hosting;

namespace HyFive.Tjenester.Rapporter.Handsmykker
{
    public class HandsmykkePdfRapportService
    {
        private const int PixelWidth = 800;
        private const string TotaltForAlleRoller = "Alle profesjoner";
        private readonly List<RolleTilFargeMap> _rolleTilFargeMapListe = new();

        public PdfResult LagRapportForAvdeling(HandsmykkerapportForSmykketypeOgRolle rapport)
        {
            LagRolleTilFargeMap(rapport);

            var grafForAvdeling = LagGraf(rapport.RapportForAvdeling, "Rapport for avdeling");
            var grafForInstitusjon = LagGraf(rapport.RapportForInstitusjon, "Rapport for institusjon");
            var pdf = LagPdf(grafForAvdeling, grafForInstitusjon, rapport);

            return pdf;
        }

        /// <summary>
        /// Finn alle roller og tilordne en farge for hver av dem
        /// Legg inn "rolle" TotaltForAlleRoller først i lista
        /// </summary>
        /// <param name="rapport"></param>
        /// <returns></returns>
        private void LagRolleTilFargeMap(HandsmykkerapportForSmykketypeOgRolle rapport)
        {
            var alleRollerAvdeling = rapport.RapportForAvdeling.SmykketypeOgAntallForRolleListe.SelectMany(p => p.AntallForRolleListe.Select(q => q.Rolle)).ToList();
            var alleRollerInstitusjon = rapport.RapportForInstitusjon.SmykketypeOgAntallForRolleListe.SelectMany(p => p.AntallForRolleListe.Select(q => q.Rolle)).ToList();

            var alleRoller = alleRollerAvdeling;
            alleRoller.AddRange(alleRollerInstitusjon);
            alleRoller = alleRoller.Distinct().ToList();
            alleRoller.Insert(0, TotaltForAlleRoller);

            foreach (var rolle in alleRoller.Select((navn, index) => (navn, index)))
            {
                _rolleTilFargeMapListe.Add(new RolleTilFargeMap { Rolle = rolle.navn, Farge = VelgFarge(rolle.index) });
            }
        }

        private class RolleTilFargeMap
        {
            public string Rolle { get; init; }
            public string Farge { get; init; }
        }

        private byte[] LagGraf(RapportForEnhet rapportForEnhet, string header)
        {
            var smykketyper = rapportForEnhet.SmykketypeOgAntallForRolleListe
                .Select(p => p.Smykketype.Name)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            var roller = rapportForEnhet.SmykketypeOgAntallForRolleListe
                .SelectMany(p => p.AntallForRolleListe)
                .Select(p => p.Rolle)
                .Distinct()
                .ToList();

            var datasets = LagDatasets(roller, smykketyper);
            var data = LagData(rapportForEnhet, smykketyper, datasets);
            var options = LagOptions(header);

            var chartconfig = new QuickChartConfig
            {
                Type = "bar",
                Data = data,
                Options = options
            };

            var graf = Hjelpere.LagChart(chartconfig, PixelWidth).Result;

            return graf;
        }

        /// <summary>
        /// Lag et dataset for total for alle roller + et for hver rolle, med plass til verdier for hver smykketype
        /// </summary>
        /// <param name="roller"></param>
        /// <param name="smykketyper"></param>
        /// <returns></returns>
        private List<Dataset> LagDatasets(List<string> roller, ICollection smykketyper)
        {
            var datasets = new List<Dataset>();

            var datasetTotalForAlleRoller = new Dataset
            {
                Label = TotaltForAlleRoller,
                Stack = TotaltForAlleRoller,
                BackgroundColor = FinnFargeForRolle(TotaltForAlleRoller),
                Data = new List<double>(new double[smykketyper.Count])
            };

            datasets.Add(datasetTotalForAlleRoller);

            foreach (var rolle in roller.OrderBy(p => p))
            {
                var dataset = new Dataset
                {
                    Label = rolle,
                    Stack = rolle,
                    BackgroundColor = FinnFargeForRolle(rolle),
                    Data = new List<double>(new double[smykketyper.Count])
                };

                datasets.Add(dataset);
            }

            return datasets;
        }

        /// <summary>
        /// Finn farge for rolle i rolleTilFargeMapListe
        /// </summary>
        /// <param name="rolle"></param>
        /// <returns></returns>
        private string FinnFargeForRolle(string rolle)
        {
            var farge = _rolleTilFargeMapListe.First(p => p.Rolle == rolle).Farge;
            return farge;
        }

        /// <summary>
        /// Fyll ut prosentvis antall for hver smykketype både totalt for alle roller og for hver rolle
        /// 
        /// Prosent for rolle for en smykketype: antall registreringer med smykketype for rolle / antall observasjoner for roll
        /// Eksempel: Av 20 observasjoner av Lege har 10 ring = 50%, 5 klokke = 25%, osv
        ///
        /// Prosent for alle roller for en smykketype: summen av bruk av smykketype for alle roller / summen av antall observasjoner for rolle som har brukt smykketype
        /// Eksempel: Det er 100 observasjoner av sykepleier, 40 bruker ring (altså 40%), det er 100 observasjoner av Lege der 20 bruker ring, altså 20%.
        /// Totalt 200 observasjoner hvorav 60 (40+20)  bruker ring (altså 30% av alle).
        /// </summary>
        /// <param name="rapportForEnhet"></param>
        /// <param name="smykketyper"></param>
        /// <param name="datasets"></param>
        /// <returns></returns>
        private static Data LagData(RapportForEnhet rapportForEnhet, IReadOnlyCollection<string> smykketyper, List<Dataset> datasets)
        {
            foreach (var smykketype in smykketyper.Select((navn, index) => (navn, index)))
            {
                var antallforRolleListe = rapportForEnhet.SmykketypeOgAntallForRolleListe
                    .Where(p => p.Smykketype.Name == smykketype.navn)
                    .SelectMany(p => p.AntallForRolleListe)
                    .ToList();

                var antallObservasjonerForAlleRoller = 0;
                foreach (var antallForRolle in antallforRolleListe)
                {
                    var antallObservasjonerForRolle = rapportForEnhet.ObservasjonerForRolleListe.First(p => p.Rolle == antallForRolle.Rolle).Antall;
                    var prosent = antallForRolle.Antall * 100 / antallObservasjonerForRolle;
                    var dataset = datasets.First(p => p.Label == antallForRolle.Rolle);
                    dataset.Data[smykketype.index] = prosent;

                    antallObservasjonerForAlleRoller += antallObservasjonerForRolle;
                }

                // Prosent for alle roller = summen av bruk av smykketype for alle roller / summen av antall observasjoner for rolle som har brukt smykketype
                var totaltAntallForRoller = antallforRolleListe.Select(p => p.Antall).Sum();
                var prosentTotalForRoller = totaltAntallForRoller * 100 / antallObservasjonerForAlleRoller;
                var datasetForAlleRoller = datasets.First(p => p.Label == TotaltForAlleRoller);
                datasetForAlleRoller.Data[smykketype.index] = prosentTotalForRoller;
            }

            var labels = smykketyper.Select(p => new[] { p }).ToList();
            var data = new Data
            {
                Labels = labels,
                Datasets = datasets
            };

            return data;
        }

        private static Options LagOptions(string header)
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
                    YAxes = new[] { new Yax { Stacked = false, Ticks = new Ticks { BeginAtZero = true }, ScaleLabel = new ScaleLabel { LabelString = "[%]", Display = true } } }
                }
            };

            return options;
        }

        /// <summary>
        /// En farge for hver rolle i grafen + en for summen for alle roller
        /// Det bør være minst like mange farger som det er roller
        /// </summary>
        private static readonly List<string> Farger = new()
        {
            ColorTranslator.ToHtml(Color.FromArgb(Color.Blue.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Brown.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.DarkOrange.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Red.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Green.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Purple.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.LimeGreen.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.DodgerBlue.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Plum.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Fuchsia.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.SlateGray.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Peru.ToArgb())),
            ColorTranslator.ToHtml(Color.FromArgb(Color.Turquoise.ToArgb())),
        };

        /// <summary>
        /// Hvis index er større enn siste farge i listen så velges den siste
        /// </summary>
        /// <returns></returns>
        private static string VelgFarge(int index)
        {
            var farge = index < Farger.Count ? Farger[index] : Farger[^1];
            return farge;
        }

        private const int YStartMedHeader = 200;
        private const int YStartUtenHeader = 450;
        private const int ImageWidth = 500;
        private const int ImageHeight = 300;
        private const int YSpacing = 50;
        private const int MarginVenstre = 50;

        private static PdfResult LagPdf(byte[] grafForAvdeling, byte[] grafForInstitusjon, HandsmykkerapportForSmykketypeOgRolle rapport)
        {
            var kopiAvTemplate = Hjelpere.LesInnKopiAvPdfTemplateFraFil("HyFive.Tjenester.Rapporter.Assets.Rapport-template.pdf");
            using var pdfMemoryStream = new MemoryStream();
            var pdfStamper = new PdfStamper(kopiAvTemplate, pdfMemoryStream);

            FyllUtHeader(rapport, pdfStamper);

            const int sideNr = 2;
            var yStart = YStartUtenHeader;
            pdfStamper.InsertPage(sideNr, kopiAvTemplate.GetPageSize(1));
            var pdfContent = pdfStamper.GetOverContent(sideNr);
            var image = LagImage(grafForAvdeling, yStart);
            pdfContent.AddImage(image);

            yStart -= ImageHeight + YSpacing;
            image = LagImage(grafForInstitusjon, yStart);
            pdfContent.AddImage(image);

            pdfStamper.FormFlattening = true;
            pdfStamper.Close();

            var pdfResult = new PdfResult
            {
                Content = pdfMemoryStream.ToArray(),
                Filename = $"{DateTime.Now.ToString(Hjelpere.FilnavnPrefiks)}-Handsmykke-Avdelingsrapport-{rapport.Avdeling}.pdf"
            };

            return pdfResult;
        }

        private static void FyllUtHeader(HandsmykkerapportForSmykketypeOgRolle rapport, PdfStamper pdfStamper)
        {
            pdfStamper.AcroFields.SetField("tittel", "Rapport observasjoner av smitteforebyggende tiltak (NOST)");
            pdfStamper.AcroFields.SetField("undertittel", "Modul 2: Smykker, klokker og negler");
            pdfStamper.AcroFields.SetField("institusjon", $"Institusjon: {rapport.Institusjon}");
            pdfStamper.AcroFields.SetField("avdeling", $"Avdeling: {rapport.Avdeling}");
            pdfStamper.AcroFields.SetField("tidsrom", "Registrert tidsrom: " +
                                                      $"{rapport.FraTidspunkt.ToString(Hjelpere.Datoformat, CultureInfo.InvariantCulture)} - " +
                                                      $"{rapport.TilTidspunkt.ToString(Hjelpere.Datoformat, CultureInfo.InvariantCulture)}");
            pdfStamper.AcroFields.SetField("rapportdato", $"Rapportdato: {DateTime.Today.ToString(Hjelpere.Datoformat)}");

            FyllUtInfoForEnhet(rapport.RapportForAvdeling, pdfStamper, "Antall observasjoner - Avdeling", "infoAvdeling");
            FyllUtInfoForEnhet(rapport.RapportForInstitusjon, pdfStamper, "Antall observasjoner - Institusjon", "infoInstitusjon");
        }

        private static void FyllUtInfoForEnhet(RapportForEnhet rapportForEnhet, PdfStamper pdfStamper, string header, string felt)
        {
            var totaltAntallObservasjoner = rapportForEnhet.ObservasjonerForRolleListe
                .Select(p => p.Antall)
                .Sum();

            var info = header + Environment.NewLine;
            info += Environment.NewLine;
            info += $"Totalt, alle profesjoner: {totaltAntallObservasjoner}" + Environment.NewLine;
            foreach (var observasjonerForRolle in rapportForEnhet.ObservasjonerForRolleListe.OrderBy(p => p.Rolle))
            {
                info += $"{observasjonerForRolle.Rolle}: {observasjonerForRolle.Antall}" + Environment.NewLine;
            }

            pdfStamper.AcroFields.SetField(felt, $"{info}");
        }

        private static Image LagImage(byte[] graf, int yPos)
        {
            var image = Image.GetInstance(graf);
            image.SetAbsolutePosition(MarginVenstre, yPos);
            image.ScaleAbsolute(ImageWidth, ImageHeight);

            return image;
        }
    }
}
