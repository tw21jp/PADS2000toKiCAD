using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private List<string> ExtractPartNumbers(string inputText)
        {
            List<string> partNumbers = new List<string>();

            // カンマで分割し、3つ目以降の要素をパーツ番号として抽出
            string[] text = inputText.Split('\n');
            foreach (string e in text)
            {
                string[] parts = e.Split(',');
                for (int i = 2; i < parts.Length; i++)
                {
                    string part = parts[i].Trim();
                    // パーツ番号を正規表現で抽出
                    Match match = Regex.Match(part, @"\w+\d+");
                    if (match.Success)
                    {
                        partNumbers.Add(match.Value);
                    }
                }
            }

            return partNumbers;
        }



        private void button2_Click(object sender, EventArgs e)
        {
            string inputText = richTextBox3.Text;
            List<string> partNumbers = ExtractPartNumbers(inputText);

            // リストボックスにパーツ番号を表示
            PartsListBox.Items.Clear();
            PartsListBox.Items.AddRange(partNumbers.ToArray());
            //出力用データ
            StringBuilder kicadNetlist = new StringBuilder();
            //タイトル　日付は適当
            kicadNetlist.AppendLine("( { EESchema Netlist Version 1.1 created 2023/10/03 11:10:05 }");
            //パーツごとにループR1,R2・・・・
            foreach (string f in partNumbers)
            {
                //パーツのネットを作成。部品と値は空に設定
                kicadNetlist.AppendLine($" ( /00000000-0000-0000-0000-000000000000 parts  {f} 0");

                //ネットリストを読み込み
                using (StringReader reader = new StringReader(richTextBox1.Text))
                {
                    string line;
                    string currentSignal = null;
                    //ネットリスト一行読み
                    while ((line = reader.ReadLine()) != null)
                    {
                        //シグナルがあればシグナル名取得
                        if (line.StartsWith("*SIGNAL*"))
                        {
                            currentSignal = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                        }
                        //それ以外はパーツ検索
                        else if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] components = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (components.Length > 0 )
                            {
                                foreach (string component in components)
                                {
                                    //パーツが一致すれば、ネットリストに登録
                                    string[] parts = component.Split('.');
                                    if (parts[0] == f)
                                    {
                                        kicadNetlist.AppendLine($"  (    {parts[1]} {currentSignal} )");
                                    }
                                }

                            }
                        }
                    }
                }
                kicadNetlist.AppendLine(" )");
            }

            richTextBox2.Text = kicadNetlist.ToString();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
