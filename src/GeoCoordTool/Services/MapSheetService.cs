using GeoCoordTool.Models;

namespace GeoCoordTool.Services
{
    public sealed class MapSheetService
    {
        // 业务规则：采用 1:100000 图幅网格（经差 30'，纬差 20'），输出覆盖范围图幅号。
        public string GetMapSheetRange(IReadOnlyCollection<CoordinatePoint> points)
        {
            if (points.Count == 0)
            {
                throw new InvalidOperationException("无法计算图幅号：点位数据为空。");
            }

        var sheetCodes = points
            .Select(p => BuildSheetCode(p.Longitude, p.Latitude))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

            return sheetCodes.Length switch
            {
                0 => string.Empty,
                1 => sheetCodes[0],
                _ => $"{sheetCodes[0]} ~ {sheetCodes[^1]}（共{sheetCodes.Length}幅）"
            };
        }

        private static string BuildSheetCode(double longitude, double latitude)
        {
            var millionLetter = (char)('A' + (int)Math.Floor((latitude - 0d) / 4d));
            var millionColumn = (int)Math.Floor((longitude - 72d) / 6d) + 1;

            var latInMillion = latitude % 4d;
            var lonInMillion = (longitude - 72d) % 6d;

            var row100k = (int)Math.Floor(latInMillion / (20d / 60d)) + 1;
            var col100k = (int)Math.Floor(lonInMillion / 0.5d) + 1;

            return $"{millionLetter}{millionColumn:00}-{row100k:00}{col100k:00}";
        }
    }
}
