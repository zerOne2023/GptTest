using ClosedXML.Excel;
using GeoCoordTool.Models;
using System.Globalization;

namespace GeoCoordTool.Services
{
    public sealed class ExcelService
    {
        public List<CoordinatePoint> LoadPoints(string inputPath)
        {
            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException("未找到输入文件。", inputPath);
            }

            using var workbook = new XLWorkbook(inputPath);
            var sourceSheet = workbook.Worksheet(1);
            var lastRow = sourceSheet.LastRowUsed()?.RowNumber() ?? 0;

            if (lastRow < 2)
            {
                throw new InvalidOperationException("Excel 数据为空或缺少数据行。");
            }

            var points = new List<CoordinatePoint>();
            for (var row = 2; row <= lastRow; row++)
            {
                if (sourceSheet.Row(row).IsEmpty())
                {
                    continue;
                }

                points.Add(new CoordinatePoint
                {
                    RegionCode = sourceSheet.Cell(row, 1).GetString().Trim(),
                    PointNo = sourceSheet.Cell(row, 2).GetString().Trim(),
                    PointFlag = sourceSheet.Cell(row, 3).GetString().Trim(),
                    X = ParseDouble(sourceSheet.Cell(row, 4).GetString(), row, "X"),
                    Y = ParseDouble(sourceSheet.Cell(row, 5).GetString(), row, "Y"),
                    Longitude = ParseDouble(sourceSheet.Cell(row, 6).GetString(), row, "经度"),
                    Latitude = ParseDouble(sourceSheet.Cell(row, 7).GetString(), row, "纬度"),
                    Elevation = ParseDouble(sourceSheet.Cell(row, 8).GetString(), row, "标高"),
                    OreBodyFlag = sourceSheet.Cell(row, 9).GetString().Trim(),
                    AreaAccumulationFlag = sourceSheet.Cell(row, 10).GetString().Trim()
                });
            }

            if (points.Count == 0)
            {
                throw new InvalidOperationException("Excel 中不存在可用坐标行。");
            }

            return points;
        }

        public string ProcessAndSaveCopySheet(
            string inputPath,
            string outputPath,
            IReadOnlyCollection<CoordinatePoint> points,
            int projectionDigits,
            int geoDigits,
            string? mapSheetRange,
            string? standardPreview)
        {
            using var workbook = new XLWorkbook(inputPath);
            var source = workbook.Worksheet(1);
            var copyName = GetUniqueSheetName(workbook, source.Name + "_Copy");
            var copy = source.CopyTo(copyName);

            var row = 2;
            foreach (var point in points)
            {
                copy.Cell(row, 4).Value = Math.Round(point.X, projectionDigits);
                copy.Cell(row, 5).Value = Math.Round(point.Y, projectionDigits);
                copy.Cell(row, 6).Value = Math.Round(point.Longitude, geoDigits);
                copy.Cell(row, 7).Value = Math.Round(point.Latitude, geoDigits);
                row++;
            }

            copy.Cell(1, 11).Value = "图幅号范围";
            copy.Cell(1, 12).Value = "标准格式预览";
            copy.Cell(2, 11).Value = mapSheetRange ?? string.Empty;
            copy.Cell(2, 12).Value = standardPreview ?? string.Empty;

            workbook.SaveAs(outputPath);
            return outputPath;
        }

        private static string GetUniqueSheetName(XLWorkbook workbook, string preferredName)
        {
            var name = preferredName;
            var idx = 1;
            while (workbook.Worksheets.Contains(name))
            {
                name = $"{preferredName}_{idx++}";
            }

            return name;
        }

        private static double ParseDouble(string value, int row, string column)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) &&
                !double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out parsed))
            {
                throw new FormatException($"第 {row} 行 {column} 列格式错误：'{value}' 不是有效数字。");
            }

            return parsed;
        }
    }
}
