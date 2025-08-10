namespace FlexID.Calc.Tests;

[TestClass]
public class SAFReadTests
{
    [TestMethod]
    public void TestReadRAD()
    {
        var expectFile = TestFiles.Combine("SAFRead", "Ca-45_RadExpected.txt");
        var expectLines = File.ReadAllLines(expectFile);

        var actualLines = SAFDataReader.ReadRAD("Ca-45");

        actualLines.ShouldBe(expectLines);
    }

    [TestMethod]
    public void TestReadBET()
    {
        var expectFile = TestFiles.Combine("SAFRead", "Sr-90_BetExpected.txt");
        var expectLines = File.ReadAllLines(expectFile);

        var actualLines = SAFDataReader.ReadBET("Sr-90");

        actualLines.ShouldBe(expectLines);
    }

    [TestMethod]
    public void TestReadSourceRegions()
    {
        var sregions = SAFDataReader.ReadSourceRegions();
        sregions.Select(s => s.Name).ShouldBe(new[]
        {
            "O-cavity",    "O-mucosa",    "Teeth-S",     "Teeth-V",     "Tongue",
            "Tonsils",     "Oesophag-s",  "Oesophag-f",  "Oesophag-w",  "St-cont",
            "St-mucosa",   "St-wall",     "SI-cont",     "SI-mucosa",   "SI-wall",
            "SI-villi",    "RC-cont",     "RC-mucosa",   "RC-wall",     "LC-cont",
            "LC-mucosa",   "LC-wall",     "RS-cont",     "RS-mucosa",   "RS-wall",
            "ET1-sur",     "ET2-sur",     "ET2-bnd",     "ET2-seq",     "LN-ET",
            "Bronchi",     "Bronchi-b",   "Bronchi-q",   "Brchiole",    "Brchiole-b",
            "Brchiole-q",  "ALV",         "LN-Th",       "Lungs",       "Adrenals",
            "Blood",       "C-bone-S",    "C-bone-V",    "T-bone-S",    "T-bone-V",
            "C-marrow",    "T-marrow",    "R-marrow",    "Y-marrow",    "Brain",
            "Breast",      "Eye-lens",    "GB-wall",     "GB-cont",     "Ht-wall",
            "Kidneys",     "Liver",       "LN-Sys",      "Ovaries",     "Pancreas",
            "P-gland",     "Prostate",    "S-glands",    "Skin",        "Spleen",
            "Testes",      "Thymus",      "Thyroid",     "Ureters",     "UB-wall",
            "UB-cont",     "Uterus",      "Adipose",     "Cartilage",   "Muscle",
            "ET1-wall",    "ET2-wall",    "Lung-Tis",    "RT-air",
        });

        sregions.Select(s => s.MaleMass).ShouldBe(new[]
        {
            0.0      , 3.583E-02, 0.000E+00, 5.000E-02, 7.300E-02,
            3.000E-03, 0.000E+00, 0.000E+00, 4.000E-02, 2.500E-01,
            4.639E-03, 1.500E-01, 3.500E-01, 3.696E-02, 6.500E-01,
            1.252E-02, 1.500E-01, 2.010E-02, 1.500E-01, 7.500E-02,
            1.875E-02, 1.500E-01, 7.500E-02, 1.128E-02, 7.000E-02,
            0.000E+00, 0.000E+00, 2.472E-03, 4.504E-04, 1.500E-02,
            0.000E+00, 1.727E-03, 2.918E-04, 0.000E+00, 4.891E-03,
            1.252E-03, 1.100E+00, 1.500E-02, 1.200E+00, 1.400E-02,
            5.600E+00, 0.000E+00, 4.400E+00, 0.000E+00, 1.100E+00,
            2.790E-01, 3.371E+00, 1.170E+00, 2.480E+00, 1.450E+00,
            2.500E-02, 4.000E-04, 1.000E-02, 5.800E-02, 3.300E-01,
            3.100E-01, 1.800E+00, 1.484E-01, 0.000E+00, 1.400E-01,
            6.000E-04, 1.700E-02, 8.500E-02, 3.300E+00, 1.500E-01,
            3.500E-02, 2.500E-02, 2.000E-02, 1.600E-02, 5.000E-02,
            2.000E-01, 0.000E+00, 1.723E+01, 1.100E+00, 2.900E+01,
            2.923E-03, 2.923E-03, 5.000E-01, 0.000E+00,
        });

        sregions.Select(s => s.FemaleMass).ShouldBe(new[]
        {
            0.0      ,  2.245E-02,  0.000E+00,  4.000E-02,  6.000E-02,
            3.000E-03,  0.000E+00,  0.000E+00,  3.500E-02,  2.300E-01,
            4.639E-03,  1.400E-01,  2.800E-01,  3.432E-02,  6.000E-01,
            1.252E-02,  1.600E-01,  1.773E-02,  1.450E-01,  8.000E-02,
            1.726E-02,  1.450E-01,  8.000E-02,  1.039E-02,  7.000E-02,
            0.000E+00,  0.000E+00,  2.137E-03,  3.894E-04,  1.200E-02,
            0.000E+00,  1.552E-03,  2.622E-04,  0.000E+00,  4.703E-03,
            1.204E-03,  9.000E-01,  1.200E-02,  9.500E-01,  1.300E-02,
            4.100E+00,  0.000E+00,  3.200E+00,  0.000E+00,  8.000E-01,
            2.580E-01,  2.442E+00,  9.000E-01,  1.800E+00,  1.300E+00,
            5.000E-01,  4.000E-04,  8.000E-03,  4.800E-02,  2.500E-01,
            2.750E-01,  1.400E+00,  1.187E-01,  1.100E-02,  1.200E-01,
            6.000E-04,  0.000E+00,  7.000E-02,  2.300E+00,  1.300E-01,
            0.000E+00,  2.000E-02,  1.700E-02,  1.500E-02,  4.000E-02,
            2.000E-01,  8.000E-02,  2.141E+01,  9.000E-01,  1.750E+01,
            2.526E-03,  2.526E-03,  4.200E-01,  0.000E+00,
        });
    }

    [TestMethod]
    public void TestReadTargetRegions()
    {
        var tregions = SAFDataReader.ReadTargetRegions();
        tregions.Select(t => t.Name).ShouldBe(new[]
        {
            "O-mucosa",     "Oesophagus",   "St-stem",      "SI-stem",      "RC-stem",
            "LC-stem",      "RS-stem",      "ET1-bas",      "ET2-bas",      "LN-ET",
            "Bronch-bas",   "Bronch-sec",   "Bchiol-sec",   "AI",           "LN-Th",
            "R-marrow",     "Endost-BS",    "Brain",        "Eye-lens",     "P-gland",
            "Tongue",       "Tonsils",      "S-glands",     "Thyroid",      "Breast",
            "Thymus",       "Ht-wall",      "Adrenals",     "Liver",        "Pancreas",
            "Kidneys",      "Spleen",       "GB-wall",      "Ureters",      "UB-wall",
            "Ovaries",      "Testes",       "Prostate",     "Uterus",       "LN-Sys",
            "Skin",         "Adipose",      "Muscle",
        });
    }

    [TestMethod]
    public void TestReadSAF_AdultMale()
    {
        var safdata = SAFDataReader.ReadSAF(Sex.Male);
        safdata.ShouldNotBeNull();
    }

    [TestMethod]
    public void TestReadSAF_AdultFemale()
    {
        var safdata = SAFDataReader.ReadSAF(Sex.Female);
        safdata.ShouldNotBeNull();
    }
}
