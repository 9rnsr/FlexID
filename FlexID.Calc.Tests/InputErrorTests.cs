using System.Text;

namespace FlexID.Calc.Tests
{
    using static InputErrorTestHelpers;

    [TestClass]
    public class InputErrorTests
    {
        [TestMethod]
        public void DuplicatedSectionErrors()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[title]",
                "dummy2",
                "",
                "[parameter]",
                "  OutputDose = true",
                "",
                "[parameter]",
                "  OutputDose = true",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "[nuclide]",
                "  Y-90   2.595247E-01",
                "",
                "[Sr-90:parameter]",
                "  ExcludeOtherSourceRegions = x",
                "",
                "[Sr-90:parameter]",
                "  ExcludeOtherSourceRegions = x",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:compartment]",
                "  acc    ST1       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
                "",
                "[Sr-90:transfer]",
                "  ST0        ST1   100%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 4: Duplicated [title] section.",
                "Line 10: Duplicated [parameter] section.",
                "Line 16: Duplicated [nuclide] section.",
                "Line 22: Duplicated [Sr-90:parameter] section.",
                "Line 29: Duplicated [Sr-90:compartment] section.",
                "Line 35: Duplicated [Sr-90:transfer] section.",
            });
        }

        [TestMethod]
        public void MissingSectionErrors1()
        {
            var reader = CreateReader(new[]
            {
                "# [title]",
                "# dummy",
                "",
                "# [nuclide]",
                "#   Sr-90  6.596156E-05",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 7: Undefined nuclide 'Sr-90' is used to define compartments.",
                "Line 11: Undefined nuclide 'Sr-90' is used to define transfers.",
                "Line 13: Missing [title] section.",
                "Line 13: Missing [nuclide] section.",
            });
        }

        [TestMethod]
        public void MissingSectionErrors2()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "# [Sr-90:compartment]",
                "#   inp    input     ---",
                "#   acc    ST0       ---",
                "",
                "# [Sr-90:transfer]",
                "#   input      ST0   100%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 13: Missing [Sr-90:compartment] section.",
                "Line 13: Missing [Sr-90:transfer] section.",
            });
        }

        [TestMethod]
        public void EmptySectionErrors1()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "",
                "[nuclide]",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 1: Empty [title] section.",
                "Line 3: Empty [nuclide] section.",
            });
        }

        [TestMethod]
        public void EmptySectionErrors2()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "[Sr-90:compartment]",
                "",
                "[Sr-90:transfer]",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 7: Empty [Sr-90:compartment] section.",
                "Line 9: Empty [Sr-90:transfer] section.",
            });
        }

        [TestMethod]
        public void TitleSection_UnrecognizedLinesError()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "unrecognized",
                "",
                "[nuclide]",
                "  Sr-90"
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 4: Unrecognized lines in [title] section." ,
            });
        }

        [TestMethod]
        public void NuclideSection_AutoModeErrors()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  Y-90",    // set AutoMode = true
                "  aaa",
                "  Sr-90",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 6: 'aaa' is not nuclide name.",
                "Line 7: Duplicated nuclide definition for 'Sr-90'.",
            });
        }

        [TestMethod]
        public void NuclideSection_ManualModeErrors()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  6.596156E-05",
                "  Sr-90  abcdefg",
                "  Y-90  -6.596156E-05",
                "  Zr-93  1.241198E-09  abc",
                "  Nb-90  1.139420E+00  /1.0",
                "  Mo-90  2.992002E+00  Nb-90/xyz",
                "  Nb-93m 1.177330E-04  Nb-93/-1.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 5: Nuclide definition should have at least 2 values.",
                "Line 6: Cannot get nuclide Lambda.",
                "Line 7: Nuclide Lambda should be positive.",
                "Line 8: Daughter name and branching fraction should be separated with '/'.",
                "Line 9: Daughter name should not be empty.",
                "Line 10: Cannot get branching fraction.",
                "Line 11: Branching fraction should be positive.",
            });
        }

        [TestMethod]
        public void CompartmentSection_SyntaxErrors()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "[Sr-90:compartment]",
                "  inp    input   # ---",
                "  acc    ST0       ---",
                "  add    ST1       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 8: Compartment definition should have 3 values.",
                "Line 10: Unrecognized compartment function 'add'.",
            });
        }

        [TestMethod]
        public void CompartmentSection_MissingInpError()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "[Sr-90:compartment]",
                "  acc    ST0   ---",
                "  acc    ST1   ---",
                "",
                "[Sr-90:transfer]",
                "  ST0    ST1   100%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 7: Missing 'inp' compartment.",
            });
        }

        [TestMethod]
        public void CompartmentSection_DefineInpErrors()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  Y-90/1.0",
                "  Y-90   2.595247E-01",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  inp    input2    ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
                "",
                "[Y-90:compartment]",
                "  inp    input2    ---",
                "  acc    ST0       ---",
                "",
                "[Y-90:transfer]",
                "  Sr-90/ST0  ST0   ---"
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 10: Duplicated 'inp' compartment.",
                "Line 17: Cannot define 'inp' compartment which belongs to progeny nuclide.",
            });
        }

        [TestMethod]
        public void CompartmentSection_UnknownSourceRegion()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       Abcde",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 9: Unknown source region name 'Abcde'.",
            });
        }

        [TestMethod]
        public void TransferSection_SyntexErrors()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input # ST0   100%",
                "  input   ST0   abc%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 12: Transfer path definition should have 3 values.",
                "Line 13: Transfer coefficient should be evaluated to a number, not 'abc%'.",
            });
        }

        [TestMethod]
        public void TransferSection_SemanticErrors()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  Y-90/1.0",
                "  Y-90   2.595247E-01",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0        100%",
                "  X-00/ST0   ST0        100",
                "  ST0        X-00/ST0   100",
                "  ST1        ST0        100",
                "  ST0        ST1        100",
                "  ST0        ST0        100",
                "  input      ST0        50%",
                "",
                "[Y-90:compartment]",
                "  acc    ST0       ---",
                "",
                "[Y-90:transfer]",
                "  ST0  Sr-90/ST0   ---",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 14: Undefined nuclide 'X-00'.",
                "Line 15: Undefined nuclide 'X-00'.",
                "Line 16: Undefined compartment 'ST1'.",
                "Line 17: Undefined compartment 'ST1'.",
                "Line 18: Cannot set transfer path to itself.",
                "Line 19: Duplicated transfer path from 'input' to 'ST0'.",
                "Line 25: Cannot set transfer path to a compartment which is not belong to 'Y-90'.",
            });
        }

        [TestMethod]
        public void TransferSection_PathErrors()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  Y-90/1.0",
                "  Y-90   2.595247E-01",
                "",
                "[Sr-90:compartment]",
                "  inp    input      ---",
                "  acc    ST0        ---",
                "  mix    mix-Blood  ---",
                "  exc    Excreta    ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0    ---",
                "  mix-Blood  ST0    ---",
                "  ST0        input  100",
                "  Excreta    ST0    100",
                "",
                "[Y-90:compartment]",
                "  acc    ST0        ---",
                "  acc    ST1        ---",
                "",
                "[Y-90:transfer]",
                "  Sr-90/input      ST0  ---",
                "  Sr-90/mix-Blood  ST0  ---",
                "  Sr-90/ST0        ST0  100%",
                "  ST0              ST1  100%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 15: Require fraction of output activity [%] from inp 'input'.",
                "Line 16: Require fraction of output activity [%] from mix 'mix-Blood'.",
                "Line 17: Cannot set input path to inp 'input'.",
                "Line 18: Cannot set output path from exc 'Excreta'.",
                "Line 25: Cannot set decay path from inp 'Sr-90/input'.",
                "Line 26: Cannot set decay path from mix 'Sr-90/mix-Blood'.",
                "Line 27: Require transfer rate [/d] from acc 'Sr-90/ST0'.",
                "Line 28: Require transfer rate [/d] from acc 'ST0'.",
            });
        }

        [TestMethod]
        public void TransferSection_NegativeCoefficient()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "[Sr-90:compartment]",
                "  inp    input      ---",
                "  acc    ST0        ---",
                "  acc    ST1        ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0        100%",
                "  ST0    ST1        -30",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 14: Transfer coefficient should be positive.",
            });
        }

        [TestMethod]
        public void Transfersection_SumOfCoefficientsIsNot100Percent()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "[Sr-90:compartment]",
                "  inp    input      ---",
                "  acc    ST0        ---",
                "  acc    ST1        ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0        63.1462%",
                "  input  ST1        36.8528%",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 13: Total [%] of transfer paths from 'input' is  not 100%, but 99.999%.",
                "Line 13:     = 63.1462%",
                "Line 14:     = 36.8528%",
            });
        }

        [TestMethod]
        public void TransferSection_DivideByZeroCoefficient()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05",
                "",
                "[Sr-90:compartment]",
                "  inp    input      ---",
                "  acc    ST0        ---",
                "  acc    ST1        ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0        100%",
                "  ST0    ST1      $(10 / 0)",
            });

            var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
            e.ErrorLines.ShouldBe(new[]
            {
                "Line 14: Transfer coefficient evaluation failed: divide by zero.",
            });
        }
    }

    class InputErrorTestHelpers
    {
        public static InputDataReader_OIR CreateReader(string[] inputLines)
        {
            var inputFileBytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, inputLines));

            var stream = new MemoryStream(inputFileBytes);
            var reader = new StreamReader(stream);
            return new InputDataReader_OIR(reader);
        }
    }
}
