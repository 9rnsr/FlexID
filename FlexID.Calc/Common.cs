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
        // 1つ前の時間メッシュにおける、臓器毎の計算結果
        public OrganActivity[] Pre;

        // 1つ前の時間メッシュにおける、臓器毎の計算結果
        public OrganActivity[] Now;

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
            Swap(ref Pre, ref Now);

            Swap(ref IntakeQuantityPre, ref IntakeQuantityNow);

            foreach (var o in data.Organs)
            {
                Now[o.Index].ini = 0;
                Now[o.Index].ave = 0;
                Now[o.Index].end = 0;
                Now[o.Index].total = 0;
                IntakeQuantityNow[o.Index] = 0;
            }
        }

        // 1つ前の時間メッシュにおける、初期・平均・末期・時間メッシュ内の積算放射能
        public OrganActivity[] rPre;

        // 処理中の時間メッシュにおける、初期・平均・末期・時間メッシュ内の積算放射能
        public OrganActivity[] rNow;

        /// <summary>
        /// 処理中の収束計算回を次に進める
        /// </summary>
        /// <param name="data"></param>
        public void NextIter(DataClass data)
        {
            Swap(ref rPre, ref rNow);

            foreach (var o in data.Organs)
            {
                rNow[o.Index].ini = 0;
                rNow[o.Index].ave = 0;
                rNow[o.Index].end = 0;
                rNow[o.Index].total = 0;
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
