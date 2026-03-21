using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;

namespace FlexID;

public class ReportGenerator
{
    static ReportGenerator()
    {
        // 非商用ライセンスを設定
        ExcelPackage.License.SetNonCommercialPersonal("FlexID");
    }

    public static void WriteReport(string filePath, ReportData report)
    {
        if (report.Errors.Any())
            return;

        using var package = new ExcelPackage();

        // 対象の残留放射能の確認シートを作成。
        var sheetRes = package.Workbook.Worksheets.Add(report.OutputName);
        WriteResultSheet(sheetRes, report);

        package.SaveAs(filePath);
    }

    public static void WriteSummary(string filePath, ReportData[] reports)
    {
        using var package = new ExcelPackage();

        var hyperlinkStyle = package.Workbook.Styles.CreateNamedStyle("HyperLink");
        hyperlinkStyle.Style.Font.UnderLine = true;
        hyperlinkStyle.BuildInId = 8; // the id for the build in hyper link style.

        var followedHyperlinkStyle = package.Workbook.Styles.CreateNamedStyle("FollowedHyperLink");
        followedHyperlinkStyle.Style.Font.UnderLine = true;
        followedHyperlinkStyle.BuildInId = 9; // the id for the build in followed hyper link style.

        // 預託実効線量と預託等価線量のシートを作成。
        var sheetEequivDose = package.Workbook.Worksheets.Add("Dose");
        WriteDoseSummary(sheetEequivDose, reports);

        // (預託実効線量と)残留放射能の要約シートを作成。
        var sheetSummary = package.Workbook.Worksheets.Add("Retention");
        WriteRetentionSummary(sheetSummary, reports);

        package.SaveAs(filePath);
    }

    private static void SetPercentColorScale(ExcelRangeBase cells)
    {
        var sheet = cells.Worksheet;
        var condition = sheet.ConditionalFormatting.AddThreeColorScale(cells);

        condition.LowValue.Type = eExcelConditionalFormattingValueObjectType.Num;
        condition.LowValue.Value = 0.5;
        condition.LowValue.Color = Color.DeepSkyBlue;
        condition.MiddleValue.Type = eExcelConditionalFormattingValueObjectType.Num;
        condition.MiddleValue.Value = 1.0;
        condition.MiddleValue.Color = Color.LightGreen;
        condition.HighValue.Type = eExcelConditionalFormattingValueObjectType.Num;
        condition.HighValue.Value = 1.5;
        condition.HighValue.Color = Color.Orange;
    }

    /// <summary>
    /// 預託実効線量と預託等価線量の比較結果シートを書き出す。
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="reports"></param>
    private static void WriteDoseSummary(ExcelWorksheet sheet, IEnumerable<ReportData> reports)
    {
        sheet.Cells[1, 1].Value = "Dose";

        var comp = reports.Any(report => report.HasExpect);
        const int rowH = 3;
        const int rowV = rowH + 1;
        const int colT = 1; // Target
        const int colD = 2; // Effective Dose
        int colE = comp ? 8 : 6; // Equivalent dose
        var rowOfs = comp ? 6 : 2;
        ExcelRangeBase cells;

        sheet.OutLineSummaryBelow = false;
        sheet.OutLineSummaryRight = false;

        // Target (header)
        {
            cells = sheet.Cells[rowH, colT];
            cells.Value = "Target";

            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Color.SetColor(Color.Black);
            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Right.Color.SetColor(Color.Black);
        }

        WriteEffectiveDoseHeader(sheet.Cells[rowH - 1, colD], comp);
        WriteEquivalentDoseHeader(sheet.Cells[rowH - 1, colE - 2]);

        var r = rowV;
        foreach (var report in reports)
        {
            var err = report.HasErrors;

            // Target
            {
                cells = sheet.Cells[r, colT];
                cells.Value = report.OutputName;
                cells.Hyperlink = new Uri(report.OutputName + ".xlsx", UriKind.Relative);
                cells.StyleName = "HyperLink";
                cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Top.Color.SetColor(Color.Black);
            }

            WriteEffectiveDoseData(sheet.Cells[r, colD], report);
            WriteEquivalentDoseData(sheet.Cells[r, colE], report);

            if (comp)
            {
                sheet.Rows[r + 2, r + 5].Group();
                sheet.Rows[r + 1].CollapseChildren(true);
            }

            r += rowOfs;
        }

        sheet.Column(colT).AutoFit();

        cells = sheet.Cells[rowV, colT, r - 1, colT];
        cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Right.Color.SetColor(Color.Black);

        cells = sheet.Cells[rowV, colE - 1, r - 1, colE - 1];
        cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Right.Color.SetColor(Color.Black);

        r = rowV;
        foreach (var report in reports)
        {
            // Target
            cells = sheet.Cells[r, colT, r + rowOfs - 1, colT];
            cells.Merge = true;

            r += rowOfs;
        }

        // ウインドウ枠の固定を設定。
        sheet.View.FreezePanes(rowV, colD);
    }

