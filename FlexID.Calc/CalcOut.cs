using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexID.Calc
{
    class CalcOut
    {
        // テンポラリファイル用
        public StreamWriter wTmp;

        // 線量係数用
        public StreamWriter dCom;

        // 線量率用
        public StreamWriter rCom;

        // 計算結果をテンポラリファイルに出力
        public void TemporaryOut(double outT, DataClass data, Activity Act, double[] OutMeshTotal, int iter)
        {
            wTmp.WriteLine(" {0:0.000000E+00}", outT);

            foreach (var organ in data.Organs)
            {
                var nucDecay = organ.NuclideDecay;

                var organId = organ.ID;
                var end = Act.Now[organ.Index].end * nucDecay;
                var total = OutMeshTotal[organ.Index] * nucDecay;
                var cumulative = Act.IntakeQuantityNow[organ.Index] * nucDecay;

                wTmp.WriteLine(" {0,3:0}  {1:0.00000000E+00}    {2:0.00000000E+00}    {3:0.00000000E+00}     {4,3:0}",
                                organId, end, total, cumulative, iter);
            }
        }

        // 預託線量のヘッダー出力
        public void CommitmentTarget(DataClass data)
        {
            var nuclide = data.Nuclides[0];

            // 線量係数
            dCom.WriteLine("{0} {1} {2}", " Effective/Equivalent_Dose ", nuclide.Nuclide, nuclide.IntakeRoute);
            dCom.Write("     Time    ");
            dCom.Write("     WholeBody   ");

            // 線量率
            rCom.WriteLine("{0} {1} {2}", " DoseRate ", nuclide.Nuclide, nuclide.IntakeRoute);
            rCom.Write("     Time    ");
            rCom.Write("     WholeBody   ");

            for (int i = 0; i < data.TargetRegions.Length; i++)
            {
                dCom.Write("  {0,-12:n}", data.TargetRegions[i]);
                rCom.Write("  {0,-12:n}", data.TargetRegions[i]);
            }
            dCom.WriteLine();
            dCom.Write("     [day]       ");
            dCom.Write("  [Sv/Bq]     ");
            rCom.WriteLine();
            rCom.Write("     [day]       ");
            rCom.Write("  [Sv/h]      ");
            for (int i = 0; i < data.TargetRegions.Length; i++)
            {
                dCom.Write("  [Sv/Bq]     ");
                rCom.Write("  [Sv/h]      ");
            }
            dCom.WriteLine();
            rCom.WriteLine();
        }

        // 預託線量の計算結果出力
        public void CommitmentOut(double now, double pre, double WholeBody, double preBody, double[] Result, double[] preResult)
        {
            dCom.Write("{0,14:0.000000E+00}  ", now);
            dCom.Write("{0,13:0.000000E+00}", WholeBody);
            rCom.Write("{0,14:0.000000E+00}  ", now);
            rCom.Write("{0,13:0.000000E+00}", (WholeBody - preBody) / ((now - pre) * 24));
            for (int i = 0; i < Result.Length; i++)
            {
                dCom.Write("  {0,12:0.000000E+00}", Result[i]);
                rCom.Write("  {0,12:0.000000E+00}", (Result[i] - preResult[i]) / ((now - pre) * 24));
            }
            dCom.WriteLine();
            rCom.WriteLine();
        }

        // テンポラリファイルを並び替えて出力
        public void ActivityOut(string RetePath, string CumuPath, string TmpFile, DataClass data)
        {
            var AllLines = File.ReadAllLines(TmpFile);

            using (var r = new StreamWriter(RetePath, false, Encoding.UTF8))
            using (var c = new StreamWriter(CumuPath, false, Encoding.UTF8))
            {
                foreach (var nuclide in data.Nuclides)
                {
                    // ヘッダー出力
                    r.WriteLine(" {0} {1} {2}", "Retention ", nuclide.Nuclide, nuclide.IntakeRoute);
                    r.Write("     Time      ");
                    c.WriteLine(" {0} {1} {2}", "CumulativeActivity ", nuclide.Nuclide, nuclide.IntakeRoute);
                    c.Write("     Time      ");

                    foreach (var Organ in data.Organs)
                    {
                        if (nuclide == Organ.Nuclide)
                        {
                            r.Write("  {0,-14:n}", Organ.Name);
                            c.Write("  {0,-14:n}", Organ.Name);
                        }
                    }
                    r.WriteLine();
                    r.Write("     [day]       ");
                    c.WriteLine();
                    c.Write("     [day]       ");

                    foreach (var Organ in data.Organs)
                    {
                        if (nuclide == Organ.Nuclide)
                        {
                            r.Write("  [Bq/Bq]       ");
                            c.Write("     [Bq]       ");
                        }
                    }

                    for (int i = 0; i < AllLines.Length;)
                    {
                        var values = AllLines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        if (values.Length == 1)
                        {
                            r.WriteLine();
                            r.Write("  {0:0.00000000E+00} ", values[0]);
                            c.WriteLine();
                            c.Write("  {0:0.00000000E+00} ", values[0]);
                            i++;
                        }
                        else if (values.Length > 4)
                        {
                            foreach (var Organ in data.Organs)
                            {
                                values = AllLines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                if (nuclide == Organ.Nuclide)
                                {
                                    r.Write("  {0:0.00000000E+00}", values[1]); // end
                                    c.Write("  {0:0.00000000E+00}", values[3]); // cumulative
                                }
                                i++;
                            }
                        }
                    }
                    r.WriteLine();
                    r.WriteLine();
                    c.WriteLine();
                    c.WriteLine();
                }
            }
        }

        public void IterOut(List<double> CalcTimeMesh, Dictionary<double, int> iterLog, string IterPath)
        {
            using (var w = new StreamWriter(IterPath, false, Encoding.UTF8))
            {
                w.WriteLine("   time(day)    Iteration");
                for (int i = 0; i < iterLog.Count; i++)
                {
                    w.WriteLine("  {0:0.00000E+00}     {1,3:0}", CalcTimeMesh[i], iterLog[CalcTimeMesh[i]]);
                }
            }
        }
    }
}
