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

    // OtherContainsMineralBoneパラメータは、インプット全体に対する指定より核種に対する指定が優先。
    // 核種に対するパラメータ指定がない場合は、インプット全体に対する指定が使用される。
    // 既定値は、インプット全体に対するOtherContainsMineralBone = auto指定と同等。

    /// <summary>
    /// 線源領域Otherの内訳を自動判定し、軟組織と判定される場合。
    /// </summary>
    [TestMethod]
    // auto設定になる場合
    [DataRow("                                ", "                                ", false)]
    [DataRow("OtherContainsMineralBone = auto ", "                                ", false)]
    [DataRow("                                ", "OtherContainsMineralBone = auto ", false)]
    [DataRow("OtherContainsMineralBone = auto ", "OtherContainsMineralBone = auto ", false)]
    [DataRow("OtherContainsMineralBone = false", "OtherContainsMineralBone = auto ", false)]
    [DataRow("OtherContainsMineralBone = true ", "OtherContainsMineralBone = auto ", false)]
    // false設定になる場合
    [DataRow("OtherContainsMineralBone = false", "                                ", false)]
    [DataRow("                                ", "OtherContainsMineralBone = false", false)]
    [DataRow("OtherContainsMineralBone = auto ", "OtherContainsMineralBone = false", false)]
    [DataRow("OtherContainsMineralBone = false", "OtherContainsMineralBone = false", false)]
    [DataRow("OtherContainsMineralBone = true ", "OtherContainsMineralBone = false", false)]
    // true設定になる場合
    [DataRow("OtherContainsMineralBone = true ", "                                ", true)]
    [DataRow("                                ", "OtherContainsMineralBone = true ", true)]
    [DataRow("OtherContainsMineralBone = auto ", "OtherContainsMineralBone = true ", true)]
    [DataRow("OtherContainsMineralBone = false", "OtherContainsMineralBone = true ", true)]
    [DataRow("OtherContainsMineralBone = true ", "OtherContainsMineralBone = true ", true)]
    public void TestAdjustAuto1_AreAllST(string wholeParameter, string nuclideParameter, bool expectContainsV)
    {
        // auto設定では、線源領域Otherを構成するコンパートメントが全て"ST～"という名称の場合に
        // その他の組織＝軟組織として扱い、無機質骨をOtherの内訳から除外する。
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[parameter]",
            "  " + wholeParameter,
            "",
            "[Sr-90:parameter]",
            "  " + nuclideParameter,
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

        if (expectContainsV)
        {
            expectOtherSourceRegions.ShouldContain("C-bone-V");
            expectOtherSourceRegions.ShouldContain("T-bone-V");
        }
        else
        {
            expectOtherSourceRegions.Remove("C-bone-V");
            expectOtherSourceRegions.Remove("T-bone-V");
        }

        data.Nuclides[0].OtherSourceRegions.ShouldBe(expectOtherSourceRegions);
    }

    /// <summary>
    /// 線源領域Otherの内訳を自動判定し、その他の組織と判定される場合。
    /// </summary>
    [TestMethod]
    // auto設定になる場合
    [DataRow("                                ", "                                ", true)]
    [DataRow("OtherContainsMineralBone = auto ", "                                ", true)]
    [DataRow("                                ", "OtherContainsMineralBone = auto ", true)]
    [DataRow("OtherContainsMineralBone = auto ", "OtherContainsMineralBone = auto ", true)]
    [DataRow("OtherContainsMineralBone = false", "OtherContainsMineralBone = auto ", true)]
    [DataRow("OtherContainsMineralBone = true ", "OtherContainsMineralBone = auto ", true)]
    // false設定になる場合
    [DataRow("OtherContainsMineralBone = false", "                                ", false)]
    [DataRow("                                ", "OtherContainsMineralBone = false", false)]
    [DataRow("OtherContainsMineralBone = auto ", "OtherContainsMineralBone = false", false)]
    [DataRow("OtherContainsMineralBone = false", "OtherContainsMineralBone = false", false)]
    [DataRow("OtherContainsMineralBone = true ", "OtherContainsMineralBone = false", false)]
    // true設定になる場合
    [DataRow("OtherContainsMineralBone = true ", "                                ", true)]
    [DataRow("                                ", "OtherContainsMineralBone = true ", true)]
    [DataRow("OtherContainsMineralBone = auto ", "OtherContainsMineralBone = true ", true)]
    [DataRow("OtherContainsMineralBone = false", "OtherContainsMineralBone = true ", true)]
    [DataRow("OtherContainsMineralBone = true ", "OtherContainsMineralBone = true ", true)]
    public void TestAdjustAuto2_AreNotAllST(string wholeParameter, string nuclideParameter, bool expectContansV)
    {
        // auto設定では、線源領域Otherを構成するコンパートメントが全て"ST～"という名称ではない場合は
        // その他の組織には無機質骨が含まれるものとして扱われる。
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[parameter]",
            "  " + wholeParameter,
            "",
            "[Sr-90:parameter]",
            "  " + nuclideParameter,
            "",
            "[Sr-90:compartment]",
            "  inp    input      ---",
            "  acc    Other1        Other",
            "  acc    Other2        Other",
            "",
            "[Sr-90:transfer]",
            "  input  Other1       100%",
            "  Other1 Other2       10",
        ]);

        var data = reader.Read();
        var expectOtherSourceRegions = ExpectOtherSourceRegions(data);

        if (expectContansV)
        {
            expectOtherSourceRegions.ShouldContain("C-bone-V");
            expectOtherSourceRegions.ShouldContain("T-bone-V");
        }
        else
        {
            expectOtherSourceRegions.Remove("C-bone-V");
            expectOtherSourceRegions.Remove("T-bone-V");
        }

        data.Nuclides[0].OtherSourceRegions.ShouldBe(expectOtherSourceRegions);
    }

    /// <summary>
    /// コンパートメントとして明示された線源領域をOtherの内訳から除く。
    /// </summary>
    [TestMethod]
    // auto設定になる場合
    [DataRow("                                ", "                                ")]
    [DataRow("OtherContainsMineralBone = auto ", "                                ")]
    [DataRow("                                ", "OtherContainsMineralBone = auto ")]
    [DataRow("OtherContainsMineralBone = auto ", "OtherContainsMineralBone = auto ")]
    [DataRow("OtherContainsMineralBone = false", "OtherContainsMineralBone = auto ")]
    [DataRow("OtherContainsMineralBone = true ", "OtherContainsMineralBone = auto ")]
    // false設定になる場合
    [DataRow("OtherContainsMineralBone = false", "                                ")]
    [DataRow("                                ", "OtherContainsMineralBone = false")]
    [DataRow("OtherContainsMineralBone = auto ", "OtherContainsMineralBone = false")]
    [DataRow("OtherContainsMineralBone = false", "OtherContainsMineralBone = false")]
    [DataRow("OtherContainsMineralBone = true ", "OtherContainsMineralBone = false")]
    // true設定になる場合
    [DataRow("OtherContainsMineralBone = true ", "                                ")]
    [DataRow("                                ", "OtherContainsMineralBone = true ")]
    [DataRow("OtherContainsMineralBone = auto ", "OtherContainsMineralBone = true ")]
    [DataRow("OtherContainsMineralBone = false", "OtherContainsMineralBone = true ")]
    [DataRow("OtherContainsMineralBone = true ", "OtherContainsMineralBone = true ")]
    public void TestAdjustAuto3_DropExplicits(string wholeParameter, string nuclideParameter)
    {
        var reader = CreateReader(
        [
            "[title]",
            "dummy",
            "",
            "[nuclide]",
            "  Sr-90  6.596156E-05",
            "",
            "[parameter]",
            "  " + wholeParameter,
            "",
            "[Sr-90:parameter]",
            "  " + nuclideParameter,
            "",
            "[Sr-90:compartment]",
            "  inp    input             ---",
            "  acc    LNET-F            LN-ET",
            "  acc    LNTH-F            LN-Th",
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

        // コンパートメントとして明示されている線源領域は、必ずOtherの内訳から除外される。
        expectOtherSourceRegions.Remove("LN-ET");
        expectOtherSourceRegions.Remove("LN-Th");
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
