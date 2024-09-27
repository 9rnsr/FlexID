using FlexID.Calc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlexID.ViewModels
{
    /// <summary>
    /// 計算対象として選択するインプットの情報を保持する。
    /// </summary>
    public class InputData
    {
        /// <summary>
        /// インプットのファイルパス。
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// 計算モデルの種別(被ばく経路/化学形態)。
        /// </summary>
        public string ModelType { get; }

        /// <summary>
        /// 計算モデルの親核種。
        /// </summary>
        public string Nuclide { get; }

        /// <summary>
        /// 子孫核種の計算モデルを持っている場合は<c>true</c>。
        /// </summary>
        public bool HasProgeny { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="modelType"></param>
        /// <param name="nuclide"></param>
        /// <param name="hasProgeny"></param>
        public InputData(string filePath, string modelType, string nuclide, bool hasProgeny)
        {
            FilePath = filePath;
            ModelType = modelType;
            Nuclide = nuclide;
            HasProgeny = hasProgeny;
        }

        public override bool Equals(object obj)
        {
            return obj is InputData other &&
                   FilePath == other.FilePath &&
                   ModelType == other.ModelType &&
                   Nuclide == other.Nuclide &&
                   HasProgeny == other.HasProgeny;
        }

        public override int GetHashCode()
        {
            int hashCode = -1273085575;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FilePath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModelType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nuclide);
            hashCode = hashCode * -1521134295 + HasProgeny.GetHashCode();
            return hashCode;
        }

        public void Deconstruct(out string filePath, out string modelType, out string nuclide, out bool hasProgeny)
        {
            filePath = FilePath;
            modelType = ModelType;
            nuclide = Nuclide;
            hasProgeny = HasProgeny;
        }

        /// <summary>
        /// 指定フォルダ内にある、ある核種のOIRのインプット群の情報を取得する。
        /// </summary>
        /// <param name="baseDir">基準フォルダ。</param>
        /// <param name="nuclide">基準フォルダの直下にある、核種のインプットを格納したサブフォルダの名前。</param>
        /// <returns>取得したインプット群の情報。</returns>
        public static List<InputData> GetInputsOIR(string baseDir, string nuclide)
        {
            var inputs = new List<InputData>();

            var targetDir = Path.Combine(baseDir, nuclide);
            var inputFiles = Directory.GetFileSystemEntries(targetDir, @"*.inp");

            foreach (var inputFile in inputFiles)
            {
                try
                {
                    var reader = new InputDataReader(inputFile, calcProgeny: true);
                    var data = reader.Read_OIR();

                    var parentNuclide = data.Nuclides.FirstOrDefault();
                    if (parentNuclide is null)
                        continue;

                    var type = parentNuclide.IntakeRoute;
                    var hasProgeny = data.Nuclides.Count > 1;

                    inputs.Add(new InputData(inputFile, type, nuclide, hasProgeny));
                }
                catch
                {
                    // 読み込みに失敗したので一覧には出さない。
                }
            }

            return inputs;
        }

        /// <summary>
        /// 指定フォルダ内にある、ある核種のEIRのインプット群の情報を取得する。
        /// </summary>
        /// <param name="baseDir">基準フォルダ。</param>
        /// <param name="nuclide">基準フォルダの直下にある、核種のインプットを格納したサブフォルダの名前。</param>
        /// <returns>取得したインプット群の情報。</returns>
        public static List<InputData> GetInputsEIR(string baseDir, string nuclide)
        {
            var inputs = new List<InputData>();

            var targetDir = Path.Combine(baseDir, nuclide);
            var inputFiles = Directory.GetFileSystemEntries(targetDir, @"*.inp");

            foreach (var inputFile in inputFiles)
            {
                try
                {
                    var reader = new InputDataReader(inputFile, calcProgeny: true);
                    var data = reader.Read_EIR().FirstOrDefault();

                    var parentNuclide = data?.Nuclides.FirstOrDefault();
                    if (parentNuclide is null)
                        continue;

                    var type = parentNuclide.IntakeRoute;
                    var hasProgeny = data.Nuclides.Count > 1;

                    inputs.Add(new InputData(inputFile, type, nuclide, hasProgeny));
                }
                catch
                {
                    // 読み込みに失敗したので一覧には出さない。
                }
            }

            return inputs;
        }
    }
}
