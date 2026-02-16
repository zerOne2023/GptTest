namespace GeoCoordTool.Models
{
    public sealed class ProcessingResult
    {
        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }
        public double CenterLongitude { get; set; }
        public double CenterLatitude { get; set; }
        public string MapSheetRange { get; set; } = string.Empty;
        public string StandardFormatPreview { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
    }
}
