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
        /// FlexIDに整備されているインプットと、OIRデータにおけるMaterialの対応を定義する。
        /// (核種についてはTargetの名称に含める仕様となっている)
        /// </summary>
        /// <returns></returns>
        static IEnumerable<(string Target, string Material)> GetTargets()
        {
            yield return ("Ba-133_ing-Insoluble",       /**/"Ingestion, Insoluble forms: sulphate, titanate, fA=1E-4");
            yield return ("Ba-133_ing-Soluble",         /**/"Ingestion, Soluble forms, fA=0.2");
            yield return ("Ba-133_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Barium chloride, carbonate, fA=0.2, 5 µm");
            yield return ("Ba-133_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Barium sulphate, all unspecified forms, fA=4E-2, 5 µm");
            yield return ("Ba-133_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=2E-3, 5 µm");

            yield return ("C-14_ing",                   /**/"Ingestion, All chemical forms, fA=0.99");
            yield return ("C-14_inh-TypeF",             /**/"Inhalation, Aerosols Type F, fA=0.99, 5 µm");
            yield return ("C-14_inh-TypeF-Barium",      /**/"Inhalation, Aerosols Barium carbonate, fA=0.99, 5 µm");
            yield return ("C-14_inh-TypeF-Gas",         /**/"Inhalation, Gas or vapour Type F, Unspecified , fA=0.99");
            yield return ("C-14_inh-TypeM",             /**/"Inhalation, Aerosols Type M, All unspecified forms, fA=0.2, 5 µm");
            yield return ("C-14_inh-TypeS",             /**/"Inhalation, Aerosols Type S, Elemental carbon, carbon tritide, fA=1E-2, 5 µm");

            yield return ("Ca-45_ing",                  /**/"Ingestion, All unspecified forms, fA=0.4");
            yield return ("Ca-45_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Calcium chloride, fA=0.4, 5 µm");
            yield return ("Ca-45_inh-TypeM",            /**/"Inhalation, Aerosols Type M, All unspecified forms, fA=8E-2, 5 µm");
            yield return ("Ca-45_inh-TypeS",            /**/"Inhalation, Aerosols Type S, fA=4E-3, 5 µm");

            yield return ("Cs-134_ing-Insoluble",       /**/"Ingestion, Relatively insoluble forms, irradiated fuel fragments, fA=0.1");
            yield return ("Cs-134_ing-Unspecified",     /**/"Ingestion, Caesium chloride, nitrate, sulphate; all unspecified compounds, fA=0.99");
            yield return ("Cs-134_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Caesium chloride, nitrate, sulphate, fA=0.99, 5 µm");
            yield return ("Cs-134_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Irradiated fuel fragments, all unspecified forms, fA=0.2, 5 µm");
            yield return ("Cs-134_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=1E-2, 5 µm");

            yield return ("Cs-137_ing-Insoluble",       /**/"Ingestion, Relatively insoluble forms, irradiated fuel fragments, fA=0.1");
            yield return ("Cs-137_ing-Unspecified",     /**/"Ingestion, Caesium chloride, nitrate, sulphate; all unspecified compounds, fA=0.99");
            yield return ("Cs-137_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Caesium chloride, nitrate, sulphate, fA=0.99, 5 µm");
            yield return ("Cs-137_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Irradiated fuel fragments, all unspecified forms, fA=0.2, 5 µm");
            yield return ("Cs-137_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=1E-2, 5 µm");

            yield return ("Fe-59_ing",                  /**/"Ingestion, All unspecified forms, fA=0.1");
            yield return ("Fe-59_inh-TypeF",            /**/"Inhalation, Aerosols Type F, fA=0.1, 5 µm");
            yield return ("Fe-59_inh-TypeM",            /**/"Inhalation, Aerosols Type M, Ferric chloride, ferric oxide, all unspecified forms, fA=2E-2, 5 µm");
            yield return ("Fe-59_inh-TypeS",            /**/"Inhalation, Aerosols Type S, Corrosion products, fA=1E-3, 5 µm");

            yield return ("H-3_ing-Organic",            /**/"Ingestion,  Biogenic forms, fA=0.99");
            yield return ("H-3_ing-Insoluble",          /**/"Ingestion, Relatively insoluble forms, fA=0.1");
            yield return ("H-3_ing-Soluble",            /**/"Ingestion, Soluble forms, fA=0.99");
            yield return ("H-3_inh-TypeF-Gas",          /**/"Inhalation, Gas or vapour Type F, Unspecified , fA=0.99");
            yield return ("H-3_inh-TypeF-Organic",      /**/"Inhalation, Aerosols Biogenic organic compounds, fA=0.99, 5 µm");
            yield return ("H-3_inh-TypeF-Tritide",      /**/"Inhalation, Aerosols Type F, LaNiAl tritide, fA=0.99, 5 µm");
            yield return ("H-3_inh-TypeM",              /**/"Inhalation, Aerosols Type M, All unspecified compounds, glass fragments, luminous paint, titanium tritide, zirconium tritide, fA=0.2, 5 µm");
            yield return ("H-3_inh-TypeS",              /**/"Inhalation, Aerosols Type S, Carbon tritide, hafnium tritide, fA=1E-2, 5 µm");

            yield return ("I-129_ing",                  /**/"Ingestion, All unspecified forms, fA=0.99");
            yield return ("I-129_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Sodium iodide, caesium chloride vector, silver iodide, all unspecified forms, fA=0.99, 5 µm");
            yield return ("I-129_inh-TypeM",            /**/"Inhalation, Aerosols Type M, fA=0.2, 5 µm");
            yield return ("I-129_inh-TypeS",            /**/"Inhalation, Aerosols Type S, fA=1E-2, 5 µm");

            yield return ("Pu-238_ing-Insoluble",       /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-238_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-238_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-238_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-238_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");

            yield return ("Pu-239_ing-Insoluble",       /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-239_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-239_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-239_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-239_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");
            yield return ("Pu-239_inj",                 /**/"Injection, fA=5E-4");

            yield return ("Pu-240_ing-Insoluble",       /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-240_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-240_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-240_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-240_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");

            yield return ("Pu-241_ing-Insolube",        /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-241_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-241_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-241_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-241_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");

            yield return ("Pu-242_ing-Insoluble",       /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-242_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-242_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-242_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-242_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");

            yield return ("Ra-223_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Nitrate, fA=0.2, 5 µm");

            yield return ("Ra-226_ing",                 /**/"Ingestion, All forms, fA=0.2");
            yield return ("Ra-226_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Nitrate, fA=0.2, 5 µm");
            yield return ("Ra-226_inh-TypeM",           /**/"Inhalation, Aerosols Type M, All unspecified forms, fA=4E-2, 5 µm");
            yield return ("Ra-226_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=2E-3, 5 µm");

            yield return ("Sr-90_ing-Titanate",         /**/"Ingestion, Strontium titanate, fA=1E-2");
            yield return ("Sr-90_ing-Other",            /**/"Ingestion, All other chemical forms, fA=0.25");
            yield return ("Sr-90_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Strontium chloride, sulphate and carbonate, fA=0.25, 5 µm");
            yield return ("Sr-90_inh-TypeM",            /**/"Inhalation, Aerosols Type M, Fuel fragments, all unspecified forms, fA=5E-2, 5 µm");
            yield return ("Sr-90_inh-TypeS",            /**/"Inhalation, Aerosols Type S, FAP, PSL, fA=2.5E-3, 5 µm");

            yield return ("Tc-99_ing",                  /**/"Ingestion, All forms, fA=0.9");
            yield return ("Tc-99_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Pertechnetate, Tc-DTPA, fA=0.9, 5 µm");
            yield return ("Tc-99_inh-TypeM",            /**/"Inhalation, Aerosols Type M, All unspecified forms, fA=0.18, 5 µm");
            yield return ("Tc-99_inh-TypeS",            /**/"Inhalation, Aerosols Type S, fA=9E-3, 5 µm");

            yield return ("Zn-65_ing",                  /**/"Ingestion, All forms, fA=0.5");
            yield return ("Zn-65_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Oxide, chromate, fA=0.5, 5 µm");
            yield return ("Zn-65_inh-TypeM",            /**/"Inhalation, Aerosols Type M, Nitrate, phosphate, all unspecified compounds, fA=0.1, 5 µm");
            yield return ("Zn-65_inh-TypeS",            /**/"Inhalation, Aerosols Type S, Corrosion products, fA=5E-3, 5 µm");
        }
    }
}
