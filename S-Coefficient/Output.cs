using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace S_Coefficient
{
    public class Output
    {
        // 出力Excelテンプレートのファイルパス
        private const string TemplateExcelFilePath = @"lib\S-Coefficient_Tmp.xlsx";

        public string OutputExcelFilePath { get; set; }

        public string OutputTextFilePath { get; set; }

        /// <summary>
        /// 計算結果をExcelファイルに書き出す
        /// </summary>
        /// <param name="nuclideName">計算対象となった核種名</param>
        /// <param name="OutTotal">計算結果</param>
        public void WriteCalcResult(string nuclideName, List<double> OutTotal,
            List<double> OutP, List<double> OutE, List<double> OutB, List<double> OutA, List<double> OutN)
        {
            try
            {
                using (var Open = new XLWorkbook(TemplateExcelFilePath))
                {
                    int outCount = 0;
                    var SheetT = Open.Worksheet("total");
                    for (int col = 3; col < 83; col++)
                    {
                        for (int row = 5; row < 48; row++)
                        {
                            if (col == 82)
                                SheetT.Cell(row, col).Value = 0;
                            else
                                SheetT.Cell(row, col).Value = OutTotal[outCount];
                            outCount++;
                        }
                    }

                    outCount = 0;
                    var SheetP = Open.Worksheet("photon");
                    for (int col = 3; col < 82; col++)
                    {
                        for (int row = 5; row < 48; row++)
                        {
                            SheetP.Cell(row, col).Value = OutP[outCount];
                            outCount++;
                        }
                    }

                    outCount = 0;
                    var SheetE = Open.Worksheet("electron");
                    for (int col = 3; col < 82; col++)
                    {
                        for (int row = 5; row < 48; row++)
                        {
                            SheetE.Cell(row, col).Value = OutE[outCount];
                            outCount++;
                        }
                    }

                    outCount = 0;
                    var SheetB = Open.Worksheet("beta");
                    for (int col = 3; col < 82; col++)
                    {
                        for (int row = 5; row < 48; row++)
                        {
                            SheetB.Cell(row, col).Value = OutB[outCount];
                            outCount++;
                        }
                    }

                    outCount = 0;
                    var SheetA = Open.Worksheet("alpha");
                    for (int col = 3; col < 82; col++)
                    {
                        for (int row = 5; row < 48; row++)
                        {
                            SheetA.Cell(row, col).Value = OutA[outCount];
                            outCount++;
                        }
                    }

                    outCount = 0;
                    var SheetN = Open.Worksheet("neutron");
                    for (int col = 3; col < 82; col++)
                    {
                        for (int row = 5; row < 48; row++)
                        {
                            SheetN.Cell(row, col).Value = OutN[outCount];
                            outCount++;
                        }
                    }

                    Open.SaveAs(OutputExcelFilePath);

                    // セルの値を読んでテキストに出力
                    var resultList = new List<string>();
                    for (int row = 4; row < 48; row++)
                    {
                        string line = "";
                        for (int col = 2; col < 83; col++)
                        {
                            var text = SheetT.Cell(row, col).Value.ToString();
                            if (row == 4)
                            {
                                if (col == 2)
                                {
                                    text = "  T/S";
                                    line += $"{text,-11}";
                                }
                                else
                                    line += $"{text,-15}";
                            }
                            else
                            {
                                if (col == 2)
                                    line += $"{text,-11}";
                                else
                                {
                                    var value = double.Parse(text).ToString("0.00000000E+00");
                                    line += $"{value,-15}";
                                }
                            }
                        }
                        resultList.Add(line);
                    }

                    File.WriteAllLines(OutputTextFilePath, resultList, System.Text.Encoding.UTF8);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
