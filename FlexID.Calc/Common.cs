namespace FlexID.Calc
{
    /// <summary>
    /// あるコンパートメントにおける、計算時間メッシュ内の放射能を保持する。
    /// </summary>
    public struct OrganActivity
    {
        /// <summary>
        /// 計算時間メッシュにおける初期放射能。
        /// </summary>
        public double ini;

        /// <summary>
        /// 計算時間メッシュにおける平均放射能。
        /// </summary>
        public double ave;

        /// <summary>
        /// 計算時間メッシュにおける末期放射能。
        /// </summary>
        public double end;

        /// <summary>
        /// 計算時間メッシュにおける積算放射能。
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

        // 1つ前の時間メッシュにおける、摂取時からの積算放射能
        public double[] IntakeQuantityPre;

        // 1つ前の時間メッシュにおける、摂取時からの積算放射能
        public double[] IntakeQuantityNow;

        public double[] Excreta;
        public double[] PreExcreta;

        /// <summary>
        /// 処理中の時間メッシュを次に進める
        /// </summary>
        /// <param name="data"></param>
        public void NextTime(DataClass data)
        {
            Swap(ref CalcPre, ref CalcNow);

            Swap(ref IntakeQuantityPre, ref IntakeQuantityNow);

            foreach (var o in data.Organs)
            {
                CalcNow[o.Index].ini = 0;
                CalcNow[o.Index].ave = 0;
                CalcNow[o.Index].end = 0;
                CalcNow[o.Index].total = 0;
                IntakeQuantityNow[o.Index] = 0;
            }
        }

        /// <summary>
        /// 処理中の収束計算回を次に進める
        /// </summary>
        /// <param name="data"></param>
        public void NextIter(DataClass data)
        {
            Swap(ref IterPre, ref IterNow);

            foreach (var o in data.Organs)
            {
                IterNow[o.Index].ini = 0;
                IterNow[o.Index].ave = 0;
                IterNow[o.Index].end = 0;
                IterNow[o.Index].total = 0;
                PreExcreta[o.Index] = 0;
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
