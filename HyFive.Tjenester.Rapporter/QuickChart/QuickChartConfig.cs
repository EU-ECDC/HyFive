using System.Collections.Generic;

namespace HyFive.Services.Rapporter.QuickChart
{
    public class QuickChartConfig
    {
        public string Type { get; set; }
        public Data Data { get; set; }
        public Options Options { get; set; }
    }

    public class Data
    {
        public List<string[]> Labels { get; set; }
        public List<Dataset> Datasets { get; set; }
    }

    public class Dataset
    {
        public string Label { get; set; }
        public string BackgroundColor { get; set; }
        public string Stack { get; set; }
        public List<double> Data { get; set; }
    }

    public class Options
    {
        public Plugins Plugins { get; set; }
        public Title Title { get; set; }
        public Scales Scales { get; set; }
    }

    public class Plugins
    {
        public Datalabels Datalabels { get; set; }
    }

    public class Datalabels
    {
        public string Anchor { get; set; }
        public string Align { get; set; }
        public string Color { get; set; }
        public ChartFont Font { get; set; }
        public bool Display { get; set; }
    }

    public class ChartFont
    {
        public string Weight { get; set; }
    }

    public class Title
    {
        public bool Display { get; set; }
        public string Text { get; set; }
    }

    public class Scales
    {
        public Xax[] XAxes { get; set; }
        public Yax[] YAxes { get; set; }
    }

    public class Xax
    {
        public bool Stacked { get; set; }
    }

    public class Yax
    {
        public bool Stacked { get; set; }
        public Ticks Ticks { get; set; }
        public ScaleLabel ScaleLabel { get; set; }
    }

    public class Ticks
    {
        public bool BeginAtZero { get; set; }
    }

    public class ScaleLabel
    {
        public string LabelString { get; set; } = string.Empty;
        public bool Display { get; set; }
    }
}
