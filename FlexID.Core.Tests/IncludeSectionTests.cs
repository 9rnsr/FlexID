namespace FlexID;

[TestClass]
public class IncludeSectionTests
{
    [TestMethod]
    public void TestRoot1()
    {
        var target = "root1.inp";
        var targetDir = TestFiles.Combine("Include");

        var data = new InputDataReader_OIR(Path.Combine(targetDir, target)).Read();

        data.Organs.ShouldContain(o => o.Name == "Oralcavity");
    }

    [TestMethod]
    public void TestRoot2()
    {
        var target = "subfolder/root2.inp";
        var targetDir = TestFiles.Combine("Include");

        var data = new InputDataReader_OIR(Path.Combine(targetDir, target)).Read();

        data.Organs.ShouldContain(o => o.Name == "Oralcavity");
    }

    [TestMethod]
    public void TestError1()
    {
        var target = "error1.inp";
        var targetDir = TestFiles.Combine("Include");

        var reader = new InputDataReader_OIR(Path.Combine(targetDir, target));
        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();

        e.ErrorLines.ShouldBe(
        [
            "error-sub1.inc(2): Unrecognized compartment function 'abc'.",
            "error-sub2.inc(2): Transfer path definition should have 3 values.",
            "error1.inp(14): Cannot open include file 'unexisting.inc'.",
        ]);
    }
}
