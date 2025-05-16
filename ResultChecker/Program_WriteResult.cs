using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Drawing;
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
                // 預託実効線量と預託等価線量のシートを作成。
                var sheetEequivDose = package.Workbook.Worksheets.Add("Dose");
                WriteEquivDoseSheet(sheetEequivDose, sortedResults);

                // (預託実効線量と)残留放射能の要約シートを作成。
                var sheetSummary = package.Workbook.Worksheets.Add("Retention");
                WriteSummarySheet(sheetSummary, sortedResults);

                // 対象毎の残留放射能の確認シートを作成。
                foreach (var res in sortedResults)
                {
                    if (res.HasErrors)
                        continue;

                    var sheetRes = package.Workbook.Worksheets.Add(res.Target);
                    WriteResultSheet(sheetRes, res);
                }

                package.SaveAs(filePath);
            }
        }

        /// <summary>
        /// 預託実効線量の比較結果と、残留放射能の要約比較を含んだシートを書き出す。
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="results"></param>
        static void WriteSummarySheet(ExcelWorksheet sheet, IEnumerable<Result> results)
        {
            sheet.Cells[1, 1].Value = "Retention";

            const int rowH = 3;
            const int rowV = rowH + 2;
            const int colT = 1;
            const int colD = 2;
            const int colA = 6;

            // Target
            {
                sheet.Cells[rowH + 0, colT].Value = "Target";
                sheet.Cells[rowH + 0, colT].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[rowH + 0, colT, rowH + 1, colT].Merge = true;
            }

            // Effective Dose
            {
                var (r0, r1) = (rowH + 0, rowH + 1);
                var (c0, c1, c2) = (colD + 0, colD + 1, colD + 2);
                sheet.Cells[r0, c0].Value = "Whole Body Effective Dose";
                sheet.Cells[r0, c0].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[r0, c0, r0, c2].Merge = true;
                sheet.Cells[r1, c0].Value = "OIR";
                sheet.Cells[r1, c1].Value = "FlexID";
                sheet.Cells[r1, c2].Value = "Diff";
                sheet.Cells[r1, c0, r1, c2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[r1, c0, r1, c2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[r1, c0, r1, c2].Style.WrapText = true;
            }

            // Activity
            void WriteActivityHeader(int c, string header)
            {
                var (r0, r1) = (rowH + 0, rowH + 1);
                var (c0, c1) = (c + 0, c + 1);
                sheet.Cells[r0, c0].Value = header;
                sheet.Cells[r0, c0].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[r0, c0, r0, c1].Merge = true;
                sheet.Cells[r1, c0].Value = "Diff (min)";
                sheet.Cells[r1, c1].Value = "Diff (max)";
                sheet.Cells[r1, c0, r1, c1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[r1, c0, r1, c1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[r1, c0, r1, c1].Style.WrapText = true;
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
            foreach (var res in results)
            {
                // Target
                sheet.Cells[r, 1].Value = res.Target;

                if (res.HasErrors)
                {
                    sheet.Cells[r, 2, r, 4].Value = "-";
                    sheet.Cells[r, 6, r, 11].Value = "-";
                    r++;
                    continue;
                }

                // Effective Dose
                {
                    var cellEffDoseE = sheet.Cells[r, colD + 0];
                    var cellEffDoseA = sheet.Cells[r, colD + 1];
                    var cellEffDoseR = sheet.Cells[r, colD + 2];
                    cellEffDoseE.Value = res.ExpectDose.EffectiveDose;
                    cellEffDoseA.Value = res.ActualDose.EffectiveDose;
                    cellEffDoseR.Formula = $"{cellEffDoseA.Address}/{cellEffDoseE.Address}";
                    cellEffDoseE.Style.Numberformat.Format = "0.0E+00";
                    cellEffDoseA.Style.Numberformat.Format = "0.0E+00";
                    cellEffDoseR.Style.Numberformat.Format = "0.0%";
                }

                // Activity
                void WriteActivityValue(int c, (double Min, double Max) minmax)
                {
                    object GetValue(double value) =>
                        double.IsInfinity(value) ? (object)"-" : value;

                    var cell1 = sheet.Cells[r, c + 0];
                    var cell2 = sheet.Cells[r, c + 1];
                    cell1.Value = GetValue(minmax.Min);
                    cell2.Value = GetValue(minmax.Max);
                    cell1.Style.Numberformat.Format = "0.0%";
                    cell2.Style.Numberformat.Format = "0.0%";
                }
                WriteActivityValue(colA + 0, res.FractionsWholeBody);
                WriteActivityValue(colA + 2, res.FractionsUrine);
                WriteActivityValue(colA + 4, res.FractionsFaeces);
                WriteActivityValue(colA + 6, res.FractionsAtract);
                WriteActivityValue(colA + 8, res.FractionsLungs);
                WriteActivityValue(colA + 10, res.FractionsSkeleton);
                WriteActivityValue(colA + 12, res.FractionsLiver);
                WriteActivityValue(colA + 14, res.FractionsThyroid);

                r++;
            }

            // 預託実効線量のFlexID/OIR比にカラースケールを設定。
            var cellsDose = sheet.Cells[rowV, colD + 2, r - 1, colD + 2];
            SetPercentColorScale(cellsDose);

            // 残留放射能のFlexID/OIR比にカラースケールを設定。
            var cellsActivity = sheet.Cells[rowV, colA, r - 1, colA + 15];
            SetPercentColorScale(cellsActivity);

            sheet.Column(1).AutoFit();
            //sheet.Cells.AutoFitColumns(0);  // Autofit columns for all cells
        }

        /// <summary>
        /// 預託実効線量と預託等価線量の比較結果シートを書き出す。
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="results"></param>
        static void WriteEquivDoseSheet(ExcelWorksheet sheet, IEnumerable<Result> results)
        {
            sheet.Cells[1, 1].Value = "Dose";

            const int rowH = 3;
            const int rowV = rowH + 2;
            const int colT = 1;
            const int colD = 2;
            const int colE = 7;

            sheet.OutLineSummaryBelow = false;
            sheet.OutLineSummaryRight = false;

            // Target
            {
                sheet.Cells[rowH + 0, colT].Value = "Target";
                sheet.Cells[rowH + 0, colT].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[rowH + 0, colT, rowH + 1, colT].Merge = true;
            }

            // Effective Dose
            {
                var (r0, r1) = (rowH + 0, rowH + 1);
                var (c0, c1, c2) = (colD + 0, colD + 1, colD + 2);
                sheet.Cells[r0, c0].Value = "Whole Body Effective Dose";
                sheet.Cells[r0, c0].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[r0, c0, r0, c2].Merge = true;
                sheet.Cells[r1, c0].Value = "OIR";
                sheet.Cells[r1, c1].Value = "FlexID";
                sheet.Cells[r1, c2].Value = "Diff";
                sheet.Cells[r1, c0, r1, c2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[r1, c0, r1, c2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[r1, c0, r1, c2].Style.WrapText = true;
            }

            var targetRegions = new[]
            {
                "Bone marrow", "Colon", "Lung", "Stomach", "Breast", "Ovaries",
                "Testes", "Urinary bladder", "Oesophagus", "Liver", "Thyroid",
                "Bone Surface", "Brain", "Salivary glands", "Skin", "Adrenals",
                "ET of HRTM", "Gall bladder", "Heart", "Kidneys", "Lymphatic nodes",
                "Muscle", "Oral mucosa", "Pancreas", "Prostate", "Small intestine",
                "Spleen", "Thymus", "Uterus" ,
            };

            // Equivalent Dose
            {
                int targetRegionCount = targetRegions.Length;
                for (int i = 0; i < targetRegionCount; i++)
                {
                    var targetRegion = targetRegions[i];

                    var (r0, r1) = (rowH + 0, rowH + 1);
                    var c = colE + i;
                    var cell = sheet.Cells[r0, c];
                    cell.Value = targetRegion;
                    sheet.Cells[r0, c, r1, c].Merge = true;
                    sheet.Cells[r0, c, r1, c].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[r0, c, r1, c].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[r0, c, r1, c].Style.WrapText = true;
                }
            }

            var r = rowV;
            foreach (var res in results)
            {
                // Target
                sheet.Cells[r, colT].Value = res.Target;
                sheet.Cells[r, colT].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Effective Dose
                {
                    if (res.HasErrors)
                    {
                        var cells = sheet.Cells[r, colD + 0, r, colD + 2];
                        cells.Value = "-";
                        cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    else
                    {
                        var cellEffDoseE = sheet.Cells[r, colD + 0];
                        var cellEffDoseA = sheet.Cells[r, colD + 1];
                        var cellEffDoseR = sheet.Cells[r, colD + 2];

                        cellEffDoseE.Value = res.ExpectDose.EffectiveDose;
                        cellEffDoseA.Value = res.ActualDose.EffectiveDose;
                        cellEffDoseR.Formula = $"{cellEffDoseA.Address}/{cellEffDoseE.Address}";
                        cellEffDoseE.Style.Numberformat.Format = "0.0E+00";
                        cellEffDoseA.Style.Numberformat.Format = "0.0E+00";
                        cellEffDoseR.Style.Numberformat.Format = "0.0%";

                        // 預託実効線量のFlexID/OIR比にカラースケールを設定。
                        SetPercentColorScale(cellEffDoseR);
                    }

                    sheet.Cells[r, colD + 0, r + 4, colD + 0].Merge = true;
                    sheet.Cells[r, colD + 1, r + 4, colD + 1].Merge = true;
                    sheet.Cells[r, colD + 2, r + 4, colD + 2].Merge = true;

                    sheet.Cells[r, colD + 0, r + 4, colD + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                // Equivalent Dose
                {
                    sheet.Cells[r + 0, colE - 1].Value = "Diff";
                    sheet.Cells[r + 1, colE - 1].Value = "OIR Male";
                    sheet.Cells[r + 2, colE - 1].Value = "OIR Female";
                    sheet.Cells[r + 3, colE - 1].Value = "OIR Ave";
                    sheet.Cells[r + 4, colE - 1].Value = "FlexID";
                    sheet.Cells[r + 0, colE - 1, r + 4, colE - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int targetRegionCount = res.ExpectDose.EquivalentDosesMale.Length;
                    if (res.HasErrors)
                    {
                        var cells = sheet.Cells[r + 0, colE, r + 4, colE + targetRegionCount - 1];
                        cells.Value = "-";
                        cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    else
                    {
                        for (int i = 0; i < targetRegionCount; i++)
                        {
                            var cellEquivDoseR = sheet.Cells[r + 0, colE + i];
                            var cellEquivDoseM = sheet.Cells[r + 1, colE + i];
                            var cellEquivDoseF = sheet.Cells[r + 2, colE + i];
                            var cellEquivDoseE = sheet.Cells[r + 3, colE + i];
                            var cellEquivDoseA = sheet.Cells[r + 4, colE + i];

                            cellEquivDoseM.Value = res.ExpectDose.EquivalentDosesMale[i];
                            cellEquivDoseF.Value = res.ExpectDose.EquivalentDosesFemale[i];
                            cellEquivDoseE.Formula = $"AVERAGE({cellEquivDoseM.Address},{cellEquivDoseF.Address})";
                            cellEquivDoseA.Value = res.ActualDose.EquivalentDoses[i];
                            cellEquivDoseR.Formula = $"{cellEquivDoseA.Address}/{cellEquivDoseE.Address}";

                            if (double.IsNaN(res.ActualDose.EquivalentDoses[i]))
                            {
                                cellEquivDoseA.Value = "-";
                                cellEquivDoseR.Value = "-";
                                cellEquivDoseA.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                cellEquivDoseR.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            cellEquivDoseM.Style.Numberformat.Format = "0.0E+00";
                            cellEquivDoseF.Style.Numberformat.Format = "0.0E+00";
                            cellEquivDoseE.Style.Numberformat.Format = "0.0E+00";
                            cellEquivDoseA.Style.Numberformat.Format = "0.0E+00";
                            cellEquivDoseR.Style.Numberformat.Format = "0.0%";
                        }

                        // 預託等価線量のFlexID/OIR比にカラースケールを設定。
                        var cellsEquivDose = sheet.Cells[r, colE, r, colE + targetRegions.Length - 1];
                        SetPercentColorScale(cellsEquivDose);
                    }
                }

                sheet.Rows[r + 1, r + 5].Group();
                sheet.Rows[r].CollapseChildren(true);

                r += 6;
            }

            sheet.Column(1).AutoFit();
            sheet.Column(6).AutoFit();

            r = rowV;
            foreach (var res in results)
            {
                // Target
                sheet.Cells[r, colT, r + 4, colT].Merge = true;
                r += 6;
            }
        }

        private static void SetPercentColorScale(ExcelRange cells)
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
        /// 計算対象の、残留放射能の比較結果シートを書き出す。
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="result"></param>
        static void WriteResultSheet(ExcelWorksheet sheet, Result result)
        {
            var expectActs = GetExpectRetentions(result.Target, out _, out var retentionNuc);
            var actualActs = GetResultRetentions(result.Target, retentionNuc);

            sheet.Cells[1, 1].Value = result.Target;

            const int rowH = 4;
            const int rowT = rowH + 1;
            const int colE = 23;  // 1;
            const int colA = 33;  // 11;
            const int colD = 14;  // 21;
            const int colC = 1;   // 30;

            sheet.Cells[rowH - 1, colE + 0].Value = "OIR";
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

            sheet.Cells[rowH - 1, colA + 0].Value = "FlexID";
            sheet.Cells[rowH, colA + 0].Value = "Time, days";
            sheet.Cells[rowH, colA + 1].Value = "Whole Body";
            sheet.Cells[rowH, colA + 2].Value = "Urine\n(24-hour)";
            sheet.Cells[rowH, colA + 3].Value = "Faeces\n(24-hour)";
            sheet.Cells[rowH, colA + 4].Value = "Alimentary Tract";
            sheet.Cells[rowH, colA + 5].Value = "Lungs";
            sheet.Cells[rowH, colA + 6].Value = "Skeleton";
            sheet.Cells[rowH, colA + 7].Value = "Liver";
            sheet.Cells[rowH, colA + 8].Value = "Thyroid";
            sheet.Cells[rowH, colA + 0, rowH, colA + 8].Style.WrapText = true;
            sheet.Cells[rowH, colA + 0, rowH, colA + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            sheet.Cells[rowH - 1, colD + 0].Value = "Difference";
            sheet.Cells[rowH, colD + 0].Value = "Whole Body";
            sheet.Cells[rowH, colD + 1].Value = "Urine";
            sheet.Cells[rowH, colD + 2].Value = "Faeces";
            sheet.Cells[rowH, colD + 3].Value = "Alimentary Tract";
            sheet.Cells[rowH, colD + 4].Value = "Lungs";
            sheet.Cells[rowH, colD + 5].Value = "Skeleton";
            sheet.Cells[rowH, colD + 6].Value = "Liver";
            sheet.Cells[rowH, colD + 7].Value = "Thyroid";
            sheet.Cells[rowH, colD + 0, rowH, colD + 7].Style.WrapText = true;
            sheet.Cells[rowH, colD + 0, rowH, colD + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            sheet.Row(rowH).Height *= 3;
            //sheet.Cells[rowH, colE, rowH, colE + 3].AutoFitColumns(0);
            //sheet.Cells[rowH, colA, rowH, colA + 3].AutoFitColumns(0);
            //sheet.Cells[rowH, colD, rowH, colD + 3].AutoFitColumns(0);

            var nrow = 0;
            foreach (var (actualAct, expectAct) in actualActs.Zip(expectActs, (a, e) => (a, e)))
            {
                var r = rowT + nrow;

                var cellTimeE      /**/= sheet.Cells[r, colE + 0];
                var cellWholeBodyE /**/= sheet.Cells[r, colE + 1];
                var cellUrineE     /**/= sheet.Cells[r, colE + 2];
                var cellFaecesE    /**/= sheet.Cells[r, colE + 3];
                var cellAtractE    /**/= sheet.Cells[r, colE + 4];
                var cellLungsE     /**/= sheet.Cells[r, colE + 5];
                var cellSkeletonE  /**/= sheet.Cells[r, colE + 6];
                var cellLiverE     /**/= sheet.Cells[r, colE + 7];
                var cellThyroidE   /**/= sheet.Cells[r, colE + 8];
                cellTimeE      /**/.Value = expectAct.EndTime;
                cellWholeBodyE /**/.Value = expectAct.WholeBody;
                cellUrineE     /**/.Value = expectAct.Urine    /**/?? (object)"-";
                cellFaecesE    /**/.Value = expectAct.Faeces   /**/?? (object)"-";
                cellAtractE    /**/.Value = expectAct.Atract   /**/?? (object)"-";
                cellLungsE     /**/.Value = expectAct.Lungs    /**/?? (object)"-";
                cellSkeletonE  /**/.Value = expectAct.Skeleton /**/?? (object)"-";
                cellLiverE     /**/.Value = expectAct.Liver    /**/?? (object)"-";
                cellThyroidE   /**/.Value = expectAct.Thyroid  /**/?? (object)"-";

                var cellTimeA      /**/= sheet.Cells[r, colA + 0];
                var cellWholeBodyA /**/= sheet.Cells[r, colA + 1];
                var cellUrineA     /**/= sheet.Cells[r, colA + 2];
                var cellFaecesA    /**/= sheet.Cells[r, colA + 3];
                var cellAtractA    /**/= sheet.Cells[r, colA + 4];
                var cellLungsA     /**/= sheet.Cells[r, colA + 5];
                var cellSkeletonA  /**/= sheet.Cells[r, colA + 6];
                var cellLiverA     /**/= sheet.Cells[r, colA + 7];
                var cellThyroidA   /**/= sheet.Cells[r, colA + 8];
                cellTimeA      /**/.Value = actualAct.EndTime;
                cellWholeBodyA /**/.Value = actualAct.WholeBody;
                cellUrineA     /**/.Value = actualAct.Urine    /**/?? (object)"-";
                cellFaecesA    /**/.Value = actualAct.Faeces   /**/?? (object)"-";
                cellAtractA    /**/.Value = actualAct.Atract   /**/?? (object)"-";
                cellLungsA     /**/.Value = actualAct.Lungs    /**/?? (object)"-";
                cellSkeletonA  /**/.Value = actualAct.Skeleton /**/?? (object)"-";
                cellLiverA     /**/.Value = actualAct.Liver    /**/?? (object)"-";
                cellThyroidA   /**/.Value = actualAct.Thyroid  /**/?? (object)"-";

                var cellWholeBodyR /**/= sheet.Cells[r, colD + 0];
                var cellUrineR     /**/= sheet.Cells[r, colD + 1];
                var cellFaecesR    /**/= sheet.Cells[r, colD + 2];
                var cellAtractR    /**/= sheet.Cells[r, colD + 3];
                var cellLungsR     /**/= sheet.Cells[r, colD + 4];
                var cellSkeletonR  /**/= sheet.Cells[r, colD + 5];
                var cellLiverR     /**/= sheet.Cells[r, colD + 6];
                var cellThyroidR   /**/= sheet.Cells[r, colD + 7];
                cellWholeBodyR/**/.Formula = $"{cellWholeBodyA.Address}/{cellWholeBodyE.Address}";
                cellUrineR    /**/.Formula = $"IFERROR({cellUrineA    /**/.Address}/{cellUrineE    /**/.Address},\"-\")";
                cellFaecesR   /**/.Formula = $"IFERROR({cellFaecesA   /**/.Address}/{cellFaecesE   /**/.Address},\"-\")";
                cellAtractR   /**/.Formula = $"IFERROR({cellAtractA   /**/.Address}/{cellAtractE   /**/.Address},\"-\")";
                cellLungsR    /**/.Formula = $"IFERROR({cellLungsA    /**/.Address}/{cellLungsE    /**/.Address},\"-\")";
                cellSkeletonR /**/.Formula = $"IFERROR({cellSkeletonA /**/.Address}/{cellSkeletonE /**/.Address},\"-\")";
                cellLiverR    /**/.Formula = $"IFERROR({cellLiverA    /**/.Address}/{cellLiverE    /**/.Address},\"-\")";
                cellThyroidR  /**/.Formula = $"IFERROR({cellThyroidA  /**/.Address}/{cellThyroidE  /**/.Address},\"-\")";

                nrow++;
            }

            var sr = rowT;
            var er = rowT + nrow - 1;

            var cellsE = sheet.Cells[sr, colE + 1, er, colE + 8];
            var cellsA = sheet.Cells[sr, colA + 1, er, colA + 8];
            var cellsD = sheet.Cells[sr, colD, er, colD + 7];
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
            var atractE    /**/= sheet.Cells[sr, colE + 4, er, colE + 4];
            var lungsE     /**/= sheet.Cells[sr, colE + 5, er, colE + 5];
            var skeletonE  /**/= sheet.Cells[sr, colE + 6, er, colE + 6];
            var liverE     /**/= sheet.Cells[sr, colE + 7, er, colE + 7];
            var thyroidE   /**/= sheet.Cells[sr, colE + 8, er, colE + 8];

            var timesA     /**/= sheet.Cells[sr, colA + 0, er, colA + 0];
            var wholeBodyA /**/= sheet.Cells[sr, colA + 1, er, colA + 1];
            var urineA     /**/= sheet.Cells[sr, colA + 2, er, colA + 2];
            var faecesA    /**/= sheet.Cells[sr, colA + 3, er, colA + 3];
            var aractA     /**/= sheet.Cells[sr, colA + 4, er, colA + 4];
            var lungsA     /**/= sheet.Cells[sr, colA + 5, er, colA + 5];
            var skeletonA  /**/= sheet.Cells[sr, colA + 6, er, colA + 6];
            var liverA     /**/= sheet.Cells[sr, colA + 7, er, colA + 7];
            var thyroidA   /**/= sheet.Cells[sr, colA + 8, er, colA + 8];

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

            var serieWholeBodyE = chartWholeBody.Series.Add(wholeBodyE, timesE);
            var serieWholeBodyA = chartWholeBody.Series.Add(wholeBodyA, timesA);
            SetExpectSerieStyle(serieWholeBodyE, "Whole Body");
            SetActualSerieStyle(serieWholeBodyA, "Whole Body");

            var serieUrineE = chartUrine.Series.Add(urineE, timesE);
            var serieUrineA = chartUrine.Series.Add(urineA, timesA);
            SetExpectSerieStyle(serieUrineE, "Urine");
            SetActualSerieStyle(serieUrineA, "Urine");

            var serieFaecesE = chartFaeces.Series.Add(faecesE, timesE);
            var serieFaecesA = chartFaeces.Series.Add(faecesA, timesA);
            SetExpectSerieStyle(serieFaecesE, "Faeces");
            SetActualSerieStyle(serieFaecesA, "Faeces");

            var serieAtractE = chartAtract.Series.Add(atractE, timesE);
            var serieAtractA = chartAtract.Series.Add(aractA, timesA);
            SetExpectSerieStyle(serieAtractE, "Aimentary Tract");
            SetActualSerieStyle(serieAtractA, "Aimentary Tract");

            var serieLungsE = chartLungs.Series.Add(lungsE, timesE);
            var serieLungsA = chartLungs.Series.Add(lungsA, timesA);
            SetExpectSerieStyle(serieLungsE, "Lungs");
            SetActualSerieStyle(serieLungsA, "Lungs");

            var serieSkeletonE = chartSkeleton.Series.Add(skeletonE, timesE);
            var serieSkeletonA = chartSkeleton.Series.Add(skeletonA, timesA);
            SetExpectSerieStyle(serieSkeletonE, "Skeleton");
            SetActualSerieStyle(serieSkeletonA, "Skeleton");

            var serieLiverE = chartLiver.Series.Add(liverE, timesE);
            var serieLiverA = chartLiver.Series.Add(liverA, timesA);
            SetExpectSerieStyle(serieLiverE, "Liver");
            SetActualSerieStyle(serieLiverA, "Liver");

            var serieThyroidE = chartThyroid.Series.Add(thyroidE, timesE);
            var serieThyroidA = chartThyroid.Series.Add(thyroidA, timesA);
            SetExpectSerieStyle(serieThyroidE, "Thyroid");
            SetActualSerieStyle(serieThyroidA, "Thyroid");
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
    }
}
