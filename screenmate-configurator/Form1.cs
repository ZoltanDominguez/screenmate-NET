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
		private string configFilePath = @"..\..\..\..\screenmate-NET\bin\Debug\netcoreapp3.1\config.json";
		private Dictionary<int, string> folderPaths;
		SettingsSerializable settingsSerializable;
		public Form1()
		{
			settingsSerializable = new SettingsSerializable();
			if (File.Exists(configFilePath))
			{
				settingsSerializable = settingsSerializable.Load(configFilePath, false);
			}
				// TODO: path-ok betöltése
			if (folderPaths == null)
            {
				folderPaths = new Dictionary<int, string>();
            }
			folderPaths.Add(0, settingsSerializable.idlePath);
			folderPaths.Add(1, settingsSerializable.cursorChasingPath);
			folderPaths.Add(2, settingsSerializable.boredPath);
			folderPaths.Add(3, settingsSerializable.goTopOfWindowPath);
			folderPaths.Add(4, settingsSerializable.warmPath);
			// folderPaths.Add(5, settingsSerializable.cursorReachedPath);
			
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
			numericUpDown1.Value = settingsSerializable.cpuPercentLimit;
			numericUpDown2.Value = settingsSerializable.memoryPercentLimit;
			numericUpDown3.Value = settingsSerializable.waitingToBoredInSec;
			checkBox1.CheckState = settingsSerializable.isBoringNeeded ? CheckState.Checked : CheckState.Unchecked;
			checkBox2.CheckState = settingsSerializable.isCursorChasing ? CheckState.Checked : CheckState.Unchecked;
		}

        private void button2_Click(object sender, EventArgs e)
        {
			settingsSerializable.idlePath = folderPaths[0];
			settingsSerializable.cursorChasingPath = folderPaths[1];
			settingsSerializable.boredPath = folderPaths[2];
			settingsSerializable.goTopOfWindowPath = folderPaths[3];
			settingsSerializable.warmPath = folderPaths[4];
			settingsSerializable.cpuPercentLimit = (int)numericUpDown1.Value;
			settingsSerializable.memoryPercentLimit = (int)numericUpDown2.Value;
			settingsSerializable.waitingToBoredInSec = (int)numericUpDown2.Value;
			settingsSerializable.isBoringNeeded = checkBox1.Checked;
			settingsSerializable.isCursorChasing = checkBox2.Checked;
			settingsSerializable.SaveConfigToJSON(configFilePath);
		}
	}
}
