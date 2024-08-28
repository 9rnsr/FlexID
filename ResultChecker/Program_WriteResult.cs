using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ResultChecker
{
    internal partial class Program
    {
        static void WriteSummaryExcel(string filePath, Result[] sortedResults)
        {
            // 非商用ライセンスを設定
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // 結果の要約シートを作成。
                var sheetSummary = package.Workbook.Worksheets.Add("Summary");
                WriteSummarySheet(sheetSummary, sortedResults);

                package.SaveAs(filePath);
            }
        }

        static void WriteSummarySheet(ExcelWorksheet sheet, IEnumerable<Result> results)
        {
            sheet.Cells[1, 1].Value = "Summary";

            sheet.Cells[3, 1].Value = "Target";
            sheet.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[3, 1, 4, 1].Merge = true;

            sheet.Cells[3, 2].Value = "Whole Body Effective Dose";
            sheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[3, 2, 3, 4].Merge = true;
            sheet.Cells[4, 2].Value = "OIR";
            sheet.Cells[4, 3].Value = "FlexID";
            sheet.Cells[4, 4].Value = "Diff";
            sheet.Cells[4, 2, 4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[4, 2, 4, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[4, 2, 4, 4].Style.WrapText = true;

            sheet.Cells[3, 6].Value = "Whole Body";
            sheet.Cells[3, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[3, 6, 3, 7].Merge = true;
            sheet.Cells[4, 6].Value = "Diff (min)";
            sheet.Cells[4, 7].Value = "Diff (max)";
            sheet.Cells[4, 6, 4, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[4, 6, 4, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[4, 6, 4, 7].Style.WrapText = true;

            sheet.Cells[3, 8].Value = "Urine";
            sheet.Cells[3, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[3, 8, 3, 9].Merge = true;
            sheet.Cells[4, 8].Value = "Diff (min)";
            sheet.Cells[4, 9].Value = "Diff (max)";
            sheet.Cells[4, 8, 4, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[4, 8, 4, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[4, 8, 4, 9].Style.WrapText = true;

            sheet.Cells[3, 10].Value = "Faeces";
            sheet.Cells[3, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[3, 10, 3, 11].Merge = true;
            sheet.Cells[4, 10].Value = "Diff (min)";
            sheet.Cells[4, 11].Value = "Diff (max)";
            sheet.Cells[4, 10, 4, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[4, 10, 4, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[4, 10, 4, 11].Style.WrapText = true;

            var r = 5;
            foreach (var res in results)
            {
                sheet.Cells[r, 1].Value = res.Target;

                if (res.HasErrors)
                {
                    sheet.Cells[r, 2, r, 4].Value = "-";
                    sheet.Cells[r, 6, r, 11].Value = "-";
                }
                else
                {
                    var cellEffDoseE = sheet.Cells[r, 2];
                    var cellEffDoseA = sheet.Cells[r, 3];
                    var cellEffDoseR = sheet.Cells[r, 4];
                    cellEffDoseE.Value = double.Parse(res.ExpectEffectiveDose);
                    cellEffDoseA.Value = double.Parse(res.ActualEffectiveDose);
                    cellEffDoseR.Formula = $"{cellEffDoseA.Address}/{cellEffDoseE.Address}";
                    cellEffDoseE.Style.Numberformat.Format = "0.0E+00";
                    cellEffDoseA.Style.Numberformat.Format = "0.0E+00";
                    cellEffDoseR.Style.Numberformat.Format = "0.0%";

                    var cellWholeBody1 = sheet.Cells[r, 6];
                    var cellWholeBody2 = sheet.Cells[r, 7];
                    cellWholeBody1.Value = res.WholeBodyActivityFractionMin;
                    cellWholeBody2.Value = res.WholeBodyActivityFractionMax;
                    cellWholeBody1.Style.Numberformat.Format = "0.0%";
                    cellWholeBody2.Style.Numberformat.Format = "0.0%";

                    var cellUrine1 = sheet.Cells[r, 8];
                    var cellUrine2 = sheet.Cells[r, 9];
                    cellUrine1.Value = res.UrineActivityFractionMin;
                    cellUrine2.Value = res.UrineActivityFractionMax;
                    cellUrine1.Style.Numberformat.Format = "0.0%";
                    cellUrine2.Style.Numberformat.Format = "0.0%";

                    var cellFaeces1 = sheet.Cells[r, 10];
                    var cellFaeces2 = sheet.Cells[r, 11];
                    cellFaeces1.Value = res.FaecesActivityFractionMin;
                    cellFaeces2.Value = res.FaecesActivityFractionMax;
                    cellFaeces1.Style.Numberformat.Format = "0.0%";
                    cellFaeces2.Style.Numberformat.Format = "0.0%";
                }

                r++;
            }

            // 預託実効線量のFlexID/OIR比にカラースケールを設定。
            var cellsDose = sheet.Cells[5, 4, r - 1, 4];
            SetPercentColorScale(cellsDose);

            // 残留放射能のFlexID/OIR比にカラースケールを設定。
            var cellsActivity = sheet.Cells[5, 6, r - 1, 11];
            SetPercentColorScale(cellsActivity);

            sheet.Column(1).AutoFit();
            //sheet.Cells.AutoFitColumns(0);  // Autofit columns for all cells
        }

        private static void SetPercentColorScale(ExcelRange cells)
        {
            var sheet = cells.Worksheet;
            var condition = sheet.ConditionalFormatting.AddThreeColorScale(cells);

            condition.LowValue.Type = eExcelConditionalFormattingValueObjectType.Num;
            condition.LowValue.Value = 0.5;
            condition.LowValue.Color = Color.LightBlue;
            condition.MiddleValue.Type = eExcelConditionalFormattingValueObjectType.Num;
            condition.MiddleValue.Value = 1.0;
            condition.MiddleValue.Color = Color.LightGreen;
            condition.HighValue.Type = eExcelConditionalFormattingValueObjectType.Num;
            condition.HighValue.Value = 1.5;
            condition.HighValue.Color = Color.Orange;
        }

        static void WriteSummaryCsv(IEnumerable<Result> results)
        {
            var summaryHeaders = new[]
            {
                "Summary",
                "",
                "Target,Whole Body Effective Dose,,,,Whole Body,,Urine,,Faeces",
                ",OIR,FlexID,Diff (FlexID/OIR),,Diff (min),Diff (max),Diff (min),Diff (max),Diff (min),Diff (max)",
            };
            var summaryLines = results.Select(res =>
            {
                var line = $"{res.Target}";
                if (res.HasErrors)
                    line += ",-,-,-,-,-";
                else
                {
                    line += $",{res.ExpectEffectiveDose}" +
                            $",{res.ActualEffectiveDose}" +
                            $",{res.FractionEffectiveDose:0.00%},";
                    line += $",{res.WholeBodyActivityFractionMin:0.00%}" +
                            $",{res.WholeBodyActivityFractionMax:0.00%}" +
                            $",{res.UrineActivityFractionMin:0.00%}" +
                            $",{res.UrineActivityFractionMax:0.00%}" +
                            $",{res.FaecesActivityFractionMin:0.00%}" +
                            $",{res.FaecesActivityFractionMax:0.00%}";
                }
                return line;
            });
            File.WriteAllLines("summary.csv", summaryHeaders.Concat(summaryLines));
        }
    }
}
