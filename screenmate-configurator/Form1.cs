using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenMateConfigurator
{
	public partial class Form1 : Form
	{

		private Dictionary<int, string> folderPaths;
		public Form1()
		{
			// TODO: path-ok betöltése
            if (folderPaths == null)
            {
				folderPaths = new Dictionary<int, string>();
            }
			InitializeComponent();
		}

        private void button1_Click(object sender, EventArgs e)
        {
			DialogResult result = folderBrowserDialog1.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (folderPaths.ContainsKey(comboBox1.SelectedIndex))
                {
					folderPaths[comboBox1.SelectedIndex] = folderBrowserDialog1.SelectedPath;
				}
				else
                {
					folderPaths.Add(comboBox1.SelectedIndex, folderBrowserDialog1.SelectedPath);
                }
				textBox1.Text = folderBrowserDialog1.SelectedPath;
			}
		}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
			textBox1.Text = folderPaths.ContainsKey(comboBox1.SelectedIndex) ? folderPaths[comboBox1.SelectedIndex] : "";
		}

        private void Form1_Load(object sender, EventArgs e)
        {
			// TODO: értékek betöltése
			numericUpDown1.Value = 100;
			numericUpDown2.Value = 100;
			numericUpDown3.Value = 10;
		}

        private void button2_Click(object sender, EventArgs e)
        {
			
        }
    }
}
