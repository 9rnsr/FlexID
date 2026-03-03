using System.IO;

namespace FlexID.Models;

/// <summary>
/// 計算対象となるインプットの情報を保持する。
/// </summary>
public class InputTarget
{
    public InputTarget(string filePath, InputData data)
    {
        Name = Path.GetFileNameWithoutExtension(filePath);
        FilePath = filePath;
        Title = data.Title;
        Nuclide = data.Nuclides[0].Name;
        Progenies = [.. data.Nuclides.Skip(1).Select(nuc => nuc.Name)];
    }

    /// <summary>
    /// インプットの名前。
    /// </summary>
    public string Name { get; }

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
    /// 計算モデルの子孫核種。
    /// </summary>
    public IReadOnlyList<string> Progenies { get; }

    /// <summary>
    /// 指定フォルダ内にある、OIRのインプット群を列挙する。
    /// </summary>
    /// <param name="dirPath">格納フォルダへの相対パス。</param>
    /// <returns>取得したインプット群の情報。</returns>
    public static async IAsyncEnumerable<InputTarget> GetInputsOIR(string dirPath)
    {
        IEnumerable<string> inputFiles;
        try
        {
            dirPath = Path.Combine(AppResource.BaseDir, dirPath);
            inputFiles = Directory.EnumerateFiles(dirPath, "*.inp", SearchOption.AllDirectories);
        }
        catch (Exception e) when (e is IOException || e is SystemException)
        {
            // dirPathが存在しない場合など。
            yield break;
        }

        foreach (var inputFile in inputFiles)
        {
            var data = await Task.Run(() =>
            {
                try
                {
                    var reader = new InputDataReader_OIR(inputFile);
                    return reader.ReadRough();
                }
                catch
                {
                    // 読み込みに失敗した。
                    return null;
                }
            });
            if (data is null)
                continue;

            var parentNuclide = data.Nuclides.FirstOrDefault();
            if (parentNuclide is null)
                continue;

            yield return new InputTarget(inputFile, data);
        }
    }

    /// <summary>
    /// 指定フォルダ内にある、EIRのインプット群を列挙する。
    /// </summary>
    /// <param name="dirPath">格納フォルダへの相対パス。</param>
    /// <returns>取得したインプット群の情報。</returns>
    public static async IAsyncEnumerable<InputTarget> GetInputsEIR(string dirPath)
    {
        IEnumerable<string> inputFiles;
        try
        {
            dirPath = Path.Combine(AppResource.BaseDir, dirPath);
            inputFiles = Directory.EnumerateFiles(dirPath, "*.inp", SearchOption.AllDirectories);
        }
        catch (Exception e) when (e is IOException || e is SystemException)
        {
            // dirPathが存在しない場合など。
            yield break;
        }

        foreach (var inputFile in inputFiles)
        {
            var data = await Task.Run(() =>
            {
                try
                {
                    var reader = new InputDataReader_EIR(inputFile);
                    return reader.Read().FirstOrDefault();
                }
                catch
                {
                    // 読み込みに失敗した。
                    return null;
                }
            });
            if (data is null)
                continue;

            var parentNuclide = data.Nuclides.FirstOrDefault();
            if (parentNuclide is null)
                continue;

            yield return new InputTarget(inputFile, data);
        }
    }
}
