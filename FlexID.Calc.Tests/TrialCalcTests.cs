namespace FlexID.Calc.Tests
{
    [TestClass]
    public class TrialCalcTests
    {
        private string TestDir => TestFiles.Combine("TrialCalc");

        [TestMethod]
        [DataRow("56-Ba/Ba-133/Ba-133_ing-Insoluble")]
        [DataRow("56-Ba/Ba-133/Ba-133_ing-Soluble")]
        [DataRow("56-Ba/Ba-133/Ba-133_inh-TypeF")]
        [DataRow("56-Ba/Ba-133/Ba-133_inh-TypeM")]
        [DataRow("56-Ba/Ba-133/Ba-133_inh-TypeS")]
        [DataRow("38-Sr/Sr-90/Sr-90_ing-Other")]
        [DataRow("38-Sr/Sr-90/Sr-90_ing-Titanate")]
        [DataRow("38-Sr/Sr-90/Sr-90_inh-TypeF")]
        [DataRow("38-Sr/Sr-90/Sr-90_inh-TypeM")]
        [DataRow("38-Sr/Sr-90/Sr-90_inh-TypeS")]
        public void Test_OIR(string path)
        {
            var target = Path.GetFileNameWithoutExtension(path);
            var nuclide = target.Split('_')[0];
            var inputPath = Path.Combine("inp", "OIR", path + ".inp");

            var expectDir = Path.Combine(TestDir, "Expect_OIR", nuclide);
            var resultDir = Path.Combine(TestDir, "Result_OIR~", nuclide);
            Directory.CreateDirectory(resultDir);

            var outputPath = Path.Combine(resultDir, target);

            var cTimeMeshFile = @"lib\TimeMesh\time.dat";
            var oTimeMeshFile = Path.Combine(TestDir, "out-time.dat");

            var commitmentPeriod = "50years";

            var data = new InputDataReader_OIR(inputPath).Read();

            var main = new MainRoutine_OIR();
            main.OutputPath       /**/= outputPath;
            main.CalcTimeMeshPath /**/= cTimeMeshFile;
            main.OutTimeMeshPath  /**/= oTimeMeshFile;
            main.CommitmentPeriod /**/= commitmentPeriod;

            main.Main(data);

            File.ReadAllLines(Path.Combine(resultDir, target + "_Dose.out")).ShouldBe(
            File.ReadAllLines(Path.Combine(expectDir, target + "_Dose.out")));

            File.ReadAllLines(Path.Combine(resultDir, target + "_DoseRate.out")).ShouldBe(
            File.ReadAllLines(Path.Combine(expectDir, target + "_DoseRate.out")));

            File.ReadAllLines(Path.Combine(resultDir, target + "_Retention.out")).ShouldBe(
            File.ReadAllLines(Path.Combine(expectDir, target + "_Retention.out")));

            File.ReadAllLines(Path.Combine(resultDir, target + "_Cumulative.out")).ShouldBe(
            File.ReadAllLines(Path.Combine(expectDir, target + "_Cumulative.out")));
        }

        [TestMethod]
        [DataRow("Sr-90_ing", "3months old")]
        [DataRow("Sr-90_ing", "1years old")]
        [DataRow("Sr-90_ing", "5years old")]
        [DataRow("Sr-90_ing", "10years old")]
        [DataRow("Sr-90_ing", "15years old")]
        [DataRow("Sr-90_ing", "Adult")]
        public void Test_EIR(string target, string exposureAge)
        {
            var nuclide = target.Split('_')[0];
            var inputPath = Path.Combine("inp", "EIR", nuclide, target + ".inp");

            var expectDir = Path.Combine(TestDir, "Expect_EIR", nuclide, exposureAge.Replace(' ', '_'));
            var resultDir = Path.Combine(TestDir, "Result_EIR~", nuclide, exposureAge.Replace(' ', '_'));
            Directory.CreateDirectory(resultDir);

            var outputPath = Path.Combine(resultDir, target);

            var cTimeMeshFile = @"lib\TimeMesh\time.dat";
            var oTimeMeshFile = Path.Combine(TestDir, "out-time.dat");

            var commitmentPeriod =
                exposureAge == "3months old" /**/? "25450days" : // 70years - 100days = 25550days - 100days
                exposureAge == "1years old"  /**/? "69years" :   // 70years -  1years
                exposureAge == "5years old"  /**/? "65years" :   // 70years -  5years
                exposureAge == "10years old" /**/? "60years" :   // 70years - 10years
                exposureAge == "15years old" /**/? "55years" :   // 70years - 15years
                exposureAge == "Adult"       /**/? "50years" :   // 75years - 25years
                throw new NotSupportedException();

            var dataList = new InputDataReader_EIR(inputPath).Read();

            var main = new MainRoutine_EIR();
            main.OutputPath       /**/= outputPath;
            main.CalcTimeMeshPath /**/= cTimeMeshFile;
            main.OutTimeMeshPath  /**/= oTimeMeshFile;
            main.CommitmentPeriod /**/= commitmentPeriod;
            main.ExposureAge      /**/= exposureAge;

            main.Main(dataList);

            File.ReadAllLines(Path.Combine(resultDir, target + "_Dose.out")).ShouldBe(
            File.ReadAllLines(Path.Combine(expectDir, target + "_Dose.out")));

            File.ReadAllLines(Path.Combine(resultDir, target + "_DoseRate.out")).ShouldBe(
            File.ReadAllLines(Path.Combine(expectDir, target + "_DoseRate.out")));

            File.ReadAllLines(Path.Combine(resultDir, target + "_Retention.out")).ShouldBe(
            File.ReadAllLines(Path.Combine(expectDir, target + "_Retention.out")));

            File.ReadAllLines(Path.Combine(resultDir, target + "_Cumulative.out")).ShouldBe(
            File.ReadAllLines(Path.Combine(expectDir, target + "_Cumulative.out")));
        }
    }
}
