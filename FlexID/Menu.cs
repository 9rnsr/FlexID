using FlexID.Calc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlexID
{
    public partial class Menu : Form
    {
        // インプットファイルパス
        private Dictionary<string, string> inpFile = new Dictionary<string, string>();

        public Menu()
        {
            InitializeComponent();

            comboBox_Commitment.SelectedItem = "years";
            combo_Commitment_EIR.SelectedItem = "years";
            string[] Nuclide = Directory.GetFileSystemEntries(@"inp\OIR");
            foreach (var x in Nuclide)
            {
                var nuc = x.Replace(@"inp\OIR\", "");
                comboBox_Nuclide.Items.Add(nuc);
                //Radionuclide_EIR.Items.Add(nuc);
            }

            Nuclide = Directory.GetFileSystemEntries(@"inp\EIR");
            foreach (var x in Nuclide)
            {
                var nuc = x.Replace(@"inp\EIR\", "");
                //comboBox_Nuclide.Items.Add(nuc);
                Radionuclide_EIR.Items.Add(nuc);
            }
        }

        /// <summary>
        /// inpフォルダから各元素の経路取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_Nuclide_SelectedIndexChanged(object sender, EventArgs e)
        {
            var nuc = comboBox_Nuclide.SelectedItem.ToString();
            comboBox_Route.Items.Clear();

            // 被ばく経路設定
            inpFile = new Dictionary<string, string>();

            var nuclideDir = @"inp\OIR\" + nuc;
            var inputFiles = Directory.GetFileSystemEntries(nuclideDir, @"*.inp");

            var anyProgeny = false;
            foreach (var inputFile in inputFiles)
            {
                var inputLines = File.ReadLines(inputFile).GetEnumerator();

                string GetNextLine()
                {
                Lagain:
                    if (!inputLines.MoveNext())
                        return null;
                    var ln = inputLines.Current.Trim();

                    // 空行を読み飛ばす。
                    if (ln.Length == 0)
                        goto Lagain;

                    // コメント行を読み飛ばす。
                    if (ln.StartsWith("#"))
                        goto Lagain;

                    // 行末コメントを除去する。
                    var trailingComment = ln.IndexOf("#");
                    if (trailingComment != -1)
                        ln = ln.Substring(0, trailingComment).TrimEnd();
                    return ln;
                }

                var line = GetNextLine();
                var parts = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var type = parts[1];

                // 子孫核種の有無を判断する。
                var hasProgeny = false;
                while ((line = GetNextLine()) != null)
                {
                    if (line.Trim().StartsWith("cont"))
                    {
                        hasProgeny = true;
                        break;
                    }
                }

                comboBox_Route.Items.Add(type);
                inpFile.Add(type, inputFile);

                if (hasProgeny)
                    anyProgeny = true;
            }

            if (anyProgeny)
            {
                radioButton_Yes.Enabled = true;
                radioButton_Yes.Checked = true;
            }
            else
            {
                radioButton_Yes.Enabled = false;
                radioButton_No.Checked = true;
            }
        }

        private void Radionuclide_EIR_SelectedIndexChanged(object sender, EventArgs e)
        {
            var nuc = Radionuclide_EIR.SelectedItem.ToString();
            Route_EIR.Items.Clear();

            // 被ばく経路設定
            inpFile = new Dictionary<string, string>();

            var nuclideDir = @"inp\EIR\" + nuc;
            var inputFiles = Directory.GetFileSystemEntries(nuclideDir, @"*.inp");

            var anyProgeny = false;
            foreach (var inputFile in inputFiles)
            {
                var inputLines = File.ReadLines(inputFile).GetEnumerator();

                string GetNextLine()
                {
                Lagain:
                    if (!inputLines.MoveNext())
                        return null;
                    var ln = inputLines.Current.Trim();

                    // 空行を読み飛ばす。
                    if (ln.Length == 0)
                        goto Lagain;

                    // コメント行を読み飛ばす。
                    if (ln.StartsWith("#"))
                        goto Lagain;

                    // 行末コメントを除去する。
                    var trailingComment = ln.IndexOf("#");
                    if (trailingComment != -1)
                        ln = ln.Substring(0, trailingComment).TrimEnd();
                    return ln;
                }

                var line = GetNextLine();
                var parts = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var type = parts[1];

                // 子孫核種の有無を判断する。
                var hasProgeny = false;
                while ((line = GetNextLine()) != null)
                {
                    if (line.Trim().StartsWith("cont"))
                    {
                        hasProgeny = true;
                        break;
                    }
                }

                Route_EIR.Items.Add(type);
                inpFile.Add(type, inputFile);

                if (hasProgeny)
                    anyProgeny = true;
            }

            if (anyProgeny)
            {
                Progeny_EIR.Enabled = true;
                Progeny_EIR.Checked = true;
            }
            else
            {
                Progeny_EIR.Enabled = false;
                radioButton2.Checked = true;
            }
        }


        /// <summary>
        /// 計算開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Start_Click(object sender, EventArgs e)
        {
            button_Start.Enabled = false;
            Button_start_EIR.Enabled = false;
            var main = new MainRoutine_OIR();
            try
            {
                bool check = CheckParam();
                if (check)
                {
                    main.OutputPath = textBox_OutPath.Text;
                    main.InputPath = inpFile[comboBox_Route.SelectedItem.ToString()];
                    main.CalcTimeMeshPath = textBox_CalcMesh.Text;
                    main.OutTimeMeshPath = textBox_OutMesh.Text;
                    main.CommitmentPeriod = textBox_Commitment.Text + comboBox_Commitment.SelectedItem;
                    main.CalcProgeny = radioButton_Yes.Checked;

                    await Task.Run(() => main.Main());

                    // ファイルパスを引数にして出力GUI実行
                    Process p = Process.Start("FlexID.Viewer.exe", main.OutputPath);
                    p.WaitForExit();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            finally
            {
                button_Start.Enabled = true;
                Button_start_EIR.Enabled = true;
            }
        }

        /// <summary>
        /// 各パラメータの入力確認
        /// </summary>
        /// <returns></returns>
        private bool CheckParam()
        {
            if (textBox_OutPath.Text == "")
            {
                MessageBox.Show("Please enter the Output File Path.");
                return false;
            }
            if (comboBox_Nuclide.SelectedItem == null)
            {
                MessageBox.Show("Please select Nuclide.");
                return false;
            }
            if (comboBox_Route.SelectedItem == null)
            {
                MessageBox.Show("Please select Route of Intake.");
                return false;
            }
            if (textBox_CalcMesh.Text == "")
            {
                MessageBox.Show("Please enter the Calculation Time Mesh file path.");
                return false;
            }
            if (textBox_OutMesh.Text == "")
            {
                MessageBox.Show("Please enter the Output Time Mesh file path.");
                return false;
            }
            if (textBox_Commitment.Text == "")
            {
                MessageBox.Show("Please enter Commitment Period.");
                return false;
            }
            if (comboBox_Commitment.SelectedItem == null)
            {
                MessageBox.Show("Please select Commitment Period.");
                return false;
            }

            return true;
        }

        private async void Button_start_EIR_Click(object sender, EventArgs e)
        {
            button_Start.Enabled = false;
            Button_start_EIR.Enabled = false;
            var main = new MainRoutine_EIR();
            try
            {
                bool check = CheckParam_EIR();
                if (check)
                {
                    main.OutputPath = OutputPath_EIR.Text;
                    main.InputPath = inpFile[Route_EIR.SelectedItem.ToString()];
                    main.CalcTimeMeshPath = CalcMesh_EIR.Text;
                    main.OutTimeMeshPath = OutMesh_EIR.Text;
                    main.CommitmentPeriod = text_Commitment_EIR.Text + combo_Commitment_EIR.SelectedItem;
                    main.CalcProgeny = Progeny_EIR.Checked;
                    main.ExposureAge = Age_EIR.Text;

                    await Task.Run(() => main.Main());

                    // ファイルパスを引数にして出力GUI実行
                    Process p = Process.Start("FlexID.Viewer.exe", main.OutputPath);
                    p.WaitForExit();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            finally
            {
                button_Start.Enabled = true;
                Button_start_EIR.Enabled = true;
            }
        }

        private bool CheckParam_EIR()
        {
            if (OutputPath_EIR.Text == "")
            {
                MessageBox.Show("Please enter the Output File Path.");
                return false;
            }
            if (Radionuclide_EIR.SelectedItem == null)
            {
                MessageBox.Show("Please select Nuclide.");
                return false;
            }
            if (Route_EIR.SelectedItem == null)
            {
                MessageBox.Show("Please select Route of Intake.");
                return false;
            }
            if (CalcMesh_EIR.Text == "")
            {
                MessageBox.Show("Please enter the Calculation Time Mesh file path.");
                return false;
            }
            if (OutMesh_EIR.Text == "")
            {
                MessageBox.Show("Please enter the Output Time Mesh file path.");
                return false;
            }
            if (text_Commitment_EIR.Text == "")
            {
                MessageBox.Show("Please enter Commitment Period.");
                return false;
            }
            if (combo_Commitment_EIR.SelectedItem == null)
            {
                MessageBox.Show("Please select Commitment Period.");
                return false;
            }
            if (Age_EIR.SelectedItem == null)
            {
                MessageBox.Show("Please select Exposure Age.");
                return false;
            }

            return true;
        }

        #region TextBoxのフォルダ参照処理
        private void Button_Output_Click(object sender, EventArgs e)
        {
            using (var openFileDialog1 = new SaveFileDialog())
            {
                openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBox_OutPath.Text = openFileDialog1.FileName;
                }
            }
        }

        private void Button_CalcMesh_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_CalcMesh.Text = openFileDialog1.FileName;
            }
        }

        private void Button_OutMesh_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_OutMesh.Text = openFileDialog1.FileName;
            }
        }

        private void Btn_OutPath_EIR_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                OutputPath_EIR.Text = openFileDialog1.FileName;
            }
        }

        private void Btn_CalcMesh_EIR_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CalcMesh_EIR.Text = openFileDialog1.FileName;
            }
        }
        private void Btn_OutMesh_EIR_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                OutMesh_EIR.Text = openFileDialog1.FileName;
            }
        }
        #endregion

        #region TextBoxにドラッグ&ドロップで入力できるようにする処理
        private void OutputDragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            textBox_OutPath.Text += files[0];
        }

        private void CalcMeshDragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            textBox_CalcMesh.Text += files[0];
        }

        private void OutMeshDragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            textBox_OutMesh.Text += files[0];
        }

        private void OutputDragDrop_EIR(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            OutputPath_EIR.Text += files[0];
        }

        private void CalcMeshDragDrop_EIR(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            CalcMesh_EIR.Text += files[0];
        }

        private void OutMeshDragDrop_EIR(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            OutMesh_EIR.Text += files[0];
        }

        private void MenuDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }
        #endregion
    }
}
