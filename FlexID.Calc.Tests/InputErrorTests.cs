using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;
using System.Text;

namespace FlexID.Calc.Tests
{
    using static InputErrorTestHelpers;

    [TestClass]
    public class InputErrorTests
    {
        [TestMethod]
        public void SectionTitleDuplicated()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[title]",
                "dummy2",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "  Y-90   2.595247E-01  1.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 4: Duplicated [title] section.");
        }

        [TestMethod]
        public void SectionTitleMissing()
        {
            var reader = CreateReader(new[]
            {
                "# [title]",
                "# dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Missing [title] section.");
        }

        [TestMethod]
        public void SectionTitleReachToEnd()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 2: Reach to EOF while reading title section.");
        }

        [TestMethod]
        public void SectionTitleUnrecognizedLine()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "unrecognized",
                "",
                "[nuclide]",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 4: Unrecognized line in [title] section.");
        }

        [TestMethod]
        public void SectionNuclideDuplicated()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[nuclide]",
                "  Y-90   2.595247E-01  1.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 7: Duplicated [nuclide] section.");
        }

        [TestMethod]
        public void SectionNuclideMissing()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "# [nuclide]",
                "#   Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Missing [nuclide] section.");
        }

        [TestMethod]
        public void SectionNuclideEmpty()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("None of nuclides defined.");
        }

        [TestMethod]
        public void SectionCompartmentDuplicated()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
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
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 11: Duplicated [Sr-90:compartment] section.");
        }

        [TestMethod]
        public void SectionCompartmentMissing()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "# [Sr-90:compartment]",
                "#   inp    input     ---",
                "#   acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Missing [Sr-90:compartment] section.");
        }

        [TestMethod]
        public void SectionCompartmentEmpty()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "",
                "[Sr-90:transfer]",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("None of compartments defined for nuclide 'Sr-90'.");
        }

        [TestMethod]
        public void SectionTransferDuplicated()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "  acc    ST1       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
                "",
                "[Sr-90:transfer]",
                "  ST0        ST1   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 15: Duplicated [Sr-90:transfer] section.");
        }

        [TestMethod]
        public void SectionTransferMissing()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "# [Sr-90:transfer]",
                "#   input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Missing [Sr-90:transfer] section.");
        }

        [TestMethod]
        public void SectionTransferEmpty()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("None of transfers defined for nuclide 'Sr-90'.");
        }

        [TestMethod]
        public void NuclideShouldHave4Values()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  # 0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 5: Nuclide definition should have 3 values.");
        }

        [TestMethod]
        public void NuclideLambdaIsNotValue()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  abcdefg  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 5: Cannot get nuclide Lambda.");
        }

        [TestMethod]
        public void NuclideLambdaIsNotPositive()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  -6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 5: Nuclide Lambda should be positive.");
        }

        [TestMethod]
        public void NuclideDecayRateIsNotValue()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  xyz",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 5: Cannot get nuclide DecayRate.");
        }

        [TestMethod]
        public void NuclideDecayIsNotPositive()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  -1.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 5: Nuclide DecayRate should be positive.");
        }

        [TestMethod]
        public void CompartmentShouldHave3Values()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input   # ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 8: Compartment definition should have 3 values.");
        }

        [TestMethod]
        public void CompartmentUnrecognizedFunc()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  add    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 9: Unrecognized compartment function 'add'.");
        }

        [TestMethod]
        public void CompartmentInpDuplicated()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  inp    input2    ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 9: Duplicated 'inp' compartment.");
        }

        [TestMethod]
        public void CompartmentInpMissing()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  acc    ST0   ---",
                "  acc    ST1   ---",
                "",
                "[Sr-90:transfer]",
                "  ST0    ST1   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Missing 'inp' compartment.");
        }

        [TestMethod]
        public void CompartmentInpInProgeny()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "  Y-90   2.595247E-01  1.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
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

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 16: Cannot define 'inp' compartment which belongs to progeny nuclide.");
        }

        [TestMethod]
        public void CompartmentHasUnknownSourceRegion()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       Abcde",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 9: Unknown source region name 'Abcde'.");
        }

        [TestMethod]
        public void TransferShouldHave3Values()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input    # ST0   100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 12: Transfer path definition should have 3 values.");
        }

        [TestMethod]
        public void TransferCoefficientIsNotValue()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0   abc%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 12: Transfer coefficient should be evaluated to a number, not 'abc%'.");
        }

        [TestMethod]
        public void TransferFromUndefinedNuclide()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
                "  Y-90/ST0   ST0   100",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 13: Undefined nuclide 'Y-90'.");
        }

        [TestMethod]
        public void TransferToUndefinedNuclide()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
                "  ST0   Y-90/ST0   100",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 13: Undefined nuclide 'Y-90'.");
        }

        [TestMethod]
        public void TransferInpInProgeny()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "  Y-90   2.595247E-01  1.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
                "",
                "[Y-90:compartment]",
                "  acc    ST0       ---",
                "",
                "[Y-90:transfer]",
                "  ST0  Sr-90/ST0   ---",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 19: Cannot set transfer path to a compartment which is not belong to 'Y-90'.");
        }

        [TestMethod]
        public void TransferFromUndefinedCompartment()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0   100%",
                "  ST1    ST0   100",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 13: Undefined compartment 'ST1'.");
        }

        [TestMethod]
        public void TransferToUndefinedCompartment()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0   100%",
                "  ST0    ST1   100",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 13: Undefined compartment 'ST1'.");
        }

        [TestMethod]
        public void TransferToItself()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
                "  ST0        ST0   100",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 13: Cannot set transfer path to itself.");
        }

        [TestMethod]
        public void TransferDuplicated()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   50%",
                "  input      ST0   50%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 13: Duplicated transfer path from 'input' to 'ST0'.");
        }

        [TestMethod]
        public void TransferToInp()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0       100%",
                "  ST0    input     100",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 13: Cannot set input path to inp 'input'.");
        }

        [TestMethod]
        public void TransferFromExc()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "  exc    Excreta   ---",
                "",
                "[Sr-90:transfer]",
                "  input    ST0     100%",
                "  Excreta  ST0     100",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 14: Cannot set output path from exc 'Excreta'.");
        }

        [TestMethod]
        public void TransferDecayFromInp()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "  Y-90   2.595247E-01  1.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input     ---",
                "  acc    ST0       ---",
                "",
                "[Sr-90:transfer]",
                "  input      ST0   100%",
                "",
                "[Y-90:compartment]",
                "  acc    ST0       ---",
                "",
                "[Y-90:transfer]",
                "  Sr-90/input ST0  ---",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 19: Cannot set decay path from inp 'input'.");
        }

        [TestMethod]
        public void TransferDecayFromMix()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "  Y-90   2.595247E-01  1.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input      ---",
                "  acc    ST0        ---",
                "  mix    mix-Blood  ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0        100%",
                "  ST0    mix-Blood  100",
                "",
                "[Y-90:compartment]",
                "  acc    ST0       ---",
                "",
                "[Y-90:transfer]",
                "  Sr-90/mix-Blood   ST0  ---",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 21: Cannot set decay path from mix 'mix-Blood'.");
        }

        [TestMethod]
        public void TransferDecayWithCoefficient()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "  Y-90   2.595247E-01  1.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input      ---",
                "  acc    ST0        ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0        100%",
                "",
                "[Y-90:compartment]",
                "  acc    ST0        ---",
                "",
                "[Y-90:transfer]",
                "  Sr-90/ST0   ST0   100",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 19: Cannot set transfer coefficient on decay path.");
        }

        [TestMethod]
        public void TransferFractionRequiredFromInp()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input      ---",
                "  acc    ST0        ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0        ---",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 12: Require fraction of output activity [%] from inp 'input'.");
        }

        [TestMethod]
        public void TransferFractionRequiredFromMix()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input      ---",
                "  mix    mix-Blood  ---",
                "  acc    ST0        ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0        100%",
                "  ST0    mix-Blood  100",
                "  mix-Blood  ST0    ---",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 15: Require fraction of output activity [%] from mix 'mix-Blood'.");
        }

        [TestMethod]
        public void TransferRateRequiredFromAcc()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
                "",
                "[Sr-90:compartment]",
                "  inp    input      ---",
                "  acc    ST0        ---",
                "  acc    ST1        ---",
                "",
                "[Sr-90:transfer]",
                "  input  ST0        100%",
                "  ST0    ST1        100%",
            });

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 14: Require transfer rate [/d] from acc 'ST0'.");
        }

        [TestMethod]
        public void TransferCoefficientNegative()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
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

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Line 14: Transfer coefficient should be positive.");
        }

        [TestMethod]
        public void TransferConsistentUnits()
        {
            var reader = CreateReader(new[]
            {
                "[title]",
                "dummy",
                "",
                "[nuclide]",
                "  Sr-90  6.596156E-05  0.0",
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

            var e = new Action(() => reader.Read_OIR()).ShouldThrow<ApplicationException>();
            e.Message.ShouldBe("Total [%] of transfer paths from 'input' is  not 100%, but 99.999%.");
        }
    }

    class InputErrorTestHelpers
    {
        public static InputDataReader CreateReader(string[] inputLines)
        {
            var inputFileBytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, inputLines));

            var stream = new MemoryStream(inputFileBytes);
            var reader = new StreamReader(stream);
            return new InputDataReader(reader);
        }
    }
}
