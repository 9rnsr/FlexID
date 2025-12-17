namespace FlexID.Calc.Tests;

[TestClass]
public class TimeMeshTests
{
    private const long SecondsOfMinute = 60L;
    private const long SecondsOfHour = 60L * SecondsOfMinute;
    private const long SecondsOfDay = 24L * SecondsOfHour;
    private const long SecondsOfYear = 365L * SecondsOfDay;

    [TestMethod]
    [DataRow("123s",          /**/ 123L)]
    [DataRow("123sec",        /**/ 123L)]
    [DataRow("123secs",       /**/ 123L)]
    [DataRow("123second",     /**/ 123L)]
    [DataRow("123seconds",    /**/ 123L)]
    [DataRow(" 123 S ",       /**/ 123L)]
    [DataRow(" 123 sec ",     /**/ 123L)]
    [DataRow(" 123 Secs ",    /**/ 123L)]
    [DataRow(" 123 secOnd ",  /**/ 123L)]
    [DataRow(" 123 secondS ", /**/ 123L)]
    [DataRow("123m",          /**/ 123L * SecondsOfMinute)]
    [DataRow("123min",        /**/ 123L * SecondsOfMinute)]
    [DataRow("123mins",       /**/ 123L * SecondsOfMinute)]
    [DataRow("123minute",     /**/ 123L * SecondsOfMinute)]
    [DataRow("123minutes",    /**/ 123L * SecondsOfMinute)]
    [DataRow(" 123 M ",       /**/ 123L * SecondsOfMinute)]
    [DataRow(" 123 MIN ",     /**/ 123L * SecondsOfMinute)]
    [DataRow(" 123 mIns ",    /**/ 123L * SecondsOfMinute)]
    [DataRow(" 123 minUte ",  /**/ 123L * SecondsOfMinute)]
    [DataRow(" 123 minuteS ", /**/ 123L * SecondsOfMinute)]
    [DataRow("123h",          /**/ 123L * SecondsOfHour)]
    [DataRow("123hour",       /**/ 123L * SecondsOfHour)]
    [DataRow("123hours",      /**/ 123L * SecondsOfHour)]
    [DataRow(" 123 H ",       /**/ 123L * SecondsOfHour)]
    [DataRow(" 123 hOUr ",    /**/ 123L * SecondsOfHour)]
    [DataRow(" 123 Hours ",   /**/ 123L * SecondsOfHour)]
    [DataRow("123d",          /**/ 123L * SecondsOfDay)]
    [DataRow("123day",        /**/ 123L * SecondsOfDay)]
    [DataRow("123days",       /**/ 123L * SecondsOfDay)]
    [DataRow(" 123 D ",       /**/ 123L * SecondsOfDay)]
    [DataRow(" 123 daY ",     /**/ 123L * SecondsOfDay)]
    [DataRow(" 123 DAYS ",    /**/ 123L * SecondsOfDay)]
    [DataRow("123y",          /**/ 123L * SecondsOfYear)]
    [DataRow("123year",       /**/ 123L * SecondsOfYear)]
    [DataRow("123years",      /**/ 123L * SecondsOfYear)]
    [DataRow(" 123 y ",       /**/ 123L * SecondsOfYear)]
    [DataRow(" 123 Year ",    /**/ 123L * SecondsOfYear)]
    [DataRow(" 123 yeaRs ",   /**/ 123L * SecondsOfYear)]
    public void TestToSeconds(string time, long expect)
    {
        var actual = TimeMesh.ToSeconds(time);
        actual.ShouldBe(expect);
    }

    [TestMethod]
    [DataRow("123")]
    [DataRow("123ss")]
    [DataRow("123ms")]
    [DataRow("123hs")]
    [DataRow("123ds")]
    [DataRow("123ys")]
    [DataRow("abc")]
    [DataRow("seconds")]
    [DataRow("d")]
    [DataRow("hour")]
    [DataRow("years")]
    public void TestToSecondsError(string time)
    {
        new Action(() => TimeMesh.ToSeconds(time)).ShouldThrow<FormatException>();
    }

