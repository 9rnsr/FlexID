using System.Collections.Generic;
using System.IO;
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
        /// FlexIDに整備されているインプットを列挙する。
        /// (核種についてはTargetの名称に含める仕様となっている)
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> GetTargets()
        {
            yield return "Ba-133_ing-Insoluble";
            yield return "Ba-133_ing-Soluble";
            yield return "Ba-133_inh-TypeF";
            yield return "Ba-133_inh-TypeM";
            yield return "Ba-133_inh-TypeS";

            yield return "C-14_ing";
            yield return "C-14_inh-TypeF";
            yield return "C-14_inh-TypeF-Barium";
            yield return "C-14_inh-TypeF-Gas";
            yield return "C-14_inh-TypeM";
            yield return "C-14_inh-TypeS";

            yield return "Ca-45_ing";
            yield return "Ca-45_inh-TypeF";
            yield return "Ca-45_inh-TypeM";
            yield return "Ca-45_inh-TypeS";

            yield return "Cs-134_ing-Insoluble";
            yield return "Cs-134_ing-Unspecified";
            yield return "Cs-134_inh-TypeF";
            yield return "Cs-134_inh-TypeM";
            yield return "Cs-134_inh-TypeS";

            yield return "Cs-137_ing-Insoluble";
            yield return "Cs-137_ing-Unspecified";
            yield return "Cs-137_inh-TypeF";
            yield return "Cs-137_inh-TypeM";
            yield return "Cs-137_inh-TypeS";

            yield return "Fe-59_ing";
            yield return "Fe-59_inh-TypeF";
            yield return "Fe-59_inh-TypeM";
            yield return "Fe-59_inh-TypeS";

            yield return "H-3_ing-Organic";
            yield return "H-3_ing-Insoluble";
            yield return "H-3_ing-Soluble";
            yield return "H-3_inh-TypeF-Gas";
            yield return "H-3_inh-TypeF-Organic";
            yield return "H-3_inh-TypeF-Tritide";
            yield return "H-3_inh-TypeM";
            yield return "H-3_inh-TypeS";

            yield return "I-129_ing";
            yield return "I-129_inh-TypeF";
            yield return "I-129_inh-TypeM";
            yield return "I-129_inh-TypeS";

            yield return "Mo-93_ing-Other";
            yield return "Mo-93_ing-Sulphide";
            yield return "Mo-93_inh-TypeF";
            yield return "Mo-93_inh-TypeM";
            yield return "Mo-93_inh-TypeS";

            yield return "Pu-238_ing-Insoluble";
            yield return "Pu-238_ing-Unidentified";
            yield return "Pu-238_inh-TypeF";
            yield return "Pu-238_inh-TypeM";
            yield return "Pu-238_inh-TypeS";

            yield return "Pu-239_ing-Insoluble";
            yield return "Pu-239_ing-Unidentified";
            yield return "Pu-239_inh-TypeF";
            yield return "Pu-239_inh-TypeM";
            yield return "Pu-239_inh-TypeS";
            yield return "Pu-239_inj";

            yield return "Pu-240_ing-Insoluble";
            yield return "Pu-240_ing-Unidentified";
            yield return "Pu-240_inh-TypeF";
            yield return "Pu-240_inh-TypeM";
            yield return "Pu-240_inh-TypeS";

            yield return "Pu-241_ing-Insolube";
            yield return "Pu-241_ing-Unidentified";
            yield return "Pu-241_inh-TypeF";
            yield return "Pu-241_inh-TypeM";
            yield return "Pu-241_inh-TypeS";

            yield return "Pu-242_ing-Insoluble";
            yield return "Pu-242_ing-Unidentified";
            yield return "Pu-242_inh-TypeF";
            yield return "Pu-242_inh-TypeM";
            yield return "Pu-242_inh-TypeS";

            yield return "Ra-223_inh-TypeF";

            yield return "Ra-226_ing";
            yield return "Ra-226_inh-TypeF";
            yield return "Ra-226_inh-TypeM";
            yield return "Ra-226_inh-TypeS";

            yield return "Sr-90_ing-Titanate";
            yield return "Sr-90_ing-Other";
            yield return "Sr-90_inh-TypeF";
            yield return "Sr-90_inh-TypeM";
            yield return "Sr-90_inh-TypeS";

            yield return "Tc-99_ing";
            yield return "Tc-99_inh-TypeF";
            yield return "Tc-99_inh-TypeM";
            yield return "Tc-99_inh-TypeS";

            yield return "Zn-65_ing";
            yield return "Zn-65_inh-TypeF";
            yield return "Zn-65_inh-TypeM";
            yield return "Zn-65_inh-TypeS";
        }
    }
}
