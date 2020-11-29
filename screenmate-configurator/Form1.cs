using ScreenMateNET;
using ScreenMateNET.Model;
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
			// Értékek betöltése a form-ra
			numericUpDown1.Value = LocalSettings.Instance.Settings.CpuPercentLimit;
			numericUpDown2.Value = LocalSettings.Instance.Settings.MemoryPercentLimit;
			numericUpDown3.Value = LocalSettings.Instance.Settings.WaitingToBoredInSec;
			checkBox1.CheckState = LocalSettings.Instance.Settings.IsBoringNeeded ? CheckState.Checked : CheckState.Unchecked;
			checkBox2.CheckState = LocalSettings.Instance.Settings.IsCursorChasing ? CheckState.Checked : CheckState.Unchecked;

			Dictionary<ScreenMateStateID, StateSetting> settings = LocalSettings.Instance.StateSettings;
			int index = 0;
			folderPaths = new Dictionary<int, string>();
			foreach (ScreenMateStateID id in settings.Keys)
			{
				folderPaths.Add(index, settings[id].FilePath);
				this.comboBox1.Items.Add(id.ToString());
				index++;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			LocalSettings.Instance.Settings.IdlePath = folderPaths[0];
			LocalSettings.Instance.Settings.CursorChasingPath = folderPaths[1];
			LocalSettings.Instance.Settings.BoredPath = folderPaths[2];
			LocalSettings.Instance.Settings.GoTopOfWindowPath = folderPaths[3];
			LocalSettings.Instance.Settings.WarmPath = folderPaths[4];
			LocalSettings.Instance.Settings.CpuPercentLimit = (int)numericUpDown1.Value;
			LocalSettings.Instance.Settings.MemoryPercentLimit = (int)numericUpDown2.Value;
			LocalSettings.Instance.Settings.WaitingToBoredInSec = (int)numericUpDown3.Value;
			LocalSettings.Instance.Settings.IsBoringNeeded = checkBox1.Checked;
			LocalSettings.Instance.Settings.IsCursorChasing = checkBox2.Checked;

			LocalSettings.Instance.SaveStatePermanent();
		}

	}
}
