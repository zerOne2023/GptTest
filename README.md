# GeoCoordTool

一个基于 .NET WPF + MVVM 的坐标处理工具，用于批量读取 Excel 点位数据、统一精度处理并输出四至、中心点、图幅号与标准格式串。

## 目录结构

- `GeoCoordTool.sln`
- `src/GeoCoordTool/`
  - `Views/`
  - `ViewModels/`
  - `Services/`
  - `Models/`

## 示例输入列定义（Sheet1）

从第 1 行作为表头、第 2 行起为数据，列顺序如下：

1. 区域号（`RegionCode`）
2. 点号（`PointNo`）
3. 点标识（`PointFlag`）
4. X 坐标（投影坐标）
5. Y 坐标（投影坐标）
6. 经度（十进制度）
7. 纬度（十进制度）
8. 标高（`Elevation`）
9. 矿体标识（`OreBodyFlag`）
10. 面积累加标识（`AreaAccumulationFlag`，`1` 表示顺向，其他记为 `-1`）

## 输出内容

- 复制首个 Sheet 到新 Sheet（默认命名 `Sheet1_Copy`）并写回处理后坐标。
- 计算并展示：
  - 经纬度四至（最小/最大经度、最小/最大纬度）
  - 中心点（**范围中心法**）
  - 拐点范围图幅号（1:100000 业务分幅规则）
  - 标准格式串预览

## 标准格式串样例

```txt
4,1N,P1,F1,345678.123,2897654.987,30.12345678,102.12345678,S1,E1,KT1,1,2N,P2,F2,345688.123,2897644.987,30.12005678,102.12545678,S2,E2,KT2,-1
```

说明：
- `FNN` / `KTN` 固定格式输出，不包含逗号。
- `1[0/-1]` 按面积累加标识转换为 `1` 或 `-1`。

## 运行提示

- 开发环境建议：Visual Studio 2022+ / .NET 8 Windows SDK。
- 本项目使用 `ClosedXML` 处理 Excel。

## SDK 故障排查（“无法找到 .NET SDK”）

如果打开项目时出现“无法找到 .NET SDK，请检查 global.json 指定版本”错误，可按以下步骤排查：

1. 先检查本机已安装 SDK：`dotnet --list-sdks`。
2. 本仓库已通过 `global.json` 锁定到 `.NET 8`（`8.0.100`，允许 feature band 内向前滚动）。
3. 若本机没有 .NET 8 SDK，请安装 **.NET 8 SDK（Windows）** 后重启 Visual Studio。
4. 若你已安装更高 patch（如 `8.0.2xx/8.0.3xx`），通常会自动匹配；如果仍报错，可删除 VS 缓存后重新加载解决方案。

> 提示：本项目目标框架为 `net8.0-windows`，仅安装 Runtime 不够，必须安装 **SDK**。
