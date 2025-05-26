namespace FlexID.Calc.Tests;

using System.Text;
using static InputErrorTestHelpers;

[TestClass]
public class InputErrorTests
{
    [TestMethod]
    public void SectionTitleDuplicated()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[title]",
            "dummy2",
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
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 4: Duplicated [title] section.", e.Message);
    }

    [TestMethod]
    public void SectionTitleMissing()
    {
        var reader = CreateReader(
        [
            "# [title]",
            "# dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Missing [title] section.", e.Message);
    }

    [TestMethod]
    public void SectionTitleReachToEnd()
    {
        var reader = CreateReader(
        [
            "[title]",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 2: Reach to EOF while reading title section.", e.Message);
    }

    [TestMethod]
    public void SectionTitleUnrecognizedLine()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "unrecognized",
            "",
            "[nuclide]",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 4: Unrecognized line in [title] section.", e.Message);
    }

    [TestMethod]
    public void SectionNuclideDuplicated()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[nuclide]",
            "  Y-90   2.595247E-01",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 7: Duplicated [nuclide] section.", e.Message);
    }

    [TestMethod]
    public void SectionNuclideMissing()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
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
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Missing [nuclide] section.", e.Message);
    }

    [TestMethod]
    public void SectionNuclideEmpty()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("None of nuclides defined.", e.Message);
    }

    [TestMethod]
    public void SectionCompartmentDuplicated()
    {
        var reader = CreateReader(
        [
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
            "[Sr-90:compartment]",
            "  acc    ST1       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 11: Duplicated [Sr-90:compartment] section.", e.Message);
    }

    [TestMethod]
    public void SectionCompartmentMissing()
    {
        var reader = CreateReader(
        [
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
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Missing [Sr-90:compartment] section.", e.Message);
    }

    [TestMethod]
    public void SectionCompartmentEmpty()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[Sr-90:compartment]",
            "",
            "[Sr-90:transfer]",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("None of compartments defined for nuclide 'Sr-90'.", e.Message);
    }

    [TestMethod]
    public void SectionTransferDuplicated()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
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
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 15: Duplicated [Sr-90:transfer] section.", e.Message);
    }

    [TestMethod]
    public void SectionTransferMissing()
    {
        var reader = CreateReader(
        [
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
            "# [Sr-90:transfer]",
            "#   input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Missing [Sr-90:transfer] section.", e.Message);
    }

    [TestMethod]
    public void SectionTransferEmpty()
    {
        var reader = CreateReader(
        [
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
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("None of transfers defined for nuclide 'Sr-90'.", e.Message);
    }

    [TestMethod]
    public void NuclideAutoModeInvalidName()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  Y-90",
            "  aaa",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 6: 'aaa' is not nuclide name.", e.Message);
    }

    [TestMethod]
    public void NuclideAutoModeDuplicatedDefinition()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  Sr-90",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 5: Duplicated nuclide definition for 'Sr-90'.", e.Message);
    }

    [TestMethod]
    public void NuclideDuplicatedDefinition()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05  Y-90/1.0",
            "  Sr-90  2.595247E-01",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 6: Duplicated nuclide definition for 'Sr-90'.", e.Message);
    }

    [TestMethod]
    public void NuclideShouldHaveAtLeast2Values()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  6.596156E-05",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 5: Nuclide definition should have at least 2 values.", e.Message);
    }

    [TestMethod]
    public void NuclideLambdaIsNotValue()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  abcdefg",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 5: Cannot get nuclide Lambda.", e.Message);
    }

    [TestMethod]
    public void NuclideLambdaIsNotPositive()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  -6.596156E-05",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 5: Nuclide Lambda should be positive.", e.Message);
    }

    [TestMethod]
    public void NuclideBranchingIncorrect()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05  abc",
            "  Y-90   2.595247E-01",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 5: Daughter name and branching fraction should be separated with '/'.", e.Message);
    }

    [TestMethod]
    public void NuclideBranchingDaughterIsEmpty()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05  /1.0",
            "  Y-90   2.595247E-01",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 5: Daughter name should not be empty.", e.Message);
    }

    [TestMethod]
    public void NuclideBranchingFractionIsNotValue()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05  Y-90/xyz",
            "  Y-90   2.595247E-01",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 5: Cannot get branching fraction.", e.Message);
    }

    [TestMethod]
    public void NuclideBranchingFractionIsNotPositive()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05  Y-90/-1.0",
            "  Y-90   2.595247E-01",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 5: Branching fraction should be positive.", e.Message);
    }

    [TestMethod]
    public void CompartmentShouldHave3Values()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[Sr-90:compartment]",
            "  inp    input   # ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 8: Compartment definition should have 3 values.", e.Message);
    }

    [TestMethod]
    public void CompartmentUnrecognizedFunc()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  add    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 9: Unrecognized compartment function 'add'.", e.Message);
    }

    [TestMethod]
    public void CompartmentInpDuplicated()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  inp    input2    ---",
            "  acc    ST0       ---",
            "",
            "[Sr-90:transfer]",
            "  input      ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 9: Duplicated 'inp' compartment.", e.Message);
    }

    [TestMethod]
    public void CompartmentInpMissing()
    {
        var reader = CreateReader(
        [
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
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Missing 'inp' compartment.", e.Message);
    }

    [TestMethod]
    public void CompartmentInpInProgeny()
    {
        var reader = CreateReader(
        [
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
            "  input      ST0   100%",
            "",
            "[Y-90:compartment]",
            "  inp    input2    ---",
            "  acc    ST0       ---",
            "",
            "[Y-90:transfer]",
            "  Sr-90/ST0  ST0   ---"
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 16: Cannot define 'inp' compartment which belongs to progeny nuclide.", e.Message);
    }

    [TestMethod]
    public void CompartmentHasUnknownSourceRegion()
    {
        var reader = CreateReader(
        [
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
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 9: Unknown source region name 'Abcde'.", e.Message);
    }

    [TestMethod]
    public void TransferShouldHave3Values()
    {
        var reader = CreateReader(
        [
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
            "  input    # ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 12: Transfer path definition should have 3 values.", e.Message);
    }

    [TestMethod]
    public void TransferCoefficientIsNotValue()
    {
        var reader = CreateReader(
        [
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
            "  input  ST0   abc%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 12: Transfer coefficient should be evaluated to a number, not 'abc%'.", e.Message);
    }

    [TestMethod]
    public void TransferFromUndefinedNuclide()
    {
        var reader = CreateReader(
        [
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
            "  input      ST0   100%",
            "  Y-90/ST0   ST0   100",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 13: Undefined nuclide 'Y-90'.", e.Message);
    }

    [TestMethod]
    public void TransferToUndefinedNuclide()
    {
        var reader = CreateReader(
        [
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
            "  input      ST0   100%",
            "  ST0   Y-90/ST0   100",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 13: Undefined nuclide 'Y-90'.", e.Message);
    }

    [TestMethod]
    public void TransferInpInProgeny()
    {
        var reader = CreateReader(
        [
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
            "  input      ST0   100%",
            "",
            "[Y-90:compartment]",
            "  acc    ST0       ---",
            "",
            "[Y-90:transfer]",
            "  ST0  Sr-90/ST0   ---",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 19: Cannot set transfer path to a compartment which is not belong to 'Y-90'.", e.Message);
    }

    [TestMethod]
    public void TransferFromUndefinedCompartment()
    {
        var reader = CreateReader(
        [
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
            "  input  ST0   100%",
            "  ST1    ST0   100",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 13: Undefined compartment 'ST1'.", e.Message);
    }

    [TestMethod]
    public void TransferToUndefinedCompartment()
    {
        var reader = CreateReader(
        [
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
            "  input  ST0   100%",
            "  ST0    ST1   100",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 13: Undefined compartment 'ST1'.", e.Message);
    }

    [TestMethod]
    public void TransferToItself()
    {
        var reader = CreateReader(
        [
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
            "  input      ST0   100%",
            "  ST0        ST0   100",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 13: Cannot set transfer path to itself.", e.Message);
    }

    [TestMethod]
    public void TransferDuplicated()
    {
        var reader = CreateReader(
        [
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
            "  input      ST0   50%",
            "  input      ST0   50%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 13: Duplicated transfer path from 'input' to 'ST0'.", e.Message);
    }

    [TestMethod]
    public void TransferToInp()
    {
        var reader = CreateReader(
        [
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
            "  input  ST0       100%",
            "  ST0    input     100",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 13: Cannot set input path to inp 'input'.", e.Message);
    }

    [TestMethod]
    public void TransferFromExc()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[Sr-90:compartment]",
            "  inp    input     ---",
            "  acc    ST0       ---",
            "  exc    Excreta   ---",
            "",
            "[Sr-90:transfer]",
            "  input    ST0     100%",
            "  Excreta  ST0     100",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 14: Cannot set output path from exc 'Excreta'.", e.Message);
    }

    [TestMethod]
    public void TransferDecayFromInp()
    {
        var reader = CreateReader(
        [
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
            "  input      ST0   100%",
            "",
            "[Y-90:compartment]",
            "  acc    ST0       ---",
            "",
            "[Y-90:transfer]",
            "  Sr-90/input ST0  ---",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 19: Cannot set decay path from inp 'Sr-90/input'.", e.Message);
    }

    [TestMethod]
    public void TransferDecayFromMix()
    {
        var reader = CreateReader(
        [
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
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 21: Cannot set decay path from mix 'Sr-90/mix-Blood'.", e.Message);
    }

    [TestMethod]
    public void TransferDecayWithPercent()
    {
        var reader = CreateReader(
        [
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
            "",
            "[Sr-90:transfer]",
            "  input  ST0        100%",
            "",
            "[Y-90:compartment]",
            "  acc    ST0        ---",
            "",
            "[Y-90:transfer]",
            "  Sr-90/ST0   ST0   100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 19: Require transfer rate [/d] from acc 'Sr-90/ST0'.", e.Message);
    }

    [TestMethod]
    public void TransferFractionRequiredFromInp()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[Sr-90:compartment]",
            "  inp    input      ---",
            "  acc    ST0        ---",
            "",
            "[Sr-90:transfer]",
            "  input  ST0        ---",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 12: Require fraction of output activity [%] from inp 'input'.", e.Message);
    }

    [TestMethod]
    public void TransferFractionRequiredFromMix()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
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
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 15: Require fraction of output activity [%] from mix 'mix-Blood'.", e.Message);
    }

    [TestMethod]
    public void TransferRateRequiredFromAcc()
    {
        var reader = CreateReader(
        [
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
            "  ST0    ST1        100%",
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 14: Require transfer rate [/d] from acc 'ST0'.", e.Message);
    }

    [TestMethod]
    public void TransferCoefficientNegative()
    {
        var reader = CreateReader(
        [
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
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Line 14: Transfer coefficient should be positive.", e.Message);
    }

    [TestMethod]
    public void TransferConsistentUnits()
    {
        var reader = CreateReader(
        [
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
        ]);

        var e = Assert.ThrowsException<ApplicationException>(() => reader.Read());
        Assert.AreEqual("Total [%] of transfer paths from 'input' is  not 100%, but 99.999%.", e.Message);
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
