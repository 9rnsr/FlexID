using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
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

                // 対象毎の残留放射能の確認シートを作成。
                foreach (var res in sortedResults)
                {
                    var sheetRes = package.Workbook.Worksheets.Add(res.Target);
                    WriteResultSheet(sheetRes, res);
                }

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

        static void WriteResultSheet(ExcelWorksheet sheet, Result res)
        {
            var actualActs = GetResultRetentions(res.Target);
            var expectActs = GetExpectRetentions(res.Target, res.Material);

            sheet.Cells[1, 1].Value = res.Target;

            const int rowH = 4;
            const int rowT = rowH + 1;
            const int colE = 1;
            const int colA = 6;
            const int colD = 11;
            const int colC = 16;

            sheet.Cells[rowH - 1, colE + 0].Value = "OIR";
            sheet.Cells[rowH, colE + 0].Value = "Time, days";
            sheet.Cells[rowH, colE + 1].Value = "Whole Body";
            sheet.Cells[rowH, colE + 2].Value = "Urine\n(24-hour sample)";
            sheet.Cells[rowH, colE + 3].Value = "Faeces\n(24-hour sample)";
            sheet.Cells[rowH, colE + 0, rowH, colE + 3].Style.WrapText = true;
            sheet.Cells[rowH, colE + 0, rowH, colE + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            sheet.Cells[rowH - 1, colA + 0].Value = "FlexID";
            sheet.Cells[rowH, colA + 0].Value = "Time, days";
            sheet.Cells[rowH, colA + 1].Value = "Whole Body";
            sheet.Cells[rowH, colA + 2].Value = "Urine\n(24-hour)";
            sheet.Cells[rowH, colA + 3].Value = "Faeces\n(24-hour)";
            sheet.Cells[rowH, colA + 0, rowH, colA + 3].Style.WrapText = true;
            sheet.Cells[rowH, colA + 0, rowH, colA + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            sheet.Cells[rowH - 1, colD + 0].Value = "Difference";
            sheet.Cells[rowH, colD + 0].Value = "Whole Body";
            sheet.Cells[rowH, colD + 1].Value = "Urine";
            sheet.Cells[rowH, colD + 2].Value = "Faeces";
            sheet.Cells[rowH, colD + 0, rowH, colD + 2].Style.WrapText = true;
            sheet.Cells[rowH, colD + 0, rowH, colD + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            sheet.Row(rowH).Height *= 3;
            //sheet.Cells[rowH, colE, rowH, colE + 3].AutoFitColumns(0);
            //sheet.Cells[rowH, colA, rowH, colA + 3].AutoFitColumns(0);
            //sheet.Cells[rowH, colD, rowH, colD + 3].AutoFitColumns(0);

            var nrow = 0;
            foreach (var (actualAct, expectAct) in CompareRetentions(actualActs, expectActs))
            {
                var r = rowT + nrow;

                var cellTimeE      /**/= sheet.Cells[r, colE + 0];
                var cellWholeBodyE /**/= sheet.Cells[r, colE + 1];
                var cellUrineE     /**/= sheet.Cells[r, colE + 2];
                var cellFaecesE    /**/= sheet.Cells[r, colE + 3];
                cellTimeE      /**/.Value = expectAct.EndTime;
                cellWholeBodyE /**/.Value = expectAct.WholeBody;
                cellUrineE     /**/.Value = expectAct.Urine ?? (object)"-";
                cellFaecesE    /**/.Value = expectAct.Faeces ?? (object)"-";

                var cellTimeA      /**/= sheet.Cells[r, colA + 0];
                var cellWholeBodyA /**/= sheet.Cells[r, colA + 1];
                var cellUrineA     /**/= sheet.Cells[r, colA + 2];
                var cellFaecesA    /**/= sheet.Cells[r, colA + 3];
                cellTimeA      /**/.Value = actualAct.EndTime;
                cellWholeBodyA /**/.Value = actualAct.WholeBody;
                cellUrineA     /**/.Value = actualAct.Urine ?? (object)"-";
                cellFaecesA    /**/.Value = actualAct.Faeces ?? (object)"-";

                var cellWholeBodyR /**/= sheet.Cells[r, colD + 0];
                var cellUrineR     /**/= sheet.Cells[r, colD + 1];
                var cellFaecesR    /**/= sheet.Cells[r, colD + 2];
                cellWholeBodyR/**/.Formula = $"{cellWholeBodyA.Address}/{cellWholeBodyE.Address}";
                cellUrineR    /**/.Formula = $"IFERROR({cellUrineA.Address}/{cellUrineE.Address},\"-\")";
                cellFaecesR   /**/.Formula = $"IFERROR({cellFaecesA.Address}/{cellFaecesE.Address},\"-\")";

                nrow++;
            }

            var sr = rowT;
            var er = rowT + nrow - 1;

            var cellsE = sheet.Cells[sr, colE + 1, er, colE + 3];
            var cellsA = sheet.Cells[sr, colA + 1, er, colA + 3];
            var cellsD = sheet.Cells[sr, colD, er, colD + 2];
            cellsE.Style.Numberformat.Format = "0.0E+00";
            cellsA.Style.Numberformat.Format = "0.0E+00";
            cellsD.Style.Numberformat.Format = "0.0%";

            // 時間メッシュ毎の残留放射能のFlexID/OIR比にカラースケールを設定。
            SetPercentColorScale(cellsD);

            //sheet.Cells[rowT, colE, er, colE + 3].AutoFitColumns(0);
            //sheet.Cells[rowT, colA, er, colA + 3].AutoFitColumns(0);
            //sheet.Cells[rowT, colD, er, colD + 3].AutoFitColumns(0);

            var timesE     /**/= sheet.Cells[sr, colE + 0, er, colE + 0];
            var wholeBodyE /**/= sheet.Cells[sr, colE + 1, er, colE + 1];
            var urineE     /**/= sheet.Cells[sr, colE + 2, er, colE + 2];
            var faecesE    /**/= sheet.Cells[sr, colE + 3, er, colE + 3];

            var timesA     /**/= sheet.Cells[sr, colA + 0, er, colA + 0];
            var wholeBodyA /**/= sheet.Cells[sr, colA + 1, er, colA + 1];
            var urineA     /**/= sheet.Cells[sr, colA + 2, er, colA + 2];
            var faecesA    /**/= sheet.Cells[sr, colA + 3, er, colA + 3];

            var chartW = sheet.Drawings.AddScatterChart("ChartWholeBody", eScatterChartType.XYScatter);
            var chartU = sheet.Drawings.AddScatterChart("ChartUrine", eScatterChartType.XYScatter);
            var chartF = sheet.Drawings.AddScatterChart("ChartFaeces", eScatterChartType.XYScatter);
            chartW.Title.Text = "Whole Body";
            chartU.Title.Text = "Urine";
            chartF.Title.Text = "Faeces";

            SetActivityChartStyle(chartW, rowT/*         */, colC, 22, 12);
            SetActivityChartStyle(chartU, chartW.To.Row + 2, colC, 22, 12);
            SetActivityChartStyle(chartF, chartU.To.Row + 2, colC, 22, 12);

            var serieWholeBodyE = chartW.Series.Add(wholeBodyE, timesE);
            var serieWholeBodyA = chartW.Series.Add(wholeBodyA, timesA);
            SetExpectSerieStyle(serieWholeBodyE, "Whole Body");
            SetActualSerieStyle(serieWholeBodyA, "Whole Body");

            var serieUrineE = chartU.Series.Add(urineE, timesE);
            var serieUrineA = chartU.Series.Add(urineA, timesA);
            SetExpectSerieStyle(serieUrineE, "Urine");
            SetActualSerieStyle(serieUrineA, "Urine");

            var serieFaecesE = chartF.Series.Add(faecesE, timesE);
            var serieFaecesA = chartF.Series.Add(faecesA, timesA);
            SetExpectSerieStyle(serieFaecesE, "Faeces");
            SetActualSerieStyle(serieFaecesA, "Faeces");
        }

        static ExcelScatterChart SetActivityChartStyle(ExcelScatterChart chart, int row, int col, int nrow, int ncol)
        {
            row--;
            col--;

            chart.From.Row = row;
            chart.From.Column = col;
            chart.To.Row = row + nrow;
            chart.To.Column = col + ncol;

            chart.DisplayBlanksAs = eDisplayBlanksAs.Span;

            chart.RoundedCorners = false;

            chart.Fill.Color = Color.White;

            chart.Border.LineStyle = eLineStyle.Solid;
            chart.Border.Width = 1;
            chart.Border.Fill.Color = Color.Black;

            chart.PlotArea.Border.LineStyle = eLineStyle.Solid;
            chart.PlotArea.Border.Width = 1;
            chart.PlotArea.Border.Fill.Color = Color.Black;

            chart.XAxis.AddGridlines(addMajor: true, addMinor: false);
            chart.XAxis.MajorGridlines.LineStyle = eLineStyle.Solid;
            chart.XAxis.MajorGridlines.Width = 1;
            chart.XAxis.MajorGridlines.Fill.Color = Color.LightGray;

            chart.YAxis.AddGridlines(addMajor: true, addMinor: false);
            chart.YAxis.MajorGridlines.LineStyle = eLineStyle.Solid;
            chart.YAxis.MajorGridlines.Width = 1;
            chart.YAxis.MajorGridlines.Fill.Color = Color.LightGray;

            chart.XAxis.Format = "0E+0";
            chart.YAxis.Format = "0E+0";
            chart.XAxis.Title.Text = "Time, days";
            chart.YAxis.Title.Text = "Content per Intake, [Bq/Bq]";
            chart.YAxis.Title.Rotation = 270;
            chart.XAxis.LogBase = 10;
            chart.YAxis.LogBase = 10;
            chart.XAxis.Crosses = eCrosses.Min;
            chart.YAxis.Crosses = eCrosses.Min;

            chart.XAxis.MinValue = 1E-2;
            chart.XAxis.MaxValue = 1E+5;

            chart.YAxis.MinValue = 1E-10;
            chart.YAxis.MaxValue = 1E+00;

            return chart;
        }

        static void SetExpectSerieStyle(ExcelScatterChartSerie serie, string name)
        {
            serie.Header = name + " (OIR)";

            serie.Marker.Style = eMarkerStyle.Square;
            serie.Marker.Fill.Style = eFillStyle.SolidFill;
            serie.Marker.Fill.Color = Color.Indigo;
        }

        static void SetActualSerieStyle(ExcelScatterChartSerie serie, string name)
        {
            serie.Header = name + " (FlexID)";

            serie.Marker.Style = eMarkerStyle.Triangle;
            serie.Marker.Fill.Style = eFillStyle.SolidFill;
            serie.Marker.Fill.Color = Color.OrangeRed;
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
