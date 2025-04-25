using System;
using System.Collections.Generic;
using System.Diagnostics;

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
    [DebuggerDisplay("{Func} {Name} ({Nuclide})")]
    public class Organ
    {
        /// <summary>
        /// 臓器が対象とする核種。
        /// </summary>
        public NuclideData Nuclide;

        /// <summary>
        /// 崩壊定数[/day]。(＝ ln(2) / 半減期[day])
        /// </summary>
        public double NuclideDecay => Nuclide.Lambda;

        /// <summary>
        /// 臓器番号。
        /// </summary>
        public int ID;

        /// <summary>
        /// 臓器毎のデータを配列から引くためのインデックス。
        /// </summary>
        public int Index;

        /// <summary>
        /// 臓器名。
        /// </summary>
        public string Name;

        /// <summary>
        /// 臓器機能。
        /// </summary>
        public OrganFunc Func;

        /// <summary>
        /// excコンパートメントにおいて、OIRにおける24-hour smaple値を模擬した
        /// 残留放射能を出力する場合に <see langword="true"/>。
        /// </summary>
        public bool ExcretaCompatibleWithOIR;

        /// <summary>
        /// 生物学的崩壊定数[/day]。
        /// 蓄積コンパートメントのみで意味を持ち、それ以外では1.0となる。
        /// </summary>
        public double BioDecay;

        /// <summary>
        /// 流入経路。
        /// </summary>
        public List<Inflow> Inflows;

        /// <summary>
        /// コンパートメントへの流入がない場合に<see langword="true"/>。
        /// </summary>
        public bool IsZeroInflow;

        /// <summary>
        /// コンパートメントからの流出が即時に処理される場合に<see langword="true"/>。
        /// </summary>
        public bool IsInstantOutflow => Func == OrganFunc.inp || Func == OrganFunc.mix;

        /// <summary>
        /// 線源領域の名称。
        /// </summary>
        public string SourceRegion;

        /// <summary>
        /// コンパートメントに対応付けられた線源領域から各標的領域へのS係数。
        /// </summary>
        public double[] S_Coefficients;
    }

    [DebuggerDisplay("{Name}")]
    public class NuclideData
    {
        /// <summary>
        /// 核種名。
        /// </summary>
        public string Name;

        /// <summary>
        /// 半減期(単位付き)。
        /// </summary>
        public string HalfLife;

        /// <summary>
        /// 崩壊定数λ[/day]。(＝ ln(2) / 半減期[day])
        /// </summary>
        public double Lambda;

        /// <summary>
        /// 親核種からの崩壊割合(100%＝1.00と置いた比で持つ)。
        /// </summary>
        public (string Parent, double Branch)[] DecayRates;

        /// <summary>
        /// 子孫核種の場合は<c>true</c>。
        /// </summary>
        public bool IsProgeny => DecayRates.Length != 0;

        /// <summary>
        /// S係数データにおける各線源領域の名称。
        /// </summary>
        public string[] SourceRegions;

        /// <summary>
        /// 動態モデルでコンパートメントとして定義されておらず線源領域Otherの一部として取り扱う
        /// 各線源領域の名称。
        /// </summary>
        public string[] OtherSourceRegions;

        /// <summary>
        /// 消化管に対応するコンパートメント群のインデックスと寄与率。
        /// </summary>
        public (int Index, double Rate)[] AtractIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// 肺に対応するコンパートメント群のインデックスと寄与率。
        /// </summary>
        public (int Index, double Rate)[] LungsIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// 骨格に対応するコンパートメント群のインデックスと寄与率。
        /// </summary>
        public (int Index, double Rate)[] SkeletonIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// 肝臓に対応するコンパートメント群のインデックスと寄与率。
        /// </summary>
        public (int Index, double Rate)[] LiverIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// 甲状腺に対応するコンパートメント群のインデックスと寄与率。
        /// </summary>
        public (int Index, double Rate)[] ThyroidIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// パラメータ定義。
        /// </summary>
        public Dictionary<string, string> Parameters;

        /// <summary>
        /// 有効なパラメータ名の配列。
        /// </summary>
        public static readonly string[] ParameterNames = new[]
        {
            "ExcludeOtherSourceRegions",
            "IncludeOtherSourceRegions"
        };
    }

    public class InputData
    {
        /// <summary>
        /// インプットのタイトル文字列。
        /// </summary>
        public string Title;

        // このコンパートメントモデルが対象とする被ばく評価期間の開始年齢[day]。
        public int StartAge;

        /// <summary>
        /// 全ての核種。
        /// </summary>
        public List<NuclideData> Nuclides = new List<NuclideData>();

        /// <summary>
        /// SAFデータにおける各線源領域の情報。
        /// </summary>
        public SourceRegionData[] SourceRegions;

        /// <summary>
        /// SAFデータにおける各標的領域の名称。
        /// </summary>
        public string[] TargetRegions;

        /// <summary>
        /// 組織加重係数データにおける各標的領域の係数。
        /// </summary>
        public double[] TargetWeights;

        /// <summary>
        /// 核種毎のS係数データ表。
        /// </summary>
        public List<Dictionary<string, double[]>> SCoeffTables = new List<Dictionary<string, double[]>>();

        /// <summary>
        /// 全ての臓器。
        /// </summary>
        public List<Organ> Organs = new List<Organ>();

        /// <summary>
        /// パラメータ定義。
        /// </summary>
        public Dictionary<string, string> Parameters;

        /// <summary>
        /// 有効なパラメータ名の配列。
        /// </summary>
        public static readonly string[] ParameterNames = new[]
        {
            "ExcludeOtherSourceRegions",
            "IncludeOtherSourceRegions",
            "OutputDose",
            "OutputDoseRate",
            "OutputRetention",
            "OutputCumulative",
        };

        /// <summary>
        /// 線量の計算結果をファイルに出力する場合は <see langword="true"/>。
        /// </summary>
        public bool OutputDose { get; set; } = true;

        /// <summary>
        /// 線量率の計算結果をファイルに出力する場合は <see langword="true"/>。
        /// </summary>
        public bool OutputDoseRate { get; set; } = true;

        /// <summary>
        /// 残留放射能の計算結果をファイルに出力する場合は <see langword="true"/>。
        /// </summary>
        public bool OutputRetention { get; set; } = true;

        /// <summary>
        /// 積算放射能の計算結果をファイルに出力する場合は <see langword="true"/>。
        /// </summary>
        public bool OutputCumulative { get; set; } = true;
    }
}