    [TestMethod]
    [DataRow("$(TestFiles)/TimeMesh/calc-time.dat")]
    [DataRow("$(TestFiles)/TimeMesh/out-time.dat")]
    [DataRow("lib/TimeMesh/time.dat")]
    [DataRow("lib/TimeMesh/out-time.dat")]
    [DataRow("lib/TimeMesh/out-per-h.dat")]
    [DataRow("lib/TimeMesh/out-time-OIR.dat")]
    public void ReadTimeMeshFile(string filePath)
    {
        filePath = TestFiles.ReplaceVar(filePath);
        var timeMesh = new TimeMesh(filePath);
    }

    [TestMethod]
    [DynamicData(nameof(CoverMeshes))]
    public void TestCover(TimeMesh coarseMesh, TimeMesh fineMesh)
    {
        // 細かいメッシュは粗いメッシュを網羅する。
        fineMesh.Cover(coarseMesh).ShouldBeTrue();

        // 粗いメッシュは細かいメッシュを網羅しない。
        coarseMesh.Cover(fineMesh).ShouldBeFalse();
    }

    public static IEnumerable<object[]> CoverMeshes()
    {
        yield return new object[]
        {
            // 全体の期間：365日
            new TimeMesh([new TimeMeshBoundary("365d", "5d"),]),
            new TimeMesh([new TimeMeshBoundary("365d", "1d"),]),
        };

        yield return new object[]
        {
            // 全体の期間：600日
            new TimeMesh(
            [
                new TimeMeshBoundary("100d", "1d"),
                new TimeMeshBoundary("200d", "5d"),
                new TimeMeshBoundary("300d", "10d"),
            ]),
            new TimeMesh(
            [
                new TimeMeshBoundary("600d", "1d"),
            ]),
        };

        yield return new object[]
        {
            // 全体の期間：75年
            new TimeMesh(
            [
                new TimeMeshBoundary(  "75 years",  "1 day"),
            ]),
            new TimeMesh(
            [
                new TimeMeshBoundary(   "30 mins",  "1 min"),
                new TimeMeshBoundary(   "50 days",  "3 mins"),
                new TimeMeshBoundary(  "100 days",  "5 mins"),
                new TimeMeshBoundary(  "250 days", "10 mins"),
                new TimeMeshBoundary(  "365 days", "30 mins"),
                new TimeMeshBoundary( "1000 days",  "1 hour"),
                new TimeMeshBoundary( "2000 days",  "4 hours"),
                new TimeMeshBoundary( "3650 days", "12 hours"),
                new TimeMeshBoundary("27375 days", "24 hours"),
            ])
        };

        yield return new object[]
        {
            new TimeMesh("lib/TimeMesh/out-time-OIR.dat"),
            new TimeMesh("lib/TimeMesh/time.dat"),
        };
    }

    [TestMethod]
    public void TestCover_LongerCoerseMesh()
    {
        var longerCoarseMesh = new TimeMesh(
        [
            new TimeMeshBoundary("10 days", "1 day"),
        ]);
        var shorterFineMesh = new TimeMesh(
        [
            new TimeMeshBoundary("8 days", "1 hour"),
        ]);

        // 細かいメッシュより長い期間を持つ粗いメッシュは網羅できない。
        shorterFineMesh.Cover(longerCoarseMesh).ShouldBeFalse();
    }

    [TestMethod]
    public void TestCover_LongerFineMesh()
    {
        var shorterCoarseMesh = new TimeMesh(
        [
            new TimeMeshBoundary("8 days", "1 day"),
        ]);
        var longerFineMesh = new TimeMesh(
        [
            new TimeMeshBoundary("10 days", "1 hour"),
        ]);

        // 細かいメッシュが、粗いメッシュより長い期間を網羅している。
        longerFineMesh.Cover(shorterCoarseMesh).ShouldBeTrue();
    }
}
