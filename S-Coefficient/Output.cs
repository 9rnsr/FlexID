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
        /// <param name="nuclide">計算対象となった核種名</param>
        /// <param name="outTotal">計算結果</param>
        public void WriteCalcResult(string nuclide, double[] outTotal,
            double[] outP, double[] outE, double[] outB, double[] outA, double[] outN)
        {
            try
            {
                using (var Open = new XLWorkbook(TemplateExcelFilePath))
                {
                    void WriteTS(double[] outValuesTS, IXLWorksheet sheet)
                    {
                        const int offsetC = 3;
                        const int offsetR = 5;

                        int outTS = 0;
                        for (int col = 0; col < 79; col++)
                        {
                            for (int row = 0; row < 43; row++)
                            {
                                var r = row + offsetR;
                                var c = col + offsetC;
                                sheet.Cell(r, c).Value = outValuesTS[outTS++];
                            }
                        }
                    }

                    var sheetT = Open.Worksheet("total");
                    WriteTS(outTotal, sheetT);
                    {
                        // 'Other'の列をゼロで埋める。
                        var col = 82;
                        for (int row = 5; row < 48; row++)
                        {
                            sheetT.Cell(row, col).Value = 0;
                        }
                    }

                    var sheetP = Open.Worksheet("photon");
                    WriteTS(outP, sheetP);

                    var sheetE = Open.Worksheet("electron");
                    WriteTS(outE, sheetE);

                    var sheetB = Open.Worksheet("beta");
                    WriteTS(outB, sheetB);

                    var sheetA = Open.Worksheet("alpha");
                    WriteTS(outA, sheetA);

                    var sheetN = Open.Worksheet("neutron");
                    WriteTS(outN, sheetN);

                    Open.SaveAs(OutputExcelFilePath);

                    // セルの値を読んでテキストに出力
                    var resultList = new List<string>();
                    for (int row = 4; row < 48; row++)
                    {
                        string line = "";
                        for (int col = 2; col < 83; col++)
                        {
                            var text = sheetT.Cell(row, col).Value.ToString();
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
