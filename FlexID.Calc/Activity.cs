namespace FlexID.Calc
{
    /// <summary>
    /// あるコンパートメントにおける、計算時間メッシュ内の放射能を保持する。
    /// </summary>
    public struct OrganActivity
    {
        /// <summary>
        /// 計算時間メッシュにおける初期放射能[Bq/day]。
        /// </summary>
        public double ini;

        /// <summary>
        /// 計算時間メッシュにおける平均放射能[Bq/day]。
        /// </summary>
        public double ave;

        /// <summary>
        /// 計算時間メッシュにおける末期放射能[Bq/day]。
        /// </summary>
        public double end;

        /// <summary>
        /// 計算時間メッシュにおける積算放射能[Bq]。
        /// </summary>
        public double total;
    }

    public class Activity
    {
        /// <summary>
        /// 前回の収束計算回における、コンパートメント毎の放射能。
        /// </summary>
        public OrganActivity[] IterPre;

        /// <summary>
        /// 今回の収束計算回における、コンパートメント毎の放射能。
        /// </summary>
        public OrganActivity[] IterNow;

        /// <summary>
        /// 前回の計算時間メッシュにおける、コンパートメント毎の放射能。
        /// </summary>
        public OrganActivity[] CalcPre;

        /// <summary>
        /// 今回の計算時間メッシュにおける、コンパートメント毎の放射能。
        /// </summary>
        public OrganActivity[] CalcNow;

        /// <summary>
        /// 今回の出力時間メッシュにおける、コンパートメント毎の放射能。
        /// </summary>
        public OrganActivity[] OutNow;

        /// <summary>
        /// 摂取時からの、コンパートメント毎の積算放射能[Bq]。
        /// </summary>
        public double[] OutTotalFromIntake;

        /// <summary>
        /// 次の計算時間メッシュのための準備を行う。
        /// </summary>
        /// <param name="data"></param>
        public void NextCalc(InputData data)
        {
            Swap(ref CalcPre, ref CalcNow);

            foreach (var o in data.Organs)
            {
                CalcNow[o.Index].ini = 0;
                CalcNow[o.Index].ave = 0;
                CalcNow[o.Index].end = 0;
                CalcNow[o.Index].total = 0;
            }
        }

        /// <summary>
        /// 次の収束計算回のための準備を行う。
        /// </summary>
        /// <param name="data"></param>
        public void NextIter(InputData data)
        {
            Swap(ref IterPre, ref IterNow);

            foreach (var o in data.Organs)
            {
                IterNow[o.Index].ini = 0;
                IterNow[o.Index].ave = 0;
                IterNow[o.Index].end = 0;
                IterNow[o.Index].total = 0;
            }
        }

        /// <summary>
        /// 次の出力時間メッシュのための準備を行う。
        /// </summary>
        /// <param name="data"></param>
        public void NextOut(InputData data)
        {
            foreach (var o in data.Organs)
            {
                // 前回の末期放射能を今回の初期放射能とする。
                OutNow[o.Index].ini = OutNow[o.Index].end;
                OutNow[o.Index].ave = 0;
                OutNow[o.Index].end = 0;
                OutNow[o.Index].total = 0;
            }
        }

        private static void Swap<T>(ref T[] array1, ref T[] array2)
        {
            var tmp = array1;
            array1 = array2;
            array2 = tmp;
        }
    }
}
