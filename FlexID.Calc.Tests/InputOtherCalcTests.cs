namespace FlexID.Calc.Tests;

using static InputErrorTestHelpers;

[TestClass]
public class InputOtherCalcTests
{
    List<string> ExpectOtherSourceRegions(InputData data) =>
        data.SourceRegions.Where(s => s.MaleID != 0 || s.FemaleID != 0)
                          .Select(sr => sr.Name).ToList();

    void Include(List<string> list, string item)
    {
        // 包含設定：リストに存在しない場合に追加する。
        if (!list.Contains(item))
            list.Add(item);
    }

    void Exclude(List<string> list, string item)
    {
        // 除外設定：リストに存在する場合に削除する。
        var n = list.RemoveAll(x => x == item);
        (n == 0 || n == 1).ShouldBeTrue();
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
        Exclude(expectOtherSourceRegions, "C-bone-V");
        Exclude(expectOtherSourceRegions, "T-bone-V");

        data.Nuclides[0].OtherSourceRegions.ShouldBe(expectOtherSourceRegions);
    }
}
