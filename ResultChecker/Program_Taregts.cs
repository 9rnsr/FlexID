using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ResultChecker
{
    internal partial class Program
    {
        static Regex patternMaterial = new Regex(
           @"^(?<RouteOfIntake>Injection|Ingestion|Inhalation), (?<Material>.+?)(?:, (?<ParticleSize>[\d\.]+) µm)?$");

        static (string routeOfIntake, string material, string particleSize) DecomposeMaterial(string mat)
        {
            var m = patternMaterial.Match(mat);
            if (!m.Success)
                throw new InvalidDataException();

            var routeOfIntake = m.Groups["RouteOfIntake"].Value;
            var particleSize = m.Groups["ParticleSize"].Value;
            var material = m.Groups["Material"].Value;
            if (particleSize.Length == 0)
                particleSize = "-";

            return (routeOfIntake, material, particleSize);
        }

        /// <summary>
        /// FlexIDに整備されているインプットとそれに対応する期待値データを列挙する。
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="runCalc"></param>
        /// <returns></returns>
        static IEnumerable<Target> GetTargets(string outputDir, bool runCalc)
        {
            const string inputDir = @"inp\OIR";
            const string expectDir = @"Expect";

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

                    return new Target
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
                return Enumerable.Empty<Target>();
            }
        }
    }
}
