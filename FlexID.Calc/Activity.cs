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
            // 最新の計算結果を格納したCalcNowを、
            // 前回計算時間メッシュの結果であるCalcPreへ移動する。
            Swap(ref CalcNow, ref CalcPre);

            // 収束計算における各コンパートメントの初期値としてゼロを設定する。
            foreach (var o in data.Organs)
            {
                IterPre[o.Index].ini = 0;
                IterPre[o.Index].ave = 0;
                IterPre[o.Index].end = 0;
                IterPre[o.Index].total = 0;
            }
        }

        /// <summary>
        /// 次の収束計算回のための準備を行う。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="convergence"></param>
        /// <returns>次の収束計算回へ継続する場合は <see langword="true"/>、
        /// 結果が収束し次の計算回が必要ない場合は <see langword="false"/>。</returns>
        public bool NextIter(InputData data, double convergence)
        {
            var next = false;

            foreach (var o in data.Organs)
            {
                ref var iterNow = ref IterNow[o.Index];
                ref var iterPre = ref IterPre[o.Index];

                // 収束計算の初回はIterPreがゼロのため、
                // IterNowがゼロにならない限り常に収束未達と判定される。
                var s1 = iterNow.ini != 0 ? Math.Abs((iterNow.ini - iterPre.ini) / iterNow.ini) : 0;
                var s2 = iterNow.ave != 0 ? Math.Abs((iterNow.ave - iterPre.ave) / iterNow.ave) : 0;
                var s3 = iterNow.end != 0 ? Math.Abs((iterNow.end - iterPre.end) / iterNow.end) : 0;

                if (s1 > convergence || s2 > convergence || s3 > convergence)
                {
                    next = true;
                    break;
                }
            }

            // IterNowに格納された今回収束計算の結果を、前回収束計算の結果として
            // IterPreが指すよう移動する。
            Swap(ref IterNow, ref IterPre);

            return next;
        }

        /// <summary>
        /// 収束計算が完了した時点の処理を行う。
        /// </summary>
        public void FinishIter()
        {
            // 最後の収束計算回の結果(IterNowからIterPreへ移動済み)を
            // CalcNowに移動する。
            Swap(ref IterPre, ref CalcNow);
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
