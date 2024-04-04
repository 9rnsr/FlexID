using System;
using System.Collections.Generic;
using System.Linq;

namespace FlexID.Calc
{
    /// <summary>
    /// 流入経路を表現する
    /// </summary>
    public class Inflow
    {
        /// <summary>
        /// 流入元の臓器番号
        /// </summary>
        public int ID;

        /// <summary>
        /// 流入元の臓器
        /// </summary>
        public Organ Organ;

        /// <summary>
        /// 流入割合
        /// </summary>
        public double Rate;
    }

    /// <summary>
    /// 臓器機能。
    /// </summary>
    public enum OrganFunc
    {
        /// <summary>
        /// 入力。
        /// </summary>
        inp,

        /// <summary>
        /// 蓄積。
        /// </summary>
        acc,

        /// <summary>
        /// 混合。
        /// </summary>
        mix,

        /// <summary>
        /// 排泄。
        /// </summary>
        exc,
    }

    /// <summary>
    /// 臓器(コンパートメント)を表現する。
    /// </summary>
    public class Organ
    {
        /// <summary>
        /// 臓器が対象とする核種
        /// </summary>
        public string Nuclide;

        /// <summary>
        /// 崩壊定数
        /// </summary>
        public double NuclideDecay;

        /// <summary>
        /// 臓器番号
        /// </summary>
        public int ID;

        /// <summary>
        /// 臓器毎のデータを配列から引くためのインデックス
        /// </summary>
        public int Index;

        /// <summary>
        /// 臓器名
        /// </summary>
        public string Name;

        /// <summary>
        /// 臓器機能
        /// </summary>
        public OrganFunc Func;

        /// <summary>
        /// 生物学的崩壊定数
        /// </summary>
        public double BioDecay;

        /// <summary>
        /// 生物学的崩壊定数(計算用)
        /// </summary>
        public double BioDecayCalc;

        /// <summary>
        /// 流入経路
        /// </summary>
        public List<Inflow> Inflows;
    }

    public class DataClass
    {
        public List<string> TargetNuc = new List<string>();

        /// <summary>
        /// 被ばく経路
        /// </summary>
        public Dictionary<string, string> IntakeRoute = new Dictionary<string, string>();

        /// <summary>
        /// 崩壊定数
        /// </summary>
        public Dictionary<string, double> Ramd = new Dictionary<string, double>();

        /// <summary>
        /// 親核種からの崩壊割合(100%＝1.00と置いた比で持つ)
        /// </summary>
        public Dictionary<string, double> DecayRate = new Dictionary<string, double>();

        /// <summary>
        /// 全ての臓器
        /// </summary>
        public List<Organ> Organs = new List<Organ>();

        /// <summary>
        /// S-coeと対応する臓器名
        /// </summary>
        public Dictionary<Tuple<string, string>, string> CorrNum = new Dictionary<Tuple<string, string>, string>();

        /// <summary>
        /// 子孫核種のリスト
        /// </summary>
        public List<string> ListProgeny = new List<string>();

        private DataClass() { }

        public static DataClass Read(List<string> inpRead, bool calcProgeny)
        {
            var data = new DataClass();

            string nuc;
            int num = 0;
            string[] values;

            values = inpRead[num].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            nuc = values[0];
            data.TargetNuc.Add(nuc);
            data.IntakeRoute[nuc] = (values[1]);
            data.Ramd[nuc] = double.Parse(values[2]);
            data.DecayRate[nuc] = double.Parse(values[3]);
            num++;

            while (inpRead[num] != "end")
            {
                values = inpRead[num].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                var organId = int.Parse(values[0]);     // 臓器番号
                var inflowNum = int.Parse(values[4]);   // 流入臓器数

                var func = values[2];  // 臓器機能名称
                var organFunc =
                    func == "inp" ? OrganFunc.inp :
                    func == "acc" ? OrganFunc.acc :
                    func == "mix" ? OrganFunc.mix :
                    func == "exc" ? OrganFunc.exc :
                    throw Program.Error($"Unrecognized organ function '{func}'.");

                var organ = new Organ
                {
                    Nuclide = nuc,
                    NuclideDecay = data.Ramd[nuc],
                    ID = organId,
                    Index = data.Organs.Count,
                    Name = values[1],
                    Func = organFunc,
                    BioDecay = double.Parse(values[3]),
                    BioDecayCalc = double.Parse(values[3]),
                    Inflows = new List<Inflow>(inflowNum),
                };

                data.CorrNum[Tuple.Create(nuc, organ.Name)] = values[7];

                if (inflowNum == 0)
                {
                    num++;
                }
                else if (organ.Func == OrganFunc.inp)   // 入力
                {
                    if (inflowNum != 1)
                        throw Program.Error("The number of inflow paths in the Input compartment should be 0.");
                    num++;
                }
                else
                {
                    for (int i = 0; i < inflowNum; i++)
                    {
                        values = inpRead[num].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        int inflowID;
                        double inflowRate;
                        if (values.Length > 3)
                        {
                            inflowID = int.Parse(values[5]);
                            inflowRate = double.Parse(values[6]);
                        }
                        else
                        {
                            inflowID = int.Parse(values[0]);
                            inflowRate = double.Parse(values[1]);
                        }
                        organ.Inflows.Add(new Inflow
                        {
                            ID = inflowID,
                            Rate = inflowRate * 0.01,
                        });
                        num++;
                    }
                }

                data.Organs.Add(organ);

                if (inpRead[num].Trim() == "end")
                    break;

                if (inpRead[num].Trim() == "cont")
                {
                    if (calcProgeny == false)
                        break;
                    num++;

                    values = inpRead[num].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    nuc = values[0];
                    data.TargetNuc.Add(nuc);
                    data.IntakeRoute[nuc] = values[1];
                    data.Ramd[nuc] = double.Parse(values[2]);
                    data.DecayRate[nuc] = double.Parse(values[3]);
                    data.ListProgeny.Add(nuc);
                    num++;
                }
            }

            // 流入経路から流入元臓器の情報を直接引くための参照を設定する
            foreach (var organ in data.Organs)
            {
                foreach (var inflow in organ.Inflows)
                {
                    if (inflow.ID == 0)
                        continue;
                    inflow.Organ = data.Organs.First(o => o.ID == inflow.ID);
                }
            }

            return data;
        }

