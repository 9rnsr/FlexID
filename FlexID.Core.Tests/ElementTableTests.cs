namespace FlexID;

[TestClass]
public class ElementTableTests
{
    [TestMethod]
    [DataRow("H", 1)]
    [DataRow("He", 2)]
    [DataRow("Sr", 38)]
    public void ElementToAtomicNumberTests(string element, int expectAtomicNumber)
    {
        ElementTable.ElementToAtomicNumber(element).ShouldBe(expectAtomicNumber);
    }

    [TestMethod]
    [DataRow(1, "H")]
    [DataRow(2, "He")]
    [DataRow(38, "Sr")]
    public void AtomicNumberToElementTests(int atomicNumber, string expectElement)
    {
        ElementTable.AtomicNumberToElement(atomicNumber).ShouldBe(expectElement);
    }

    [TestMethod]
    [DataRow("H-3", "H", 3)]
    [DataRow("C-14", "C", 14)]
    [DataRow("Co-60m", "Co", 60, "m")]
    [DataRow("Sr-90", "Sr", 90)]
    [DataRow("Sb-124n", "Sb", 124, "n")]
    [DataRow("Cs-137", "Cs", 137)]
    [DataRow("Ac-225", "Ac", 225)]
    public void TryParseNuclideSuccessTests(string nuclide, string expectElement, int expectMassNumber, string expectMetaStable = "")
    {
        ElementTable.TryParseNuclide(nuclide, out var element, out var massNumber, out var metaStable).ShouldBeTrue();
        element.ShouldBe(expectElement);
        massNumber.ShouldBe(expectMassNumber);
        metaStable.ShouldBe(expectMetaStable);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("  ")]
    [DataRow("HHH")]
    [DataRow("123")]
    [DataRow("A-12")]
    [DataRow("Bc-34")]
    [DataRow(" Sr-90 ")]
    [DataRow("abc,Sr-90,456")]
    public void TryParseNuclideFailureTests(string nuclide)
    {
        ElementTable.TryParseNuclide(nuclide, out var element, out var massNumber, out var metaStable).ShouldBeFalse();
        element.ShouldBeNull();
        massNumber.ShouldBe(0);
        metaStable.ShouldBeNull();
    }
}
