using GeoCoordTool.Commands;
using GeoCoordTool.Models;
using GeoCoordTool.Services;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace GeoCoordTool.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly ExcelService _excelService = new();
    private readonly CoordinateService _coordinateService = new();
    private readonly MapSheetService _mapSheetService = new();

    private List<CoordinatePoint> _points = new();
    private string _inputPath = string.Empty;
    private string _outputPath = string.Empty;
    private int _projectionDigits = 3;
    private int _geoDigits = 8;
    private string _boundsText = "-";
    private string _centerText = "-";
    private string _mapSheetText = "-";
    private string _standardPreview = "-";
    private string _statusMessage = "就绪";

    public MainViewModel()
    {
        LoadExcelCommand = new RelayCommand(LoadExcel);
        ProcessCommand = new RelayCommand(Process, () => _points.Count > 0);
        ExportCommand = new RelayCommand(Export, () => _points.Count > 0 && !string.IsNullOrWhiteSpace(OutputPath));
    }

    public ICommand LoadExcelCommand { get; }
    public ICommand ProcessCommand { get; }
    public ICommand ExportCommand { get; }

    public string InputPath { get => _inputPath; set => SetProperty(ref _inputPath, value); }
    public string OutputPath { get => _outputPath; set => SetProperty(ref _outputPath, value); }
    public int ProjectionDigits { get => _projectionDigits; set => SetProperty(ref _projectionDigits, value); }
    public int GeoDigits { get => _geoDigits; set => SetProperty(ref _geoDigits, value); }
    public string BoundsText { get => _boundsText; set => SetProperty(ref _boundsText, value); }
    public string CenterText { get => _centerText; set => SetProperty(ref _centerText, value); }
    public string MapSheetText { get => _mapSheetText; set => SetProperty(ref _mapSheetText, value); }
    public string StandardPreview { get => _standardPreview; set => SetProperty(ref _standardPreview, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    private void LoadExcel()
    {
        try
        {
            var dialog = new OpenFileDialog { Filter = "Excel 文件|*.xlsx;*.xls" };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            InputPath = dialog.FileName;
            _points = _excelService.LoadPoints(InputPath);
            StatusMessage = $"已加载 {_points.Count} 个点位。";
            RefreshCommandStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "读取失败", MessageBoxButton.OK, MessageBoxImage.Warning);
            StatusMessage = "读取失败";
        }
    }

    private void Process()
    {
        try
        {
            ValidateBeforeProcess();

            var bounds = _coordinateService.CalculateBounds(_points);
            var center = _coordinateService.CalculateCenterByRange(bounds);
            var mapSheet = _mapSheetService.GetMapSheetRange(_points);
            var preview = _coordinateService.BuildStandardFormat(_points);

            BoundsText = $"经度[{bounds.MinLon:F{GeoDigits}}, {bounds.MaxLon:F{GeoDigits}}]，纬度[{bounds.MinLat:F{GeoDigits}}, {bounds.MaxLat:F{GeoDigits}}]";
            CenterText = $"中心点（范围中心法）: ({center.CenterLon:F{GeoDigits}}, {center.CenterLat:F{GeoDigits}})";
            MapSheetText = mapSheet;
            StandardPreview = preview;
            StatusMessage = "处理完成。";

            if (string.IsNullOrWhiteSpace(OutputPath) && !string.IsNullOrWhiteSpace(InputPath))
            {
                var dir = Path.GetDirectoryName(InputPath) ?? AppContext.BaseDirectory;
                var fileNoExt = Path.GetFileNameWithoutExtension(InputPath);
                OutputPath = Path.Combine(dir, fileNoExt + "_processed.xlsx");
            }

            RefreshCommandStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "处理失败", MessageBoxButton.OK, MessageBoxImage.Warning);
            StatusMessage = "处理失败";
        }
    }

    private void Export()
    {
        try
        {
            ValidateBeforeProcess();
            if (string.IsNullOrWhiteSpace(OutputPath))
            {
                throw new InvalidOperationException("导出路径不能为空。");
            }

            _excelService.ProcessAndSaveCopySheet(
                InputPath,
                OutputPath,
                _points,
                ProjectionDigits,
                GeoDigits,
                MapSheetText,
                StandardPreview);

            StatusMessage = $"导出成功：{OutputPath}";
            MessageBox.Show(StatusMessage, "导出完成", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "导出失败", MessageBoxButton.OK, MessageBoxImage.Warning);
            StatusMessage = "导出失败";
        }
    }

    private void ValidateBeforeProcess()
    {
        if (string.IsNullOrWhiteSpace(InputPath))
        {
            throw new InvalidOperationException("请先选择输入 Excel 文件。");
        }

        if (_points.Count == 0)
        {
            throw new InvalidOperationException("未加载任何可处理坐标点。");
        }

        if (ProjectionDigits is < 0 or > 10 || GeoDigits is < 0 or > 12)
        {
            throw new InvalidOperationException("保留位数超出允许范围。投影建议 0-10，经纬度建议 0-12。");
        }
    }

    private void RefreshCommandStatus()
    {
        ((RelayCommand)ProcessCommand).RaiseCanExecuteChanged();
        ((RelayCommand)ExportCommand).RaiseCanExecuteChanged();
    }
}
