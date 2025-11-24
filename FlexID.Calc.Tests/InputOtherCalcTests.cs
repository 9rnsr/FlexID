namespace FlexID.Calc.Tests;

using static InputErrorTestHelpers;

[TestClass]
public class InputOtherCalcTests
{
    List<string> ExpectOtherSourceRegions(InputData data)
    {
        var sourceRegions = data.SourceRegions.Where(s => s.MaleID != 0 || s.FemaleID != 0)
                                .Select(sr => sr.Name).ToList();

        sourceRegions.ShouldBe(new[]
        {
            "O-mucosa",    "Teeth-V",     "Tonsils",     "Oesophag-w",  "St-wall",
            "SI-wall",     "RC-wall",     "LC-wall",     "RS-wall",     "LN-ET",
            "LN-Th",       "Adrenals",    "C-bone-V",    "T-bone-V",    "R-marrow",
            "Y-marrow",    "Brain",       "Breast",      "Eye-lens",    "GB-wall",
            "Ht-wall",     "Kidneys",     "Liver",       "LN-Sys",      "Ovaries",
            "Pancreas",    "P-gland",     "Prostate",    "S-glands",    "Skin",
            "Spleen",      "Testes",      "Thymus",      "Thyroid",     "Ureters",
            "UB-wall",     "Uterus",      "Adipose",     "Cartilage",   "Muscle",
            "ET1-wall",    "ET2-wall",    "Lung-Tis",
        });

        return sourceRegions;
    }

    /// <summary>
    /// 何も調整されない場合のOtherの内訳をテストする。
    /// </summary>
    [TestMethod]
    public void TestAdjustNothing()
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
            "  acc    ST0        Other",
            "",
            "[Sr-90:transfer]",
            "  input  ST0        100%",
        ]);

        var data = reader.Read();
        var expectOtherSourceRegions = ExpectOtherSourceRegions(data);

        expectOtherSourceRegions.ShouldBe(new[]
        {
            "O-mucosa",    "Teeth-V",     "Tonsils",     "Oesophag-w",  "St-wall",
            "SI-wall",     "RC-wall",     "LC-wall",     "RS-wall",     "LN-ET",
            "LN-Th",       "Adrenals",    "C-bone-V",    "T-bone-V",    "R-marrow",
            "Y-marrow",    "Brain",       "Breast",      "Eye-lens",    "GB-wall",
            "Ht-wall",     "Kidneys",     "Liver",       "LN-Sys",      "Ovaries",
            "Pancreas",    "P-gland",     "Prostate",    "S-glands",    "Skin",
            "Spleen",      "Testes",      "Thymus",      "Thyroid",     "Ureters",
            "UB-wall",     "Uterus",      "Adipose",     "Cartilage",   "Muscle",
            "ET1-wall",    "ET2-wall",    "Lung-Tis",
        });

        data.Nuclides[0].OtherSourceRegions.ShouldBe(expectOtherSourceRegions);
    }

    /// <summary>
    /// コンパートメントとして明示された線源領域をOtherの内訳から除く処理をテストする。
    /// </summary>
    [TestMethod]
    public void TestAdjustOther()
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
            "  inp    input             ---",
            "  acc    ST0               Other",
            "  acc    C-bone-S          C-bone-S",
            "  acc    Exch-C-bone-V     C-bone-V",
            "  acc    Noch-C-bone-V     C-bone-V",
            "  acc    T-bone-S          T-bone-S",
            "  acc    Exch-T-bone-V     T-bone-V",
            "  acc    Noch-T-bone-V     T-bone-V",
            "",
            "[Sr-90:transfer]",
            "  input  ST0        100%",
        ]);

        var data = reader.Read();
        var expectOtherSourceRegions = ExpectOtherSourceRegions(data);

        // コンパートメントとして明示されているためOtherの内訳から除外される。
        expectOtherSourceRegions.Remove("C-bone-V");
        expectOtherSourceRegions.Remove("T-bone-V");

        data.Nuclides[0].OtherSourceRegions.ShouldBe(expectOtherSourceRegions);
    }

    /// <summary>
    /// コンパートメントとしてC-marrowやT-marrowが明示されている場合は、
    /// R-marrowとY-marrowの組み合わせをOtherの内訳から除外する。
    /// </summary>
    /// <param name="specifyC"></param>
    /// <param name="specifyT"></param>
    /// <param name="containsRY"></param>
    [TestMethod]
    [DataRow(false, /**/false, /**/true)]
    [DataRow(true,  /**/false, /**/false)]
    [DataRow(false, /**/true,  /**/false)]
    [DataRow(true,  /**/true,  /**/false)]
    public void TestAdjustExclusiveMarrow(bool specifyC, bool specifyT, bool containsRY)
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Th-232",
            "",
            "[Th-232:compartment]",
            "  inp    input        ---",
            "  acc    Other        Other",
            (specifyC ? "  acc    C-marrow     C-marrow" : ""),
            (specifyT ? "  acc    T-marrow     T-marrow" : ""),
            "",
            "[Th-232:transfer]",
            "  input  Other        100%",
            (specifyC ? "  Other  C-marrow     10" : ""),
            (specifyT ? "  Other  T-marrow     10" : ""),
        ]);

        var data = reader.Read();
        var expectOtherSourceRegions = ExpectOtherSourceRegions(data);

        if (containsRY)
        {
            expectOtherSourceRegions.ShouldContain("R-marrow");
            expectOtherSourceRegions.ShouldContain("Y-marrow");
        }
        else
        {
            expectOtherSourceRegions.Remove("R-marrow");
            expectOtherSourceRegions.Remove("Y-marrow");
        }

        data.Nuclides[0].OtherSourceRegions.ShouldBe(expectOtherSourceRegions);
    }

    /// <summary>
    /// C/T-marrowと、R/Y-marrowの組み合わせを両方使用するインプットはエラーとする。
    /// </summary>
    [TestMethod]
    public void TestAdjustIncorrectMarrow()
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Th-232",
            "",
            "[Th-232:compartment]",
            "  inp    input        ---",
            "  acc    Blood        Blood",
            "  acc    C-marrow     C-marrow",
            "  acc    T-marrow     T-marrow",
            "  acc    R-marrow     R-marrow",
            "  acc    Y-marrow     Y-marrow",
            "",
            "[Th-232:transfer]",
            "  input  Blood        100%",
            "  Blood  C-marrow     10",
            "  Blood  T-marrow     10",
            "  Blood  R-marrow     10",
            "  Blood  Y-marrow     10",
        ]);

        var e = new Action(() => reader.Read()).ShouldThrow<InputErrorsException>();
        e.ErrorLines.ShouldBe(
        [
            "Line 7: Both of C/T-marrow and R/Y-marrow source region pairs are used.",
        ]);
    }
}
