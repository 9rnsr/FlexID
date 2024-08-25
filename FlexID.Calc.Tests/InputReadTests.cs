using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class InputReadTests
    {
        [TestMethod]
        [DataRow("Ba-133_ing-Insoluble")]
        [DataRow("Ba-133_ing-Soluble")]
        [DataRow("Ba-133_inh-TypeF")]
        [DataRow("Ba-133_inh-TypeM")]
        [DataRow("Ba-133_inh-TypeS")]
        [DataRow("C-14_ing")]
        [DataRow("C-14_inh-TypeF-Barium")]
        [DataRow("C-14_inh-TypeF-Gas")]
        [DataRow("C-14_inh-TypeF")]
        [DataRow("C-14_inh-TypeM")]
        [DataRow("C-14_inh-TypeS")]
        [DataRow("Ca-45_ing")]
        [DataRow("Ca-45_inh-TypeF")]
        [DataRow("Ca-45_inh-TypeM")]
        [DataRow("Ca-45_inh-TypeS")]
        [DataRow("Cs-134_ing-Insoluble")]
        [DataRow("Cs-134_ing-Unspecified")]
        [DataRow("Cs-134_inh-TypeF")]
        [DataRow("Cs-134_inh-TypeM")]
        [DataRow("Cs-134_inh-TypeS")]
        [DataRow("Cs-137_ing-Insoluble")]
        [DataRow("Cs-137_ing-Unspecified")]
        [DataRow("Cs-137_inh-TypeF")]
        [DataRow("Cs-137_inh-TypeM")]
        [DataRow("Cs-137_inh-TypeS")]
        [DataRow("Fe-59_ing")]
        [DataRow("Fe-59_inh-TypeF")]
        [DataRow("Fe-59_inh-TypeM")]
        [DataRow("Fe-59_inh-TypeS")]
        [DataRow("H-3_ing-Insoluble")]
        [DataRow("H-3_ing-Organic")]
        [DataRow("H-3_ing-Soluble")]
        [DataRow("H-3_inh-TypeF-Gas")]
        [DataRow("H-3_inh-TypeF-Organic")]
        [DataRow("H-3_inh-TypeF-Tritide")]
        [DataRow("H-3_inh-TypeM")]
        [DataRow("H-3_inh-TypeS")]
        [DataRow("I-129_ing")]
        [DataRow("I-129_inh-TypeF")]
        [DataRow("I-129_inh-TypeM")]
        [DataRow("I-129_inh-TypeS")]
        [DataRow("Mo-93_ing-Other")]
        [DataRow("Mo-93_ing-Sulphide")]
        [DataRow("Mo-93_inh-TypeF")]
        [DataRow("Mo-93_inh-TypeM")]
        [DataRow("Mo-93_inh-TypeS")]
        [DataRow("Pu-238_ing-Insoluble")]
        [DataRow("Pu-238_ing-Unidentified")]
        [DataRow("Pu-238_inh-TypeF")]
        [DataRow("Pu-238_inh-TypeM")]
        [DataRow("Pu-238_inh-TypeS")]
        [DataRow("Pu-239_ing-Insoluble")]
        [DataRow("Pu-239_ing-Unidentified")]
        [DataRow("Pu-239_inh-TypeF")]
        [DataRow("Pu-239_inh-TypeM")]
        [DataRow("Pu-239_inh-TypeS")]
        [DataRow("Pu-239_inj")]
        [DataRow("Pu-240_ing-Insoluble")]
        [DataRow("Pu-240_ing-Unidentified")]
        [DataRow("Pu-240_inh-TypeF")]
        [DataRow("Pu-240_inh-TypeM")]
        [DataRow("Pu-240_inh-TypeS")]
        [DataRow("Pu-241_ing-Insolube")]
        [DataRow("Pu-241_ing-Unidentified")]
        [DataRow("Pu-241_inh-TypeF")]
        [DataRow("Pu-241_inh-TypeM")]
        [DataRow("Pu-241_inh-TypeS")]
        [DataRow("Pu-242_ing-Insoluble")]
        [DataRow("Pu-242_ing-Unidentified")]
        [DataRow("Pu-242_inh-TypeF")]
        [DataRow("Pu-242_inh-TypeM")]
        [DataRow("Pu-242_inh-TypeS")]
        [DataRow("Ra-223_inh-TypeF")]
        [DataRow("Ra-226_ing")]
        [DataRow("Ra-226_inh-TypeF")]
        [DataRow("Ra-226_inh-TypeM")]
        [DataRow("Ra-226_inh-TypeS")]
        [DataRow("Sr-90_ing-Other")]
        [DataRow("Sr-90_ing-Titanate")]
        [DataRow("Sr-90_inh-TypeF")]
        [DataRow("Sr-90_inh-TypeM")]
        [DataRow("Sr-90_inh-TypeS")]
        [DataRow("Tc-99_ing")]
        [DataRow("Tc-99_inh-TypeF")]
        [DataRow("Tc-99_inh-TypeM")]
        [DataRow("Tc-99_inh-TypeS")]
        [DataRow("Zn-65_ing")]
        [DataRow("Zn-65_inh-TypeF")]
        [DataRow("Zn-65_inh-TypeM")]
        [DataRow("Zn-65_inh-TypeS")]
        public void Test_OIR(string target)
        {
            var nuclide = target.Split('_')[0];
            var inputPath = Path.Combine("inp", "OIR", nuclide, target + ".inp");

            var calcProgeny = true;
            var data = new InputDataReader(inputPath, calcProgeny).Read_OIR();
            Assert.IsNotNull(data);
        }

        [TestMethod]
        [DataRow("Sr-90_ing")]
        public void Test_EIR(string target)
        {
            var nuclide = target.Split('_')[0];
            var inputPath = Path.Combine("inp", "EIR", nuclide, target + ".inp");

            var calcProgeny = true;
            var data = new InputDataReader(inputPath, calcProgeny).Read_EIR();
            Assert.IsNotNull(data);
        }
    }
}
