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
		public Form1()
		{
			InitializeComponent();
		}

        private void button1_Click(object sender, EventArgs e)
        {
			MessageBox.Show("Figyelem! A kiválasztott mappában a következő almappáknak kell szerepelniük: Idle, Run, Bored, Happy, Warm, Cold.");
			DialogResult result = folderBrowserDialog1.ShowDialog();
			if (result == DialogResult.OK)
				textBox1.Text = folderBrowserDialog1.SelectedPath;
		}
    }
}
