using System;
using System.Collections.Generic;
using Xunit;

namespace FlexID.Calc.Tests
{
    public class TimeMeshTests
    {
        private const long SecondsOfMinute = 60L;
        private const long SecondsOfHour = 60L * SecondsOfMinute;
        private const long SecondsOfDay = 24L * SecondsOfHour;
        private const long SecondsOfYear = 365L * SecondsOfDay;

        [Theory]
        [InlineData("123s",          /**/ 123L)]
        [InlineData("123sec",        /**/ 123L)]
        [InlineData("123secs",       /**/ 123L)]
        [InlineData("123second",     /**/ 123L)]
        [InlineData("123seconds",    /**/ 123L)]
        [InlineData(" 123 S ",       /**/ 123L)]
        [InlineData(" 123 sec ",     /**/ 123L)]
        [InlineData(" 123 Secs ",    /**/ 123L)]
        [InlineData(" 123 secOnd ",  /**/ 123L)]
        [InlineData(" 123 secondS ", /**/ 123L)]
        [InlineData("123m",          /**/ 123L * SecondsOfMinute)]
        [InlineData("123min",        /**/ 123L * SecondsOfMinute)]
        [InlineData("123mins",       /**/ 123L * SecondsOfMinute)]
        [InlineData("123minute",     /**/ 123L * SecondsOfMinute)]
        [InlineData("123minutes",    /**/ 123L * SecondsOfMinute)]
        [InlineData(" 123 M ",       /**/ 123L * SecondsOfMinute)]
        [InlineData(" 123 MIN ",     /**/ 123L * SecondsOfMinute)]
        [InlineData(" 123 mIns ",    /**/ 123L * SecondsOfMinute)]
        [InlineData(" 123 minUte ",  /**/ 123L * SecondsOfMinute)]
        [InlineData(" 123 minuteS ", /**/ 123L * SecondsOfMinute)]
        [InlineData("123h",          /**/ 123L * SecondsOfHour)]
        [InlineData("123hour",       /**/ 123L * SecondsOfHour)]
        [InlineData("123hours",      /**/ 123L * SecondsOfHour)]
        [InlineData(" 123 H ",       /**/ 123L * SecondsOfHour)]
        [InlineData(" 123 hOUr ",    /**/ 123L * SecondsOfHour)]
        [InlineData(" 123 Hours ",   /**/ 123L * SecondsOfHour)]
        [InlineData("123d",          /**/ 123L * SecondsOfDay)]
        [InlineData("123day",        /**/ 123L * SecondsOfDay)]
        [InlineData("123days",       /**/ 123L * SecondsOfDay)]
        [InlineData(" 123 D ",       /**/ 123L * SecondsOfDay)]
        [InlineData(" 123 daY ",     /**/ 123L * SecondsOfDay)]
        [InlineData(" 123 DAYS ",    /**/ 123L * SecondsOfDay)]
        [InlineData("123y",          /**/ 123L * SecondsOfYear)]
        [InlineData("123year",       /**/ 123L * SecondsOfYear)]
        [InlineData("123years",      /**/ 123L * SecondsOfYear)]
        [InlineData(" 123 y ",       /**/ 123L * SecondsOfYear)]
        [InlineData(" 123 Year ",    /**/ 123L * SecondsOfYear)]
        [InlineData(" 123 yeaRs ",   /**/ 123L * SecondsOfYear)]
        public void TestToSeconds(string time, long expect)
        {
            var actual = TimeMesh.ToSeconds(time);
            Assert.Equal(expect, actual);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123ss")]
        [InlineData("123ms")]
        [InlineData("123hs")]
        [InlineData("123ds")]
        [InlineData("123ys")]
        [InlineData("abc")]
        [InlineData("seconds")]
        [InlineData("d")]
        [InlineData("hour")]
        [InlineData("years")]
        public void TestToSecondsError(string time)
        {
            Assert.Throws<FormatException>(() => TimeMesh.ToSeconds(time));
        }

        [Theory]
        [InlineData("$(TestFiles)/TimeMesh/calc-time.dat")]
        [InlineData("$(TestFiles)/TimeMesh/out-time.dat")]
        [InlineData("lib/TimeMesh/time.dat")]
        [InlineData("lib/TimeMesh/out-time.dat")]
        [InlineData("lib/TimeMesh/out-per-h.dat")]
        [InlineData("lib/TimeMesh/out-time-OIR.dat")]
        public void ReadTimeMeshFile(string filePath)
        {
            filePath = TestFiles.ReplaceVar(filePath);
            var timeMesh = new TimeMesh(filePath);
        }

        [Theory]
        [MemberData(nameof(CoverMeshes))]
        public void TestCover(TimeMesh coarseMesh, TimeMesh fineMesh)
        {
            // 細かいメッシュは粗いメッシュを網羅する。
            Assert.True(fineMesh.Cover(coarseMesh));

            // 粗いメッシュは細かいメッシュを網羅しない。
            Assert.False(coarseMesh.Cover(fineMesh));
        }

        public static IEnumerable<object[]> CoverMeshes()
        {
            yield return new object[]
            {
                // 全体の期間：365日
                new TimeMesh(new[] { new TimeMeshBoundary("365d", "5d"), }),
                new TimeMesh(new[] { new TimeMeshBoundary("365d", "1d"), }),
            };

            yield return new object[]
            {
                // 全体の期間：600日
                new TimeMesh(new[] { new TimeMeshBoundary("100d", "1d"),
                                     new TimeMeshBoundary("200d", "5d"),
                                     new TimeMeshBoundary("300d", "10d"),}),
                new TimeMesh(new[] { new TimeMeshBoundary("600d", "1d"), }),
            };

            yield return new object[]
            {
                // 全体の期間：75年
                new TimeMesh(new[]
                {
                    new TimeMeshBoundary(  "75 years",  "1 day"),
                }),
                new TimeMesh(new[]
                {
                    new TimeMeshBoundary(   "30 mins",  "1 min"),
                    new TimeMeshBoundary(   "50 days",  "3 mins"),
                    new TimeMeshBoundary(  "100 days",  "5 mins"),
                    new TimeMeshBoundary(  "250 days", "10 mins"),
                    new TimeMeshBoundary(  "365 days", "30 mins"),
                    new TimeMeshBoundary( "1000 days",  "1 hour"),
                    new TimeMeshBoundary( "2000 days",  "4 hours"),
                    new TimeMeshBoundary( "3650 days", "12 hours"),
                    new TimeMeshBoundary("27375 days", "24 hours"),
                })
            };

            yield return new object[]
            {
                new TimeMesh("lib/TimeMesh/out-time-OIR.dat"),
                new TimeMesh("lib/TimeMesh/time.dat"),
            };
        }

        [Fact]
        public void TestCover_LongerCoerseMesh()
        {
            var longerCoarseMesh = new TimeMesh(new[]
            {
                new TimeMeshBoundary("10 days", "1 day"),
            });
            var shorterFineMesh = new TimeMesh(new[]
            {
                new TimeMeshBoundary("8 days", "1 hour"),
            });

            // 細かいメッシュより長い期間を持つ粗いメッシュは網羅できない。
            Assert.False(shorterFineMesh.Cover(longerCoarseMesh));
        }

        [Fact]
        public void TestCover_LongerFineMesh()
        {
            var shorterCoarseMesh = new TimeMesh(new[]
            {
                new TimeMeshBoundary("8 days", "1 day"),
            });
            var longerFineMesh = new TimeMesh(new[]
            {
                new TimeMeshBoundary("10 days", "1 hour"),
            });

            // 細かいメッシュが、粗いメッシュより長い期間を網羅している。
            Assert.True(longerFineMesh.Cover(shorterCoarseMesh));
        }
    }
}
