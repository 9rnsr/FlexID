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
        /// FlexIDに整備されているインプットのファイルパスを列挙する。
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> GetInputs()
        {
            const string baseDir = @"inp\OIR";

            try
            {
                return Directory.EnumerateFiles(baseDir, "*.inp", SearchOption.AllDirectories);
            }
            catch (Exception e) when (e is IOException || e is SystemException)
            {
                // baseDirが存在しない場合など。
                return Enumerable.Empty<string>();
            }
        }
    }
}
