using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FlexID.Calc
{
    public struct TimeMeshBoundary
    {
        public long EndOfPeriod { get; }

        public long Step { get; }

        public TimeMeshBoundary(long end, long step)
        {
            EndOfPeriod = end;
            Step = step;
        }

        public TimeMeshBoundary(string end, string step)
        {
            EndOfPeriod = TimeMesh.ToSeconds(end);
            Step = TimeMesh.ToSeconds(step);
        }
    }

    /// <summary>
    /// 時間メッシュを表現する。
    /// </summary>
    public class TimeMesh
    {
        private static Regex patternPeriod =
            new Regex(@"^ *(?<num>\d+) *(?<unit>days|months|years) *$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 預託期間を秒数に換算する。
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static long CommitmentPeriodToSeconds(string period)
        {
            var m = patternPeriod.Match(period);
            if (m.Success)
            {
                var num = long.Parse(m.Groups["num"].Value);
                var unit = m.Groups["unit"].Value.ToLowerInvariant();
                var days = unit == "days" ? num :
                           unit == "months" ? num * 31 :
                           unit == "years" ? num * 365 :
                           throw Program.Error("Please enter the period ('days', 'months', 'years').");
                return days * 24 * 60 * 60;
            }
            else
            {
                throw Program.Error("Please enter integer for the Commitment Period.");
            }
        }

        private static Regex patternTime = new Regex(
            @"^ *(?<num>\d+) *(?<unit>s(?:ec(?:ond)?s?)?|m(?:in(?:ute)?s?)?|h(?:ours?)?|d(?:ays?)?|y(?:ears?)?) *$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 時刻あるいは期間を表現する文字列を秒数に変換する。
        /// </summary>
        /// <param name="time">時刻あるいは期間を表現する文字列。</param>
        /// <returns></returns>
        /// <exception cref="FormatException">文字列の表現が正しくない場合。</exception>
        public static long ToSeconds(string time)
        {
            var m = patternTime.Match(time);
            if (!m.Success)
                throw new FormatException();
            var timeValue = long.Parse(m.Groups["num"].Value);
            var unit = char.ToLowerInvariant(m.Groups["unit"].Value[0]);
            var multiplier =
                unit == 's' ? 1 :
                unit == 'm' ? 60 :
                unit == 'h' ? 60 * 60 :
                unit == 'd' ? 60 * 60 * 24 :
                unit == 'y' ? 60 * 60 * 24 * 365 : throw new NotSupportedException("unreachable");
            return timeValue * multiplier;
        }

        /// <summary>
        /// 日数(整数値)を秒数に換算する。
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static long DaysToSeconds(int days)
        {
            const long factor = 24 * 60 * 60;
            return days * factor;
        }

        /// <summary>
        /// 秒数を日数(実数値)に換算する。
        /// </summary>
        /// <param name="outT"></param>
        /// <returns></returns>
        public static double SecondsToDays(long outT)
        {
            const double factor = 24 * 60 * 60;
            return outT / factor;
        }

        private readonly TimeMeshBoundary[] boundaries;

        /// <summary>
        /// ファイルから時間メッシュを読み込む。
        /// </summary>
        /// <param name="file">時間メッシュファイルのパス文字列。</param>
        /// <exception cref="FormatException"></exception>
        public TimeMesh(string file)
        {
            var boundaries = new List<TimeMeshBoundary>();

            using (var reader = new StreamReader(file))
            {
                string ReadLine()
                {
                    while (true)
                    {
                        var ln = reader.ReadLine();
                        if (ln is null)
                            return ln;

                        // コメント行や行末コメントを除去する
                        var icomment = ln.IndexOf('#');
                        if (icomment != -1)
                            ln = ln.Substring(0, icomment);
                        ln = ln.Trim();
                        if (ln.Length != 0)
                            return ln;
                    }
                }

                // ヘッダ行を読み飛ばす
                ReadLine();

                var prevEnd = 0L;
                var prevEndStr = "";
                while (true)
                {
                    var ln = ReadLine();
                    if (ln is null)
                        break;

                    var columns = ln.Split(',');
                    if (columns.Length != 2)
                        throw new FormatException("Two columns required.");
                    var currEnd = ToSeconds(columns[0]);
                    var currStep = ToSeconds(columns[1]);
                    if (currEnd <= 0 || currStep <= 0)
                        throw new FormatException("Mesh period and step should be positive value.");

                    var interval = currEnd - prevEnd;
                    if (interval < currStep)
                        throw new FormatException("Mesh interval is less than step value.");
                    if (interval % currStep != 0)
                        throw new FormatException($"Mesh interval ({columns[0]} - {prevEndStr}) should be equal to multiple of step value {columns[1]}.");

                    boundaries.Add(new TimeMeshBoundary(currEnd, currStep));
                    prevEnd = currEnd;
                    prevEndStr = columns[0];
                }
                if (boundaries.Count < 1)
                    throw new FormatException("At least one mesh boundary is required.");
            }

            this.boundaries = boundaries.ToArray();
        }

        public TimeMesh(IEnumerable<TimeMeshBoundary> boundaries)
        {
            var start = 0L;
            foreach (var b in boundaries)
            {
                if (b.EndOfPeriod <= 0 || b.EndOfPeriod <= start)
                    throw new FormatException();
                if (b.Step <= 0)
                    throw new FormatException();
                var interval = b.EndOfPeriod - start;
                if (interval % b.Step != 0)
                    throw new FormatException();

                start = b.EndOfPeriod;
            }

            this.boundaries = boundaries.ToArray();
        }

        /// <summary>
        /// この時間メッシュが、対象の時間メッシュを網羅しているかを判定する。
        /// ここでいう「網羅」とは、対象の時間メッシュに含まれる全ての時刻が、この時間メッシュにも
        /// 含まれていること、言い換えると、対象の時間メッシュはこの時間メッシュより「粗い」ことを
        /// 意味する。
        /// </summary>
        /// <param name="coarse">判定対象の時間メッシュ。</param>
        /// <returns>網羅している場合は<c>true</c>を、そうでない場合は<c>false</c>を返す。</returns>
        public bool Cover(TimeMesh coarse)
        {
            // 粗メッシュ側の情報
            var coarseIndex = 0;
            var coarseBoundary = coarse.boundaries[coarseIndex];
            var coarseNextTime = coarseBoundary.Step;

            // 細メッシュ側の情報
            var fineIndex = 0;
            var fineStartTime = 0L;
            var fineBoundary = this.boundaries[fineIndex];
            while (true)
            {
                if (fineBoundary.EndOfPeriod < coarseNextTime)
                {
                    // 注目している細メッシュの区間がcoarseNextTimeに
                    // 掛かっていないため、これを次の位置に進める。
                    ++fineIndex;
                    if (fineIndex >= this.boundaries.Length)
                    {
                        // 細メッシュが租メッシュを網羅し尽くせなかった。
                        return false;
                    }
                    fineStartTime = fineBoundary.EndOfPeriod;
                    fineBoundary = this.boundaries[fineIndex];

                    continue;
                }
                Debug.Assert(coarseNextTime <= fineBoundary.EndOfPeriod);

                if ((coarseNextTime - fineStartTime) % fineBoundary.Step != 0)
                    return false;

                if (coarseNextTime == coarseBoundary.EndOfPeriod)
                {
                    // 網羅された粗メッシュ位置が注目している粗メッシュ区間の
                    // 末尾であるため、これを次の位置に進める。
                    ++coarseIndex;
                    if (coarseIndex >= coarse.boundaries.Length)
                    {
                        // 細メッシュが粗メッシュを網羅し尽くした。
                        return true;
                    }
                    coarseBoundary = coarse.boundaries[coarseIndex];
                }

                // 次に網羅されるべき粗メッシュ位置を得る。
                coarseNextTime += coarseBoundary.Step;
            }
        }

        /// <summary>
        /// 時間メッシュの各時刻を列挙するオブジェクトを取得する。
        /// </summary>
        /// <returns>列挙オブジェクト。</returns>
        public IEnumerator<long> Start()
        {
            IEnumerable<long> Enumerate()
            {
                var time = 0L;
                yield return time;

                foreach (var b in boundaries)
                {
                    var currEnd = b.EndOfPeriod;
                    var currStep = b.Step;

                    while (time != currEnd)
                    {
                        time += currStep;
                        yield return time;
                    }
                }
            }

            var enumerator = Enumerate().GetEnumerator();

            // enumerator.Currentが初期時刻0を返すよう位置を進める。
            enumerator.MoveNext();

            return enumerator;
        }
    }
}
