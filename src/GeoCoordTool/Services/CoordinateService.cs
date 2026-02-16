using GeoCoordTool.Models;
using System.Globalization;
using System.Text;

namespace GeoCoordTool.Services;

public sealed class CoordinateService
{
    public (double MinLon, double MaxLon, double MinLat, double MaxLat) CalculateBounds(IReadOnlyCollection<CoordinatePoint> points)
    {
        if (points.Count == 0)
        {
            throw new InvalidOperationException("无法计算四至：点位数据为空。");
        }

        return (
            points.Min(p => p.Longitude),
            points.Max(p => p.Longitude),
            points.Min(p => p.Latitude),
            points.Max(p => p.Latitude));
    }

    public (double CenterLon, double CenterLat) CalculateCenterByRange((double MinLon, double MaxLon, double MinLat, double MaxLat) bounds)
    {
        return ((bounds.MinLon + bounds.MaxLon) / 2d, (bounds.MinLat + bounds.MaxLat) / 2d);
    }

    public string BuildStandardFormat(IReadOnlyList<CoordinatePoint> points)
    {
        if (points.Count == 0)
        {
            throw new InvalidOperationException("无法生成标准格式：点位数据为空。");
        }

        var sb = new StringBuilder();
        sb.Append(points.Count.ToString(CultureInfo.InvariantCulture));

        for (var i = 0; i < points.Count; i++)
        {
            var index = i + 1;
            var point = points[i];

            sb.Append($",{index}N,{EscapeComma(point.PointNo)},F{index},");
            sb.Append(point.X.ToString("F3", CultureInfo.InvariantCulture));
            sb.Append(",");
            sb.Append(point.Y.ToString("F3", CultureInfo.InvariantCulture));
            sb.Append(",");
            sb.Append(point.Latitude.ToString("F8", CultureInfo.InvariantCulture));
            sb.Append(",");
            sb.Append(point.Longitude.ToString("F8", CultureInfo.InvariantCulture));
            sb.Append($",S{index},E{index},KT{index},{(point.AreaAccumulationFlag == "1" ? "1" : "-1")}");
        }

        return sb.ToString();
    }

    private static string EscapeComma(string value) => value.Replace(",", string.Empty);
}
