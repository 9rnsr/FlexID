using FlexID.Calc;
using System;
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
        /// 計算モデルのタイトル(被ばく経路, 化学形態, etc.)。
        /// </summary>
        public string Title { get; }

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
        /// <param name="title"></param>
        /// <param name="nuclide"></param>
        /// <param name="hasProgeny"></param>
        public InputData(string filePath, string title, string nuclide, bool hasProgeny)
        {
            FilePath = filePath;
            Title = title;
            Nuclide = nuclide;
            HasProgeny = hasProgeny;
        }

        public override bool Equals(object obj)
        {
            return obj is InputData other &&
                   FilePath == other.FilePath &&
                   Title == other.Title &&
                   Nuclide == other.Nuclide &&
                   HasProgeny == other.HasProgeny;
        }

        public override int GetHashCode()
        {
            int hashCode = -1273085575;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FilePath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nuclide);
            hashCode = hashCode * -1521134295 + HasProgeny.GetHashCode();
            return hashCode;
        }

        public void Deconstruct(out string filePath, out string title, out string nuclide, out bool hasProgeny)
        {
            filePath = FilePath;
            title = Title;
            nuclide = Nuclide;
            hasProgeny = HasProgeny;
        }

        /// <summary>
        /// 指定フォルダ内にある、ある核種のOIRのインプット群を列挙する。
        /// </summary>
        /// <param name="baseDir">基準フォルダ。</param>
        /// <returns>取得したインプット群の情報。</returns>
        public static IEnumerable<InputData> GetInputsOIR(string baseDir)
        {
            IEnumerable<string> inputFiles;
            try
            {
                inputFiles = Directory.EnumerateFiles(baseDir, "*.inp", SearchOption.AllDirectories);
            }
            catch (Exception e) when (e is IOException || e is SystemException)
            {
                // baseDirが存在しない場合など。
                yield break;
            }

            foreach (var inputFile in inputFiles)
            {
                Calc.InputData data;
                try
                {
                    var reader = new InputDataReader(inputFile);
                    data = reader.Read_OIR();
                }
                catch
                {
                    // 読み込みに失敗した。
                    continue;
                }

                var parentNuclide = data.Nuclides.FirstOrDefault();
                if (parentNuclide is null)
                    continue;

                var title = data.Title;
                var nuclide = parentNuclide.Nuclide;
                var hasProgeny = data.Nuclides.Count > 1;

                yield return new InputData(inputFile, title, nuclide, hasProgeny);
            }
        }

        /// <summary>
        /// 指定フォルダ内にある、ある核種のEIRのインプット群を列挙する。
        /// </summary>
        /// <param name="baseDir">基準フォルダ。</param>
        /// <returns>取得したインプット群の情報。</returns>
        public static IEnumerable<InputData> GetInputsEIR(string baseDir)
        {
            IEnumerable<string> inputFiles;
            try
            {
                inputFiles = Directory.EnumerateFiles(baseDir, "*.inp", SearchOption.AllDirectories);
            }
            catch (Exception e) when (e is IOException || e is SystemException)
            {
                // baseDirが存在しない場合など。
                yield break;
            }

            foreach (var inputFile in inputFiles)
            {
                Calc.InputData data;
                try
                {
                    var reader = new InputDataReader(inputFile);
                    data = reader.Read_EIR().FirstOrDefault();
                }
                catch
                {
                    // 読み込みに失敗した。
                    continue;
                }

                var parentNuclide = data?.Nuclides.FirstOrDefault();
                if (parentNuclide is null)
                    continue;

                var title = data.Title;
                var nuclide = parentNuclide.Nuclide;
                var hasProgeny = data.Nuclides.Count > 1;

                yield return new InputData(inputFile, title, nuclide, hasProgeny);
            }
        }
    }
}
