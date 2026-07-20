using System.Text;

namespace FlexID;

using static InputErrorTestHelpers;

[TestClass]
public class InputErrorTests
{
    [TestMethod]
    public void DuplicatedSectionErrors()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [title]
            dummy2

            [parameter]
              OutputDose = true

            [parameter]
              OutputDose = true

            [nuclide]
              Sr-90  6.596156E-05

            [nuclide]
              Y-90   2.595247E-01

            [intake]
              ST0    100%

            [intake]
              ST0    100%

            [Sr-90:compartment]
              acc    ST0       ---

            [Sr-90:compartment]
              acc    ST1       ---

            [Sr-90:transfer]
              ST0    ST1       100

            [Sr-90:transfer]
              ST1    ST0       100
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(4): Duplicated [title] section.",
            "(10): Duplicated [parameter] section.",
            "(16): Duplicated [nuclide] section.",
            "(22): Duplicated [intake] section.",
            "(28): Duplicated [Sr-90:compartment] section.",
            "(34): Duplicated [Sr-90:transfer] section.",
        ]);
    }

    [TestMethod]
    public void MissingSectionErrors1()
    {
        var reader = CreateReader("""
            # [title]
            # dummy

            # [nuclide]
            #   Sr-90  6.596156E-05

            [Sr-90:compartment]
              acc    ST0       ---

            [Sr-90:transfer]
              input      ST0   100%
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(12): Missing [title] section.",
            "(12): Missing [nuclide] section.",
        ]);
    }

    [TestMethod]
    public void EmptySectionErrors1()
    {
        var reader = CreateReader("""
            [title]

            [nuclide]
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(1): Empty [title] section.",
            "(3): Empty [nuclide] section.",
        ]);
    }

    [TestMethod]
    public void EmptySectionErrors2()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  6.596156E-05

            [intake]

            [Sr-90:compartment]

            [Sr-90:transfer]
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(7): Empty [intake] section.",
            // "(9): Empty [Sr-90:compartment] section.",
            // "(x): Empty [Sr-90:transfer] section.",
        ]);
    }

    [TestMethod]
    public void TitleSection_UnrecognizedLinesError()
    {
        var reader = CreateReader("""
            [title]
            dummy

            unrecognized

            [nuclide]
              Sr-9
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(4): Unrecognized lines in [title] section." ,
        ]);
    }

    [TestMethod]
    public void NuclideSection_AutoModeErrors()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  Y-90   # set AutoMode = true
              aaa
              Sr-90

            [intake]
              ST0   100%

            [Sr-90:compartment]
              acc    ST0       ---

            [Sr-90:transfer]
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(6): 'aaa' is not nuclide name.",
            "(7): Duplicated nuclide definition for 'Sr-90'.",
        ]);
    }

    [TestMethod]
    public void NuclideSection_ManualModeErrors()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              6.596156E-05
              Sr-90  abcdefg
              Y-90  -6.596156E-05
              Zr-93  1.241198E-09  abc
              Nb-90  1.139420E+00  /1.0
              Mo-90  2.992002E+00  Nb-90/xyz
              Nb-93m 1.177330E-04  Nb-93/-1.0

            [intake]
              ST0   100%

            [Sr-90:compartment]
              acc    ST0       ---

            [Sr-90:transfer]
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(5): Nuclide definition should have at least 2 values.",
            "(6): Cannot get nuclide Lambda.",
            "(7): Nuclide Lambda should be positive.",
            "(8): Daughter name and branching fraction should be separated with '/'.",
            "(9): Daughter name should not be empty.",
            "(10): Cannot get branching fraction.",
            "(11): Branching fraction should be positive.",
        ]);
    }

    [TestMethod]
    public void CompartmentSection_SyntaxErrors()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  6.596156E-05

            [intake]
              ST0   100%

            [Sr-90:compartment]
              acc    ST0     # ---
              add    ST1       ---
              inp    input     ---
            
            [Sr-90:transfer]
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(11): Compartment definition should have 3 values.",
            "(12): Unrecognized compartment function 'add'.",
            "(13): Unrecognized compartment function 'inp'.",
        ]);
    }

    [TestMethod]
    public void CompartmentSection_UnknownSourceRegion()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  6.596156E-05

            [intake]
              ST0   100%

            [Sr-90:compartment]
              acc    ST0       Abcde

            [Sr-90:transfer]
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(11): Unknown source region name 'Abcde'.",
        ]);
    }

    [TestMethod]
    public void CompartmentSection_SpecifySourceRegionForStableNuclide()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Zr-90  0.000000E+00

            [intake]
              ST0   100%

            [Zr-90:compartment]
              acc    ST0       Other

            [Zr-90:transfer]
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(11): Cannot specify source region for stable nuclide 'Zr-90'.",
        ]);
    }

    [TestMethod]
    public void TransferSection_SyntexErrors()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  6.596156E-05

            [intake]
              ST0   100%

            [Sr-90:compartment]
              acc    ST0    ---
              acc    ST1    ---
            
            [Sr-90:transfer]
              ST0  # ST1    100%
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(15): Transfer path definition should have 3 values.",
        ]);
    }

    [TestMethod]
    public void TransferSection_SemanticErrors()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  6.596156E-05  Y-90/1.0
              Y-90   2.595247E-01

            [intake]
              ST0   100%

            [Sr-90:compartment]
              acc    ST0       ---
              acc    ST1       ---
              acc    ST2       ---

            [Sr-90:transfer]
              X-00/ST0   ST0        100
              ST0        X-00/ST0   100
              XX         ST0        100
              ST0        XX         100
              ST0        ST0        100
              ST0        ST1        100
              ST0        ST1        200
              ST0        ST2        abc%

            [Y-90:compartment]
              acc    ST0       ---

            [Y-90:transfer]
              ST0  Sr-90/ST0   ---
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(17): Undefined nuclide 'X-00'.",
            "(18): Undefined nuclide 'X-00'.",
            "(19): Undefined compartment 'Sr-90/XX'.",
            "(20): Undefined compartment 'Sr-90/XX'.",
            "(21): Cannot set transfer path to itself.",
            "(23): Duplicated transfer path from 'Sr-90/ST0' to 'Sr-90/ST1'.",
            "(24): Transfer coefficient should be evaluated to a number, not 'abc%'.",
            "(30): Cannot set transfer path to a compartment which is not belong to 'Y-90'.",
        ]);
    }

    [TestMethod]
    public void TransferSection_PathErrors()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  6.596156E-05  Y-90/1.0
              Y-90   2.595247E-01

            [intake]
              ST0   100%

            [Sr-90:compartment]
              acc    ST0        ---
              mix    mix-Blood  ---
              exc    Excreta    ---

            [Sr-90:transfer]
              mix-Blood  ST0    ---
              Excreta    ST0    100

            [Y-90:compartment]
              acc    ST0        ---
              acc    ST1        ---
              exc    Excreta    ---

            [Y-90:transfer]
              Sr-90/mix-Blood  ST0  ---
              Sr-90/ST0        ST0  100%
              ST0              ST1  100%
              Sr-90/Excreta    ST0  ---
              Sr-90/ST0    Excreta  ---
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(17): Require fraction of output activity [%] from mix 'mix-Blood'.",
            "(18): Cannot set output path from exc 'Excreta'.",
            "(26): Cannot set decay path from mix 'Sr-90/mix-Blood'.",
            "(27): Require transfer rate [/d] from acc 'Sr-90/ST0'.",
            "(28): Require transfer rate [/d] from acc 'ST0'.",
            "(29): Cannot set decay path from exc 'Sr-90/Excreta' to non-exc 'ST0'.",
            "(30): Cannot set decay path from acc 'Sr-90/ST0' to non-acc 'Excreta'.",
        ]);
    }

    [TestMethod]
    public void TransferSection_NegativeCoefficient()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  6.596156E-05

            [intake]
              ST0   100%

            [Sr-90:compartment]
              acc    ST0        ---
              acc    ST1        ---

            [Sr-90:transfer]
              ST0    ST1        -30
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(15): Transfer coefficient should be positive.",
        ]);
    }

    [TestMethod]
    public void Transfersection_SumOfCoefficientsIsNot100Percent()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  6.596156E-05

            [intake]
              ST0        63.1462%
              ST1        36.8528%

            [Sr-90:compartment]
              acc    ST0        ---
              acc    ST1        ---
              mix    Mix        ---

            [Sr-90:transfer]
              ST0    Mix        10
              ST1    Mix       100
              Mix    ST0        63.1462%
              Mix    ST1        36.8528%
            
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(7): Total [%] of intake paths is not 100%, but 99.999%.",
            "(8):     = 63.1462%",
            "(9):     = 36.8528%",
            "(19): Total [%] of transfer paths from 'Mix' is not 100%, but 99.999%.",
            "(19):     = 63.1462%",
            "(20):     = 36.8528%",
        ]);
    }

    [TestMethod]
    public void TransferSection_DivideByZeroCoefficient()
    {
        var reader = CreateReader("""
            [title]
            dummy

            [nuclide]
              Sr-90  6.596156E-05

            [intake]
              ST0   100%

            [Sr-90:compartment]
              acc    ST0        ---
              acc    ST1        ---

            [Sr-90:transfer]
              ST0    ST1      $(10 / 0)
            """);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "(15): Transfer coefficient evaluation failed: divide by zero.",
        ]);
    }
}

static class InputErrorTestHelpers
{
    public static InputDataReader_OIR CreateReader(string[] inputLines)
    {
        var inputFileBytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, inputLines));

        var stream = new MemoryStream(inputFileBytes);
        var reader = new StreamReader(stream);
        return new InputDataReader_OIR(reader);
    }

    public static InputDataReader_OIR CreateReader(string inputText)
    {
        var inputFileBytes = Encoding.UTF8.GetBytes(inputText);

        var stream = new MemoryStream(inputFileBytes);
        var reader = new StreamReader(stream);
        return new InputDataReader_OIR(reader);
    }

    public static IEnumerable<string> GetErrorLines(this InputErrors errors)
    {
        try { errors.RaiseIfAny(); }
        catch (InputErrorsException e) { return e.ErrorLines; }
        return Enumerable.Empty<string>();
    }
}