        public static List<DataClass> Read_EIR(List<string> inpRead, bool calcProgeny)
        {
            var dataList = new List<DataClass>();
            dataList.Add(Read_EIR(inpRead, calcProgeny, "Age:3month"));
            dataList.Add(Read_EIR(inpRead, calcProgeny, "Age:1year"));
            dataList.Add(Read_EIR(inpRead, calcProgeny, "Age:5year"));
            dataList.Add(Read_EIR(inpRead, calcProgeny, "Age:10year"));
            dataList.Add(Read_EIR(inpRead, calcProgeny, "Age:15year"));
            dataList.Add(Read_EIR(inpRead, calcProgeny, "Age:adult"));
            return dataList;
        }

        public static DataClass Read_EIR(List<string> inpRead, bool calcProgeny, string age)
        {
            List<DataClass> listclass = new List<DataClass>();

            var data = new DataClass();

            string nuc;
            int num = 0;
            string[] values;

            values = inpRead[num].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            nuc = values[0];
            data.TargetNuc.Add(nuc);
            data.IntakeRoute[nuc] = (values[1]);
            data.Ramd[nuc] = double.Parse(values[2]);
            data.DecayRate[nuc] = double.Parse(values[3]);

            while (inpRead[num] != "end")
            {
                if (inpRead[num].Trim() == age)
                {
                    num++;

                    while (inpRead[num] != "end")
                    {
                        values = inpRead[num].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        var organId = int.Parse(values[0]);     // 臓器番号
                        var inflowNum = int.Parse(values[4]);   // 流入臓器数

                        var func = values[2];  // 臓器機能名称
                        var organFunc =
                            func == "inp" ? OrganFunc.inp :
                            func == "acc" ? OrganFunc.acc :
                            func == "mix" ? OrganFunc.mix :
                            func == "exc" ? OrganFunc.exc :
                            throw Program.Error($"Unrecognized organ function '{func}'.");

                        var organ = new Organ
                        {
                            Nuclide = nuc,
                            NuclideDecay = data.Ramd[nuc],
                            ID = organId,
                            Index = data.Organs.Count,
                            Name = values[1],
                            Func = organFunc,
                            BioDecay = double.Parse(values[3]),
                            BioDecayCalc = double.Parse(values[3]),
                            Inflows = new List<Inflow>(inflowNum),
                        };

                        data.CorrNum[Tuple.Create(nuc, organ.Name)] = values[7];

                        if (inflowNum == 0)
                        {
                            num++;
                        }
                        else if (organ.Func == OrganFunc.inp)   // 入力
                        {
                            if (inflowNum != 1)
                                throw Program.Error("The number of inflow paths in the Input compartment should be 0.");
                            num++;
                        }
                        else
                        {
                            for (int i = 0; i < inflowNum; i++)
                            {
                                values = inpRead[num].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                                int inflowID;
                                double inflowRate;
                                if (values.Length > 3)
                                {
                                    inflowID = int.Parse(values[5]);
                                    inflowRate = double.Parse(values[6]);
                                }
                                else
                                {
                                    inflowID = int.Parse(values[0]);
                                    inflowRate = double.Parse(values[1]);
                                }
                                organ.Inflows.Add(new Inflow
                                {
                                    ID = inflowID,
                                    Rate = inflowRate * 0.01,
                                });
                                num++;
                            }
                        }

                        data.Organs.Add(organ);

                        if (inpRead[num].Trim() == "end")
                        {
                            break;
                        }

                        if (inpRead[num].Trim() == "cont")
                        {
                            if (calcProgeny == false)
                                break;
                            num++;

                            values = inpRead[num].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            nuc = values[0];
                            data.TargetNuc.Add(nuc);
                            data.IntakeRoute[nuc] = (values[1]);
                            data.Ramd[nuc] = double.Parse(values[2]);
                            data.DecayRate[nuc] = double.Parse(values[3]);
                            num++;
                        }
                        if (inpRead[num].Trim() == "next")
                        {
                            break;
                        }
                    }
                }
                else
                {
                    num++;
                    continue;
                }

                // 流入経路から流入元臓器の情報を直接引くための参照を設定する
                foreach (var organ in data.Organs)
                {
                    foreach (var inflow in organ.Inflows)
                    {
                        if (inflow.ID == 0)
                            continue;
                        inflow.Organ = data.Organs.First(o => o.ID == inflow.ID);
                    }
                }
            }
            return data;
        }

        public static List<string> Read_See(List<string> inpRead, string age)
        {
            List<string> data = new List<string>();
            for (int i = 0; i < inpRead.Count; i++)
            {
                if (inpRead[i].Trim() == age)
                {
                    i++;
                    i++;
                    while (true)
                    {
                        data.Add(inpRead[i]);
                        i++;

                        if ((i > inpRead.Count - 1) || inpRead[i].StartsWith("Age:"))
                            break;
                    }
                }
            }
            return data;
        }
    }
}
