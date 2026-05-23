namespace FlexID;

[TestClass]
public class Analysis
{
    const string computeTimeMeshFile = "ctime.dat";
    //const string computeTimeMeshFile = "ctime_max=12h.dat";
    //const string computeTimeMeshFile = "ctime_max=3min.dat";
    //const string computeTimeMeshFile = "ctime_max=1sec.dat";

    [TestMethod]
    [DataRow("Pattern-I_&_case-1")]
    [DataRow("Pattern-II_&_case-1")]
    [DataRow("Pattern-III_&_case-1")]
    [DataRow("Pattern-I_&_case-2")]
    [DataRow("Pattern-II_&_case-2")]
    [DataRow("Pattern-III_&_case-2")]
    [DataRow("Pattern-I_&_case-3")]
    [DataRow("Pattern-II_&_case-3")]
    [DataRow("Pattern-III_&_case-3")]
    [DataRow("Pattern-I_&_case-4")]
    [DataRow("Pattern-II_&_case-4")]
    [DataRow("Pattern-III_&_case-4")]
    [DataRow("Pattern-I_&_case-5")]
    [DataRow("Pattern-II_&_case-5")]
    [DataRow("Pattern-III_&_case-5")]
    [DataRow("Pattern-I_&_case-6")]
    [DataRow("Pattern-II_&_case-6")]
    [DataRow("Pattern-III_&_case-6")]
    [DataRow("Pattern-I_&_case-7")]
    [DataRow("Pattern-II_&_case-7")]
    [DataRow("Pattern-III_&_case-7")]
    public void Test(string target)
    {
        var testDir = TestFiles.Combine("Analysis");
        var inputFilePath = Path.Combine(testDir, target + ".inp");

        var expectDir = Path.Combine(testDir, "Expect_" + Path.GetFileNameWithoutExtension(computeTimeMeshFile));
        var resultDir = Path.Combine(testDir, "Result~");

        var data = new InputDataReader_OIR(inputFilePath).Read();
        data.OutputDose = false;
        data.OutputDoseRate = false;
        data.OutputRetention = false;
        data.OutputCumulative = false;
        data.OutputAtoms = true;

        var computeTimeMeshPath = Path.Combine(testDir, computeTimeMeshFile);
        var outputTimeMeshPath = Path.Combine(testDir, "otime.dat");

        var main = new MainRoutine_OIR(data)
        {
            OutputDirectory  /**/= resultDir,
            OutputFileName   /**/= target,
            ComputeTimeMesh  /**/= new TimeMesh(computeTimeMeshPath),
            OutputTimeMesh   /**/= new TimeMesh(outputTimeMeshPath),
            CommitmentPeriod /**/= TimeMesh.CommitmentPeriodToSeconds("70years"),
        };

        main.Main(default);

        File.Delete(Path.Combine(resultDir, target + ".log"));

        CollectionAssert.AreEqual(
            File.ReadAllLines(Path.Combine(expectDir, target + "_Atoms.out")),
            File.ReadAllLines(Path.Combine(resultDir, target + "_Atoms.out")));
    }
}
