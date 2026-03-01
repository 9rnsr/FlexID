namespace FlexID;

#if false
internal partial class Program
{
    /// <summary>
    /// FlexIDに整備されているインプットとそれに対応する期待値データを列挙する。
    /// </summary>
    /// <param name="outputDir"></param>
    /// <param name="runCalc"></param>
    /// <returns></returns>
    static IEnumerable<ReportTarget> GetTargets(string outputDir, bool runCalc)
    {
        var inputDir = Path.Combine(AppResource.BaseDir, @"inp\OIR");
        var expectDir = Path.Combine(AppResource.BaseDir, @"expect");

        try
        {
            var targets = runCalc
                ? Directory.EnumerateFiles(inputDir, "*.inp", SearchOption.AllDirectories)
                : Directory.EnumerateFiles(outputDir, "*.log");

            var expects = Directory.EnumerateFiles(expectDir, "*.dat", SearchOption.AllDirectories)
                .Select(path => (Name: Path.GetFileNameWithoutExtension(path), Path: path)).ToArray();

            return targets.Select(targetPath =>
            {
                var name = Path.GetFileNameWithoutExtension(targetPath);
                var nuclide = name.Split('_')[0];

                var expectDosePath = expects.FirstOrDefault(x => x.Name == nuclide).Path;
                var expectRetentionPath = expects.FirstOrDefault(x => x.Name == name).Path;

                var resultDosePath = Path.Combine(outputDir, $"{name}_Dose.out");
                var resultRetentionPath = Path.Combine(outputDir, $"{name}_Retention.out");

                return new ReportTarget
                {
                    Name = name,
                    TargetPath = targetPath,
                    ExpectDosePath = expectDosePath,
                    ExpectRetentionPath = expectRetentionPath,
                    ResultDosePath = resultDosePath,
                    ResultRetentionPath = resultRetentionPath,
                };
            });
        }
        catch (Exception e) when (e is IOException || e is SystemException)
        {
            // baseDirが存在しない場合など。
            return [];
        }
    }
}
#endif