    /// <summary>
    /// 預託実効線量の比較結果と、残留放射能の要約比較を含んだシートを書き出す。
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="reports"></param>
    private static void WriteRetentionSummary(ExcelWorksheet sheet, IEnumerable<ReportData> reports)
    {
        sheet.Cells[1, 1].Value = "Retention";

        var comp = reports.Any(report => report.HasExpect);
        const int rowH = 2;
        const int rowV = rowH + 2;
        const int colT = 1; // Target
        const int colD = 2; // Effective Dose
        int colA = comp ? 6 : 4; // Retention Activity
        ExcelRangeBase cells;

        // Target (header)
        {
            cells = sheet.Cells[rowH + 0, colT];
            cells.Value = "Target";

            cells = cells.Offset(0, 0, 2, 1);
            cells.Merge = true;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Color.SetColor(Color.Black);
            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Right.Color.SetColor(Color.Black);
        }

        // Effective Dose (header)
        if (comp)
        {
            cells = sheet.Cells[rowH, colD];
            cells.Value = "Effective Dose";

            cells = cells.Offset(1, 0);
            cells.Offset(0, 0).Value = "Diff";
            cells.Offset(0, 1).Value = "OIR";
            cells.Offset(0, 2).Value = "FlexID";

            cells = cells.Offset(0, 0, 1, 3);
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Color.SetColor(Color.Black);
        }
        else
        {
            cells = sheet.Cells[rowH, colD];
            cells.Value = "Effective Dose";

            cells = cells.Offset(1, 0);
            cells.Value = "FlexID";

            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Color.SetColor(Color.Black);
        }

        // Retention Activity (header)
        void WriteActivityHeader(int c, string header)
        {
            var (r0, r1) = (rowH + 0, rowH + 1);
            var (c0, c1) = (c + 0, c + 1);
            sheet.Cells[r0, c0].Value = header;
            sheet.Cells[r0, c0].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[r0, c0, r0, c1].Merge = true;
            sheet.Cells[r1, c0].Value = comp ? "Diff (min)" : "min";
            sheet.Cells[r1, c1].Value = comp ? "Diff (max)" : "max";

            cells = sheet.Cells[r1, c0, r1, c1];
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Color.SetColor(Color.Black);
        }
        WriteActivityHeader(colA + 0, "Whole Body");
        WriteActivityHeader(colA + 2, "Urine");
        WriteActivityHeader(colA + 4, "Faeces");
        WriteActivityHeader(colA + 6, "Alimentary Tract");
        WriteActivityHeader(colA + 8, "Lungs");
        WriteActivityHeader(colA + 10, "Skeleton");
        WriteActivityHeader(colA + 12, "Liver");
        WriteActivityHeader(colA + 14, "Thyroid");

        var r = rowV;
        foreach (var report in reports)
        {
            var err = report.HasErrors;

            // Target
            sheet.Cells[r, 1].Value = report.OutputName;
            sheet.Cells[r, 1].Hyperlink = new Uri(report.OutputName + ".xlsx", UriKind.Relative);
            sheet.Cells[r, 1].StyleName = "HyperLink";

            // Effective Dose
            if (comp)
            {
                var cellEffDoseD = sheet.Cells[r, colD + 0]; // Diff
                var cellEffDoseE = sheet.Cells[r, colD + 1]; // Expect
                var cellEffDoseO = sheet.Cells[r, colD + 2]; // Output
                cellEffDoseD.Formula = $"IFERROR({cellEffDoseO.Address}/{cellEffDoseE.Address},\"-\")";
                cellEffDoseE.Value = err ? "-" : report.ExpectDose?.EffectiveDose ?? (object)"-";
                cellEffDoseO.Value = err ? "-" : report.OutputDose!.EffectiveDose;
                cellEffDoseD.Style.Numberformat.Format = "??0.0%";
                cellEffDoseE.Style.Numberformat.Format = "0.0E+00";
                cellEffDoseO.Style.Numberformat.Format = "0.0E+00";
            }
            else
            {
                var cellEffDoseO = sheet.Cells[r, colD];
                cellEffDoseO.Value = err ? "-" : report.OutputDose!.EffectiveDose;
                cellEffDoseO.Style.Numberformat.Format = "0.0E+00";
            }

            // Retention Activity
            void WriteActivityValue(int c, (double Min, double Max) minmax)
            {
                object GetValue(double value) =>
                    err || double.IsInfinity(value) ? "-" : value;

                var cell1 = sheet.Cells[r, c + 0];
                var cell2 = sheet.Cells[r, c + 1];
                cell1.Value = GetValue(minmax.Min);
                cell2.Value = GetValue(minmax.Max);
                cell1.Style.Numberformat.Format = comp ? "??0.0%" : "0.0E+00";
                cell2.Style.Numberformat.Format = comp ? "??0.0%" : "0.0E+00";
            }
            WriteActivityValue(colA + /**/  0, comp ? report.FractionsWholeBody /**/ : report.OutputMinMaxWholeBody);
            WriteActivityValue(colA + /**/  2, comp ? report.FractionsUrine     /**/ : report.OutputMinMaxUrine);
            WriteActivityValue(colA + /**/  4, comp ? report.FractionsFaeces    /**/ : report.OutputMinMaxFaeces);
            WriteActivityValue(colA + /**/  6, comp ? report.FractionsAtract    /**/ : report.OutputMinMaxAtract);
            WriteActivityValue(colA + /**/  8, comp ? report.FractionsLungs     /**/ : report.OutputMinMaxLungs);
            WriteActivityValue(colA + /**/ 10, comp ? report.FractionsSkeleton  /**/ : report.OutputMinMaxSkeleton);
            WriteActivityValue(colA + /**/ 12, comp ? report.FractionsLiver     /**/ : report.OutputMinMaxLiver);
            WriteActivityValue(colA + /**/ 14, comp ? report.FractionsThyroid   /**/ : report.OutputMinMaxThyroid);

            r++;
        }

        sheet.Column(colT).AutoFit();

        cells = sheet.Cells[rowV, colT, r - 1, colT];
        cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Right.Color.SetColor(Color.Black);

        if (comp)
        {
            sheet.Cells[rowV, colD, r - 1, colD + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[rowV, colA, r - 1, colA + 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // 預託実効線量のFlexID/OIR比にカラースケールを設定。
            var cellsDose = sheet.Cells[rowV, colD, r - 1, colD];
            SetPercentColorScale(cellsDose);

            // 残留放射能のFlexID/OIR比にカラースケールを設定。
            var cellsActivity = sheet.Cells[rowV, colA, r - 1, colA + 15];
            SetPercentColorScale(cellsActivity);
        }
        else
        {
            sheet.Cells[rowV, colD, r - 1, colD].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[rowV, colA, r - 1, colA + 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // ウインドウ枠の固定を設定。
        sheet.View.FreezePanes(rowV, colD);
    }

    /// <summary>
    /// 計算対象の、残留放射能の比較結果シートを書き出す。
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="report"></param>
    private static void WriteResultSheet(ExcelWorksheet sheet, ReportData report)
    {
        var comp = report.HasExpect;
        var err = report.HasErrors;

        sheet.Cells[1, 1].Value = report.OutputName;

        int rowH = comp ? 12 : 8;
        int rowT = rowH + 1;
        int colO = comp ? 34 : 14; // Output
        int colE = comp ? 24 : -1; // Expect
        int colD = comp ? 14 : -1; // Diff
        const int colC = 1;

        // Effective Dose
        {
            int rowDoseH = 2;
            int rowDoseV = rowDoseH + 2;
            int colDoseV = comp ? 9 : 11;

            WriteEffectiveDoseHeader(sheet.Cells[rowDoseH, colDoseV], comp);
            WriteEffectiveDoseData(sheet.Cells[rowDoseV, colDoseV], report);
        }

        // Equivalent Dose
        {
            int rowDoseH = 2;
            int rowDoseV = rowDoseH + 2;
            int colEquiv = 15;

            var cellHdr = sheet.Cells[rowDoseH, colEquiv - 2];
            var cellVal = sheet.Cells[rowDoseV, colEquiv];
            WriteEquivalentDoseHeader(cellHdr);
            WriteEquivalentDoseData(cellVal, report);
        }

        if (true) sheet.Cells[rowH - 1, colO + 0].Value = "FlexID";
        if (comp) sheet.Cells[rowH - 1, colE + 0].Value = "OIR";
        if (comp) sheet.Cells[rowH - 1, colD + 0].Value = "Diff";

        sheet.Cells[rowH, colO + 0].Value = "Time, days";
        sheet.Cells[rowH, colO + 1].Value = "Whole Body";
        sheet.Cells[rowH, colO + 2].Value = "Urine\n(24-hour)";
        sheet.Cells[rowH, colO + 3].Value = "Faeces\n(24-hour)";
        sheet.Cells[rowH, colO + 4].Value = "Alimentary Tract";
        sheet.Cells[rowH, colO + 5].Value = "Lungs";
        sheet.Cells[rowH, colO + 6].Value = "Skeleton";
        sheet.Cells[rowH, colO + 7].Value = "Liver";
        sheet.Cells[rowH, colO + 8].Value = "Thyroid";
        sheet.Cells[rowH, colO + 0, rowH, colO + 8].Style.WrapText = true;
        sheet.Cells[rowH, colO + 0, rowH, colO + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        sheet.Cells[rowH, colO + 0, rowH, colO + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        if (comp)
        {
            sheet.Cells[rowH, colE + 0].Value = "Time, days";
            sheet.Cells[rowH, colE + 1].Value = "Whole Body";
            sheet.Cells[rowH, colE + 2].Value = "Urine\n(24-hour sample)";
            sheet.Cells[rowH, colE + 3].Value = "Faeces\n(24-hour sample)";
            sheet.Cells[rowH, colE + 4].Value = "Alimentary Tract";
            sheet.Cells[rowH, colE + 5].Value = "Lungs";
            sheet.Cells[rowH, colE + 6].Value = "Skeleton";
            sheet.Cells[rowH, colE + 7].Value = "Liver";
            sheet.Cells[rowH, colE + 8].Value = "Thyroid";
            sheet.Cells[rowH, colE + 0, rowH, colE + 8].Style.WrapText = true;
            sheet.Cells[rowH, colE + 0, rowH, colE + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[rowH, colE + 0, rowH, colE + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[rowH, colD + 0].Value = "Time, days";
            sheet.Cells[rowH, colD + 1].Value = "Whole Body";
            sheet.Cells[rowH, colD + 2].Value = "Urine";
            sheet.Cells[rowH, colD + 3].Value = "Faeces";
            sheet.Cells[rowH, colD + 4].Value = "Alimentary Tract";
            sheet.Cells[rowH, colD + 5].Value = "Lungs";
            sheet.Cells[rowH, colD + 6].Value = "Skeleton";
            sheet.Cells[rowH, colD + 7].Value = "Liver";
            sheet.Cells[rowH, colD + 8].Value = "Thyroid";
            sheet.Cells[rowH, colD + 0, rowH, colD + 8].Style.WrapText = true;
            sheet.Cells[rowH, colD + 0, rowH, colD + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[rowH, colD + 0, rowH, colD + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            string GetCompartmentNuclide(string? nuclide) =>
                nuclide is null ? "-" : nuclide == report.OutputNuclide ? "" : $"({nuclide})";
            sheet.Cells[rowH - 1, colD + 1].Value = GetCompartmentNuclide(report.NuclideWholeBody);
            sheet.Cells[rowH - 1, colD + 2].Value = GetCompartmentNuclide(report.NuclideUrine);
            sheet.Cells[rowH - 1, colD + 3].Value = GetCompartmentNuclide(report.NuclideFaeces);
            sheet.Cells[rowH - 1, colD + 4].Value = GetCompartmentNuclide(report.NuclideAtract);
            sheet.Cells[rowH - 1, colD + 5].Value = GetCompartmentNuclide(report.NuclideLungs);
            sheet.Cells[rowH - 1, colD + 6].Value = GetCompartmentNuclide(report.NuclideSkeleton);
            sheet.Cells[rowH - 1, colD + 7].Value = GetCompartmentNuclide(report.NuclideLiver);
            sheet.Cells[rowH - 1, colD + 8].Value = GetCompartmentNuclide(report.NuclideThyroid);
            sheet.Cells[rowH - 1, colD + 1, rowH - 1, colD + 8].Style.ShrinkToFit = true;
            sheet.Cells[rowH - 1, colD + 1, rowH - 1, colD + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        sheet.Row(rowH).Height *= 3;

        var nrow = 0;
        for (; ; nrow++)
        {
            var outputActs = report.OutputActs ?? [];
            var expectActs = report.ExpectActs ?? [];

            var inrangeO = nrow < outputActs.Count;
            var inrangeE = nrow < expectActs.Count;
            if (!inrangeO && !inrangeE)
                break;

            var r = rowT + nrow;

            void SetValues(int col, in ReportData.Retention act)
            {
                sheet.Cells[r, col + 0].Value = act.EndTime;
                sheet.Cells[r, col + 1].Value = act.WholeBody/**/?? (object)"-";
                sheet.Cells[r, col + 2].Value = act.Urine    /**/?? (object)"-";
                sheet.Cells[r, col + 3].Value = act.Faeces   /**/?? (object)"-";
                sheet.Cells[r, col + 4].Value = act.Atract   /**/?? (object)"-";
                sheet.Cells[r, col + 5].Value = act.Lungs    /**/?? (object)"-";
                sheet.Cells[r, col + 6].Value = act.Skeleton /**/?? (object)"-";
                sheet.Cells[r, col + 7].Value = act.Liver    /**/?? (object)"-";
                sheet.Cells[r, col + 8].Value = act.Thyroid  /**/?? (object)"-";
            }
            if (true && inrangeO) SetValues(colO, outputActs[nrow]);
            if (comp && inrangeE) SetValues(colE, expectActs[nrow]);

            if (inrangeO && inrangeE)
            {
                sheet.Cells[r, colD + 0].Value = outputActs[nrow].EndTime;
                sheet.Cells[r, colD + 1].Formula = $"IFERROR({sheet.Cells[r, colO + 1].Address}/{sheet.Cells[r, colE + 1].Address},\"-\")";
                sheet.Cells[r, colD + 2].Formula = $"IFERROR({sheet.Cells[r, colO + 2].Address}/{sheet.Cells[r, colE + 2].Address},\"-\")";
                sheet.Cells[r, colD + 3].Formula = $"IFERROR({sheet.Cells[r, colO + 3].Address}/{sheet.Cells[r, colE + 3].Address},\"-\")";
                sheet.Cells[r, colD + 4].Formula = $"IFERROR({sheet.Cells[r, colO + 4].Address}/{sheet.Cells[r, colE + 4].Address},\"-\")";
                sheet.Cells[r, colD + 5].Formula = $"IFERROR({sheet.Cells[r, colO + 5].Address}/{sheet.Cells[r, colE + 5].Address},\"-\")";
                sheet.Cells[r, colD + 6].Formula = $"IFERROR({sheet.Cells[r, colO + 6].Address}/{sheet.Cells[r, colE + 6].Address},\"-\")";
                sheet.Cells[r, colD + 7].Formula = $"IFERROR({sheet.Cells[r, colO + 7].Address}/{sheet.Cells[r, colE + 7].Address},\"-\")";
                sheet.Cells[r, colD + 8].Formula = $"IFERROR({sheet.Cells[r, colO + 8].Address}/{sheet.Cells[r, colE + 8].Address},\"-\")";
            }
        }

        var rowS = rowT;            // row start
        var rowE = rowT + nrow - 1; // row end

        var cellsO = sheet.Cells[rowS, colO + 1, rowE, colO + 8];
        cellsO.Style.Numberformat.Format = "0.0E+00";
        cellsO.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        if (comp)
        {
            var cellsE = sheet.Cells[rowS, colE + 1, rowE, colE + 8];
            cellsE.Style.Numberformat.Format = "0.0E+00";
            cellsE.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var cellsD = sheet.Cells[rowS, colD + 1, rowE, colD + 8];
            cellsD.Style.Numberformat.Format = "??0.0%";
            cellsD.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // 時間メッシュ毎の残留放射能のFlexID/OIR比にカラースケールを設定。
            SetPercentColorScale(cellsD);
        }

        var chartWholeBody /**/= sheet.Drawings.AddScatterChart("ChartWholeBody", /**/eScatterChartType.XYScatter);
        var chartUrine     /**/= sheet.Drawings.AddScatterChart("ChartUrine",     /**/eScatterChartType.XYScatter);
        var chartFaeces    /**/= sheet.Drawings.AddScatterChart("ChartFaeces",    /**/eScatterChartType.XYScatter);
        var chartAtract    /**/= sheet.Drawings.AddScatterChart("ChartAtract",    /**/eScatterChartType.XYScatter);
        var chartLungs     /**/= sheet.Drawings.AddScatterChart("ChartLungs",     /**/eScatterChartType.XYScatter);
        var chartSkeleton  /**/= sheet.Drawings.AddScatterChart("ChartSkeleton",  /**/eScatterChartType.XYScatter);
        var chartLiver     /**/= sheet.Drawings.AddScatterChart("ChartLiver",     /**/eScatterChartType.XYScatter);
        var chartThyroid   /**/= sheet.Drawings.AddScatterChart("ChartThyroid",   /**/eScatterChartType.XYScatter);
        chartWholeBody/**/.Title.Text = "Whole Body";
        chartUrine    /**/.Title.Text = "Urine";
        chartFaeces   /**/.Title.Text = "Faeces";
        chartAtract   /**/.Title.Text = "Alimentary Tract";
        chartLungs    /**/.Title.Text = "Lungs";
        chartSkeleton /**/.Title.Text = "Skeleton";
        chartLiver    /**/.Title.Text = "Liver";
        chartThyroid  /**/.Title.Text = "Thyroid";
        SetActivityChartStyle(chartWholeBody, /**/rowT/*                     */, colC, 22, 12);
        SetActivityChartStyle(chartUrine,     /**/chartWholeBody/**/.To.Row + 2, colC, 22, 12);
        SetActivityChartStyle(chartFaeces,    /**/chartUrine    /**/.To.Row + 2, colC, 22, 12);
        SetActivityChartStyle(chartAtract,    /**/chartFaeces   /**/.To.Row + 2, colC, 22, 12);
        SetActivityChartStyle(chartLungs,     /**/chartAtract   /**/.To.Row + 2, colC, 22, 12);
        SetActivityChartStyle(chartSkeleton,  /**/chartLungs    /**/.To.Row + 2, colC, 22, 12);
        SetActivityChartStyle(chartLiver,     /**/chartSkeleton /**/.To.Row + 2, colC, 22, 12);
        SetActivityChartStyle(chartThyroid,   /**/chartLiver    /**/.To.Row + 2, colC, 22, 12);

        void AddSeries(int col, Action<ExcelScatterChartSerie, string> setSerieStyle)
        {
            var times     /**/= sheet.Cells[rowS, col + 0, rowE, col + 0];
            var wholeBody /**/= sheet.Cells[rowS, col + 1, rowE, col + 1];
            var urine     /**/= sheet.Cells[rowS, col + 2, rowE, col + 2];
            var faeces    /**/= sheet.Cells[rowS, col + 3, rowE, col + 3];
            var atract    /**/= sheet.Cells[rowS, col + 4, rowE, col + 4];
            var lungs     /**/= sheet.Cells[rowS, col + 5, rowE, col + 5];
            var skeleton  /**/= sheet.Cells[rowS, col + 6, rowE, col + 6];
            var liver     /**/= sheet.Cells[rowS, col + 7, rowE, col + 7];
            var thyroid   /**/= sheet.Cells[rowS, col + 8, rowE, col + 8];
            var serieWholeBody /**/= chartWholeBody/**/.Series.Add(wholeBody, /**/times);
            var serieUrine     /**/= chartUrine    /**/.Series.Add(urine,     /**/times);
            var serieFaeces    /**/= chartFaeces   /**/.Series.Add(faeces,    /**/times);
            var serieAtract    /**/= chartAtract   /**/.Series.Add(atract,    /**/times);
            var serieLungs     /**/= chartLungs    /**/.Series.Add(lungs,     /**/times);
            var serieSkeleton  /**/= chartSkeleton /**/.Series.Add(skeleton,  /**/times);
            var serieLiver     /**/= chartLiver    /**/.Series.Add(liver,     /**/times);
            var serieThyroid   /**/= chartThyroid  /**/.Series.Add(thyroid,   /**/times);
            setSerieStyle(serieWholeBody, /**/"Whole Body");
            setSerieStyle(serieUrine,     /**/"Urine");
            setSerieStyle(serieFaeces,    /**/"Faeces");
            setSerieStyle(serieAtract,    /**/"Aimentary Tract");
            setSerieStyle(serieLungs,     /**/"Lungs");
            setSerieStyle(serieSkeleton,  /**/"Skeleton");
            setSerieStyle(serieLiver,     /**/"Liver");
            setSerieStyle(serieThyroid,   /**/"Thyroid");
        }
        // 従来同等のグラフ描画のため、期待値の系列を先に追加する。
        if (comp) AddSeries(colE, SetExpectSerieStyle);
        if (true) AddSeries(colO, SetOutputSerieStyle);

        // ウインドウ枠の固定を設定。
        sheet.View.FreezePanes(rowT, (comp ? colD : colO) + 1);
    }

    private static void WriteEffectiveDoseHeader(ExcelRangeBase cellHeader, bool comp)
    {
        ExcelRangeBase cells;
        if (comp)
        {
            cellHeader.Value = "Effective Dose";

            cells = cellHeader.Offset(1, 0);
            cells.Offset(0, 0).Value = "Diff";
            cells.Offset(0, 1).Value = "OIR";
            cells.Offset(0, 2).Value = "FlexID";

            cells = cells.Offset(0, 0, 1, 3);
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Color.SetColor(Color.Black);
        }
        else
        {
            cellHeader.Value = "Effective Dose";

            cells = cellHeader.Offset(1, 0);
            cells.Offset(0, 0).Value = "FlexID";

            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Color.SetColor(Color.Black);
        }
    }

    private static void WriteEffectiveDoseData(ExcelRangeBase cellValue, ReportData report)
    {
        var comp = report.HasExpect;
        var err = report.HasErrors;
        ExcelRangeBase cells;

        if (comp)
        {
            var cellEffDoseD = cellValue.Offset(0, 0);
            var cellEffDoseE = cellValue.Offset(0, 1);
            var cellEffDoseO = cellValue.Offset(0, 2);

            cellEffDoseD.Formula = $"IFERROR({cellEffDoseO.Address}/{cellEffDoseE.Address},\"-\")";
            cellEffDoseE.Value = err ? "-" : report.ExpectDose?.EffectiveDose ?? (object)"-";
            cellEffDoseO.Value = err ? "-" : report.OutputDose!.EffectiveDose;

            cellEffDoseD.Style.Numberformat.Format = "??0.0%";
            cellEffDoseE.Style.Numberformat.Format = "0.0E+00";
            cellEffDoseO.Style.Numberformat.Format = "0.0E+00";

            // 預託実効線量のFlexID/OIR比にカラースケールを設定。
            SetPercentColorScale(cellEffDoseD);

            cellEffDoseD.Offset(0, 0, 6, 1).Merge = true;
            cellEffDoseE.Offset(0, 0, 6, 1).Merge = true;
            cellEffDoseO.Offset(0, 0, 6, 1).Merge = true;

            cells = cellValue.Offset(0, 0, 6, 3);
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            cells = cellValue.Offset(0, 0, 1, 3);
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Top.Color.SetColor(Color.Black);
        }
        else
        {
            var cellEffDoseO = cellValue;

            cellEffDoseO.Value = err ? "-" : report.OutputDose!.EffectiveDose;
            cellEffDoseO.Style.Numberformat.Format = "0.0E+00";

            cellEffDoseO.Offset(0, 0, 2, 1).Merge = true;

            cells = cellValue.Offset(0, 0, 2, 1);
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            cells = cellValue.Offset(0, 0, 1, 1);
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Top.Color.SetColor(Color.Black);
        }
    }

    static readonly string[] TargetRegions =
    [
        "Bone marrow", "Colon", "Lung", "Stomach", "Breast", "Ovaries",
        "Testes", "Urinary bladder", "Oesophagus", "Liver", "Thyroid",
        "Bone Surface", "Brain", "Salivary glands", "Skin", "Adrenals",
        "ET of HRTM", "Gall bladder", "Heart", "Kidneys", "Lymphatic nodes",
        "Muscle", "Oral mucosa", "Pancreas", "Prostate", "Small intestine",
        "Spleen", "Thymus", "Uterus" ,
    ];

    private static void WriteEquivalentDoseHeader(ExcelRangeBase cellHeader)
    {
        ExcelRangeBase cells;

        cellHeader.Value = "Equivalent Dose";

        cells = cellHeader.Offset(1, 0);
        cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Bottom.Color.SetColor(Color.Black);

        cells = cells.Offset(0, 1);
        cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Bottom.Color.SetColor(Color.Black);
        cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        cells.Style.Border.Right.Color.SetColor(Color.Black);

        foreach (var targetRegion in TargetRegions)
        {
            cells = cells.Offset(0, 1);

            cells.Value = targetRegion;
            cells.Style.WrapText = true;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Color.SetColor(Color.Black);
        }
    }

    private static void WriteEquivalentDoseData(ExcelRangeBase cellValue, ReportData report)
    {
        var comp = report.HasExpect;
        var err = report.HasErrors;

        var cellRowHeader = cellValue.Offset(0, -2);
        ExcelRangeBase cells;

        if (comp)
        {
            cells = cellRowHeader;
            cells.Offset(0, 0).Value = "Diff";
            cells.Offset(2, 0).Value = "OIR";
            cells.Offset(4, 0).Value = "FlexID";
            cells.Offset(0, 1).Value = cells.Offset(2, 1).Value = cells.Offset(4, 1).Value = "Male";
            cells.Offset(1, 1).Value = cells.Offset(3, 1).Value = cells.Offset(5, 1).Value = "Female";
            cells.Offset(0, 0, 2, 1).Merge = true;
            cells.Offset(2, 0, 2, 1).Merge = true;
            cells.Offset(4, 0, 2, 1).Merge = true;
            cells = cells.Offset(0, 0, 6, 2);
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells = cells.Offset(0, 0, 1, 2);
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Top.Color.SetColor(Color.Black);

            for (int i = 0; i < TargetRegions.Length; i++)
            {
                var cellEquivDoseDM = cellValue.Offset(0, i); // Diff Male
                var cellEquivDoseDF = cellValue.Offset(1, i); // Diff Female
                var cellEquivDoseEM = cellValue.Offset(2, i); // Expect Male
                var cellEquivDoseEF = cellValue.Offset(3, i); // Expect Female
                var cellEquivDoseOM = cellValue.Offset(4, i); // Output Male
                var cellEquivDoseOF = cellValue.Offset(5, i); // Output Female

                cellEquivDoseDM.Formula = $"IFERROR({cellEquivDoseOM.Address}/{cellEquivDoseEM.Address},\"-\")";
                cellEquivDoseDF.Formula = $"IFERROR({cellEquivDoseOF.Address}/{cellEquivDoseEF.Address},\"-\")";
                cellEquivDoseEM.Value = err ? "-" : report.ExpectDose?.EquivalentDosesMale[i] ?? (object)"-";
                cellEquivDoseEF.Value = err ? "-" : report.ExpectDose?.EquivalentDosesFemale[i] ?? (object)"-";
                cellEquivDoseOM.Value = err ? "-" : report.OutputDose!.EquivalentDosesMale[i];
                cellEquivDoseOF.Value = err ? "-" : report.OutputDose!.EquivalentDosesFemale[i];

                cellEquivDoseDM.Style.Numberformat.Format = "??0.0%";
                cellEquivDoseDF.Style.Numberformat.Format = "??0.0%";
                cellEquivDoseEM.Style.Numberformat.Format = "0.0E+00";
                cellEquivDoseEF.Style.Numberformat.Format = "0.0E+00";
                cellEquivDoseOM.Style.Numberformat.Format = "0.0E+00";
                cellEquivDoseOF.Style.Numberformat.Format = "0.0E+00";
            }

            // 預託等価線量のFlexID/OIR比にカラースケールを設定。
            var cellsEquivDose = cellValue.Offset(0, 0, 2, TargetRegions.Length);
            SetPercentColorScale(cellsEquivDose);

            cells = cellValue.Offset(0, 0, 6, TargetRegions.Length);
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            cells = cellValue.Offset(0, 0, 1, TargetRegions.Length);
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Top.Color.SetColor(Color.Black);
        }
        else
        {
            cells = cellRowHeader;
            cells.Offset(0, 0).Value = "FlexID";
            cells.Offset(0, 1).Value = "Male";
            cells.Offset(1, 1).Value = "Female";
            cells.Offset(0, 0, 2, 1).Merge = true;
            cells = cells.Offset(0, 0, 2, 2);
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells = cells.Offset(0, 0, 1, 2);
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Top.Color.SetColor(Color.Black);

            for (int i = 0; i < TargetRegions.Length; i++)
            {
                var cellEquivDoseOM = cellValue.Offset(0, i); // Output Male
                var cellEquivDoseOF = cellValue.Offset(1, i); // Output Female

                cellEquivDoseOM.Value = err ? "-" : report.OutputDose!.EquivalentDosesMale[i];
                cellEquivDoseOF.Value = err ? "-" : report.OutputDose!.EquivalentDosesFemale[i];

                cellEquivDoseOM.Style.Numberformat.Format = "0.0E+00";
                cellEquivDoseOF.Style.Numberformat.Format = "0.0E+00";
            }

            cells = cellValue.Offset(0, 0, 2, TargetRegions.Length);
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            cells = cellValue.Offset(0, 0, 1, TargetRegions.Length);
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Top.Color.SetColor(Color.Black);
        }
    }

    private static ExcelScatterChart SetActivityChartStyle(ExcelScatterChart chart, int row, int col, int nrow, int ncol)
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

    private static void SetExpectSerieStyle(ExcelScatterChartSerie serie, string name)
    {
        serie.Header = name + " (OIR)";

        serie.Marker.Style = eMarkerStyle.Square;
        serie.Marker.Fill.Style = eFillStyle.SolidFill;
        serie.Marker.Fill.Color = Color.Indigo;
    }

    private static void SetOutputSerieStyle(ExcelScatterChartSerie serie, string name)
    {
        serie.Header = name + " (FlexID)";

        serie.Marker.Style = eMarkerStyle.Triangle;
        serie.Marker.Fill.Style = eFillStyle.SolidFill;
        serie.Marker.Fill.Color = Color.OrangeRed;
    }
}
