using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace S_Coefficient
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 計算対象となる核種群の名前を格納したファイルのパス
        /// </summary>
        private const string NuclideListFilePath = @"lib\NuclideList.txt";

        private void CalcStart_Click(object sender, EventArgs e)
        {
            // // review:男性/女性の選択肢はGUIで既に制約されている
            // Debug.Assert(AMbutton.Checked != AFbutton.Checked);
            // var sex = (AMbutton.Checked ? Sex.Male : Sex.Female);
            // 
            // var CalcS = new CalcScoeff();
            // 
            // // 1行＝計算対象の核種名としてファイルから読み出す。
            // CalcS.Nuclides.AddRange(File.ReadLines(NuclideListFilePath));
            // 
            // if (PCHIP.Checked == true)
            //     CalcS.InterpolationMethod = "PCHIP";
            // else if (Interpolation.Checked == true)
            //     CalcS.InterpolationMethod = "線形補間";
            // 
            // (string mes, string info) = CalcS.CalcS(sex);
            // MessageBox.Show(mes, info);
        }
    }
}
