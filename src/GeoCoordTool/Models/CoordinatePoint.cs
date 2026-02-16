namespace GeoCoordTool.Models
{
    public sealed class CoordinatePoint
    {
        public string RegionCode { get; set; } = string.Empty;
        public string PointNo { get; set; } = string.Empty;
        public string PointFlag { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Elevation { get; set; }
        public string OreBodyFlag { get; set; } = string.Empty;
        public string AreaAccumulationFlag { get; set; } = string.Empty;
    }
}
